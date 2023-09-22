namespace LcrSimulator.Core
{
    public class SimulationResult
    {
        public int Average { get; set; }
        public GameResult[] GameResults { get; set; }
        public int Longest { get; set; }
        public int MostWins { get; set; }
        public int Shortest { get; set; }
    }
}