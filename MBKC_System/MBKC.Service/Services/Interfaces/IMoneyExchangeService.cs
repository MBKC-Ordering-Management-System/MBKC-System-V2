using MBKC.Service.DTOs.MoneyExchanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Services.Interfaces
{
    public interface IMoneyExchangeService
    {
        public Task MoneyExchangeToKitchenCenterAsync(IEnumerable<Claim> claims);
        public Task WithdrawMoneyAsync(IEnumerable<Claim> claims, WithdrawMoneyRequest withdrawMoneyRequest);
    }
}
