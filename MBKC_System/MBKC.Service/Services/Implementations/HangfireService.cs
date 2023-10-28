using AutoMapper;
using Hangfire;
using Hangfire.Server;
using MBKC.Repository.Enums;
using MBKC.Repository.Infrastructures;
using MBKC.Repository.Models;
using MBKC.Repository.SMTPs.Models;
using MBKC.Service.Constants;
using MBKC.Service.DTOs.MoneyExchanges;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using Microsoft.Extensions.Logging;
using Redis.OM.Searching.Query;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static MBKC.Service.Constants.EmailMessageConstant;

namespace MBKC.Service.Services.Implementations
{
    public class HangfireService : IHangfireService
    {
        private UnitOfWork _unitOfWork;
        private readonly ILogger<HangfireService> _logger;

        public HangfireService(IUnitOfWork unitOfWork, ILogger<HangfireService> logger)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._logger = logger;
        }

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

        #region update cron by key
        public async Task UpdateCronAsync(UpdateCronJobRequest updateCronJobRequest)
        {
            try
            {
                #region validation
                var existedCron = await Task.FromResult(this._unitOfWork.HangfireRepository.GetCronByKey(updateCronJobRequest.JobId));
                if (existedCron == null)
                {
                    throw new NotFoundException(MessageConstant.MoneyExchangeMessage.NotExistJobId);
                }

                // check time job money exchange to kitchen center
                if (updateCronJobRequest.JobId.Equals(HangfireConstant.MoneyExchangeToKitchenCenter_ID))
                {
                    DateTime timeUpdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, updateCronJobRequest.hour, updateCronJobRequest.minute, 0);
                    DateTime timeExistedToStore = StringUtil.ConvertCronToTime(this._unitOfWork.HangfireRepository.GetCronByKey(HangfireConstant.MoneyExchangeToStore_ID)!);
                    if (DateUtil.IsTimeUpdateValid(timeExistedToStore, timeUpdate, 1) == false)
                    {
                        throw new NotFoundException(MessageConstant.MoneyExchangeMessage.TimeKitchenCenterMustEarlier);
                    }
                }

                // check time job money exchange to store
                if (updateCronJobRequest.JobId.Equals(HangfireConstant.MoneyExchangeToStore_ID))
                {
                    DateTime timeUpdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, updateCronJobRequest.hour, updateCronJobRequest.minute, 0);
                    DateTime timeExistedToKitchenCenter = StringUtil.ConvertCronToTime(this._unitOfWork.HangfireRepository.GetCronByKey(HangfireConstant.MoneyExchangeToKitchenCenter_ID)!);
                    if (DateUtil.IsTimeUpdateValid(timeUpdate, timeExistedToKitchenCenter, 1) == false)
                    {
                        throw new NotFoundException(MessageConstant.MoneyExchangeMessage.TimeStoreMustLater);
                    }
                }
                #endregion

                #region operation
                Expression<Func<Task>> methodCall = () => Task.CompletedTask;
                switch (updateCronJobRequest.JobId)
                {
                    case HangfireConstant.MoneyExchangeToKitchenCenter_ID:
                        methodCall = () => MoneyExchangeKitchenCentersync();
                        break;

                    case HangfireConstant.MoneyExchangeToStore_ID:
                        methodCall = () => MoneyExchangeToStoreAsync();
                        break;
                }

                RecurringJob.AddOrUpdate(updateCronJobRequest.JobId,
                                methodCall: methodCall,
                                cronExpression: StringUtil.ConvertTimeToCron(updateCronJobRequest.hour, updateCronJobRequest.minute),
                                new RecurringJobOptions
                                {
                                    // sync time(utc +7)
                                    TimeZone = TimeZoneInfo.Local,
                                });
                #endregion

            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                switch (ex.Message)
                {
                    case MessageConstant.MoneyExchangeMessage.NotExistJobId:
                        fieldName = "Job id";
                        break;

                    default:
                        fieldName = "Exception";
                        break;
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                switch (ex.Message)
                {
                    case MessageConstant.MoneyExchangeMessage.TimeKitchenCenterMustEarlier:
                    case MessageConstant.MoneyExchangeMessage.TimeStoreMustLater:
                        fieldName = "Scheduling time";
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

        #region money exchange to kitchen center
        public async Task MoneyExchangeKitchenCentersync()
        {
            try
            {
                var cashiers = await this._unitOfWork.CashierRepository.GetCashiersAsync();
                if (!cashiers.Any())
                {
                    this._logger.LogWarning($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second} - " + MessageConstant.CashierMessage.NoOneActive);
                    return;
                }

                if (cashiers.FirstOrDefault(c => c.CashierMoneyExchanges.Any()) != null)
                {
                    this._logger.LogWarning($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second} - " + MessageConstant.MoneyExchangeMessage.AlreadyTransferredToKitchenCenter);
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

                this._logger.LogInformation($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second} - " + MessageConstant.MoneyExchangeMessage.TransferToKitchenCenterSuccessfully);
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
                    this._logger.LogWarning($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second} - " + MessageConstant.KitchenCenterMessage.NoOneActive);
                    return;
                }

                if (kitchenCenters.FirstOrDefault(f => f.KitchenCenterMoneyExchanges.Any()) != null)
                {
                    this._logger.LogWarning($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second} - " + MessageConstant.MoneyExchangeMessage.AlreadyTransferredToStore);
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
                            decimal finalToTalPriceSubstractDeliveryFee = order.FinalTotalPrice - order.DeliveryFee;
                            if (exchangeWallets.ContainsKey(store.StoreId))
                            {
                                exchangeWallets[store.StoreId] += finalToTalPriceSubstractDeliveryFee - (finalToTalPriceSubstractDeliveryFee * order.Commission / 100);
                            }
                            else
                            {
                                exchangeWallets.Add(store.StoreId, finalToTalPriceSubstractDeliveryFee - (finalToTalPriceSubstractDeliveryFee * order.Commission / 100));
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

                this._logger.LogInformation($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second} - " + MessageConstant.MoneyExchangeMessage.TransferToStoreSuccessfully);
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
