using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PhotoVis.Data;

namespace PhotoVis.Helpers
{
    public class RangeToAgeConverter : ObservableObject, IValueConverter
    {
        private bool isInitialized = false;
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                // From a range of 0 to 100 to a datetime start and datetime end
                if (!this.isInitialized)
                {
                    IntervalToAgeFilter.SetIntervalToAgeFilter();
                    this.isInitialized = true;
                }

                DateTime pickedTime = IntervalToAgeFilter.ValueToDateTime((double)value);
                return pickedTime.ToString("yyyy-MM-dd");
            }
            catch
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //TODO:
            return 0;
        }
    }
}
