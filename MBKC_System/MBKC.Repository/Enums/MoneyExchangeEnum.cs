using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Enums
{
    public class MoneyExchangeEnum
    {
        public enum ExchangeType
        {
            SEND,
            RECEIVE,
            WITHDRAW,
        }

        public enum Status
        {
            FAIL = 0,
            SUCCESS = 1,
        }
    }
}
