using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Enums
{
    public static class GrabFoodItemEnum
    {
        public enum AvailableStatus
        {
            AVAILABLE = 1,
            OUT_OF_STOCK_TODAY = 2,
            OUT_OF_STOCK_INDENTIFINITELY = 3,
            DEACTIVE = 4
        }
    }
}
