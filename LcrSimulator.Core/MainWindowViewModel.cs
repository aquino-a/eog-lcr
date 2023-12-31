﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LcrSimulator.Core
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private static readonly int MAX_STEPS = 1_000;
        private static readonly int MAX_STEPS_CUTOFF = 5_000;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(GameResults))]
        [NotifyPropertyChangedFor(nameof(AveragePoints))]
        [NotifyPropertyChangedFor(nameof(ShortestPoints))]
        [NotifyPropertyChangedFor(nameof(LongestPoints))]
        [NotifyPropertyChangedFor(nameof(Players))]
        [NotifyCanExecuteChangedFor(nameof(PlayCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
        public SimulationResult _currentResult;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(GameCount))]
        [NotifyPropertyChangedFor(nameof(PlayerCount))]
        [NotifyCanExecuteChangedFor(nameof(PlayCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
        public Setting _currentSetting;

        private readonly ISimulator _simulator;
        private CancellationTokenSource _cancelSource;

        public MainWindowViewModel(ISimulator simulator)
        {
            _simulator = simulator;
            PlayCommand = new RelayCommand<Setting>(setting => Play(setting), (setting) => setting != null && setting.PlayerCount >= 3 && setting.GameCount > 0 && _cancelSource == null);
            CancelCommand = new RelayCommand(Cancel, () => _cancelSource != null && _cancelSource.Token.CanBeCanceled);
        }

        public IEnumerable<KeyValuePair<int, int>> AveragePoints
        {
            get
            {
                if (CurrentResult == null)
                {
                    return Enumerable.Empty<KeyValuePair<int, int>>();
                }

                return new[]
                {
                    new KeyValuePair<int, int>(CurrentResult.Average, 0),
                    new KeyValuePair<int, int>(CurrentResult.Average, CurrentResult.GameResults.Length),
                };
            }
        }

        public IRelayCommand CancelCommand { get; private set; }

        public int GameCount { get; set; }

        public IEnumerable<KeyValuePair<int, int>> GameResults
        {
            get
            {
                if (CurrentResult?.GameResults.Length > MAX_STEPS_CUTOFF)
                {
                    var length = CurrentResult?.GameResults.Length;
                    var step = length / MAX_STEPS;

                    return CurrentResult?.GameResults
                        .Where((gr, i) => i % step == 0)
                        .Select(gr => new KeyValuePair<int, int>(gr.Turns, gr.GameNumber))!;
                }

                return CurrentResult?.GameResults
                    .Select(gr => new KeyValuePair<int, int>(gr.Turns, gr.GameNumber))!;
            }
        }

        public IEnumerable<KeyValuePair<int, int>> LongestPoints
        {
            get
            {
                return CurrentResult?.GameResults
                    .Where(gr => gr.Turns == CurrentResult.Longest)
                    .Select(gr => new KeyValuePair<int, int>(gr.Turns, gr.GameNumber))!;
            }
        }

        public IRelayCommand PlayCommand { get; private set; }

        public int PlayerCount { get; set; }

        public IEnumerable<Player> Players
        {
            get
            {
                if (CurrentResult == null)
                {
                    return Enumerable.Empty<Player>();
                }

                return Enumerable.Range(1, (int)CurrentResult.TotalPlayers)
                    .Select(i => new Player { Number = i, IsWinner = i == CurrentResult.MostWins });
            }
        }

        public List<Setting> Presets { get; set; } = new List<Setting>()
        {
            new Setting{ PlayerCount = 3, GameCount = 100 },
            new Setting{ PlayerCount = 4, GameCount = 100 },
            new Setting{ PlayerCount = 5, GameCount = 100 },
            new Setting{ PlayerCount = 5, GameCount = 1_000 },
            new Setting{ PlayerCount = 5, GameCount = 10_000 },
            new Setting{ PlayerCount = 5, GameCount = 100_000 },
            new Setting{ PlayerCount = 6, GameCount = 100 },
            new Setting{ PlayerCount = 7, GameCount = 100 },
        };

        public IEnumerable<KeyValuePair<int, int>> ShortestPoints
        {
            get
            {
                return CurrentResult?.GameResults
                    .Where(gr => gr.Turns == CurrentResult.Shortest)
                    .Select(gr => new KeyValuePair<int, int>(gr.Turns, gr.GameNumber))!;
            }
        }

        public void Cancel()
        {
            if (_cancelSource == null)
            {
                return;
            }

            _cancelSource.Cancel();
        }

        private async void Play(Setting setting)
        {
            if (_cancelSource != null || setting == null)
            {
                return;
            }

            try
            {
                _cancelSource = new CancellationTokenSource();
                var simulationTask = Task.Factory.StartNew(() =>
                {
                    return _simulator.Simulate(setting.PlayerCount, setting.GameCount, _cancelSource.Token);
                }, _cancelSource.Token);

                PlayCommand.NotifyCanExecuteChanged();
                CancelCommand.NotifyCanExecuteChanged();

                var result = await simulationTask;

                if (result == null || _cancelSource.IsCancellationRequested)
                {
                    return;
                }

                CurrentResult = result;
            }
            finally
            {
                _cancelSource?.Dispose();
                _cancelSource = null;

                PlayCommand.NotifyCanExecuteChanged();
                CancelCommand.NotifyCanExecuteChanged();
            }
        }
    }
}