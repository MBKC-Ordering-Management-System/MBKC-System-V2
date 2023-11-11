using AutoMapper;
using MBKC.Service.Services.Interfaces;
using MBKC.Repository.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBKC.Service.DTOs.Wallets;
using System.Security.Claims;
using MBKC.Service.Constants;
using MBKC.Repository.Models;
using MBKC.Service.DTOs.Transactions;
using MBKC.Service.DTOs.MoneyExchanges;
using MBKC.Service.DTOs.ShipperPayments;
using MBKC.Service.Utils;

namespace MBKC.Service.Services.Implementations
{
    public class WalletService : IWalletService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public WalletService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        #region Get Wallet by kitchen center id, cashier id, store id
        public async Task<GetWalletResponse> GetWallet(IEnumerable<Claim> claims)
        {
            // Get email, role, account id from claims
            Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
            Claim registeredRoleClaim = claims.First(x => x.Type.ToLower().Equals("role"));
            Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));

            var email = registeredEmailClaim.Value;
            var role = registeredRoleClaim.Value;
            KitchenCenter? kitchenCenter = null;
            StoreAccount? storeAccount = null;
            Cashier? cashier = null;

            // Check role when user login 
            if (registeredRoleClaim.Value.Equals(RoleConstant.Kitchen_Center_Manager))
            {
                kitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(email);
            }
            else if (registeredRoleClaim.Value.Equals(RoleConstant.Cashier))
            {
                cashier = await this._unitOfWork.CashierRepository.GetCashierAsync(int.Parse(accountId.Value));
            }
            else if (registeredRoleClaim.Value.Equals(RoleConstant.Store_Manager))
            {
                storeAccount = await this._unitOfWork.StoreAccountRepository.GetStoreAccountAsync(int.Parse(accountId.Value));
            }
            GetWalletResponse getWalletResponse = null;
            if (cashier != null)
            {
                getWalletResponse = new GetWalletResponse()
                {
                    WalletId = kitchenCenter.WalletId,
                    Balance = kitchenCenter.Wallet.Balance,
                    TotalDailyMoneyExchange = kitchenCenter.KitchenCenterMoneyExchanges.Select(x => x.MoneyExchange.Amount).Sum(),
                    TotalDailyShipperPayment = kitchenCenter.BankingAccounts.Select(x => x.ShipperPayments.Select(x => x.Amount).Sum());
                };
            }

            return getWalletResponse;
        }
    }
}


#endregion
