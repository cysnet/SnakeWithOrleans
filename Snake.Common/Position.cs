namespace Snake.Common
{
    [GenerateSerializer]
    public class Position
    {
        [Id(0)]
        public int X { get; set; }
        [Id(1)]
        public int Y { get; set; }
    }
}
