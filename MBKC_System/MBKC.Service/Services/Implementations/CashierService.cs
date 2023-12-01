using AutoMapper;
using MBKC.Service.Services.Interfaces;
using MBKC.Repository.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using MBKC.Service.Utils;
using MBKC.Repository.Models;
using MBKC.Service.Exceptions;
using MBKC.Service.Constants;
using MBKC.Repository.Enums;
using MBKC.Service.DTOs.Cashiers.Requests;
using MBKC.Service.DTOs.Cashiers.Responses;
using MBKC.Repository.Redis.Models;
using MBKC.Service.DTOs.KitchenCenters;

namespace MBKC.Service.Services.Implementations
{
    public class CashierService : ICashierService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public CashierService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        public async Task<GetCashiersResponse> GetCashiersAsync(GetCashiersRequest getCashiersRequest, IEnumerable<Claim> claims)
        {
            try
            {
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                string email = registeredEmailClaim.Value;

                KitchenCenter existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(email);

                int numberItems = 0;
                List<Cashier> cashiers = null;

                if (getCashiersRequest.SearchValue is not null && StringUtil.IsUnicode(getCashiersRequest.SearchValue))
                {
                    numberItems = await this._unitOfWork.CashierRepository.GetNumberCashiersAsync(getCashiersRequest.SearchValue, null, existedKitchenCenter.KitchenCenterId);

                    cashiers = await this._unitOfWork.CashierRepository.GetCashiersAsync(getCashiersRequest.SearchValue, null, getCashiersRequest.CurrentPage.Value, getCashiersRequest.ItemsPerPage.Value,
                                                                                     getCashiersRequest.SortBy != null && getCashiersRequest.SortBy.ToLower().EndsWith("asc") ? getCashiersRequest.SortBy.Split("_")[0] : null,
                                                                                     getCashiersRequest.SortBy != null && getCashiersRequest.SortBy.ToLower().EndsWith("desc") ? getCashiersRequest.SortBy.Split("_")[0] : null,
                                                                                     existedKitchenCenter.KitchenCenterId);
                }
                else if (getCashiersRequest.SearchValue is not null && StringUtil.IsUnicode(getCashiersRequest.SearchValue) == false)
                {
                    numberItems = await this._unitOfWork.CashierRepository.GetNumberCashiersAsync(null, getCashiersRequest.SearchValue, existedKitchenCenter.KitchenCenterId);
                    cashiers = await this._unitOfWork.CashierRepository.GetCashiersAsync(null, getCashiersRequest.SearchValue, getCashiersRequest.CurrentPage.Value, getCashiersRequest.ItemsPerPage.Value,
                                                                                     getCashiersRequest.SortBy != null && getCashiersRequest.SortBy.ToLower().EndsWith("asc") ? getCashiersRequest.SortBy.Split("_")[0] : null,
                                                                                     getCashiersRequest.SortBy != null && getCashiersRequest.SortBy.ToLower().EndsWith("desc") ? getCashiersRequest.SortBy.Split("_")[0] : null,
                                                                                     existedKitchenCenter.KitchenCenterId);
                }
                else if (getCashiersRequest.SearchValue is null)
                {
                    numberItems = await this._unitOfWork.CashierRepository.GetNumberCashiersAsync(null, null, existedKitchenCenter.KitchenCenterId);
                    cashiers = await this._unitOfWork.CashierRepository.GetCashiersAsync(null, null, getCashiersRequest.CurrentPage.Value, getCashiersRequest.ItemsPerPage.Value,
                                                                                     getCashiersRequest.SortBy != null && getCashiersRequest.SortBy.ToLower().EndsWith("asc") ? getCashiersRequest.SortBy.Split("_")[0] : null,
                                                                                     getCashiersRequest.SortBy != null && getCashiersRequest.SortBy.ToLower().EndsWith("desc") ? getCashiersRequest.SortBy.Split("_")[0] : null,
                                                                                     existedKitchenCenter.KitchenCenterId);
                }
                int totalPages = 0;
                if (numberItems > 0)
                {
                    totalPages = (int)((numberItems + getCashiersRequest.ItemsPerPage.Value) / getCashiersRequest.ItemsPerPage.Value);
                }

                if (numberItems == 0)
                {
                    totalPages = 0;
                }
                List<GetCashierResponse> getCashierResponses = this._mapper.Map<List<GetCashierResponse>>(cashiers);
                return new GetCashiersResponse()
                {
                    TotalPages = totalPages,
                    NumberItems = numberItems,
                    Cashiers = getCashierResponses
                };
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task CreateCashierAsync(CreateCashierRequest createCashierRequest, IEnumerable<Claim> claims)
        {
            string folderName = "Cashiers";
            string logoId = "";
            bool isUploaded = false;
            try
            {
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                string email = registeredEmailClaim.Value;

                KitchenCenter existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(email);

                Account existedAccount = await this._unitOfWork.AccountRepository.GetAccountAsync(createCashierRequest.Email);
                if (existedAccount != null && existedAccount.Status != (int) AccountEnum.Status.DISABLE)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.AlreadyExistEmail);
                }
                Cashier existedCashierWithCitizenNumber = await this._unitOfWork.CashierRepository.GetCashierWithCitizenNumberAsync(createCashierRequest.CitizenNumber);
                if (existedCashierWithCitizenNumber is not null)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.AlreadyExistCitizenNumber);
                }

