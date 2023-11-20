using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Enums
{
    public static class BrandEnum
    {
        public enum Status
        {
            INACTIVE = 0,
            ACTIVE = 1,
            DISABLE = 2
        }

        public enum StatusFilter
        {
            INACTIVE = 0,
            ACTIVE = 1
        }

        public enum KeySort
        {
            ASC,
            DESC,
        }
    }
}
