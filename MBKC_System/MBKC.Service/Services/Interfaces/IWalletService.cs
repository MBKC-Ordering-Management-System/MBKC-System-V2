using MBKC.Service.DTOs.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Services.Interfaces
{
    public interface IWalletService
    {
        public Task<GetWalletResponse> GetWallet(GetSearchDateWalletRequest searchDateWallet,IEnumerable<Claim> claims);
    }
}
