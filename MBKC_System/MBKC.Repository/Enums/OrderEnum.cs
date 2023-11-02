using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Enums
{
    public static class OrderEnum
    {
        public enum Status
        {
            PREPARING,
            READY,
            UPCOMING,
            COMPLETED,
            CANCELLED,
        }

        public enum SystemStatus
        {
            IN_STORE = 1,
            READY_DELIVERY = 2,
            COMPLETED = 3,
            CANCELLED = 4,
        }

        public enum PaymentMethod
        {
            CASH,
            CASHLESS,
        }
    }
}
