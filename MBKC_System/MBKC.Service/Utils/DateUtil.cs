using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Utils
{
    public static class DateUtil
    {
        public static DateTime ConvertUnixTimeToDateTime(long utcExpiredDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeInterval = dateTimeInterval.AddSeconds(utcExpiredDate).ToUniversalTime();
            return dateTimeInterval;
        }

        public static bool IsTimeUpdateValid(TimeSpan timeSpanLater, TimeSpan timeSpanEarlier, int condition)
        {
            // Subtract the two TimeSpan objects to get the difference.
            TimeSpan difference = timeSpanLater.Subtract(timeSpanEarlier);

            // Check if the difference is at least condition minute.
            return difference.TotalHours >= condition;
        }
    }
}
