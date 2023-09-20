using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Utils
{
    public static class RandomNumberUtil
    {
       public static int GenerateSixDigitNumber()
        {
            Random random = new Random();
            int sixDigitNumber = random.Next(100000, 1000000);
            return sixDigitNumber;
        }
    }
}
