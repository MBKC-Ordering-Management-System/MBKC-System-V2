namespace MBKC.API.Constants
{
    public static class HangfireConstant
    {
        public const string DatabaseName = "hangfire.db";
        public const string DashboardEndpoint = "/hangfire";

        // job id
        public const string MoneyExchangeToStore_ID = "money_exchange_to_store";
        public const string MoneyExchangeToKitchenCenter_ID = "money_exchange_to_kitchen_center";

        // cron expression
        public const string MoneyExchangeToStore_CronExpression = "* 23 * * *";
        public const string MoneyExchangeToKitchenCenter_CronExpression = "* 22 * * *";

    }
}
