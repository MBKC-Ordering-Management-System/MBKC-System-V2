using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Enums
{
    public static class StoreEnum
    {
        public enum Status
        {
            INACTIVE = 0,
            ACTIVE = 1,
            DEACTIVE = 2,
            BE_CONFIRMING = 3,
            REJECTED = 4
        }
    }
}
