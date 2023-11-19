using MBKC.Service.DTOs.DashBoards;
using MBKC.Service.DTOs.DashBoards.Brand;
using MBKC.Service.DTOs.DashBoards.Cashier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Services.Interfaces
{
    public interface IDashBoardService
    {
        public Task<GetAdminDashBoardResponse> GetAdminDashBoardAsync();
        public Task<GetKitchenCenterDashBoardResponse> GetKitchenCenterDashBoardAsync(IEnumerable<Claim> claims);
        public Task<GetBrandDashBoardResponse> GetBrandDashBoardAsync(IEnumerable<Claim> claims, GetBrandDashBoardRequest getBrandDashBoardRequest);
        public Task<GetStoreDashBoardResponse> GetStoreDashBoardAsync(IEnumerable<Claim> claims);
        public Task<GetCashierDashBoardResponse> GetCashierDashBoardAsync(IEnumerable<Claim> claims, GetCashierDashBoardRequest getCashierDashBoardRequest);

    }
}
