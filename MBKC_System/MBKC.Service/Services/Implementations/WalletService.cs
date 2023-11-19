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
using MBKC.Repository.Enums;

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
        public async Task<GetWalletResponse> GetWallet(GetSearchDateWalletRequest searchDateWallet, IEnumerable<Claim> claims)
        {
            try
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
                List<ShipperPayment> shipperPaymentOfCashier = null;
                List<ShipperPayment> shipperPaymentOfKitchenCenter = new List<ShipperPayment>();
                // Check role when user login 
                if (registeredRoleClaim.Value.Equals(RoleConstant.Kitchen_Center_Manager))
                {
                    kitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterWalletAsync(email);
                    foreach (var c in kitchenCenter.Cashiers)
                    {
                        var shipperPayments = await _unitOfWork.ShipperPaymentRepository.GetShiperPaymentsByCashierIdAsync(c.AccountId);
                        shipperPaymentOfKitchenCenter.AddRange(shipperPayments);
                    }
                }
                else if (registeredRoleClaim.Value.Equals(RoleConstant.Cashier))
                {
                    cashier = await this._unitOfWork.CashierRepository.GetCashierWalletAsync(int.Parse(accountId.Value));
                    shipperPaymentOfCashier = await _unitOfWork.ShipperPaymentRepository.GetShiperPaymentsByCashierIdAsync(cashier.AccountId);
                }
                else if (registeredRoleClaim.Value.Equals(RoleConstant.Store_Manager))
                {
                    storeAccount = await this._unitOfWork.StoreAccountRepository.GetStoreAccountWalletAsync(int.Parse(accountId.Value));
                }
                GetWalletResponse getWalletResponse = null;
                DateTime currentDate = DateTime.Now.Date;
                if (cashier != null)
                {
                    DateTime searchDate = DateTime.Now.Date;
                    if (searchDateWallet.SearchDate != null)
                    {
                        searchDate = DateTime.ParseExact(searchDateWallet.SearchDate, "dd/MM/yyyy", null);
                    }
                    getWalletResponse = new GetWalletResponse()
                    {
                        WalletId = cashier.WalletId,
                        Balance = cashier.Wallet.Balance,
                        TotalDailyMoneyExchange = cashier.CashierMoneyExchanges.Where(x => x.MoneyExchange.Transactions.Any(x => x.TransactionTime.Date == searchDate.Date)).Select(x => x.MoneyExchange.Amount).Sum(),
                        TotalDailyShipperPayment = shipperPaymentOfCashier.Where(x => x.CreateDate.Date == searchDate.Date).Select(x => x.Amount).Sum()
                    };
                }

                if (storeAccount != null)
                {

                    getWalletResponse = new GetWalletResponse()
                    {
                        WalletId = storeAccount.Store.Wallet.WalletId,
                        Balance = storeAccount.Store.Wallet.Balance,
                        TotalRevenueDaily = storeAccount.Store.Orders.SelectMany(x => x.ShipperPayments).Where(x => x.CreateDate.Date == currentDate).Select(x => x.Amount).Sum(),
                        ToTalOrderDaily = storeAccount.Store.Orders.Where(order => order.OrderHistories.Any(history =>
                                                                       history.SystemStatus.Equals(OrderEnum.SystemStatus.COMPLETED.ToString()) &&
                                                                       history.PartnerOrderStatus.Equals(OrderEnum.Status.COMPLETED.ToString()) &&
                                                                       history.CreatedDate.Date == currentDate)).Count()
                    };
                }

                if (kitchenCenter != null)
                {
                    DateTime searchDate = DateTime.Now.Date;
                    if (searchDateWallet.SearchDate != null)
                    {
                        searchDate = DateTime.ParseExact(searchDateWallet.SearchDate, "dd/MM/yyyy", null);
                    }
                    getWalletResponse = new GetWalletResponse()
                    {
                        WalletId = kitchenCenter.WalletId,
                        Balance = kitchenCenter.Wallet.Balance,
                        TotalDailySend = kitchenCenter.KitchenCenterMoneyExchanges
                                                      .Where(x => x.MoneyExchange.Transactions.Any(x => x.TransactionTime.Date == searchDate.Date)
                                                                                                  && x.MoneyExchange.ExchangeType.Equals(MoneyExchangeEnum.ExchangeType.SEND.ToString()))
                                                                                                  .Select(x => x.MoneyExchange.Amount).Sum(),
                        TotalDailyReceive = kitchenCenter.KitchenCenterMoneyExchanges
                                                         .Where(x => x.MoneyExchange.Transactions.Any(x => x.TransactionTime.Date == searchDate.Date)
                                                                                                      && x.MoneyExchange.ExchangeType.Equals(MoneyExchangeEnum.ExchangeType.RECEIVE.ToString()))
                                                                                                     .Select(x => x.MoneyExchange.Amount).Sum(),
                        TotalDailyMoneyExchange = kitchenCenter.KitchenCenterMoneyExchanges.Where(x => x.MoneyExchange.Transactions.Any(x => x.TransactionTime.Date == currentDate)).Select(x => x.MoneyExchange.Amount).Sum(),
                        TotalDailyShipperPayment = shipperPaymentOfKitchenCenter.Where(x => x.CreateDate.Date == currentDate).Select(x => x.Amount).Sum()
                    };
                }

                return getWalletResponse;
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }

        }
    }
}


#endregion
