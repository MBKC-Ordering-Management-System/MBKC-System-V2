using MBKC.Service.DTOs.Accounts;
using MBKC.Service.DTOs.AccountTokens;
using MBKC.Service.DTOs.JWTs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Services.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<AccountResponse> LoginAsync(AccountRequest accountRequest, JWTAuth jwtAuth);
        public Task<AccountTokenResponse> ReGenerateTokensAsync(AccountTokenRequest accountTokenRequest, JWTAuth jwtAuth);
        public Task ChangePasswordAsync(ResetPasswordRequest resetPassword);
    }
}
