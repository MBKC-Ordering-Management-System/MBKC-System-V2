using MBKC.Service.DTOs.Cashiers.Requests;
using MBKC.Service.DTOs.Cashiers.Responses;
using System.Security.Claims;

namespace MBKC.Service.Services.Interfaces
{
    public interface ICashierService
    {
        public Task<GetCashiersResponse> GetCashiersAsync(GetCashiersRequest getCashiersRequest, IEnumerable<Claim> claims);
        public Task<GetCashierReportResponse> GetCashierReportAsync(IEnumerable<Claim> claims);
        public Task CreateCashierAsync(CreateCashierRequest createCashierRequest, IEnumerable<Claim> claims);
        public Task<GetCashierResponse> GetCashierAsync(int idCashier, IEnumerable<Claim> claims);
        public Task UpdateCashierAsync(int idCashier, UpdateCashierRequest updateCashierRequest, IEnumerable<Claim> claims);
        public Task UpdateCashierStatusAsync(int idCashier, UpdateCashierStatusRequest updateCashierStatusRequest, IEnumerable<Claim> claims);
        public Task DeleteCashierAsync(int idCashier, IEnumerable<Claim> claims);
    }
}
