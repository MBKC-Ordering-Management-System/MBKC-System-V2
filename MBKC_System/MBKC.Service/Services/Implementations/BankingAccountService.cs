using AutoMapper;
using MBKC.Service.Services.Interfaces;
using MBKC.Repository.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBKC.Repository.Models;
using MBKC.Service.Constants;
using MBKC.Service.DTOs.BankingAccounts;
using MBKC.Service.Exceptions;
using MBKC.Service.Utils;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using MBKC.Repository.Enums;

namespace MBKC.Service.Services.Implementations
{
    public class BankingAccountService : IBankingAccountService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public BankingAccountService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        public async Task<GetBankingAccountsResponse> GetBankingAccountsAsync(string? searchValue, int? currentPage, int? itemsPerPage, IEnumerable<Claim> claims)
        {
            try
            {
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                string email = registeredEmailClaim.Value;
                KitchenCenter existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(email);
                if (existedKitchenCenter == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistKitchenCenter);
                }
                if (currentPage != null && currentPage <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidCurrentPage);
                }
                else
                {
                    currentPage = 1;
                }

                if (itemsPerPage != null && itemsPerPage <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidItemsPerPage);
                }
                else
                {
                    itemsPerPage = 5;
                }

                int numberItems = 0;
                List<BankingAccount> bankingAccounts = null;
                if (searchValue != null && StringUtil.IsUnicode(searchValue))
                {
                    numberItems = await this._unitOfWork.BankingAccountRepository.GetNumberBankingAccountsAsync(existedKitchenCenter.KitchenCenterId, searchValue, null);
                    bankingAccounts = await this._unitOfWork.BankingAccountRepository.GetBankingAccountsAsync(existedKitchenCenter.KitchenCenterId, searchValue, null, currentPage.Value, itemsPerPage.Value);
                }
                else if (searchValue != null && StringUtil.IsUnicode(searchValue) == false)
                {
                    numberItems = await this._unitOfWork.BankingAccountRepository.GetNumberBankingAccountsAsync(existedKitchenCenter.KitchenCenterId, null, searchValue);
                    bankingAccounts = await this._unitOfWork.BankingAccountRepository.GetBankingAccountsAsync(existedKitchenCenter.KitchenCenterId, null, searchValue, currentPage.Value, itemsPerPage.Value);
                }
                else if (searchValue == null)
                {
                    numberItems = await this._unitOfWork.BankingAccountRepository.GetNumberBankingAccountsAsync(existedKitchenCenter.KitchenCenterId, null, null);
                    bankingAccounts = await this._unitOfWork.BankingAccountRepository.GetBankingAccountsAsync(existedKitchenCenter.KitchenCenterId, null, null, currentPage.Value, itemsPerPage.Value);
                }

                int totalPages = (int)((numberItems + itemsPerPage) / itemsPerPage);
                if (numberItems == 0)
                {
                    totalPages = 0;
                }

                List<GetBankingAccountResponse> getBankingAccountResponses = this._mapper.Map<List<GetBankingAccountResponse>>(bankingAccounts);
                return new GetBankingAccountsResponse()
                {
                    TotalPages = totalPages,
                    NumberItems = numberItems,
                    BankingAccounts = getBankingAccountResponses
                };
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidCurrentPage))
                {
                    fieldName = "Current page";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidItemsPerPage))
                {
                    fieldName = "Items per page";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistKitchenCenter))
                {
                    fieldName = "Get kitchen center failed";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task<GetBankingAccountResponse> GetBankingAccountAsync(int bankingAccountId, IEnumerable<Claim> claims)
        {
            try
            {
                if (bankingAccountId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidBankingAccountId);
                }
                BankingAccount existedBankingAccount = await this._unitOfWork.BankingAccountRepository.GetBankingAccountAsync(bankingAccountId);
                if (existedBankingAccount == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBankingAccountId);
                }
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                string email = registeredEmailClaim.Value;
                KitchenCenter existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(email);
                if (existedKitchenCenter == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistKitchenCenter);
                }
                if (existedBankingAccount.KitchenCenterId != existedKitchenCenter.KitchenCenterId)
                {
                    throw new BadRequestException(MessageConstant.BankingAccountMessage.BankingAccountNotBelongToKitchenCenter);
                }

                GetBankingAccountResponse getBankingAccountResponse = this._mapper.Map<GetBankingAccountResponse>(existedBankingAccount);
                return getBankingAccountResponse;
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidBankingAccountId)
                    || ex.Message.Equals(MessageConstant.BankingAccountMessage.BankingAccountNotBelongToKitchenCenter))
                {
                    fieldName = "Banking account id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistBankingAccountId))
                {
                    fieldName = "Banking account id";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistKitchenCenter))
                {
                    fieldName = "Get banking account failed";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task CreateBankingAccountAsync(CreateBankingAccountRequest createBankingAccountRequest, IEnumerable<Claim> claims)
        {
            string folderName = "BankingAccounts";
            bool isUploaded = false;
            string logoId = "";
            try
            {
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                string email = registeredEmailClaim.Value;
                KitchenCenter existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(email);
                if (existedKitchenCenter == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistKitchenCenter);
                }
                BankingAccount existedBankingAccount = await this._unitOfWork.BankingAccountRepository.GetBankingAccountAsync(createBankingAccountRequest.NumberAccount);
                if (existedBankingAccount != null)
                {
                    throw new BadRequestException(MessageConstant.BankingAccountMessage.NumberAccountExisted);
                }

                Guid guid = Guid.NewGuid();
                logoId = guid.ToString();
                FileStream BankLogoStream = FileUtil.ConvertFormFileToStream(createBankingAccountRequest.BankLogo);
                string logoUrl = await this._unitOfWork.FirebaseStorageRepository.UploadImageAsync(BankLogoStream, folderName, logoId);
                if (logoUrl != null && logoUrl.Length > 0)
                {
                    isUploaded = true;
                }
                BankingAccount bankingAccount = new BankingAccount()
                {
                    KitchenCenter = existedKitchenCenter,
                    KitchenCenterId = existedKitchenCenter.KitchenCenterId,
                    LogoUrl = logoUrl + $"&logoId={logoId}",
                    Name = createBankingAccountRequest.BankName.Trim(),
                    NumberAccount = createBankingAccountRequest.NumberAccount.Trim(),
                    Status = (int)BankingAccountEnum.Status.ACTIVE
                };

                await this._unitOfWork.BankingAccountRepository.CreateBankingAccountAsync(bankingAccount);
                await this._unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {
                string error = ErrorUtil.GetErrorString("Number account", ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Created banking account failed", ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                if (isUploaded)
                {
                    await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(logoId, folderName);
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task DeleteBankingAccountAsync(int bankingAccountId, IEnumerable<Claim> claims)
        {
            try
            {
                if (bankingAccountId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidBankingAccountId);
                }
                BankingAccount existedBankingAccount = await this._unitOfWork.BankingAccountRepository.GetBankingAccountAsync(bankingAccountId);
                if (existedBankingAccount == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBankingAccountId);
                }
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                string email = registeredEmailClaim.Value;
                KitchenCenter existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(email);
                if (existedKitchenCenter == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistKitchenCenter);
                }
                if (existedBankingAccount.KitchenCenterId != existedKitchenCenter.KitchenCenterId)
                {
                    throw new BadRequestException(MessageConstant.BankingAccountMessage.BankingAccountNotBelongToKitchenCenter);
                }

                existedBankingAccount.Status = (int)BankingAccountEnum.Status.DEACTIVE;
                this._unitOfWork.BankingAccountRepository.UpdateBankingAccount(existedBankingAccount);
                await this._unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidBankingAccountId)
                    || ex.Message.Equals(MessageConstant.BankingAccountMessage.BankingAccountNotBelongToKitchenCenter))
                {
                    fieldName = "Banking account id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistBankingAccountId))
                {
                    fieldName = "Banking account id";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistKitchenCenter))
                {
                    fieldName = "Get banking account failed";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task UpdateBankingAccountStatusAsync(int bankingAccountId, UpdateBankingAccountStatusRequest updateBankingAccountStatusRequest, IEnumerable<Claim> claims)
        {
            try
            {
                if (bankingAccountId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidBankingAccountId);
                }
                BankingAccount existedBankingAccount = await this._unitOfWork.BankingAccountRepository.GetBankingAccountAsync(bankingAccountId);
                if (existedBankingAccount == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBankingAccountId);
                }
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                string email = registeredEmailClaim.Value;
                KitchenCenter existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(email);
                if (existedKitchenCenter == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistKitchenCenter);
                }
                if (existedBankingAccount.KitchenCenterId != existedKitchenCenter.KitchenCenterId)
                {
                    throw new BadRequestException(MessageConstant.BankingAccountMessage.BankingAccountNotBelongToKitchenCenter);
                }

                if (updateBankingAccountStatusRequest.Status.Trim().ToLower().Equals(BankingAccountEnum.Status.ACTIVE.ToString().ToLower()))
                {
                    existedBankingAccount.Status = (int)BankingAccountEnum.Status.ACTIVE;
                }
                else if (updateBankingAccountStatusRequest.Status.Trim().ToLower().Equals(BankingAccountEnum.Status.INACTIVE.ToString().ToLower()))
                {
                    existedBankingAccount.Status = (int)BankingAccountEnum.Status.INACTIVE;
                }

                this._unitOfWork.BankingAccountRepository.UpdateBankingAccount(existedBankingAccount);
                await this._unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidBankingAccountId)
                    || ex.Message.Equals(MessageConstant.BankingAccountMessage.BankingAccountNotBelongToKitchenCenter))
                {
                    fieldName = "Banking account id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistBankingAccountId))
                {
                    fieldName = "Banking account id";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistKitchenCenter))
                {
                    fieldName = "Get banking account failed";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task UpdateBankingAccountAsync(int bankingAccountId, UpdateBankingAccountRequest updateBankingAccountRequest, IEnumerable<Claim> claims)
        {
            string folderName = "BankingAccounts";
            bool isUploaded = false;
            bool isDeleted = false;
            string logoId = "";
            try
            {
                if (bankingAccountId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidBankingAccountId);
                }
                BankingAccount existedBankingAccount = await this._unitOfWork.BankingAccountRepository.GetBankingAccountAsync(bankingAccountId);
                if (existedBankingAccount == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBankingAccountId);
                }
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                string email = registeredEmailClaim.Value;
                KitchenCenter existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(email);
                if (existedKitchenCenter == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistKitchenCenter);
                }
                if (existedBankingAccount.KitchenCenterId != existedKitchenCenter.KitchenCenterId)
                {
                    throw new BadRequestException(MessageConstant.BankingAccountMessage.BankingAccountNotBelongToKitchenCenter);
                }

                string oldLogo = existedBankingAccount.LogoUrl;
                if (updateBankingAccountRequest.BankLogo != null)
                {
                    Guid guid = Guid.NewGuid();
                    logoId = guid.ToString();
                    FileStream logoFileStream = FileUtil.ConvertFormFileToStream(updateBankingAccountRequest.BankLogo);
                    string logoLink = await this._unitOfWork.FirebaseStorageRepository.UploadImageAsync(logoFileStream, folderName, logoId);
                    if (logoLink != null && logoLink.Length > 0)
                    {
                        isUploaded = true;
                    }
                    logoLink += $"&logoId={logoId}";
                    existedBankingAccount.LogoUrl = logoLink;
                    await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(FileUtil.GetImageIdFromUrlImage(oldLogo, "logoId"), folderName);
                    isDeleted = true;
                }

                if (updateBankingAccountRequest.Status.Trim().ToLower().Equals(BankingAccountEnum.Status.ACTIVE.ToString().ToLower()))
                {
                    existedBankingAccount.Status = (int)BankingAccountEnum.Status.ACTIVE;
                }
                else if (updateBankingAccountRequest.Status.Trim().ToLower().Equals(BankingAccountEnum.Status.INACTIVE.ToString().ToLower()))
                {
                    existedBankingAccount.Status = (int)BankingAccountEnum.Status.INACTIVE;
                }

                existedBankingAccount.Name = updateBankingAccountRequest.BankName;
                this._unitOfWork.BankingAccountRepository.UpdateBankingAccount(existedBankingAccount);
                await this._unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidBankingAccountId)
                    || ex.Message.Equals(MessageConstant.BankingAccountMessage.BankingAccountNotBelongToKitchenCenter))
                {
                    fieldName = "Banking account id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistBankingAccountId))
                {
                    fieldName = "Banking account id";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistKitchenCenter))
                {
                    fieldName = "Get banking account failed";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                if (isUploaded && isDeleted == false)
                {
                    await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(logoId, folderName);
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
    }
}