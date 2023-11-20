using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Enums
{
    public class ProductEnum
    {
        public enum Status
        {
            INACTIVE = 0,
            ACTIVE = 1,
            DISABLE = 2
        }

        public enum Type
        {
            SINGLE,
            PARENT,
            CHILD,
            EXTRA
        }

        public enum Size
        {
            S,
            M,
            L
        }
    }
}
