using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raid.Toolkit.UI.WinUI.Xaml
{
    public class GridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Trace.WriteLine($"Convert: {value}");
            if (value is double dbl && targetType == typeof(GridLength))
                return new GridLength(dbl, GridUnitType.Pixel);

            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
