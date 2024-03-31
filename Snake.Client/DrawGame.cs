using Orleans;
using Snake.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Snake.Client
{
    public class DrawGame
    {
        private static Color GamePanelColor = Color.FromArgb(255, 255, 255);
        private static Brush GamePanelBrush = new SolidBrush(GamePanelColor);

        public static void DrawGamePanel(Graphics g)
        {
            for (int i = 0; i < GameSize.Width; i++)
            {
                for (int j = 0; j < GameSize.Height; j++)
                {
                    var rect = new Rectangle(i * GameSize.CellSize, j * GameSize.CellSize, GameSize.CellSize, GameSize.CellSize);
                    g.FillRectangle(GamePanelBrush, rect);
                    g.DrawRectangle(Pens.Black, rect);
                }
            }
        }

        private static Color Snake1Color = Color.FromArgb(255, 0, 0);
        private static Brush Snake1Brush = new SolidBrush(Snake1Color);
        private static Color Snake2Color = Color.FromArgb(0, 255, 0);
        private static Brush Snake2Brush = new SolidBrush(Snake2Color);
        private static Color Snake3Color = Color.FromArgb(0, 0, 255);
        private static Brush Snake3Brush = new SolidBrush(Snake3Color);
        private static Color Snake4Color = Color.FromArgb(255, 0, 255);
        private static Brush Snake4Brush = new SolidBrush(Snake4Color);
        private static Color Snake5Color = Color.FromArgb(0, 255, 255);
        private static Brush Snake5Brush = new SolidBrush(Snake5Color);
        private static Color Snake6Color = Color.FromArgb(0, 0, 0);
        private static Brush Snake6Brush = new SolidBrush(Snake6Color);
        private static Brush[] SnakeBrushes = { Snake1Brush, Snake2Brush, Snake3Brush, Snake4Brush, Snake5Brush, Snake6Brush };
        private static Random _random = new();
        public static void DrawSnake(Graphics g, List<SnakeState> snakeStates)
        {
            if (snakeStates == null) return;

            foreach (var snakeState in snakeStates)
            {
                var m = GetFixedHash(snakeState.SnakeId) % 6;
                if (m < 0) m = -m;
                var brush = SnakeBrushes[m];

                for (int i = 0; i < GameSize.Width; i++)
                {
                    for (int j = 0; j < GameSize.Height; j++)
                    {
                        var rect = new Rectangle(i * GameSize.CellSize, j * GameSize.CellSize, GameSize.CellSize, GameSize.CellSize);

                        var snakeBody = snakeState.SnakeBody;
                        foreach (var b in snakeBody)
                        {
                            if (b.X == i && b.Y == j)
                            {
                                g.FillRectangle(brush, rect);
                                g.DrawRectangle(Pens.Black, rect);
                            }
                        }
                    }
                }
            }

        }

        private static Color FoodColor = Color.FromArgb(255, 175, 0);
        private static Brush FoodBrush = new SolidBrush(FoodColor);
        public static void DrawFood(Graphics g, GameState gameState)
        {
            if (gameState == null) return;
            for (int i = 0; i < GameSize.Width; i++)
            {
                for (int j = 0; j < GameSize.Height; j++)
                {
                    if (gameState.SnakeFood.X == i && gameState.SnakeFood.Y == j)
                    {
                        var rect = new Rectangle(i * GameSize.CellSize, j * GameSize.CellSize, GameSize.CellSize, GameSize.CellSize);
                        g.FillEllipse(FoodBrush, rect);
                        return;
                    }
                }
            }
        }

        static int GetFixedHash(Guid guid)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(guid.ToString()));
                return BitConverter.ToInt32(hash, 0);
            }
        }
    }
}
