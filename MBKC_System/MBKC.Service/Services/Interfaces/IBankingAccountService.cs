using MBKC.Service.DTOs.BankingAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Services.Interfaces
{
    public interface IBankingAccountService
    {
        public Task<GetBankingAccountsResponse> GetBankingAccountsAsync(GetBankingAccountsRequest getBankingAccountsRequest, IEnumerable<Claim> claims);
        public Task<GetBankingAccountResponse> GetBankingAccountAsync(int bankingAccountId, IEnumerable<Claim> claims);
        public Task CreateBankingAccountAsync(CreateBankingAccountRequest createBankingAccountRequest, IEnumerable<Claim> claims);
        public Task DeleteBankingAccountAsync(int bankingAccountId, IEnumerable<Claim> claims);
        public Task UpdateBankingAccountStatusAsync(int bankingAccountId, UpdateBankingAccountStatusRequest updateBankingAccountStatusRequest, IEnumerable<Claim> claims);
        public Task UpdateBankingAccountAsync(int bankingAccountId, UpdateBankingAccountRequest updateBankingAccountRequest, IEnumerable<Claim> claims);
    }
}
