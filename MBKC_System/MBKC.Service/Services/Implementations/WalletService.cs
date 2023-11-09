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
            List<GetTransactionWalletResponse> getTransactionWalletsResponse = new List<GetTransactionWalletResponse>();
            GetWalletResponse? getWalletResponse = null;
            GetMoneyExchangeResponse? getMoneyExchangeResponse = null;
            GetShipperPaymentResponse? getShipperPaymentWalletResponse = null;

            // Get wallet when user login with role kitchen center manager
            if (kitchenCenter != null)
            {
                foreach (var transaction in kitchenCenter.Wallet.Transactions)
                {
                    if (transaction.ShipperPayment != null)
                    {
                        getShipperPaymentWalletResponse = new GetShipperPaymentResponse()
                        {
                            PaymentId = transaction.ShipperPayment.PaymentId,
                            Amount = transaction.ShipperPayment.Amount,
                            Content = transaction.ShipperPayment.Content,
                            CreateDate = transaction.ShipperPayment.CreateDate,
                            KCBankingAccountId = transaction.ShipperPayment.KCBankingAccountId,
                            OrderId = transaction.ShipperPayment.OrderId,
                            PaymentMethod = transaction.ShipperPayment.PaymentMethod,
                            Status = StatusUtil.ChangeShipperPaymentStatus(transaction.ShipperPayment.Status),
                            KCBankingAccountName = transaction.ShipperPayment.BankingAccount.Name
                        };
                    }

                    if (transaction.MoneyExchange != null)
                    {
                        getMoneyExchangeResponse = new GetMoneyExchangeResponse()
                        {
                            ExchangeId = transaction.MoneyExchange.ExchangeId,
                            Amount = transaction.MoneyExchange.Amount,
                            Content = transaction.MoneyExchange.Content,
                            ExchangeImage = transaction.MoneyExchange.ExchangeImage,
                            ExchangeType = transaction.MoneyExchange.ExchangeType,
                            ReceiveId = transaction.MoneyExchange.ReceiveId,
                            SenderId = transaction.MoneyExchange.SenderId,
                            SenderName = kitchenCenter.Name,
                            Status = StatusUtil.ChangeMoneyExchangeStatus(transaction.Status),
                            ReceiveName = kitchenCenter.Stores
                        .Where(store => store.StoreId == transaction.MoneyExchange.ReceiveId)
                        .Select(store => store.Name)
                        .SingleOrDefault()
                        };
                    }

                    GetTransactionWalletResponse trans = new GetTransactionWalletResponse()
                    {
                        TracsactionId = transaction.TracsactionId,
                        Status = StatusUtil.ChangeTransactionStatus(transaction.Status),
                        TransactionTime = transaction.TransactionTime,
                        MoneyExchange = getMoneyExchangeResponse,
                        ShipperPayment = getShipperPaymentWalletResponse
                    };
                    getTransactionWalletsResponse.Add(trans);
                }
                getWalletResponse = new GetWalletResponse()
                {
                    WalletId = kitchenCenter.WalletId,
                    Balance = kitchenCenter.Wallet.Balance,
                    Transactions = getTransactionWalletsResponse
                };
            }
            // Get wallet when user login with role cashier
            if (cashier != null)
            {
                foreach (var transaction in cashier.Wallet.Transactions)
                {
                    if (transaction.ShipperPayment != null)
                    {
                        getShipperPaymentWalletResponse = new GetShipperPaymentResponse()
                        {
                            PaymentId = transaction.ShipperPayment.PaymentId,
                            Amount = transaction.ShipperPayment.Amount,
                            Content = transaction.ShipperPayment.Content,
                            CreateDate = transaction.ShipperPayment.CreateDate,
                            KCBankingAccountId = transaction.ShipperPayment.KCBankingAccountId,
                            OrderId = transaction.ShipperPayment.OrderId,
                            PaymentMethod = transaction.ShipperPayment.PaymentMethod,
                            Status = StatusUtil.ChangeShipperPaymentStatus(transaction.ShipperPayment.Status),
                            KCBankingAccountName = transaction.ShipperPayment.BankingAccount.Name
                        };
                    }

                    if (transaction.MoneyExchange != null)
                    {
                        getMoneyExchangeResponse = new GetMoneyExchangeResponse()
                        {
                            ExchangeId = transaction.MoneyExchange.ExchangeId,
                            Amount = transaction.MoneyExchange.Amount,
                            Content = transaction.MoneyExchange.Content,
                            ExchangeImage = transaction.MoneyExchange.ExchangeImage,
                            ExchangeType = transaction.MoneyExchange.ExchangeType,
                            ReceiveId = transaction.MoneyExchange.ReceiveId,
                            SenderId = transaction.MoneyExchange.SenderId,
                            SenderName = cashier.FullName,
                            ReceiveName = cashier.KitchenCenter.Name,
                            Status = StatusUtil.ChangeMoneyExchangeStatus(transaction.MoneyExchange.Status),
                        };
                    }
                    GetTransactionWalletResponse trans = new GetTransactionWalletResponse()
                    {
                        TracsactionId = transaction.TracsactionId,
                        Status = StatusUtil.ChangeTransactionStatus(transaction.Status),
                        TransactionTime = transaction.TransactionTime,
                        MoneyExchange = getMoneyExchangeResponse,
                        ShipperPayment = getShipperPaymentWalletResponse
                    };
                    getTransactionWalletsResponse.Add(trans);
                }
                getWalletResponse = new GetWalletResponse()
                {
                    WalletId = cashier.Wallet.WalletId,
                    Balance = cashier.Wallet.Balance,
                    Transactions = getTransactionWalletsResponse
                };
            }

            // Get wallet when user login with role store manager
            if (storeAccount != null)
            {
                foreach (var transaction in storeAccount.Store.Wallet.Transactions)
                {
                    if (transaction.ShipperPayment != null)
                    {
                        getShipperPaymentWalletResponse = new GetShipperPaymentResponse()
                        {
                            PaymentId = transaction.ShipperPayment.PaymentId,
                            Amount = transaction.ShipperPayment.Amount,
                            Content = transaction.ShipperPayment.Content,
                            CreateDate = transaction.ShipperPayment.CreateDate,
                            KCBankingAccountId = transaction.ShipperPayment.KCBankingAccountId,
                            OrderId = transaction.ShipperPayment.OrderId,
                            PaymentMethod = transaction.ShipperPayment.PaymentMethod,
                            Status = StatusUtil.ChangeShipperPaymentStatus(transaction.ShipperPayment.Status),
                            KCBankingAccountName = transaction.ShipperPayment.BankingAccount.Name
                        };
                    }

                    if (transaction.MoneyExchange != null)
                    {
                        getMoneyExchangeResponse = new GetMoneyExchangeResponse()
                        {
                            ExchangeId = transaction.MoneyExchange.ExchangeId,
                            Amount = transaction.MoneyExchange.Amount,
                            Status = StatusUtil.ChangeMoneyExchangeStatus(transaction.Status),
                            Content = transaction.MoneyExchange.Content,
                            ExchangeImage = transaction.MoneyExchange.ExchangeImage,
                            ExchangeType = transaction.MoneyExchange.ExchangeType,
                            ReceiveId = transaction.MoneyExchange.ReceiveId,
                            SenderId = transaction.MoneyExchange.SenderId,
                            SenderName = storeAccount.Store.KitchenCenter.Name,
                            ReceiveName = storeAccount.Store.Name
                        };
                    }

                    GetTransactionWalletResponse trans = new GetTransactionWalletResponse()
                    {
                        TracsactionId = transaction.TracsactionId,
                        Status = StatusUtil.ChangeTransactionStatus(transaction.Status),
                        TransactionTime = transaction.TransactionTime,
                        MoneyExchange = getMoneyExchangeResponse,
                        ShipperPayment = getShipperPaymentWalletResponse
                    };
                    getTransactionWalletsResponse.Add(trans);
                }
                getWalletResponse = new GetWalletResponse()
                {
                    WalletId = storeAccount.Store.Wallet.WalletId,
                    Balance = storeAccount.Store.Wallet.Balance,
                    Transactions = getTransactionWalletsResponse
                };
            }
            return getWalletResponse;
        }
        #endregion
    }
}
