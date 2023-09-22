namespace LcrSimulator.Core
{
    //public record GameResult(int gameNum, int turns, int winner);
    public class GameResult
    {
        public int GameNumber { get; set; }
        public int Turns { get; set; }
        public int Winner { get; set; }
    }
}