                bool gender = false;
                if (createCashierRequest.Gender.ToLower().Equals(CashierEnum.Gender.MALE.ToString().ToLower()))
                {
                    gender = true;
                }
                Wallet cashierWallet = new Wallet()
                {
                    Balance = 0
                };
                Role cashierRole = await this._unitOfWork.RoleRepository.GetRoleById((int)RoleEnum.Role.CASHIER);
                string password = PasswordUtil.CreateRandomPassword();
                Account cashierAccount = new Account()
                {
                    Email = createCashierRequest.Email,
                    Password = StringUtil.EncryptData(password),
                    Role = cashierRole,
                    Status = (int)AccountEnum.Status.ACTIVE,
                    IsConfirmed = false
                };
                FileStream avatarFileStream = FileUtil.ConvertFormFileToStream(createCashierRequest.Avatar);
                Guid guid = Guid.NewGuid();
                logoId = guid.ToString();
                string avatarUrl = await this._unitOfWork.FirebaseStorageRepository.UploadImageAsync(avatarFileStream, folderName, logoId);
                if (avatarUrl is not null && avatarUrl.Length > 0)
                {
                    isUploaded = true;
                }
                avatarUrl += $"&avatarId={logoId}";

                Cashier newCashier = new Cashier()
                {
                    Wallet = cashierWallet,
                    Account = cashierAccount,
                    Avatar = avatarUrl,
                    CitizenNumber = createCashierRequest.CitizenNumber,
                    DateOfBirth = createCashierRequest.DateOfBirth,
                    FullName = createCashierRequest.FullName,
                    Gender = gender,
                    KitchenCenter = existedKitchenCenter
                };

                await this._unitOfWork.CashierRepository.CreateCashierAsync(newCashier);
                await this._unitOfWork.CommitAsync();

