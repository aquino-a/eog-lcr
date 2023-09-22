using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace LcrSimulator.Core
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        public SimulationResult _currentResult;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(GameCount))]
        [NotifyPropertyChangedFor(nameof(PlayerCount))]
        public Setting _currentSetting;

        private readonly ISimulator _simulator;
        private CancellationTokenSource _cancelSource;

        public MainWindowViewModel(ISimulator simulator)
        {
            _simulator = simulator;
            PlayCommand = new RelayCommand(Play, () => _currentSetting != null && _cancelSource == null);
            CancelCommand = new RelayCommand(Cancel, () => _cancelSource != null && _cancelSource.Token.CanBeCanceled);
        }

        public ICommand CancelCommand { get; private set; }
        public int GameCount { get; set; }

        public ICommand PlayCommand { get; private set; }

        public int PlayerCount { get; set; }

        public List<Setting> Presets { get; set; } = new List<Setting>()
        {
            new Setting{ GameCount = 3, PlayerCount = 100 },
            new Setting{ GameCount = 4, PlayerCount = 100 },
            new Setting{ GameCount = 5, PlayerCount = 100 },
            new Setting{ GameCount = 5, PlayerCount = 1_000 },
            new Setting{ GameCount = 5, PlayerCount = 10_000 },
            new Setting{ GameCount = 5, PlayerCount = 100_000 },
            new Setting{ GameCount = 6, PlayerCount = 100 },
            new Setting{ GameCount = 7, PlayerCount = 100 },
        };

        private void Cancel()
        {
            if (_cancelSource == null)
            {
                return;
            }

            _cancelSource.Cancel();
        }

        private async void Play()
        {
            if (_cancelSource != null)
            {
                return;
            }

            try
            {
                _cancelSource = new CancellationTokenSource();
                var result = await Task.Factory.StartNew(() =>
                {
                    return _simulator.Simulate(CurrentSetting.PlayerCount, CurrentSetting.GameCount);
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