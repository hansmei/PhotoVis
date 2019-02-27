using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoVis.Data;

namespace PhotoVis.Helpers
{
    class IntervalToAgeFilter
    {
        private static DateTime _lowerAge;
        private static DateTime _upperAge;

        private static double numDaysInSpan;

        public static void SetIntervalToAgeFilter()
        {
            if (App.MapVM.ImageLocations.Count == 0)
                return;

            // From a range of 0 to 100 to a datetime start and datetime end
            IOrderedEnumerable<ImageAtLocation> query =
                from m in App.MapVM.ImageLocations
                orderby m.TimeImageTaken ascending
                select m;

            _lowerAge = query.First().TimeImageTaken;
            _upperAge = query.Last().TimeImageTaken;

            numDaysInSpan = (_upperAge - _lowerAge).TotalDays;
        }

        public static DateTime ValueToDateTime(double value)
        {
            if(numDaysInSpan == 0)
            {
                SetIntervalToAgeFilter();
            }

            double append = value * numDaysInSpan / 100;
            DateTime pickedTime = _lowerAge.AddDays(append);
            return pickedTime;
        }

        public static double ImageToParameter(ImageAtLocation image)
        {
            if (numDaysInSpan == 0)
            {
                SetIntervalToAgeFilter();
            }

            TimeSpan distanceFromStart = image.TimeImageTaken.Subtract(_lowerAge);
            double percent = distanceFromStart.TotalDays / numDaysInSpan;

            // Clamp to 0-1 span
            if (percent > 1)
                percent = 1;
            else if (percent < 0)
                percent = 0;
            return percent;
        }

    }
}
