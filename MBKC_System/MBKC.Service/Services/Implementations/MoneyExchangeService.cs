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
using MBKC.Service.DTOs.Orders.MBKC.Service.DTOs.Orders;

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

        #region get money exchanges
        public async Task<GetMoneyExchangesResponse> GetMoneyExchangesAsync(IEnumerable<Claim> claims, GetMoneyExchangeRequest getMoneyExchangeRequest)
        {
            try
            {
                string email = claims.First(x => x.Type == ClaimTypes.Email).Value;
                string roleClaim = claims.First(x => x.Type.ToLower().Equals("role")).Value;
                int? idSearching = null;

                switch (roleClaim)
                {
                    case RoleConstant.Kitchen_Center_Manager:
                        var accountKI = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(email);
                        idSearching = accountKI.KitchenCenterId;
                        break;

                    case RoleConstant.Store_Manager:
                        var accountST = await this._unitOfWork.StoreRepository.GetStoreAsync(email);
                        idSearching = accountST.StoreId;
                        break;

                    case RoleConstant.Cashier:
                        var accountCA = await this._unitOfWork.CashierRepository.GetCashierAsync(email);
                        idSearching = accountCA.AccountId;
                        break;

                    default:
                        throw new BadRequestException("Undefine role.");
                }




                int numberItems = 0;
                List<MoneyExchange>? moneyExchanges = null;
                int totalPages = 0;
                totalPages = (int)((numberItems + getMoneyExchangeRequest.ItemsPerPage) / getMoneyExchangeRequest.ItemsPerPage);
                if (numberItems == 0)
                {
                    totalPages = 0;
                }

                List<GetMoneyExchangeResponse> getMoneyExchangesResponse = this._mapper.Map<List<GetMoneyExchangeResponse>>(moneyExchanges);
                GetMoneyExchangesResponse result = new GetMoneyExchangesResponse()
                {
                    NumberItems = numberItems,
                    TotalPages = totalPages,
                    MoneyExchanges = getMoneyExchangesResponse
                };
                return result;

            }
            catch(BadRequestException ex)
            {
                string fieldName = "Role";
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

        #region get money exchange detail
        public Task<GetMoneyExchangeResponse> GetMoneyExchangeAsync(IEnumerable<Claim> claims, MoneyExchangeRequest moneyExchangeRequest)
        {
            throw new NotImplementedException();
        }
        #endregion

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

    }
}

