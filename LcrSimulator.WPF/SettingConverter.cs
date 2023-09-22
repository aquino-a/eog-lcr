using LcrSimulator.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LcrSimulator.WPF
{
    public class SettingConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2)
            {
                return null;
            }

            if (!int.TryParse((string)values[0], out var playerCount))
            {
                return null;
            }
            if (!int.TryParse((string)values[1], out var gameCount))
            {
                return null;
            }

            return new Setting
            {
                GameCount = gameCount,
                PlayerCount = playerCount
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}