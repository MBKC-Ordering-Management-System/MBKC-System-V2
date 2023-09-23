using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.Enums
{
    public class CategoryEnum
    {
        public enum Status
        {
            INACTIVE = 0,
            ACTIVE = 1,
            DEACTIVE = 2,
        }

        public enum Type
        {
            NORMAL,
            EXTRA 
        }
    }
}
