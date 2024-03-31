using Microsoft.AspNetCore.SignalR;
using Snake.Common;

namespace Snake.Server
{
    public class SnakeHub : Hub
    {
        public async Task SendGameState(GameState gameState,List<SnakeState> snakeStates)
        {
            if (Clients != null)
            {
                await Clients.Group(gameState.GameId).SendAsync("ReceiveGameState", gameState,snakeStates);
            }
        }

        public async Task JoinGame(string gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        }
    }
}
