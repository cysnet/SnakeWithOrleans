using Microsoft.AspNetCore.SignalR.Client;
using Orleans;
using Snake.Common;
using System;
using System.Data.Common;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Snake.Client
{
    public partial class Form1 : Form
    {
        private IClusterClient clusterClient { get; set; }
        private string DefaultGameName { get; set; } = "DefaultGame02";
        private Guid sid { get; set; } = Guid.NewGuid();
        private GameState gameState { get; set; }
        private List<SnakeState> snakeStates { get; set; }
        public Form1(IClusterClient _clusterClient)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            InitializeComponent();

            clusterClient = _clusterClient;
            this.ClientSize = new Size(GameSize.Width * GameSize.CellSize, GameSize.Height * GameSize.CellSize);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            var _connection = new HubConnectionBuilder()
           .WithUrl("http://localhost:5251/ping")
               .Build();

            _connection.Closed += async (error) =>
            {
                await Task.Delay(1000);
                await _connection.StartAsync();
            };

            _connection.On<GameState, List<SnakeState>>("ReceiveGameState", (_gameState, _snakeStates) =>
            {
                Invoke(async () =>
                {
                    snakeStates = _snakeStates;
                    gameState = _gameState;
                    Invalidate();

                });
            });
            while (_connection.State != HubConnectionState.Connected)
            {
                await Task.Delay(1000);

                try
                {
                    await _connection.StartAsync();
                }
                catch (Exception ex)
                {

                }
            }
            var snake = clusterClient.GetGrain<ISnakeGrain>(sid);
            var game = clusterClient.GetGrain<IGameGrain>(DefaultGameName);

            await game.JoinSnake(snake.GetPrimaryKey());
            await _connection.SendAsync("JoinGame", DefaultGameName);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            DrawGame.DrawGamePanel(g);
            DrawGame.DrawSnake(g, snakeStates);
            DrawGame.DrawFood(g, gameState);
        }

        private async void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                var snake = clusterClient.GetGrain<ISnakeGrain>(sid);
                await snake.ClientChangeDirection(Direction.Up);
            }
            else if (e.KeyCode == Keys.Down)
            {
                var snake = clusterClient.GetGrain<ISnakeGrain>(sid);
                await snake.ClientChangeDirection(Direction.Down);
            }
            else if (e.KeyCode == Keys.Left)
            {
                var snake = clusterClient.GetGrain<ISnakeGrain>(sid);
                await snake.ClientChangeDirection(Direction.Left);
            }
            else if (e.KeyCode == Keys.Right)
            {
                var snake = clusterClient.GetGrain<ISnakeGrain>(sid);
                await snake.ClientChangeDirection(Direction.Right);
            }
        }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var game = clusterClient.GetGrain<IGameGrain>(DefaultGameName);
            await game.RemoveSnake(sid);
        }
    }
}
