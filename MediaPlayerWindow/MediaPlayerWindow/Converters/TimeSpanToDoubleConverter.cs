using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MediaPlayerWindow.Converters
{
    class TimeSpanToDoubleConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //TimeSpan interval = TimeSpan.Parse(value.ToString());
            
            //return interval.TotalSeconds;

            return System.Convert.ToDouble(((TimeSpan)value).Seconds);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
          
            //TimeSpan interval = TimeSpan.FromHours(System.Convert.ToDouble(value));
          
            //return interval;
            return TimeSpan.FromSeconds((double)value);

        }
    }
}
