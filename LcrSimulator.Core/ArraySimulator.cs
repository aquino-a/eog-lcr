namespace LcrSimulator.Core
{
    public class ArraySimulator : ISimulator
    {
        private static readonly char[] DICE_FACES = { 'L', 'C', 'R', '.', '.', '.' };

        private Random _random = new();

        public SimulationResult Simulate(int playerCount, int gameCount, CancellationToken cancelToken)
        {
            if (playerCount < 3 || playerCount > 100)
            {
                throw new ArgumentException("Players must be greater than or equal to 3 and less than 100.");
            }

            if (gameCount <= 0 || gameCount > 100_000)
            {
                throw new ArgumentException("Game count must be greater than or equal to 1 and less than 100,000.");
            }

            var results = new GameResult[gameCount];
            for (int i = 0; i < gameCount; i++)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return null;
                }

                results[i] = Play(playerCount);
                results[i].GameNumber = i + 1;
            }

            var turns = results.Select(gr => gr.Turns).ToArray();

            return new SimulationResult(results)
            {
                TotalPlayers = playerCount
            };
        }

        private int[] GetInitialPlayers(int playerCount)
        {
            var players = new int[playerCount];
            for (int i = 0; i < playerCount; i++)
            {
                players[i] = 3;
            }

            return players;
        }

        private int GetWinner(int[] players)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] > 0)
                {
                    return i + 1;
                }
            }

            throw new Exception("Winner not found!!");
        }

        private bool IsOver(int[] players)
        {
            var isChipsFound = false;
            for (int i = 0; i < players.Length; i++)
            {
                if (!isChipsFound && players[i] > 0)
                {
                    isChipsFound = true;
                }
                else if (isChipsFound && players[i] > 0)
                {
                    return false;
                }
            }

            return isChipsFound;
        }

        private GameResult Play(int playerCount)
        {
            var players = GetInitialPlayers(playerCount);

            var playingCount = playerCount;

            int turnCount = 0;
            for (; !IsOver(players); turnCount++)
            {
                for (var i = 0; i < playerCount; i++)
                {
                    if (playingCount == 1)
                    {
                        break;
                    }

                    if (players[i] == 0)
                    {
                        continue;
                    }

                    var playerChips = players[i];
                    var diceCount = playerChips >= 3
                        ? 3
                        : playerChips;

                    for (var j = 0; j < diceCount; j++)
                    {
                        var roll = DICE_FACES[_random.Next(DICE_FACES.Length)];
                        switch (roll)
                        {
                            case '.':
                                continue;
                            case 'L':
                                var leftIndex = i == 0
                                    ? players.Length - 2
                                    : i - 1;

                                players[i]--;

                                if (players[leftIndex] == 0)
                                {
                                    playingCount++;
                                }

                                players[leftIndex]++;
                                break;

                            case 'R':
                                var rightIndex = i == players.Length - 1
                                    ? 0
                                    : i + 1;

                                players[i]--;
                                if (players[rightIndex] == 0)
                                {
                                    playingCount++;
                                }

                                players[rightIndex]++;
                                break;

                            case 'C':
                                players[i]--;
                                break;

                            default:
                                throw new ArgumentException("roll must be L, C, R or .");
                        }
                    }

                    if (players[i] == 0)
                    {
                        playingCount--;
                    }
                }
            }

            return new GameResult
            {
                Turns = turnCount,
                Winner = GetWinner(players)
            };
        }
    }
}