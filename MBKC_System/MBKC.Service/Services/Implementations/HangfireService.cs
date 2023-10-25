using AutoMapper;
using Hangfire;
using MBKC.Repository.Enums;
using MBKC.Repository.Infrastructures;
using MBKC.Repository.Models;
using MBKC.Repository.SMTPs.Models;
using MBKC.Service.Constants;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MBKC.Service.Constants.EmailMessageConstant;

namespace MBKC.Service.Services.Implementations
{
    public class HangfireService : IHangfireService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public HangfireService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }   

        #region get cron by key
        public string GetCronByKey()
        {
            try
            {
                return this._unitOfWork.HangfireRepository.GetCronByKey(HangfireConstant.MoneyExchangeToStore_ID) ?? "not existed.";
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region update cron by key and field
        public void UpdateCronAsync(string key, int hour, int minute)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region start all background job
        public void StartAllBackgroundJob()
        {
            // cashier money exchange to kitchen center
            RecurringJob.AddOrUpdate(HangfireConstant.MoneyExchangeToKitchenCenter_ID,
                                  () => MoneyExchangeKitchenCentersync(),
                                  cronExpression: this._unitOfWork.HangfireRepository
                                 .GetCronByKey(HangfireConstant.MoneyExchangeToKitchenCenter_ID) ?? HangfireConstant.Default_MoneyExchangeToKitchenCenter_CronExpression,
                                  new RecurringJobOptions
                                  {
                                      // sync time(utc +7)
                                      TimeZone = TimeZoneInfo.Local,
                                  });

            // kitchen center money exchange to store
            RecurringJob.AddOrUpdate(HangfireConstant.MoneyExchangeToStore_ID,
                                  () => MoneyExchangeToStoreAsync(),
                                  cronExpression: this._unitOfWork.HangfireRepository
                                 .GetCronByKey(HangfireConstant.MoneyExchangeToStore_ID) ?? HangfireConstant.Default_MoneyExchangeToStore_CronExpression,
                                  new RecurringJobOptions
                                  {
                                      // sync time(utc +7)
                                      TimeZone = TimeZoneInfo.Local,
                                  });
        }
        #endregion

        #region money exchange to kitchen center
        public async Task MoneyExchangeKitchenCentersync()
        {
            try
            {
                var cashiers = await this._unitOfWork.CashierRepository.GetCashiersAsync();
                if (!cashiers.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(MessageConstant.CashierMessage.NoOneActive);
                    return;
                }

                if (cashiers.FirstOrDefault(c => c.CashierMoneyExchanges.Any()) != null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(MessageConstant.MoneyExchangeMessage.AlreadyTransferredToKitchenCenter);
                    return;
                }

                #region transfer money to kitchen center
                List<Wallet> wallets = new List<Wallet>();
                List<CashierMoneyExchange> cashierMoneyExchanges = new List<CashierMoneyExchange>();
                List<KitchenCenterMoneyExchange> kitchenCenterMoneyExchanges = new List<KitchenCenterMoneyExchange>();
                foreach (var cashier in cashiers)
                {

                    // create cashier money exchange (sender)
                    CashierMoneyExchange cashierMoneyExchange = new CashierMoneyExchange()
                    {
                        Cashier = cashier,
                        MoneyExchange = new MoneyExchange()
                        {
                            Amount = cashier.Wallet.Balance,
                            ExchangeType = MoneyExchangeEnum.ExchangeType.SEND.ToString(),
                            Content = $"Transfer money to kitchen center[id:{cashier.KitchenCenter.KitchenCenterId} - name:{cashier.KitchenCenter.Name}] {StringUtil.GetContentAmountAndTime(cashier.Wallet.Balance)}",
                            Status = (int)MoneyExchangeEnum.Status.SUCCESS,
                            SenderId = cashier.AccountId,
                            ReceiveId = cashier.KitchenCenter.KitchenCenterId,
                            Transactions = new List<Transaction>()
                            {
                                new Transaction()
                                {
                                    TransactionTime = DateTime.Now,
                                    Wallet = cashier.Wallet,
                                    Status = (int)TransactionEnum.Status.SUCCESS,
                                },
                            }
                        }
                    };
                    cashierMoneyExchanges.Add(cashierMoneyExchange);

                    // create kitchen center money exchange (Receiver)
                    KitchenCenterMoneyExchange kitchenCenterMoneyExchange = new KitchenCenterMoneyExchange()
                    {
                        KitchenCenter = cashier.KitchenCenter,
                        MoneyExchange = new MoneyExchange()
                        {
                            Amount = cashier.Wallet.Balance,
                            ExchangeType = MoneyExchangeEnum.ExchangeType.RECEIVE.ToString(),
                            Content = $"Receive money from cashier[id:{cashier.AccountId} - name:{cashier.FullName}] {StringUtil.GetContentAmountAndTime(cashier.Wallet.Balance)}",
                            Status = (int)MoneyExchangeEnum.Status.SUCCESS,
                            SenderId = cashier.AccountId,
                            ReceiveId = cashier.KitchenCenter.KitchenCenterId,
                            Transactions = new List<Transaction>()
                            {
                                new Transaction()
                                {
                                    TransactionTime = DateTime.Now,
                                    Wallet = cashier.KitchenCenter.Wallet,
                                    Status = (int)TransactionEnum.Status.SUCCESS,
                                },
                            }
                        }
                    };
                    kitchenCenterMoneyExchanges.Add(kitchenCenterMoneyExchange);

                    // update balance of cashier and kitchen center wallet
                    cashier.KitchenCenter.Wallet.Balance += cashier.Wallet.Balance;
                    cashier.Wallet.Balance = 0;
                    wallets.Add(cashier.Wallet);

                    var existedWalletKitchenCenter = wallets.FirstOrDefault(w => w.WalletId == cashier.KitchenCenter.WalletId);
                    if (existedWalletKitchenCenter != null)
                    {
                        wallets[wallets.IndexOf(existedWalletKitchenCenter)].Balance += cashier.Wallet.Balance;
                    }
                    else
                    {
                        wallets.Add(cashier.Wallet);
                    }
                }
                #endregion

                await this._unitOfWork.CashierMoneyExchangeRepository.CreateRangeCashierMoneyExchangeAsync(cashierMoneyExchanges);
                await this._unitOfWork.KitchenCenterMoneyExchangeRepository.CreateRangeKitchenCenterMoneyExchangeAsync(kitchenCenterMoneyExchanges);
                this._unitOfWork.WalletRepository.UpdateRangeWallet(wallets);
                await this._unitOfWork.CommitAsync();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(MessageConstant.MoneyExchangeMessage.TransferToKitchenCenterSuccessfully);
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
                var kitchenCenters = await this._unitOfWork.KitchenCenterRepository.GetKitchenCentersIncludeOrderAsync();
                if (!kitchenCenters.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(MessageConstant.KitchenCenterMessage.NoOneActive);
                    return;
                }

                if (kitchenCenters.FirstOrDefault(f => f.KitchenCenterMoneyExchanges.Any()) != null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(MessageConstant.MoneyExchangeMessage.AlreadyTransferredToStore);
                    return;
                }

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
                        // creat kitchen center money exchange (sender)
                        KitchenCenterMoneyExchange kitchenCenterMoneyExchange = new KitchenCenterMoneyExchange()
                        {
                            KitchenCenter = kitchenCenter,
                            MoneyExchange = new MoneyExchange()
                            {
                                Amount = exchangeWalletValue,
                                ExchangeType = MoneyExchangeEnum.ExchangeType.SEND.ToString(),
                                Content = $"Transfer money to store[id:{store.StoreId} - name:{store.Name}] {StringUtil.GetContentAmountAndTime(exchangeWalletValue)}",
                                Status = (int)MoneyExchangeEnum.Status.SUCCESS,
                                SenderId = kitchenCenter.KitchenCenterId,
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
                            }
                        };
                        kitchenCenterMoneyExchanges.Add(kitchenCenterMoneyExchange);

                        // creat store money exchange (receiver)
                        StoreMoneyExchange storeMoneyExchange = new StoreMoneyExchange()
                        {
                            Store = store,
                            MoneyExchange = new MoneyExchange()
                            {
                                Amount = exchangeWalletValue,
                                ExchangeType = MoneyExchangeEnum.ExchangeType.RECEIVE.ToString(),
                                Content = $"Receiver money from kitchen center[id:{kitchenCenter.KitchenCenterId} - name:{kitchenCenter.Name}] {StringUtil.GetContentAmountAndTime(exchangeWalletValue)}",
                                Status = (int)MoneyExchangeEnum.Status.SUCCESS,
                                SenderId = kitchenCenter.KitchenCenterId,
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
                            }
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

                await this._unitOfWork.KitchenCenterMoneyExchangeRepository.CreateRangeKitchenCenterMoneyExchangeAsync(kitchenCenterMoneyExchanges);
                await this._unitOfWork.StoreMoneyExchangeRepository.CreateRangeStoreMoneyExchangeAsync(storeMoneyExchanges);
                this._unitOfWork.WalletRepository.UpdateRangeWallet(wallets);
                await this._unitOfWork.CommitAsync();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(MessageConstant.MoneyExchangeMessage.TransferToStoreSuccessfully);
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
