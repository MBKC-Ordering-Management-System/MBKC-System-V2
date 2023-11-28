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
using MBKC.Service.DTOs.MoneyExchanges;
using MBKC.Service.DTOs.Brands;
using static MBKC.Service.Constants.EmailMessageConstant;
using Cashier = MBKC.Repository.Models.Cashier;
using Store = MBKC.Repository.Models.Store;
using KitchenCenter = MBKC.Repository.Models.KitchenCenter;
using MBKC.Service.Profiles.MoneyExchanges;

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
                var existedCashier = await this._unitOfWork.CashierRepository.GetCashiersIncludeMoneyExchangeAsync(email);
                if (existedCashier.Wallet.Balance <= 0)
                {
                    throw new BadRequestException(MessageConstant.WalletMessage.BalanceIsInvalid);
                }

                if (existedCashier.CashierMoneyExchanges.Any())
                {
                    throw new BadRequestException(MessageConstant.MoneyExchangeMessage.AlreadyTransferredToKitchenCenter);
                }
                #endregion

                #region operation

                #region create money exchange
                // create cashier money exchange (sender)
                CashierMoneyExchange cashierMoneyExchange = new CashierMoneyExchange()
                {
                    Cashier = existedCashier,
                    MoneyExchange = new MoneyExchange()
                    {
                        Amount = existedCashier.Wallet.Balance,
                        ExchangeType = MoneyExchangeEnum.ExchangeType.SEND.ToString(),
                        Content = $"Transfer money to kitchen center[id:{existedCashier.KitchenCenter.KitchenCenterId} - name:{existedCashier.KitchenCenter.Name}] {StringUtil.GetContentAmountAndTime(existedCashier.Wallet.Balance)}",
                        Status = (int)MoneyExchangeEnum.Status.SUCCESS,
                        SenderId = existedCashier.AccountId,
                        ReceiveId = existedCashier.KitchenCenter.KitchenCenterId,
                        Transactions = new List<Transaction>()
                        {
                            new Transaction()
                            {
                                TransactionTime = DateTime.Now,
                                Wallet = existedCashier.Wallet,
                                Status = (int)TransactionEnum.Status.SUCCESS,
                            },
                        }
                    }
                };
                await this._unitOfWork.CashierMoneyExchangeRepository.CreateCashierMoneyExchangeAsync(cashierMoneyExchange);

                // create kitchen center money exchange (receiver)
                KitchenCenterMoneyExchange kitchenCenterMoneyExchange = new KitchenCenterMoneyExchange()
                {
                    KitchenCenter = existedCashier.KitchenCenter,
                    MoneyExchange = new MoneyExchange()
                    {
                        Amount = existedCashier.Wallet.Balance,
                        ExchangeType = MoneyExchangeEnum.ExchangeType.RECEIVE.ToString(),
                        Content = $"Receive money from cashier[id:{existedCashier.AccountId} - name:{existedCashier.FullName}] {StringUtil.GetContentAmountAndTime(existedCashier.Wallet.Balance)}",
                        Status = (int)MoneyExchangeEnum.Status.SUCCESS,
                        SenderId = existedCashier.AccountId,
                        ReceiveId = existedCashier.KitchenCenter.KitchenCenterId,
                        Transactions = new List<Transaction>()
                        {
                            new Transaction()
                            {
                                TransactionTime = DateTime.Now,
                                Wallet = existedCashier.KitchenCenter.Wallet,
                                Status = (int)TransactionEnum.Status.SUCCESS,
                            },
                        }
                    }
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
                    case MessageConstant.MoneyExchangeMessage.AlreadyTransferredToKitchenCenter:
                        fieldName = "Money exchange";
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

        #region withdraw money for store
        public async Task WithdrawMoneyAsync(IEnumerable<Claim> claims, WithdrawMoneyRequest withdrawMoneyRequest)
        {
            string folderName = "MoneyExchanges";
            string imageId = "";
            bool uploaded = false;
            try
            {
                #region validation
                string email = claims.First(x => x.Type == ClaimTypes.Email).Value;
                var existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(email);
                var existedStore = await this._unitOfWork.StoreRepository.GetStoreByIdAsync(withdrawMoneyRequest.StoreId);
                if (existedStore == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistStoreId);
                }

                if (!existedKitchenCenter.Stores.Any(s => s.StoreId == existedStore.StoreId))
                {
                    throw new BadRequestException(MessageConstant.MoneyExchangeMessage.StoreIdNotBelogToKitchenCenter);
                }

                if (existedStore.Wallet.Balance <= 0)
                {
                    throw new BadRequestException(MessageConstant.MoneyExchangeMessage.BalanceIsInvalid);
                }

                if (existedStore.Wallet.Balance < withdrawMoneyRequest.Amount)
                {
                    throw new BadRequestException(MessageConstant.MoneyExchangeMessage.BalanceDoesNotEnough);
                }
                #endregion

                #region operation

                #region upload image
                FileStream fileStream = FileUtil.ConvertFormFileToStream(withdrawMoneyRequest.Image);
                imageId = Guid.NewGuid().ToString();
                string urlImage = await this._unitOfWork.FirebaseStorageRepository.UploadImageAsync(fileStream, folderName, imageId);
                if (urlImage != null && urlImage.Length > 0)
                {
                    uploaded = true;
                    urlImage += $"&imageId={imageId}";
                }
                #endregion

                #region create store exchange, money exchange and transaction
                // create store money exchange
                StoreMoneyExchange storeMoneyExchange = new StoreMoneyExchange()
                {
                    Store = existedStore,
                    MoneyExchange = new MoneyExchange()
                    {
                        Amount = withdrawMoneyRequest.Amount,
                        ExchangeType = MoneyExchangeEnum.ExchangeType.WITHDRAW.ToString(),
                        Content = $"Withdraw money {StringUtil.GetContentAmountAndTime(withdrawMoneyRequest.Amount)}",
                        Status = (int)MoneyExchangeEnum.Status.SUCCESS,
                        SenderId = existedKitchenCenter.KitchenCenterId,
                        ReceiveId = existedStore.StoreId,
                        ExchangeImage = urlImage,
                        Transactions = new List<Transaction>()
                        {
                            new Transaction()
                            {
                                TransactionTime = DateTime.Now,
                                Wallet = existedStore.Wallet,
                                Status = (int)TransactionEnum.Status.SUCCESS,
                            },
                        },
                    },
                };
                await this._unitOfWork.StoreMoneyExchangeRepository.CreateStoreMoneyExchangeAsync(storeMoneyExchange);

                // update wallet
                existedStore.Wallet.Balance -= withdrawMoneyRequest.Amount;
                this._unitOfWork.WalletRepository.UpdateWallet(existedStore.Wallet);
                #endregion

                await this._unitOfWork.CommitAsync();
                #endregion
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                switch (ex.Message)
                {
                    case MessageConstant.CommonMessage.NotExistStoreId:
                        fieldName = "StoreId";
                        break;

                    default:
                        fieldName = "Exception";
                        break;
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                switch (ex.Message)
                {
                    case MessageConstant.MoneyExchangeMessage.StoreIdNotBelogToKitchenCenter:
                        fieldName = "StoreId";
                        break;

                    case MessageConstant.MoneyExchangeMessage.BalanceIsInvalid:
                    case MessageConstant.MoneyExchangeMessage.BalanceDoesNotEnough:
                        fieldName = "Balance";
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
                if (uploaded)
                {
                    await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(imageId, folderName);
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                throw new Exception(error);
            }

        }
        #endregion

        #region Get money exchanges
        public async Task<GetMoneyExchangesResponse> GetMoneyExchanges(IEnumerable<Claim> claims, GetMoneyExchangesRequest getMoneyExchangesRequest)
        {
            try
            {
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                Claim registeredRoleClaim = claims.First(x => x.Type.ToLower().Equals("role"));
                string email = registeredEmailClaim.Value;
                string role = registeredRoleClaim.Value;
                Store? existedStore = null;
                Cashier? existedCashier = null;
                KitchenCenter? existedKitchenCenter = null;
                List<MoneyExchange>? existedMoneyExchanges = null;

                // Check role when user login
                if (role.ToLower().Equals(RoleConstant.Cashier.ToLower()))
                {
                    existedCashier = await this._unitOfWork.CashierRepository.GetCashierMoneyExchangeAsync(email);
                    existedMoneyExchanges = existedCashier.CashierMoneyExchanges.Select(x => x.MoneyExchange).ToList();
                }
                else if (role.ToLower().Equals(RoleConstant.Store_Manager.ToLower()))
                {
                    existedStore = await this._unitOfWork.StoreRepository.GetStoreMoneyExchangeAsync(email);
                    existedMoneyExchanges = existedStore.StoreMoneyExchanges.Select(x => x.MoneyExchange).ToList();
                }
                else if (role.ToLower().Equals(RoleConstant.Kitchen_Center_Manager.ToLower()))
                {
                    existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterMoneyExchangeAsync(email);
                    /*var getMoneyExchangeWithdraw = await this._unitOfWork.MoneyExchangeRepository.GetMoneyExchangesBySendIdAsync(existedKitchenCenter.KitchenCenterId);*/
                    existedMoneyExchanges = existedKitchenCenter.KitchenCenterMoneyExchanges.Select(x => x.MoneyExchange).ToList();
                }

                // Change status string to int
                int? status = null;
                if (getMoneyExchangesRequest.Status != null)
                {
                    status = StatusUtil.ChangeMoneyExchangeStatus(getMoneyExchangesRequest.Status);
                }

                int numberItems = 0;
                List<MoneyExchange>? listMoneyExchanges = null;
                numberItems = this._unitOfWork.MoneyExchangeRepository.GetNumberMoneyExchangesAsync(existedMoneyExchanges,
                                                                         getMoneyExchangesRequest.ExchangeType, status,
                                                                         getMoneyExchangesRequest.SearchDateFrom,
                                                                         getMoneyExchangesRequest.SearchDateTo);

                listMoneyExchanges = this._unitOfWork.MoneyExchangeRepository.GetMoneyExchangesAsync(existedMoneyExchanges,
                                                                                 getMoneyExchangesRequest.CurrentPage, getMoneyExchangesRequest.ItemsPerPage,
                                                                                 getMoneyExchangesRequest.SortBy != null && getMoneyExchangesRequest.SortBy.ToLower().EndsWith("asc") ? getMoneyExchangesRequest.SortBy.Split("_")[0] : null,
                                                                                 getMoneyExchangesRequest.SortBy != null && getMoneyExchangesRequest.SortBy.ToLower().EndsWith("desc") ? getMoneyExchangesRequest.SortBy.Split("_")[0] : null,
                                                                                 getMoneyExchangesRequest.ExchangeType, status, getMoneyExchangesRequest.SearchDateFrom, getMoneyExchangesRequest.SearchDateTo);

                var getMoneyExchangeResponse = this._mapper.Map<List<GetMoneyExchangeResponse>>(listMoneyExchanges);

                // Assign sender name, receive name, transaction time role Store.
                if (existedStore != null)
                {
                    foreach (var item in getMoneyExchangeResponse)
                    {
                        item.SenderName = existedStore.KitchenCenter.Name;
                        item.ReceiveName = existedStore.Name;
                        item.TransactionTime = existedMoneyExchanges
                             .SelectMany(x => x.Transactions.Where(x => x.ExchangeId == item.ExchangeId)
                             .Select(x => x.TransactionTime))
                             .SingleOrDefault();
                    }
                }

                // Assign sender name and receiver name, transaction time role Kitchen Center
                if (existedKitchenCenter != null)
                {
                    foreach (var item in listMoneyExchanges)
                    {
                        if (item.ExchangeType.Equals(MoneyExchangeEnum.ExchangeType.RECEIVE.ToString()))
                        {
                            var cashierSender = await this._unitOfWork.CashierRepository.GetCashierAsync(item.SenderId);

                            foreach (var itemResponse in getMoneyExchangeResponse)
                            {
                                if (itemResponse.ExchangeId == item.ExchangeId)
                                {
                                    itemResponse.SenderName = cashierSender.FullName;
                                    itemResponse.ReceiveName = existedKitchenCenter.Name;
                                    itemResponse.TransactionTime = existedMoneyExchanges
                                        .SelectMany(x => x.Transactions.Where(x => x.ExchangeId == item.ExchangeId)
                                        .Select(x => x.TransactionTime))
                                        .SingleOrDefault();
                                }
                            }
                        }
                        else
                        {
                            var storeRecieve = await _unitOfWork.StoreRepository.GetStoreAsync(item.ReceiveId);
                            foreach (var itemResponse in getMoneyExchangeResponse)
                            {
                                if (itemResponse.ExchangeId == item.ExchangeId)
                                {
                                    itemResponse.SenderName = existedKitchenCenter.Name;
                                    itemResponse.ReceiveName = storeRecieve.Name;
                                    itemResponse.TransactionTime = existedMoneyExchanges
                                        .SelectMany(x => x.Transactions.Where(x => x.ExchangeId == item.ExchangeId)
                                        .Select(x => x.TransactionTime))
                                        .SingleOrDefault();
                                }
                            }
                        }
                    }
                }

                // Assign sender name and receiver name, transaction time role Cashier
                if (existedCashier != null)
                {
                    foreach (var item in getMoneyExchangeResponse)
                    {
                        item.SenderName = existedCashier.FullName;
                        item.ReceiveName = existedCashier.KitchenCenter.Name;
                        item.TransactionTime = existedMoneyExchanges
                            .SelectMany(x => x.Transactions.Where(x => x.ExchangeId == item.ExchangeId)
                            .Select(x => x.TransactionTime))
                            .SingleOrDefault();
                    }
                }

                int totalPages = 0;
                totalPages = (int)((numberItems + getMoneyExchangesRequest.ItemsPerPage) / getMoneyExchangesRequest.ItemsPerPage);

                if (numberItems == 0)
                {
                    totalPages = 0;
                }
                return new GetMoneyExchangesResponse
                {
                    TotalPages = totalPages,
                    NumberItems = numberItems,
                    MoneyExchanges = getMoneyExchangeResponse
                };
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region Get money exchanges with draw
        public async Task<GetMoneyExchangesResponse> GetMoneyExchangesWithDraw(IEnumerable<Claim> claims, GetMoneyExchangesWithDrawRequest getMoneyExchangesWithDrawRequest)
        {
            try
            {
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                Claim registeredRoleClaim = claims.First(x => x.Type.ToLower().Equals("role"));
                string email = registeredEmailClaim.Value;
                string role = registeredRoleClaim.Value;
                KitchenCenter? existedKitchenCenter = null;
                List<MoneyExchange>? existedMoneyExchanges = null;

                if (role.ToLower().Equals(RoleConstant.Kitchen_Center_Manager.ToLower()))
                {
                    existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterMoneyExchangeAsync(email);
                    var getMoneyExchangeWithdraw = await this._unitOfWork.MoneyExchangeRepository.GetMoneyExchangesBySendIdAsync(existedKitchenCenter.KitchenCenterId);
                    existedMoneyExchanges = getMoneyExchangeWithdraw;
                }

                // Change status string to int
                int? status = null;
                if (getMoneyExchangesWithDrawRequest.Status != null)
                {
                    status = StatusUtil.ChangeMoneyExchangeStatus(getMoneyExchangesWithDrawRequest.Status);
                }

                int numberItems = 0;
                List<MoneyExchange>? listMoneyExchanges = null;
                numberItems = this._unitOfWork.MoneyExchangeRepository.GetNumberMoneyExchangesWithDrawAsync(existedMoneyExchanges,
                                                                         status,
                                                                         getMoneyExchangesWithDrawRequest.SearchDateFrom,
                                                                         getMoneyExchangesWithDrawRequest.SearchDateTo);

                listMoneyExchanges = this._unitOfWork.MoneyExchangeRepository.GetMoneyExchangesWithDrawAsync(existedMoneyExchanges,
                                                                                 getMoneyExchangesWithDrawRequest.CurrentPage, getMoneyExchangesWithDrawRequest.ItemsPerPage,
                                                                                 getMoneyExchangesWithDrawRequest.SortBy != null && getMoneyExchangesWithDrawRequest.SortBy.ToLower().EndsWith("asc") ? getMoneyExchangesWithDrawRequest.SortBy.Split("_")[0] : null,
                                                                                 getMoneyExchangesWithDrawRequest.SortBy != null && getMoneyExchangesWithDrawRequest.SortBy.ToLower().EndsWith("desc") ? getMoneyExchangesWithDrawRequest.SortBy.Split("_")[0] : null,
                                                                                 status, getMoneyExchangesWithDrawRequest.SearchDateFrom, getMoneyExchangesWithDrawRequest.SearchDateTo);

                var getMoneyExchangeResponse = this._mapper.Map<List<GetMoneyExchangeResponse>>(listMoneyExchanges);

                // Assign sender name and receiver name, transaction time role Kitchen Center
                if (existedKitchenCenter != null)
                {
                    foreach (var item in listMoneyExchanges)
                    {
                        if (item.ExchangeType != null && item.ExchangeType.Equals(MoneyExchangeEnum.ExchangeType.WITHDRAW.ToString()))
                        {
                            var storeRecieve = await _unitOfWork.StoreRepository.GetStoreAsync(item.ReceiveId);
                            foreach (var itemResponse in getMoneyExchangeResponse)
                            {
                                if (itemResponse.ExchangeId == item.ExchangeId)
                                {
                                    itemResponse.SenderName = existedKitchenCenter.Name;
                                    itemResponse.ReceiveName = storeRecieve.Name;
                                    itemResponse.TransactionTime = existedMoneyExchanges
                                        .SelectMany(x => x.Transactions.Where(x => x.ExchangeId == item.ExchangeId)
                                        .Select(x => x.TransactionTime))
                                        .SingleOrDefault();
                                }
                            }
                        }
                    }
                }

                int totalPages = 0;
                totalPages = (int)((numberItems + getMoneyExchangesWithDrawRequest.ItemsPerPage) / getMoneyExchangesWithDrawRequest.ItemsPerPage);

                if (numberItems == 0)
                {
                    totalPages = 0;
                }
                return new GetMoneyExchangesResponse
                {
                    TotalPages = totalPages,
                    NumberItems = numberItems,
                    MoneyExchanges = getMoneyExchangeResponse
                };
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

