using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Constants
{
    public static class HangfireConstant
    {
        // job id
        public const string MoneyExchangeToStore_ID = "job_money_exchange_to_store";
        public const string MoneyExchangeToKitchenCenter_ID = "job_money_exchange_to_kitchen_center";

        // cron expression
        public const string Default_MoneyExchangeToKitchenCenter_CronExpression = "* 22 * * *";
        public const string Default_MoneyExchangeToStore_CronExpression = "* 23 * * *";
        
    }
}
