using System.Xml.Linq;

namespace Snake.Common
{
    [GenerateSerializer]
    public class SnakeState
    {
        [Id(0)]
        public Direction CurrentDirection { get; set; }
        [Id(1)]
        public List<Position> SnakeBody { get; set; }
        [Id(2)]
        public Guid SnakeId { get; set; }
        [Id(3)]
        public List<Direction> NextDirections{get;set;} = new List<Direction>();

        private static Random _random = new();
        public static SnakeState CreateSnakeData()
        {
            var snakeData = new SnakeState();
            snakeData.CurrentDirection = (Direction)_random.Next(4);
            snakeData.SnakeBody = new List<Position>();
            var randomX = _random.Next(SnakeSize.Size, GameSize.Width - SnakeSize.Size);
            var randomY = _random.Next(SnakeSize.Size, GameSize.Height - SnakeSize.Size);

            for (int i = 0; i < SnakeSize.Size; i++)
            {
                if (snakeData.CurrentDirection == Direction.Up)
                {
                    snakeData.SnakeBody.Add(new Position { X = randomX, Y = randomY + i });
                }
                if (snakeData.CurrentDirection == Direction.Down)
                {
                    snakeData.SnakeBody.Add(new Position { X = randomX, Y = randomY - i });
                }
                if (snakeData.CurrentDirection == Direction.Right)
                {
                    snakeData.SnakeBody.Add(new Position { X = randomX + i, Y = randomY });
                }
                if (snakeData.CurrentDirection == Direction.Left)
                {
                    snakeData.SnakeBody.Add(new Position { X = randomX - i, Y = randomY });
                }
            }

            return snakeData;
        }

    }
    public interface ISnakeGrain : IGrainWithGuidKey
    {
        Task ClientChangeDirection(Direction direction);
        Task ServerChangeDirection();
        Task<bool> Move(Position foodPosition);
        Task<SnakeState> GetSnakeState();
        void LeaveGame();
    }
}
