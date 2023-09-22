using System.Windows.Input;

namespace LcrSimulator.Core
{
    public class MainWindowViewModel
    {
        private readonly ISimulator _simulator;

        public MainWindowViewModel(ISimulator simulator)
        {
            _simulator = simulator;
        }

        public record Setting(int playerCount, int gameCount);

        public ICommand CancelCommand { get; set; }

        public SimulationResult CurrentResult { get; set; }

        public Setting CurrentSetting { get; set; }

        public int GameCount { get; set; }

        public ICommand PlayCommand { get; set; }

        public int PlayerCount { get; set; }

        public List<Setting> Presets { get; set; } = new List<Setting>()
        {
            new Setting(3, 100),
            new Setting(4, 100),
            new Setting(5, 100),
            new Setting(5, 1_000),
            new Setting(5, 10_000),
            new Setting(5, 100_000),
            new Setting(6, 100),
            new Setting(7, 100),
        };
    }
}