﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace LcrSimulator.Core
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PlayCommand))]
        [NotifyPropertyChangedFor(nameof(CancelCommand))]
        public SimulationResult _currentResult;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(GameCount))]
        [NotifyPropertyChangedFor(nameof(PlayerCount))]
        [NotifyPropertyChangedFor(nameof(PlayCommand))]
        [NotifyPropertyChangedFor(nameof(CancelCommand))]
        public Setting _currentSetting;

        private readonly ISimulator _simulator;
        private CancellationTokenSource _cancelSource;

        public MainWindowViewModel(ISimulator simulator)
        {
            _simulator = simulator;
            PlayCommand = new RelayCommand<Setting>(setting => Play(setting), (setting) => setting != null && setting.PlayerCount >= 3 && setting.GameCount > 0 && _cancelSource == null);
            CancelCommand = new RelayCommand(Cancel, () => _cancelSource != null && _cancelSource.Token.CanBeCanceled);
        }

        public ICommand CancelCommand { get; private set; }
        public int GameCount { get; set; }

        public ICommand PlayCommand { get; private set; }

        public int PlayerCount { get; set; }

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

        private void Cancel()
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
                var result = await Task.Factory.StartNew(() =>
                {
                    return _simulator.Simulate(setting.PlayerCount, setting.GameCount);
                }, _cancelSource.Token);

                CurrentResult = result;
            }
            finally
            {
                _cancelSource = null;
            }
        }
    }
}