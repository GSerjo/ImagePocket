using System;
using Foundation;

namespace Core
{
    //http://developer.xamarin.com/guides/cross-platform/macios/unified/#Converting_DateTime_to_NSDate
    public static class NSDateExtensions
    {
        public static DateTime ToDateTime(this NSDate date)
        {
            // NSDate has a wider range than DateTime, so clip
            // the converted date to DateTime.Min|MaxValue.
            double secs = date.SecondsSinceReferenceDate;
            if (secs < -63113904000)
            {
                return DateTime.MinValue;
            }
            if (secs > 252423993599)
            {
                return DateTime.MaxValue;
            }
            return (DateTime)date;
        }
    }
}
