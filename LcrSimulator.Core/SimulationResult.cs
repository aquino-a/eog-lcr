namespace LcrSimulator.Core
{
    public class SimulationResult
    {
        public SimulationResult(GameResult[] gameResults)
        {
            GameResults = gameResults;
            Average = (int)GameResults?.Select(gr => gr.Turns).Average()!;
            Longest = (int)(GameResults?.Select(gr => gr.Turns).Max())!;
            MostWins = (int)GameResults?.GroupBy(gr => gr.Winner)
                        .OrderByDescending(g => g.Count())
                        .First()
                        .Key!;
            Shortest = (int)(GameResults?.Select(gr => gr.Turns).Min())!;
        }

        public int Average { get; private set; }

        public GameResult[] GameResults { get; private set; }

        public int Longest { get; private set; }

        public int MostWins { get; private set; }

        public int Shortest { get; private set; }
        public int TotalPlayers { get; internal set; }
    }
}