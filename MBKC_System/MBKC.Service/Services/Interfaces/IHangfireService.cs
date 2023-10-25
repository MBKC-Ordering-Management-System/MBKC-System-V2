using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Services.Interfaces
{
    public interface IHangfireService
    {
        public string GetCronByKey();
        public void UpdateCronAsync(string key, int hour, int minute);
        public void StartAllBackgroundJob();
        public Task MoneyExchangeToStoreAsync();
        public Task MoneyExchangeKitchenCentersync();
    }
}
