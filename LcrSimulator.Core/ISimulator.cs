using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LcrSimulator.Core
{
    public interface ISimulator
    {
        public SimulationResult Simulate(int players, int games, CancellationToken cancellationToken);
    }
}