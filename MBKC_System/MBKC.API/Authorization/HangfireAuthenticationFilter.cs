using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace MBKC.API.Authorization
{
    public class HangfireAuthenticationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
        }
    }
}
