using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LcrSimulator.Core
{
    public class Setting
    {
        public int PlayerCount { get; set; }
        public int GameCount { get; set; }
        public string Display { get => $"{PlayerCount} Players x {GameCount} games"; }
    }
}