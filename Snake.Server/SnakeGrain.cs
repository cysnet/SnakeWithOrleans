using Orleans;
using Orleans.Runtime;
using Snake.Common;
using System.Xml.Linq;

namespace Snake.Server
{
    public class SnakeGrain : Grain, ISnakeGrain
    {
        IPersistentState<SnakeState> state;
        public SnakeGrain([PersistentState(stateName: "snake", storageName: "memorysnake")] IPersistentState<SnakeState> _state)
        {
            state = _state;
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await base.OnActivateAsync(cancellationToken);

            await state.ReadStateAsync();

            if (state.State.SnakeBody == null)
            {
                state.State = SnakeState.CreateSnakeData();
                state.State.SnakeId = this.GetPrimaryKey();
                await state.WriteStateAsync();
            }
        }

        public async Task ClientChangeDirection(Direction direction)
        {
            if (direction == Direction.Down && state.State.CurrentDirection == Direction.Up) return;
            if (direction == Direction.Up && state.State.CurrentDirection == Direction.Down) return;
            if (direction == Direction.Left && state.State.CurrentDirection == Direction.Right) return;
            if (direction == Direction.Right && state.State.CurrentDirection == Direction.Left) return;
            state.State.NextDirections.Add(direction);
            await state.WriteStateAsync();
        }

        public async Task ServerChangeDirection()
        {
            if (state.State.NextDirections.Count>0 && state.State.NextDirections.FirstOrDefault() != null)
            {
                state.State.CurrentDirection = state.State.NextDirections.FirstOrDefault();
                state.State.NextDirections.RemoveAt(0);
                await state.WriteStateAsync();
            }
        }

        public async Task<bool> Move(Position foodPosition)
        {
            var needReGenerateFood = false;
            var newBody = new List<Position>();

            for (int i = 0; i < state.State.SnakeBody.Count; i++)
            {
                var p = new Position();
                if (i == 0)
                {
                    if (state.State.CurrentDirection == Direction.Up)
                    {
                        if (state.State.SnakeBody[i].Y == 0)
                        {
                            p.Y = GameSize.Height - 1;
                            p.X = state.State.SnakeBody[i].X;
                        }
                        else
                        {
                            p.Y = state.State.SnakeBody[i].Y - 1;
                            p.X = state.State.SnakeBody[i].X;
                        }
                        newBody.Add(p);
                    }

                    if (state.State.CurrentDirection == Direction.Down)
                    {
                        if (state.State.SnakeBody[i].Y == GameSize.Height - 1)
                        {
                            p.Y = 0;
                            p.X = state.State.SnakeBody[i].X;
                        }
                        else
                        {
                            p.Y = state.State.SnakeBody[i].Y + 1;
                            p.X = state.State.SnakeBody[i].X;
                        }
                        newBody.Add(p);
                    }

                    if (state.State.CurrentDirection == Direction.Right)
                    {
                        if (state.State.SnakeBody[i].X == GameSize.Width - 1)
                        {
                            p.Y = state.State.SnakeBody[i].Y;
                            p.X = 0;
                        }
                        else
                        {
                            p.Y = state.State.SnakeBody[i].Y;
                            p.X = state.State.SnakeBody[i].X + 1;
                        }
                        newBody.Add(p);
                    }

                    if (state.State.CurrentDirection == Direction.Left)
                    {
                        if (state.State.SnakeBody[i].X == 0)
                        {
                            p.Y = state.State.SnakeBody[i].Y;
                            p.X = GameSize.Width - 1;
                        }
                        else
                        {
                            p.Y = state.State.SnakeBody[i].Y;
                            p.X = state.State.SnakeBody[i].X - 1;
                        }
                        newBody.Add(p);
                    }
                }
                else
                {
                    if (state.State.CurrentDirection == Direction.Up)
                    {
                        p.X = state.State.SnakeBody[i - 1].X;
                        p.Y = state.State.SnakeBody[i - 1].Y;
                        newBody.Add(p);
                    }
                    if (state.State.CurrentDirection == Direction.Down)
                    {
                        p.X = state.State.SnakeBody[i - 1].X;
                        p.Y = state.State.SnakeBody[i - 1].Y;
                        newBody.Add(p);
                    }
                    if (state.State.CurrentDirection == Direction.Right)
                    {
                        p.X = state.State.SnakeBody[i - 1].X;
                        p.Y = state.State.SnakeBody[i - 1].Y;
                        newBody.Add(p);
                    }
                    if (state.State.CurrentDirection == Direction.Left)
                    {
                        p.X = state.State.SnakeBody[i - 1].X;
                        p.Y = state.State.SnakeBody[i - 1].Y;
                        newBody.Add(p);
                    }
                }
            }

            state.State.SnakeBody = newBody;
            if (state.State.SnakeBody[0].X == foodPosition.X && state.State.SnakeBody[0].Y == foodPosition.Y)
            {
                needReGenerateFood = true;
                var lastNode = state.State.SnakeBody[state.State.SnakeBody.Count - 1];
                var newNode = new Position();
                newNode.X = lastNode.X;
                newNode.Y = lastNode.Y;

                state.State.SnakeBody.Add(newNode);
            }

            await state.WriteStateAsync();
            return needReGenerateFood;
        }

        public Task<SnakeState> GetSnakeState()
        {
            return Task.FromResult(state.State);
        }

        public void LeaveGame()
        {
            DeactivateOnIdle();
        }
    }
}
