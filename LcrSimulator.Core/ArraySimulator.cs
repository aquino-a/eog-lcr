using System;

namespace LcrSimulator.Core
{
    public class ArraySimulator : ISimulator
    {
        private static readonly char[] DICE_FACES = { 'L', 'C', 'R', '.', '.', '.' };
        private record GameResult(int turns, int winner);

        private Random _random = new();

        public SimulationResult Simulate(int playerCount, int gameCount)
        {
            if (playerCount < 3)
            {
                throw new ArgumentException("Players must be greater than or equal to 3.");
            }

            if (gameCount <= 0)
            {
                throw new ArgumentException("Game count must be greater than or equal to 1.");
            }

            var results = new GameResult[gameCount];
            for (int i = 0; i < gameCount; i++)
            {
                results[i] = Play(playerCount);
            }

            var turns = results.Select(gr => gr.turns).ToArray();

            return new SimulationResult
            {
                Longest = turns.Max(),
                Average = (int)turns.Average(),
                Shortest = turns.Min(),
                MostWins = results.GroupBy(gr => gr.winner)
                        .OrderByDescending(g => g.Count())
                        .First()
                        .Key,
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

            int turnCount = 0;
            for (; !IsOver(players); turnCount++)
            {
                for (var i = 0; i < playerCount; i++)
                {
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
                                players[leftIndex]++;
                                break;

                            case 'R':
                                var rightIndex = i == players.Length - 1
                                    ? 0
                                    : i + 1;
                                players[i]--;
                                players[rightIndex]++;
                                break;

                            case 'C':
                                players[i]--;
                                break;

                            default:
                                throw new ArgumentException("roll must be L, C, R or .");
                        }
                    }
                }
            }

            return new GameResult
            (
                turnCount,
                players.First(chips => chips > 0)
            );
        }
    }
}