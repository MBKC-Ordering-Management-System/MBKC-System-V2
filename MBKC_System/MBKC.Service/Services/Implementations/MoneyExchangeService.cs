using AutoMapper;
using MBKC.Service.Services.Interfaces;
using MBKC.Repository.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBKC.Repository.Enums;
using MBKC.Repository.Models;
using MBKC.Service.Constants;
using MBKC.Service.DTOs.Orders;
using MBKC.Service.Exceptions;
using MBKC.Service.Utils;
using System.Security.Claims;
using MBKC.Repository.SMTPModels;

namespace MBKC.Service.Services.Implementations
{

    public class MoneyExchangeService : IMoneyExchangeService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public MoneyExchangeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        #region money exchange to kitchen center
        public async Task MoneyExchangeToKitchenCenterAsync(IEnumerable<Claim> claims)
        {
            try
            {
                #region validation
                string email = claims.First(x => x.Type == ClaimTypes.Email).Value;
                Cashier existedCashier = await this._unitOfWork.CashierRepository.GetCashierAsync(email);
                if (existedCashier.Wallet.Balance <= 0)
                {
                    throw new BadRequestException(MessageConstant.WalletMessage.BalanceIsInvalid);
                }
                #endregion

                #region operation

                #region create money exchange
                List<MoneyExchange> moneyExchanges = new List<MoneyExchange>();
                // create money exchange for cashier (sender)
                MoneyExchange moneyExchangeCashier = new MoneyExchange()
                {
                    Amount = existedCashier.Wallet.Balance,
                    ExchangeType = MoneyExchangeEnum.ExchangeType.SEND.ToString(),
                    Content = $"Transfer money to kitchen center[id:{existedCashier.KitchenCenter.KitchenCenterId} - name:{existedCashier.KitchenCenter.Name}] {StringUtil.GetContentAmountAndTime(existedCashier.Wallet.Balance, DateTime.Now)}",
                    Status = (int)MoneyExchangeEnum.Status.SUCCESS,
                    SenderId = existedCashier.AccountId,
                    ReceiveId = existedCashier.KitchenCenter.Manager.AccountId,
                    Transactions = new List<Transaction>()
                    {
                        new Transaction()
                        {
                            TransactionTime = DateTime.Now,
                            Wallet = existedCashier.Wallet,
                            Status = (int)TransactionEnum.Status.SUCCESS,
                        },
                    }
                };
                moneyExchanges.Add(moneyExchangeCashier);

                // create money exchange for kitchen center (receiver)
                MoneyExchange moneyExchangeKitchenCenter = new MoneyExchange()
                {
                    Amount = existedCashier.Wallet.Balance,
                    ExchangeType = MoneyExchangeEnum.ExchangeType.RECEIVE.ToString(),
                    Content = $"Receive money from cashier[id:{existedCashier.AccountId} - name:{existedCashier.FullName}] {StringUtil.GetContentAmountAndTime(existedCashier.Wallet.Balance, DateTime.Now)}",
                    Status = (int)MoneyExchangeEnum.Status.SUCCESS,
                    SenderId = existedCashier.AccountId,
                    ReceiveId = existedCashier.KitchenCenter.Manager.AccountId,
                    Transactions = new List<Transaction>()
                    {
                        new Transaction()
                        {
                            TransactionTime = DateTime.Now,
                            Wallet = existedCashier.KitchenCenter.Wallet,
                            Status = (int)TransactionEnum.Status.SUCCESS,
                        },
                    }
                };
                moneyExchanges.Add(moneyExchangeKitchenCenter);
                await this._unitOfWork.MoneyExchangeRepository.CreateRangeMoneyExchangeAsync(moneyExchanges);
                #endregion

                #region create cashier exchange and kitchen center exchange
                // cashier exchange
                CashierMoneyExchange cashierMoneyExchange = new CashierMoneyExchange()
                {
                    Cashier = existedCashier,
                    MoneyExchange = moneyExchangeCashier,
                };
                await this._unitOfWork.CashierMoneyExchangeRepository.CreateCashierMoneyExchangeAsync(cashierMoneyExchange);

                // kitchen center exchange
                KitchenCenterMoneyExchange kitchenCenterMoneyExchange = new KitchenCenterMoneyExchange()
                {
                    KitchenCenter = existedCashier.KitchenCenter,
                    MoneyExchange = moneyExchangeKitchenCenter,
                };
                await this._unitOfWork.KitchenCenterMoneyExchangeRepository.CreateKitchenCenterMoneyExchangeAsync(kitchenCenterMoneyExchange);
                #endregion

                #region update balance of cashier and kitchen center wallet
                List<Wallet> wallets = new List<Wallet>();
                existedCashier.KitchenCenter.Wallet.Balance += existedCashier.Wallet.Balance;
                existedCashier.Wallet.Balance = 0;
                wallets.Add(existedCashier.KitchenCenter.Wallet);
                wallets.Add(existedCashier.Wallet);
                this._unitOfWork.WalletRepository.UpdateRangeWallet(wallets);
                #endregion

                await this._unitOfWork.CommitAsync();
                #endregion
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                switch (ex.Message)
                {
                    case MessageConstant.WalletMessage.BalanceIsInvalid:
                        fieldName = "Wallet balance";
                        break;

                    default:
                        fieldName = "Exception";
                        break;
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region money exchange to store
        public async Task MoneyExchangeToStoreAsync()
        {
            try
            {
                List<KitchenCenter> kitchenCenters = await this._unitOfWork.KitchenCenterRepository.GetKitchenCentersIncludeOrderAsync();

                #region count money must transfer for each store  
                Dictionary<int, decimal> exchangeWallets = new Dictionary<int, decimal>();
                foreach (var kitchenCenter in kitchenCenters)
                {
                    if (!kitchenCenter.Stores.Any())
                    {
                        continue;
                    }

                    foreach (var store in kitchenCenter.Stores)
                    {

                        if (!store.Orders.Any())
                        {
                            continue;
                        }

                        foreach (var order in store.Orders)
                        {
                            if (exchangeWallets.ContainsKey(store.StoreId))
                            {
                                exchangeWallets[store.StoreId] += order.FinalTotalPrice - (order.FinalTotalPrice * order.Commission / 100);
                            }
                            else
                            {
                                exchangeWallets.Add(store.StoreId, order.FinalTotalPrice - (order.FinalTotalPrice * order.Commission / 100));
                            }
                        }
                    }
                }

                if (!exchangeWallets.Any())
                {
                    return;
                }
                #endregion

                #region transfer money to each store wallet
                List<MoneyExchange> moneyExchanges = new List<MoneyExchange>();
                List<KitchenCenterMoneyExchange> kitchenCenterMoneyExchanges = new List<KitchenCenterMoneyExchange>();
                List<StoreMoneyExchange> storeMoneyExchanges = new List<StoreMoneyExchange>();
                List<Wallet> wallets = new List<Wallet>();

                foreach (var kitchenCenter in kitchenCenters)
                {
                    if (!kitchenCenter.Stores.Any())
                    {
                        continue;
                    }

                    foreach (var store in kitchenCenter.Stores)
                    {
                        if (!store.Orders.Any())
                        {
                            continue;
                        }

                        var exchangeWalletValue = exchangeWallets[store.StoreId];

                        #region create money exchange
                        // create money exchange for kitchen center (sender)
                        MoneyExchange moneyExchangeKitchenCenter = new MoneyExchange()
                        {
                            Amount = exchangeWalletValue,
                            ExchangeType = MoneyExchangeEnum.ExchangeType.SEND.ToString(),
                            Content = $"Transfer money to store[id:{store.StoreId} - name:{store.Name}] {StringUtil.GetContentAmountAndTime(exchangeWalletValue, DateTime.Now)}",
                            Status = (int)MoneyExchangeEnum.Status.SUCCESS,
                            SenderId = kitchenCenter.Manager.AccountId,
                            ReceiveId = store.StoreId,
                            Transactions = new List<Transaction>()
                                {
                                    new Transaction()
                                    {
                                        TransactionTime = DateTime.Now,
                                        Wallet = kitchenCenter.Wallet,
                                        Status = (int)TransactionEnum.Status.SUCCESS,
                                    },
                                }
                        };
                        moneyExchanges.Add(moneyExchangeKitchenCenter);

                        // create money exchange for store (receiver)
                        MoneyExchange moneyExchangeStore = new MoneyExchange()
                        {
                            Amount = exchangeWalletValue,
                            ExchangeType = MoneyExchangeEnum.ExchangeType.RECEIVE.ToString(),
                            Content = $"Receiver money from kitchen center[id:{kitchenCenter.KitchenCenterId} - name:{kitchenCenter.Name}] {StringUtil.GetContentAmountAndTime(exchangeWalletValue, DateTime.Now)}",
                            Status = (int)MoneyExchangeEnum.Status.SUCCESS,
                            SenderId = kitchenCenter.Manager.AccountId,
                            ReceiveId = store.StoreId,
                            Transactions = new List<Transaction>()
                                {
                                    new Transaction()
                                    {
                                        TransactionTime = DateTime.Now,
                                        Wallet = store.Wallet,
                                        Status = (int)TransactionEnum.Status.SUCCESS,
                                    },
                                }
                        };
                        moneyExchanges.Add(moneyExchangeStore);
                        #endregion

                        #region create kitchen center exchange and store exchange
                        // kitchen center exchange
                        KitchenCenterMoneyExchange kitchenCenterMoneyExchange = new KitchenCenterMoneyExchange()
                        {
                            KitchenCenter = kitchenCenter,
                            MoneyExchange = moneyExchangeKitchenCenter,
                        };
                        kitchenCenterMoneyExchanges.Add(kitchenCenterMoneyExchange);

                        // store exchange
                        StoreMoneyExchange storeMoneyExchange = new StoreMoneyExchange()
                        {
                            Store = store,
                            MoneyExchange = moneyExchangeStore,
                        };
                        storeMoneyExchanges.Add(storeMoneyExchange);
                        #endregion

                        #region update balance of kitchen center and store wallet
                        store.Wallet.Balance += exchangeWalletValue;
                        kitchenCenter.Wallet.Balance -= exchangeWalletValue;
                        wallets.Add(store.Wallet);
                        #endregion
                    }

                    wallets.Add(kitchenCenter.Wallet);
                }

                #endregion

                await this._unitOfWork.MoneyExchangeRepository.CreateRangeMoneyExchangeAsync(moneyExchanges);
                await this._unitOfWork.KitchenCenterMoneyExchangeRepository.CreateRangeKitchenCenterMoneyExchangeAsync(kitchenCenterMoneyExchanges);
                await this._unitOfWork.StoreMoneyExchangeRepository.CreateRangeStoreMoneyExchangeAsync(storeMoneyExchanges);
                this._unitOfWork.WalletRepository.UpdateRangeWallet(wallets);
                await this._unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                throw new Exception(error);
            }
        }
        #endregion
    }
}

