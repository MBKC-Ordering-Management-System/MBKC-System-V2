using MBKC.Service.DTOs.MoneyExchanges;
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
        public Task UpdateCronAsync(UpdateCronJobRequest updateCronJobRequest);
        public void StartAllBackgroundJob();
    }
}
