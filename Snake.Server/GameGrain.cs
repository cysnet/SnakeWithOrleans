using Orleans;
using Orleans.Runtime;
using Snake.Common;
using System.Security.Cryptography;

namespace Snake.Server
{
    public class GameGrain : Grain, IGameGrain
    {
        IPersistentState<GameState> state;
        SnakeHub snakeHub;
        IGrainFactory grainFactory;
        public GameGrain(IGrainFactory _grainFactory, SnakeHub _snakeHub, [PersistentState(stateName: "snake", storageName: "memorysnake")] IPersistentState<GameState> _state)
        {
            state = _state;
            snakeHub = _snakeHub;
            grainFactory = _grainFactory;
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await base.OnActivateAsync(cancellationToken);

            await state.ReadStateAsync();

            state.State.GameId = this.GetPrimaryKeyString();
            if (state.State.SnakeIds == null || state.State.SnakeFood == null)
            {
                state.State.SnakeIds = new List<Guid>();
                state.State.SnakeFood = GameState.CreateSnakeFood();

                await state.WriteStateAsync();
            }

            var timer = RegisterTimer(async (obj) =>
            {
                var snakeStates = new List<SnakeState>();
                foreach (var item in state.State.SnakeIds)
                {
                    var snake = grainFactory.GetGrain<ISnakeGrain>(item);
                    var snakeState = await snake.GetSnakeState();
                    if (snakeState.NextDirections.Count > 0)
                    {
                       await  snake.ServerChangeDirection();
                    }
                    var needReCreateFood = await snake.Move(state.State.SnakeFood);
                    if (needReCreateFood)
                    {
                        state.State.SnakeFood = GameState.CreateSnakeFood();
                    }
                    snakeStates.Add(await snake.GetSnakeState());
                }
                await snakeHub.SendGameState(state.State, snakeStates);
            }, null, TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(150));
        }

        public async Task JoinSnake(Guid sId)
        {
            state.State.SnakeIds.Add(sId);
            await state.WriteStateAsync();
        }

        public async Task RemoveSnake(Guid sId)
        {
            state.State.SnakeIds.Remove(sId);
            grainFactory.GetGrain<ISnakeGrain>(sId).LeaveGame();
            await state.WriteStateAsync();
        }

        public async Task<List<SnakeState>> GetSnakes()
        {
            var result = new List<SnakeState>();
            for (var i = 0; i < state.State.SnakeIds.Count; i++)
            {
                result.Add(await grainFactory.GetGrain<ISnakeGrain>(state.State.SnakeIds[i]).GetSnakeState());
            }
            return result;

        }

        public Task<GameState> GetGameState()
        {
            return Task.FromResult(state.State);
        }
    }
}
