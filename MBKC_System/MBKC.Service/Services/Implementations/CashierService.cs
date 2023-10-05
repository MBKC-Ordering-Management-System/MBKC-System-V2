﻿using AutoMapper;
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

                Cashier existedCashierWithEmail = await this._unitOfWork.CashierRepository.GetCashierAsync(createCashierRequest.Email);
                if (existedCashierWithEmail is not null)
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
                string password = RandomPasswordUtil.CreateRandomPassword();
                Account cashierAccount = new Account()
                {
                    Email = createCashierRequest.Email,
                    Password = StringUtil.EncryptData(password),
                    Role = cashierRole,
                    Status = (int)AccountEnum.Status.ACTIVE
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
                await this._unitOfWork.EmailRepository.SendEmailAndPasswordToEmail(createCashierRequest.Email, message);
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
                string email = registeredEmailClaim.Value;

                KitchenCenter existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(email);

                Cashier existedCashier = await this._unitOfWork.CashierRepository.GetCashierAsync(idCashier);
                if (existedCashier is null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistCashierId);
                }

                if(existedKitchenCenter.Cashiers.Any(x => x.AccountId == idCashier) == false)
                {
                    throw new BadRequestException(MessageConstant.CashierMessage.CashierIdNotBelongToKitchenCenter);
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
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistCashierId))
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

                if (updateCashierRequest.NewPassword is not null)
                {
                    existedCashier.Account.Password = updateCashierRequest.NewPassword;
                }

                existedCashier.FullName = updateCashierRequest.FullName;
                existedCashier.DateOfBirth = updateCashierRequest.DateOfBirth;
                existedCashier.CitizenNumber = updateCashierRequest.CitizenNumber;
                existedCashier.Gender = CashierEnum.Gender.FEMALE.ToString().ToLower().Equals(updateCashierRequest.Gender.Trim().ToLower()) ? false : true;

                if (AccountEnum.Status.ACTIVE.ToString().ToLower().Equals(updateCashierRequest.Status.Trim().ToLower()))
                {
                    existedCashier.Account.Status = (int)AccountEnum.Status.ACTIVE;
                }
                else if (AccountEnum.Status.INACTIVE.ToString().ToLower().Equals(updateCashierRequest.Status.Trim().ToLower()))
                {
                    existedCashier.Account.Status = (int)AccountEnum.Status.INACTIVE;
                }

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
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistCashierId))
                {
                    fieldName = "Cashier id";
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
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistCashierId))
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

                existedCashier.Account.Status = (int)AccountEnum.Status.DEACTIVE;

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
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistCashierId))
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
    }
}
