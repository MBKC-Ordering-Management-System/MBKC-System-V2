namespace MBKC.API.Constants
{
    public static class HangfireConstant
    {
        public const string DatabaseName = "hangfire.db";
        public const string DashboardEndpoint = "/hangfire";

        // job id
        public const string MoneyExchangeToStore_ID = "money_exchange_to_store";

        // cron expression
        public const string MoneyExchangeToStore_CronExpression = "0 23 * * *";

    }
}
