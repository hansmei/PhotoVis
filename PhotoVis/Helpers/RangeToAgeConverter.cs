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
        private DateTime _lowerAge;
        private DateTime _upperAge;

        private double numDaysInSpan;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // From a range of 0 to 100 to a datetime start and datetime end
            if(numDaysInSpan == 0)
            {
                IOrderedEnumerable<ImageAtLocation> query =
                    from m in App.MapVM.ImageLocations
                    orderby m.TimeImageTaken ascending
                    select m;

                _lowerAge = query.First().TimeImageTaken;
                _upperAge = query.Last().TimeImageTaken;

                numDaysInSpan = (_upperAge - _lowerAge).TotalDays;
            }

            double append = (double)value * numDaysInSpan / 100;
            DateTime pickedTime = _lowerAge.AddDays(append);

            return pickedTime.ToString("yyyy-MM-dd");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //TODO:
            return 0;
        }
    }
}
