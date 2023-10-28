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

        public enum PaymentMethod
        {
            CASH,
            CASHLESS,
        }
    }
}
