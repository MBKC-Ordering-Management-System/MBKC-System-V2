using MBKC.Service.DTOs.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Services.Interfaces
{
    public interface IAccountService
    {
        public Task<bool> IsActiveAccountAsync(string email);
        public Task<GetAccountResponse> GetAccountAsync(int idAccount, IEnumerable<Claim> claims);
        public Task UpdateAccountAsync(int idAccount, UpdateAccountRequest updateAccountRequest, IEnumerable<Claim> claims);
    }
}
