using AutoMapper;
using MBKC.Service.Services.Interfaces;
using MBKC.Repository.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBKC.Service.DTOs.Cashiers;
using MBKC.API.Validators.Cashiers;
using System.Security.Claims;
using MBKC.Service.Utils;
using MBKC.Repository.Models;
using MBKC.Service.Exceptions;
using MBKC.Service.Constants;
using MBKC.Repository.Enums;

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

                }
                else if (getCashiersRequest.SearchValue is not null && StringUtil.IsUnicode(getCashiersRequest.SearchValue))
                {

                }
                else if (getCashiersRequest.SearchValue is null)
                {

                }
                cashiers = await this._unitOfWork.CashierRepository.GetCashiersAsync(null, null, getCashiersRequest.CurrentPage.Value, getCashiersRequest.ItemsPerPage.Value, getCashiersRequest.SortBy != null ? getCashiersRequest.SortBy.Split("_")[0] : null, null);
                List<GetCashierResponse> getCashierResponses = this._mapper.Map<List<GetCashierResponse>>(cashiers);
                return new GetCashiersResponse()
                {
                    TotalPages = 0,
                    NumberItems = 0,
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
            catch(BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.AlreadyExistEmail))
                {
                    fieldName = "Email";
                } else if (ex.Message.Equals(MessageConstant.CommonMessage.AlreadyExistCitizenNumber))
                {
                    fieldName = "Citizen number";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                if(isUploaded && logoId.Length > 0)
                {
                    await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(logoId, folderName);
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
    }
}
