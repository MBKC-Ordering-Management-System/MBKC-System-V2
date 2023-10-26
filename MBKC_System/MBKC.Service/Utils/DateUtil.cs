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

        public static bool IsTimeUpdateEarlierThanTimeExitsToStoreByAtLeastOneMinute(DateTime timeUpdate, DateTime timeCheck)
        {
            // Convert the two times to TimeSpan objects.
            TimeSpan timeUpdateSpan = new TimeSpan(timeUpdate.Ticks);
            TimeSpan timeExitsToStoreSpan = new TimeSpan(timeCheck.Ticks);

            // Subtract the two TimeSpan objects to get the difference.
            TimeSpan difference = timeExitsToStoreSpan.Subtract(timeUpdateSpan);

            // Check if the difference is at least 1 minute.
            return difference.TotalMinutes >= 1;
        }
    }
}