                string messageBody = EmailMessageConstant.Cashier.Message + $" {existedKitchenCenter.Name}. " + EmailMessageConstant.CommonMessage.Message;
                string message = this._unitOfWork.EmailRepository.GetMessageToRegisterAccount(createCashierRequest.Email, password, messageBody);
                await this._unitOfWork.EmailRepository.SendAccountToEmailAsync(createCashierRequest.Email, message);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.AlreadyExistEmail))
                {
                    fieldName = "Email";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.AlreadyExistCitizenNumber))
                {
                    fieldName = "Citizen number";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                if (isUploaded && logoId.Length > 0)
                {
                    await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(logoId, folderName);
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task<GetCashierResponse> GetCashierAsync(int idCashier, IEnumerable<Claim> claims)
        {
            try
            {
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                Claim registeredRoleClaim = claims.First(x => x.Type.ToLower().Equals("role"));
                string email = registeredEmailClaim.Value;
                string role = registeredRoleClaim.Value;

                Cashier existedCashier = await this._unitOfWork.CashierRepository.GetCashierAsync(idCashier);
                if (existedCashier is null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistCashierId);
                }

                if (role.ToLower().Equals(RoleConstant.Kitchen_Center_Manager.ToLower()))
                {
                    KitchenCenter existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(email);

                    if (existedKitchenCenter.Cashiers.Any(x => x.AccountId == idCashier) == false)
                    {
                        throw new BadRequestException(MessageConstant.CashierMessage.CashierIdNotBelongToKitchenCenter);
                    }
                }

                if (role.ToLower().Equals(RoleConstant.Cashier.ToLower()))
                {
                    if (existedCashier.Account.Email.Equals(email))
                    {
                        throw new BadRequestException(MessageConstant.CashierMessage.CashierIdNotBelogToCashier);
                    }

                }

                GetCashierResponse getCashierResponse = this._mapper.Map<GetCashierResponse>(existedCashier);
                return getCashierResponse;
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistCashierId))
                {
                    fieldName = "Cashier id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistCashierId) ||
                    ex.Message.Equals(MessageConstant.CashierMessage.CashierIdNotBelongToKitchenCenter) ||
                    ex.Message.Equals(MessageConstant.CashierMessage.CashierIdNotBelogToCashier))
                {
                    fieldName = "Cashier id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task UpdateCashierAsync(int idCashier, UpdateCashierRequest updateCashierRequest, IEnumerable<Claim> claims)
        {
            string folderName = "Cashiers";
            string logoId = "";
            bool isUploaded = false;
            bool isDeleted = false;
            try
            {
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                Claim registeredRoleClaim = claims.First(x => x.Type.ToLower().Equals("role"));
                string email = registeredEmailClaim.Value;
                string role = registeredRoleClaim.Value;

                if (updateCashierRequest.Status == null && role.ToLower().Equals(RoleConstant.Kitchen_Center_Manager.ToLower()))
                {
                    throw new BadRequestException(MessageConstant.CashierMessage.StatusIsRequiredWithKitchenCenterManager);
                }

                if (updateCashierRequest.Status != null && role.ToLower().Equals(RoleConstant.Cashier.ToLower()))
                {
                    throw new BadRequestException(MessageConstant.CashierMessage.StatusIsNotRequiredWithCashier);
                }

                Cashier existedCashier = await this._unitOfWork.CashierRepository.GetCashierAsync(idCashier);
                if (existedCashier is null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistCashierId);
                }

                if (role.ToLower().Equals(RoleConstant.Kitchen_Center_Manager.ToLower()))
                {
                    KitchenCenter existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(email);

                    if (existedKitchenCenter.Cashiers.Any(x => x.AccountId == idCashier) == false)
                    {
                        throw new BadRequestException(MessageConstant.CashierMessage.CashierIdNotBelongToKitchenCenter);
                    }

                    if (AccountEnum.Status.ACTIVE.ToString().ToLower().Equals(updateCashierRequest.Status.Trim().ToLower()))
                    {
                        existedCashier.Account.Status = (int)AccountEnum.Status.ACTIVE;
                    }
                    else if (AccountEnum.Status.INACTIVE.ToString().ToLower().Equals(updateCashierRequest.Status.Trim().ToLower()))
                    {
                        existedCashier.Account.Status = (int)AccountEnum.Status.INACTIVE;
                    }
                }

                if (role.ToLower().Equals(RoleConstant.Cashier.ToLower()))
                {
                    if (existedCashier.Account.Email.Equals(email) == false)
                    {
                        throw new BadRequestException(MessageConstant.CashierMessage.CashierIdNotBelogToCashier);
                    }
                }

                if (updateCashierRequest.NewPassword is not null)
                {
                    existedCashier.Account.Password = updateCashierRequest.NewPassword;
                }

                existedCashier.FullName = updateCashierRequest.FullName;
                existedCashier.DateOfBirth = updateCashierRequest.DateOfBirth;
                existedCashier.CitizenNumber = updateCashierRequest.CitizenNumber;
                existedCashier.Gender = CashierEnum.Gender.FEMALE.ToString().ToLower().Equals(updateCashierRequest.Gender.Trim().ToLower()) ? false : true;

                string oldAvatar = existedCashier.Avatar;
                if (updateCashierRequest.Avatar is not null)
                {
                    FileStream avatarFileStream = FileUtil.ConvertFormFileToStream(updateCashierRequest.Avatar);
                    Guid guid = Guid.NewGuid();
                    logoId = guid.ToString();
                    string avatarUrl = await this._unitOfWork.FirebaseStorageRepository.UploadImageAsync(avatarFileStream, folderName, logoId);
                    if (avatarUrl is not null && avatarUrl.Length > 0)
                    {
                        isUploaded = true;
                    }
                    avatarUrl += $"&avatarId={logoId}";
                    existedCashier.Avatar = avatarUrl;
                    await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(FileUtil.GetImageIdFromUrlImage(oldAvatar, "avatarId"), folderName);
                    isDeleted = true;
                }

                this._unitOfWork.CashierRepository.UpdateCashierAsync(existedCashier);
                await this._unitOfWork.CommitAsync();
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistCashierId))
                {
                    fieldName = "Cashier id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistCashierId) ||
                    ex.Message.Equals(MessageConstant.CashierMessage.CashierIdNotBelongToKitchenCenter) ||
                    ex.Message.Equals(MessageConstant.CashierMessage.CashierIdNotBelogToCashier))
                {
                    fieldName = "Cashier id";
                }
                else if (ex.Message.Equals(MessageConstant.CashierMessage.StatusIsRequiredWithKitchenCenterManager) ||
                  ex.Message.Equals(MessageConstant.CashierMessage.StatusIsNotRequiredWithCashier))
                {
                    fieldName = "Status";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                if (isUploaded && logoId.Length > 0 && isDeleted == false)
                {
                    await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(logoId, folderName);
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task UpdateCashierStatusAsync(int idCashier, UpdateCashierStatusRequest updateCashierStatusRequest, IEnumerable<Claim> claims)
        {
            try
            {
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                string email = registeredEmailClaim.Value;

                KitchenCenter existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(email);

                Cashier existedCashier = await this._unitOfWork.CashierRepository.GetCashierAsync(idCashier);
                if (existedCashier is null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistCashierId);
                }

                if (existedKitchenCenter.Cashiers.Any(x => x.AccountId == idCashier) == false)
                {
                    throw new BadRequestException(MessageConstant.CashierMessage.CashierIdNotBelongToKitchenCenter);
                }

                if (AccountEnum.Status.ACTIVE.ToString().ToLower().Equals(updateCashierStatusRequest.Status.Trim().ToLower()))
                {
                    existedCashier.Account.Status = (int)AccountEnum.Status.ACTIVE;
                }
                else if (AccountEnum.Status.INACTIVE.ToString().ToLower().Equals(updateCashierStatusRequest.Status.Trim().ToLower()))
                {
                    existedCashier.Account.Status = (int)AccountEnum.Status.INACTIVE;
                }

                this._unitOfWork.CashierRepository.UpdateCashierAsync(existedCashier);
                await this._unitOfWork.CommitAsync();
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistCashierId))
                {
                    fieldName = "Cashier id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistCashierId) ||
                    ex.Message.Equals(MessageConstant.CashierMessage.CashierIdNotBelongToKitchenCenter))
                {
                    fieldName = "Cashier id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task DeleteCashierAsync(int idCashier, IEnumerable<Claim> claims)
        {
            try
            {
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                string email = registeredEmailClaim.Value;

                KitchenCenter existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(email);

                Cashier existedCashier = await this._unitOfWork.CashierRepository.GetCashierAsync(idCashier);
                if (existedCashier is null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistCashierId);
                }

                if (existedKitchenCenter.Cashiers.Any(x => x.AccountId == idCashier) == false)
                {
                    throw new BadRequestException(MessageConstant.CashierMessage.CashierIdNotBelongToKitchenCenter);
                }

                existedCashier.Account.Status = (int)AccountEnum.Status.DISABLE;

                this._unitOfWork.CashierRepository.UpdateCashierAsync(existedCashier);
                await this._unitOfWork.CommitAsync();
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistCashierId))
                {
                    fieldName = "Cashier id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistCashierId) ||
                    ex.Message.Equals(MessageConstant.CashierMessage.CashierIdNotBelongToKitchenCenter))
                {
                    fieldName = "Cashier id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task<GetCashierReportResponse> GetCashierReportAsync(IEnumerable<Claim> claims)
        {
            try
            {
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                var cashier = await _unitOfWork.CashierRepository.GetCashierReportAsync(registeredEmailClaim.Value);
                // Check cashier end shift or not
                var cashierMoneyExchangeSendToKitchenCenter = cashier.CashierMoneyExchanges.Select(x => x.MoneyExchange)
                                                                .Where(x => x.ExchangeType.Equals(MoneyExchangeEnum.ExchangeType.SEND.ToString()) && x.Transactions
                                                                .Any(x => x.TransactionTime.Date == DateTime.Now.Date)).SingleOrDefault();

                // if cashier ended set isShiftEnded == true => cashier cannot make any more transactions.
                bool isShiftEnded = false;
                if (cashierMoneyExchangeSendToKitchenCenter != null)
                {
                    isShiftEnded = true;
                }
                int? totalOrderToday = null;
                decimal? totalMoneyToday = null;
                decimal? balance = null;
                KitchenCenter? kitchenCenter = null;
                DateTime currentDate = DateTime.Now.Date;

                if (cashier.KitchenCenter.Stores.Select(x => x.Orders).Any())
                {
                    totalOrderToday = cashier.KitchenCenter.Stores.SelectMany(x => x.Orders).Where(x => x.ConfirmedBy == cashier.AccountId).Count(x => x.OrderHistories
                                                                                            .Any(x => x.SystemStatus.Equals(OrderEnum.SystemStatus.COMPLETED.ToString()) &&
                                                                                                        x.PartnerOrderStatus.Equals(OrderEnum.Status.COMPLETED.ToString()) && x.CreatedDate.Date == currentDate.Date));

                    var listShipperPayments = await _unitOfWork.ShipperPaymentRepository.GetShiperPaymentsByCashierIdAsync(cashier.AccountId);
                    totalMoneyToday = listShipperPayments.Where(x => x.CreateDate.Date == currentDate).Select(x => x.Amount).Sum();
                }
                if (cashier.Wallet != null)
                {
                    balance = cashier.Wallet.Balance;
                }
                GetCashierResponse getCashierResponse = _mapper.Map<GetCashierResponse>(cashier);
                getCashierResponse.KitchenCenter.KitchenCenterManagerEmail = cashier.KitchenCenter.Manager.Email;
                return new GetCashierReportResponse
                {
                    Cashier = getCashierResponse,
                    TotalMoneyToday = totalMoneyToday,
                    TotalOrderToday = totalOrderToday,
                    Balance = balance.Value,
                    IsShiftEnded = isShiftEnded
                };
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
    }
}
