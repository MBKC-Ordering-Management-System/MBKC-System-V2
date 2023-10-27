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

        public static bool IsTimeUpdateValid(DateTime timeLater, DateTime timeEarlier, int condition)
        {
            // Convert the two times to TimeSpan objects.
            TimeSpan timeLaterSpan = new TimeSpan(timeLater.Ticks);
            TimeSpan timeEarlierSpan = new TimeSpan(timeEarlier.Ticks);

            // Subtract the two TimeSpan objects to get the difference.
            TimeSpan difference = timeLaterSpan.Subtract(timeEarlierSpan);

            // Check if the difference is at least condition minute.
            return difference.TotalMinutes >= condition;
        }
    }
}
