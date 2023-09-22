using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LcrSimulator.Core
{
    public class SimulationResult
    {
        public int Shortest { get; set; }
        public int Longest { get; set; }
        public int Average { get; set; }
        public int MostWins { get; set; }
    }
}
