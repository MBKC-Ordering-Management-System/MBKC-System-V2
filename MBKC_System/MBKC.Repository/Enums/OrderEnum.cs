﻿using System;
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
            PREPARING = 0,
            READY = 1,
            UPCOMING = 2,
            COMPLETED = 3,
            CANCELLED = 4,
        }
    }
}
