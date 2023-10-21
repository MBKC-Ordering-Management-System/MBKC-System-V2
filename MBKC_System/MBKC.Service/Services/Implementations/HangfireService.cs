using AutoMapper;
using MBKC.Repository.Enums;
using MBKC.Repository.Infrastructures;
using MBKC.Repository.Models;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        #region money exchange to store
        public async Task MoneyExchangeToStoreAsync()
        {
            try
            {
                List<KitchenCenter> kitchenCenters = await this._unitOfWork.KitchenCenterRepository.GetKitchenCentersIncludeOrderAsync();
                if (!kitchenCenters.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("There are currently no kitchens.");
                    return;
                }

                var result = kitchenCenters.FirstOrDefault(f => f.KitchenCenterMoneyExchanges.Any());
                if (result != null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("The money has been transferred to the store today.");
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
                        };
                        moneyExchanges.Add(moneyExchangeKitchenCenter);

                        // create money exchange for store (receiver)
                        MoneyExchange moneyExchangeStore = new MoneyExchange()
                        {
                            Amount = exchangeWalletValue,
                            ExchangeType = MoneyExchangeEnum.ExchangeType.RECEIVE.ToString(),
                            Content = $"Receiver money from kitchen center[id:{kitchenCenter.KitchenCenterId} - name:{kitchenCenter.Name}] {StringUtil.GetContentAmountAndTime(exchangeWalletValue, DateTime.Now)}",
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
                Console.WriteLine("Transfer money to store successfully.");
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
