using MBKC.API.Validators.Cashiers;
using MBKC.Service.DTOs.Cashiers;
using System.Security.Claims;

namespace MBKC.Service.Services.Interfaces
{
    public interface ICashierService
    {
        public Task<GetCashiersResponse> GetCashiersAsync(GetCashiersRequest getCashiersRequest, IEnumerable<Claim> claims);
        public Task CreateCashierAsync(CreateCashierRequest createCashierRequest, IEnumerable<Claim> claims);
    }
}
