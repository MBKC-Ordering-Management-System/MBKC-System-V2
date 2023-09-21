using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Utils
{
    public static class RandomNumberUtil
    {
        public static int GenerateEightDigitNumber()
        {
            Random random = new Random();
            int eightDigitNumber = random.Next(10000000, 99999999);
            return eightDigitNumber;
        }
    }
}
