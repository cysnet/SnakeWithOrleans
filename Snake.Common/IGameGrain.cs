using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.Common
{
    [GenerateSerializer]
    public class GameState
    {
        [Id(0)]
        public Position SnakeFood { get; set; }
        [Id(1)]
        public List<Guid> SnakeIds { get; set; }
        [Id(2)]
        public string GameId{get;set; }

        private static Random _random = new();
        public static Position CreateSnakeFood()
        {
            var randomX = _random.Next(SnakeSize.Size, GameSize.Width - SnakeSize.Size);
            var randomY = _random.Next(SnakeSize.Size, GameSize.Height - SnakeSize.Size);

            return new Position { X = randomX, Y = randomY };
        }
    }
    public interface IGameGrain : IGrainWithStringKey
    {
        Task JoinSnake(Guid sId);
        Task RemoveSnake(Guid sId);
        Task<List<SnakeState>> GetSnakes();
        Task<GameState> GetGameState();
    }
}
