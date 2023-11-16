using AutoMapper;
using Hangfire;
using MBKC.Repository.Enums;
using MBKC.Repository.Infrastructures;
using MBKC.Repository.Models;
using MBKC.Service.Constants;
using MBKC.Service.DTOs.Configurations;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Services.Implementations
{
    public class ConfigurationService : IConfigurationService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<ConfigurationService> _logger;
        public ConfigurationService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ConfigurationService> logger)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
        }

        #region start background job
        public async Task StartAllBackgroundJob()
        {
            
            var configs = await this._unitOfWork.ConfigurationRepository.GetConfigurationsAsync();
            if (!configs.Any())
            {
                this._logger.LogWarning($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second} - " + MessageConstant.MoneyExchangeMessage.ConfigDoesNotExist);
                return;
            }
            else
            {
                // cashier money exchange to kitchen center
                RecurringJob.AddOrUpdate(HangfireConstant.MoneyExchangeToKitchenCenter_ID,
                                      () => MoneyExchangeToKitchenCenterAsync(),
                                      cronExpression: StringUtil.ConvertTimeSpanToCron(configs.First().ScrawlingMoneyExchangeToKitchenCenter),
                                      new RecurringJobOptions
                                      {
                                          // sync time(utc +7)
                                          TimeZone = TimeZoneInfo.Local,
                                      });

                // kitchen center money exchange to store
                RecurringJob.AddOrUpdate(HangfireConstant.MoneyExchangeToStore_ID,
                                      () => MoneyExchangeToStoreAsync(),
                                      cronExpression: StringUtil.ConvertTimeSpanToCron(configs.First().ScrawlingMoneyExchangeToStore),
                                      new RecurringJobOptions
                                      {
                                          // sync time(utc +7)
                                          TimeZone = TimeZoneInfo.Local,
                                      });

                // update status for partner product
                RecurringJob.AddOrUpdate(HangfireConstant.UpdateStatusPartnerProduct_ID,
                                      () => ChangeStatusPartnerProduct(),
                                      cronExpression: "* 0 * * *",
                                      new RecurringJobOptions
                                      {
                                          // sync time(utc +7)
                                          TimeZone = TimeZoneInfo.Local,
                                      });
            }
        }
        #endregion

        #region get config
        public async Task<List<GetConfigurationResponse>> GetConfigurationsAsync()
        {
            try
            {
                List<Configuration> configurations = await this._unitOfWork.ConfigurationRepository.GetConfigurationsAsync();
                return this._mapper.Map<List<GetConfigurationResponse>>(configurations);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region update config
        public async Task UpdateConfigurationAsync(PutConfigurationRequest putConfigurationRequest)
        {
            TimeSpan? oldTimeExchangeToKitchenCenter = null;
            TimeSpan? oldTimeExchangeToStore = null;
            try
            {
                List<Configuration> configurations = await this._unitOfWork.ConfigurationRepository.GetConfigurationsAsync();
                Configuration firstConfiguration = configurations.First();
                bool isChanged = false;
                TimeSpan startTime;
                TimeSpan endTime;
                TimeSpan exchangeToKitchenCenterTime;
                TimeSpan exchangeToStoreTime;

                TimeSpan.TryParse(putConfigurationRequest.ScrawlingOrderStartTime, out startTime);
                TimeSpan.TryParse(putConfigurationRequest.ScrawlingOrderEndTime, out endTime);
                TimeSpan.TryParse(putConfigurationRequest.ScrawlingMoneyExchangeToKitchenCenter, out exchangeToKitchenCenterTime);
                TimeSpan.TryParse(putConfigurationRequest.ScrawlingMoneyExchangeToStore, out exchangeToStoreTime);

                if (TimeSpan.Compare(firstConfiguration.ScrawlingOrderStartTime, startTime) != 0
                 || TimeSpan.Compare(firstConfiguration.ScrawlingOrderEndTime, endTime) != 0)
                {
                    isChanged = true;
                }

                if (TimeSpan.Compare(firstConfiguration.ScrawlingMoneyExchangeToKitchenCenter, exchangeToKitchenCenterTime) != 0)
                {
                    oldTimeExchangeToKitchenCenter = firstConfiguration.ScrawlingMoneyExchangeToKitchenCenter;
                    firstConfiguration.ScrawlingMoneyExchangeToKitchenCenter = exchangeToKitchenCenterTime;
                    RecurringJob.AddOrUpdate(HangfireConstant.MoneyExchangeToKitchenCenter_ID,
                                  () => MoneyExchangeToKitchenCenterAsync(),
                                  cronExpression: StringUtil.ConvertTimeSpanToCron(exchangeToKitchenCenterTime),
                                  new RecurringJobOptions
                                  {
                                      // sync time(utc +7)
                                      TimeZone = TimeZoneInfo.Local,
                                  });
                }

                if (TimeSpan.Compare(firstConfiguration.ScrawlingMoneyExchangeToStore, exchangeToStoreTime) != 0)
                {
                    oldTimeExchangeToStore = firstConfiguration.ScrawlingMoneyExchangeToStore;
                    firstConfiguration.ScrawlingMoneyExchangeToStore = exchangeToStoreTime;
                    RecurringJob.AddOrUpdate(HangfireConstant.MoneyExchangeToStore_ID,
                                  () => MoneyExchangeToStoreAsync(),
                                  cronExpression: StringUtil.ConvertTimeSpanToCron(exchangeToStoreTime),
                                  new RecurringJobOptions
                                  {
                                      // sync time(utc +7)
                                      TimeZone = TimeZoneInfo.Local,
                                  });
                }

                firstConfiguration.ScrawlingOrderStartTime = startTime;
                firstConfiguration.ScrawlingOrderEndTime = endTime;
                this._unitOfWork.ConfigurationRepository.UpdateConfiguration(firstConfiguration);
                await this._unitOfWork.CommitAsync();
                if (isChanged)
                {
                    string jsonMessage = JsonConvert.SerializeObject(firstConfiguration);
                    this._unitOfWork.RabbitMQRepository.SendMessage(jsonMessage, "Modified_Configurations");
                }
            }
            catch (Exception ex)
            {
                // rollback
                if (oldTimeExchangeToKitchenCenter is not null)
                {
                    RecurringJob.AddOrUpdate(HangfireConstant.MoneyExchangeToKitchenCenter_ID,
                                   () => MoneyExchangeToKitchenCenterAsync(),
                                   cronExpression: StringUtil.ConvertTimeSpanToCron(oldTimeExchangeToKitchenCenter.Value),
                                   new RecurringJobOptions
                                   {
                                       // sync time(utc +7)
                                       TimeZone = TimeZoneInfo.Local,
                                   });
                }

                if (oldTimeExchangeToStore is not null)
                {
                    RecurringJob.AddOrUpdate(HangfireConstant.MoneyExchangeToStore_ID,
                                  () => MoneyExchangeToStoreAsync(),
                                  cronExpression: StringUtil.ConvertTimeSpanToCron(oldTimeExchangeToStore.Value),
                                  new RecurringJobOptions
                                  {
                                      // sync time(utc +7)
                                      TimeZone = TimeZoneInfo.Local,
                                  });
                }

                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region money exchange to kitchen center
        public async Task MoneyExchangeToKitchenCenterAsync()
        {
            try
            {
                var cashiers = await this._unitOfWork.CashierRepository.GetCashiersAsync();
                if (!cashiers.Any())
                {
                    this._logger.LogWarning($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second} - " + MessageConstant.CashierMessage.NoOneAvailable);
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
                    this._logger.LogWarning($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second} - " + MessageConstant.KitchenCenterMessage.NoOneAvailable);
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
                            //decimal finalToTalPriceSubstractDeliveryFee = order.FinalTotalPrice - order.DeliveryFee;
                            if (exchangeWallets.ContainsKey(store.StoreId))
                            {
                                exchangeWallets[store.StoreId] += order.SubTotalPrice - (order.SubTotalPrice * (decimal)(store.StorePartners.FirstOrDefault(sp => sp.PartnerId == order.PartnerId && sp.Status == (int)StorePartnerEnum.Status.ACTIVE)!.Commission / 100));
                            }
                            else
                            {
                                exchangeWallets.Add(store.StoreId, order.SubTotalPrice - (order.SubTotalPrice * (decimal)(store.StorePartners.FirstOrDefault(sp => sp.PartnerId == order.PartnerId && sp.Status == (int)StorePartnerEnum.Status.ACTIVE)!.Commission / 100)));
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
                                        Wallet = store.Wallet!,
                                        Status = (int)TransactionEnum.Status.SUCCESS,
                                    },
                                }
                            }
                        };
                        storeMoneyExchanges.Add(storeMoneyExchange);
                        #endregion

                        #region update balance of kitchen center and store wallet
                        store.Wallet!.Balance += exchangeWalletValue;
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

        #region change stats of partner product to available
        public async Task ChangeStatusPartnerProduct()
        {
            try
            {
               List<PartnerProduct> partnerProducts = await this._unitOfWork.PartnerProductRepository.GetPartnerProductsAsync();
                if (!partnerProducts.Any())
                {
                    this._logger.LogWarning($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second} - " + MessageConstant.PartnerProductMessage.NoOneOutOfStock);
                    return;
                }

                foreach (var partnerProduct in partnerProducts) partnerProduct.Status = (int)PartnerProductEnum.AvailableStatus.AVAILABLE;
                this._unitOfWork.PartnerProductRepository.UpdatePartnerProductRange(partnerProducts);
                await this._unitOfWork.CommitAsync();

                this._logger.LogInformation($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second} - " + MessageConstant.PartnerProductMessage.UpdatePartnerProductSuccessfully);
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
