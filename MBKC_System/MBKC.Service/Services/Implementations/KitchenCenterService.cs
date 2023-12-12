using AutoMapper;
using MBKC.Service.DTOs.KitchenCenters;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Repository.Enums;
using MBKC.Repository.Infrastructures;
using MBKC.Repository.Models;
using MBKC.Service.Constants;
using MBKC.Service.Utils;
using System.Security.Claims;
using MBKC.Repository.Redis.Models;

namespace MBKC.Service.Services.Implementations
{
    public class KitchenCenterService : IKitchenCenterService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public KitchenCenterService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        public async Task<GetKitchenCentersResponse> GetKitchenCentersAsync(GetKitchenCentersRequest getKitchenCentersRequest)
        {
            try
            {
                int numberItems = 0;
                List<KitchenCenter> kitchenCenters = null;
                if (getKitchenCentersRequest.SearchValue != null && StringUtil.IsUnicode(getKitchenCentersRequest.SearchValue))
                {
                    numberItems = await this._unitOfWork.KitchenCenterRepository.GetNumberKitchenCentersAsync(getKitchenCentersRequest.SearchValue, null);
                    kitchenCenters = await this._unitOfWork.KitchenCenterRepository.GetKitchenCentersAsync(getKitchenCentersRequest.SearchValue, null, getKitchenCentersRequest.CurrentPage, getKitchenCentersRequest.ItemsPerPage,
                                                                                                              getKitchenCentersRequest.SortBy != null && getKitchenCentersRequest.SortBy.ToLower().EndsWith("asc") ? getKitchenCentersRequest.SortBy.Split("_")[0] : null,
                                                                                                              getKitchenCentersRequest.SortBy != null && getKitchenCentersRequest.SortBy.ToLower().EndsWith("desc") ? getKitchenCentersRequest.SortBy.Split("_")[0] : null,
                                                                                                              getKitchenCentersRequest.IsGetAll);
                }
                else if (getKitchenCentersRequest.SearchValue != null && StringUtil.IsUnicode(getKitchenCentersRequest.SearchValue) == false)
                {
                    numberItems = await this._unitOfWork.KitchenCenterRepository.GetNumberKitchenCentersAsync(null, getKitchenCentersRequest.SearchValue);
                    kitchenCenters = await this._unitOfWork.KitchenCenterRepository.GetKitchenCentersAsync(null, getKitchenCentersRequest.SearchValue, getKitchenCentersRequest.CurrentPage, getKitchenCentersRequest.ItemsPerPage,
                                                                                                              getKitchenCentersRequest.SortBy != null && getKitchenCentersRequest.SortBy.ToLower().EndsWith("asc") ? getKitchenCentersRequest.SortBy.Split("_")[0] : null,
                                                                                                              getKitchenCentersRequest.SortBy != null && getKitchenCentersRequest.SortBy.ToLower().EndsWith("desc") ? getKitchenCentersRequest.SortBy.Split("_")[0] : null,
                                                                                                              getKitchenCentersRequest.IsGetAll);
                }
                else if (getKitchenCentersRequest.SearchValue == null)
                {
                    numberItems = await this._unitOfWork.KitchenCenterRepository.GetNumberKitchenCentersAsync(null, null);
                    kitchenCenters = await this._unitOfWork.KitchenCenterRepository.GetKitchenCentersAsync(null, null, getKitchenCentersRequest.CurrentPage, getKitchenCentersRequest.ItemsPerPage,
                                                                                                              getKitchenCentersRequest.SortBy != null && getKitchenCentersRequest.SortBy.ToLower().EndsWith("asc") ? getKitchenCentersRequest.SortBy.Split("_")[0] : null,
                                                                                                              getKitchenCentersRequest.SortBy != null && getKitchenCentersRequest.SortBy.ToLower().EndsWith("desc") ? getKitchenCentersRequest.SortBy.Split("_")[0] : null,
                                                                                                              getKitchenCentersRequest.IsGetAll);
                }
                int totalPages = 0;
                if (numberItems > 0 && getKitchenCentersRequest.IsGetAll == null || numberItems > 0 && getKitchenCentersRequest.IsGetAll != null && getKitchenCentersRequest.IsGetAll == false)
                {
                    totalPages = (int)((numberItems + getKitchenCentersRequest.ItemsPerPage) / getKitchenCentersRequest.ItemsPerPage);
                }

                if (numberItems == 0)
                {
                    totalPages = 0;
                }
                List<GetKitchenCenterResponse> getKitchenCenterResponses = this._mapper.Map<List<GetKitchenCenterResponse>>(kitchenCenters);
                GetKitchenCentersResponse getKitchenCenters = new GetKitchenCentersResponse()
                {
                    NumberItems = numberItems,
                    TotalPages = totalPages,
                    KitchenCenters = getKitchenCenterResponses
                };
                return getKitchenCenters;
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task<GetKitchenCenterResponse> GetKitchenCenterAsync(int kitchenCenterId)
        {
            try
            {
                KitchenCenter kitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(kitchenCenterId);
                if (kitchenCenter == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistKitchenCenterId);
                }
                GetKitchenCenterResponse getKitchenCenterResponse = this._mapper.Map<GetKitchenCenterResponse>(kitchenCenter);
                return getKitchenCenterResponse;
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Kitchen center id", ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task CreateKitchenCenterAsync(CreateKitchenCenterRequest newKitchenCenter)
        {
            string logoId = "";
            string folderName = "KitchenCenters";
            bool isUploaded = false;
            try
            {
                Account existedAccount = await this._unitOfWork.AccountRepository.GetAccountAsync(newKitchenCenter.ManagerEmail);
                if (existedAccount != null)
                {
                    throw new BadRequestException(MessageConstant.KitchenCenterMessage.ManagerEmailExisted);
                }
                Role role = await this._unitOfWork.RoleRepository.GetRoleAsync((int)RoleEnum.Role.KITCHEN_CENTER_MANAGER);
                string password = PasswordUtil.CreateRandomPassword();
                Account managerAccount = new Account()
                {
                    Email = newKitchenCenter.ManagerEmail,
                    Password = StringUtil.EncryptData(password),
                    Status = (int)AccountEnum.Status.ACTIVE,
                    Role = role,
                    IsConfirmed = false
                };
                Wallet wallet = new Wallet()
                {
                    Balance = 0
                };
                Guid guid = Guid.NewGuid();
                logoId = guid.ToString();
                FileStream logoFileStream = FileUtil.ConvertFormFileToStream(newKitchenCenter.Logo);
                string logoLink = await this._unitOfWork.FirebaseStorageRepository.UploadImageAsync(logoFileStream, folderName, logoId);
                if (logoLink != null && logoLink.Length > 0)
                {
                    isUploaded = true;
                }
                logoLink += $"&logoId={logoId}";
                KitchenCenter kitchenCenter = new KitchenCenter()
                {
                    Name = newKitchenCenter.Name,
                    Address = newKitchenCenter.Address,
                    Manager = managerAccount,
                    Status = (int)KitchenCenterEnum.Status.ACTIVE,
                    Wallet = wallet,
                    Logo = logoLink
                };
                await this._unitOfWork.KitchenCenterRepository.CreateKitchenCenterAsync(kitchenCenter);
                await this._unitOfWork.CommitAsync();
                string messageBody = EmailMessageConstant.KitchenCenter.Message + $" \"{newKitchenCenter.Name}\". " + EmailMessageConstant.CommonMessage.Message;
                string message = this._unitOfWork.EmailRepository.GetMessageToRegisterAccount(newKitchenCenter.ManagerEmail, password, messageBody);
                await this._unitOfWork.EmailRepository.SendAccountToEmailAsync(newKitchenCenter.ManagerEmail, message);
            }
            catch (BadRequestException ex)
            {
                string error = ErrorUtil.GetErrorString("Manager email", ex.Message);
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

        public async Task UpdateKitchenCenterAsync(int kitchenCenterId, UpdateKitchenCenterRequest updatedKitchenCenter)
        {
            string folderName = "KitchenCenters";
            bool isUploaded = false;
            bool isDeleted = false;
            string logoId = "";
            bool isNewManager = false;
            try
            {
                KitchenCenter existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(kitchenCenterId);
                if (existedKitchenCenter == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistKitchenCenterId);
                }

                if (existedKitchenCenter.Status == (int)KitchenCenterEnum.Status.DISABLE)
                {
                    throw new BadRequestException(MessageConstant.KitchenCenterMessage.DeactiveKitchenCenter_Update);
                }

                string password = "";
                if (existedKitchenCenter.Manager.Email.Equals(updatedKitchenCenter.ManagerEmail) == false)
                {
                    Account existedAccount = await this._unitOfWork.AccountRepository.GetAccountAsync(updatedKitchenCenter.ManagerEmail);
                    if (existedAccount != null)
                    {
                        throw new BadRequestException(MessageConstant.KitchenCenterMessage.ManagerEmailExisted);
                    }

                    existedKitchenCenter.Manager.Status = (int)AccountEnum.Status.DISABLE;
                    this._unitOfWork.AccountRepository.UpdateAccount(existedKitchenCenter.Manager);

                    Role kitchenCenterManagerRole = await this._unitOfWork.RoleRepository.GetRoleAsync((int)RoleEnum.Role.KITCHEN_CENTER_MANAGER);
                    password = PasswordUtil.CreateRandomPassword();
                    Account newManagerAccount = new Account()
                    {
                        Email = updatedKitchenCenter.ManagerEmail,
                        Password = StringUtil.EncryptData(password),
                        Status = (int)AccountEnum.Status.ACTIVE,
                        Role = kitchenCenterManagerRole
                    };
                    existedKitchenCenter.Manager = newManagerAccount;
                    isNewManager = true;
                }

                string oldLogo = existedKitchenCenter.Logo;
                if (updatedKitchenCenter.Logo != null)
                {
                    Guid guid = Guid.NewGuid();
                    logoId = guid.ToString();
                    FileStream logoFileStream = FileUtil.ConvertFormFileToStream(updatedKitchenCenter.Logo);
                    string logoLink = await this._unitOfWork.FirebaseStorageRepository.UploadImageAsync(logoFileStream, folderName, logoId);
                    if (logoLink != null && logoLink.Length > 0)
                    {
                        isUploaded = true;
                    }
                    logoLink += $"&logoId={logoId}";
                    existedKitchenCenter.Logo = logoLink;
                    await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(FileUtil.GetImageIdFromUrlImage(oldLogo, "logoId"), folderName);
                    isDeleted = true;
                }

                existedKitchenCenter.Name = updatedKitchenCenter.Name;
                existedKitchenCenter.Address = updatedKitchenCenter.Address;
                if (updatedKitchenCenter.Status.ToLower().Equals(KitchenCenterEnum.Status.ACTIVE.ToString().ToLower()))
                {
                    existedKitchenCenter.Status = (int)KitchenCenterEnum.Status.ACTIVE;
                }
                else if (updatedKitchenCenter.Status.ToLower().Equals(KitchenCenterEnum.Status.INACTIVE.ToString().ToLower()))
                {
                    existedKitchenCenter.Status = (int)KitchenCenterEnum.Status.INACTIVE;
                }
                this._unitOfWork.KitchenCenterRepository.UpdateKitchenCenter(existedKitchenCenter);
                await this._unitOfWork.CommitAsync();

                if (isNewManager)
                {
                    string messageBody = EmailMessageConstant.KitchenCenter.Message + $" \"{existedKitchenCenter.Name}\". " + EmailMessageConstant.CommonMessage.Message;
                    string message = this._unitOfWork.EmailRepository.GetMessageToRegisterAccount(updatedKitchenCenter.ManagerEmail, password, messageBody);
                    await this._unitOfWork.EmailRepository.SendAccountToEmailAsync(updatedKitchenCenter.ManagerEmail, message);
                }
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Kitchen Center Id", ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.KitchenCenterMessage.ManagerEmailExisted))
                {
                    fieldName = "Manager email";
                }
                else if (ex.Message.Equals(MessageConstant.KitchenCenterMessage.DeactiveKitchenCenter_Update))
                {
                    fieldName = "Updated kitchen center failed";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
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

        public async Task UpdateKitchenCenterStatusAsync(int kitchenCenterId, UpdateKitchenCenterStatusRequest updateKitchenCenterStatus)
        {
            try
            {
                KitchenCenter existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(kitchenCenterId);
                if (existedKitchenCenter == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistKitchenCenterId);
                }
                else if (existedKitchenCenter.Status == (int)KitchenCenterEnum.Status.DISABLE)
                {
                    throw new BadRequestException(MessageConstant.KitchenCenterMessage.DeactiveKitchenCenter_Update);
                }
                if (updateKitchenCenterStatus.Status.Trim().ToLower().Equals(KitchenCenterEnum.Status.ACTIVE.ToString().ToLower()))
                {
                    existedKitchenCenter.Status = (int)KitchenCenterEnum.Status.ACTIVE;
                }
                else if (updateKitchenCenterStatus.Status.Trim().ToLower().Equals(KitchenCenterEnum.Status.INACTIVE.ToString().ToLower()))
                {
                    existedKitchenCenter.Status = (int)KitchenCenterEnum.Status.INACTIVE;
                }
                this._unitOfWork.KitchenCenterRepository.UpdateKitchenCenter(existedKitchenCenter);
                await this._unitOfWork.CommitAsync();

            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.KitchenCenterMessage.DeactiveKitchenCenter_Update))
                {
                    fieldName = "Updated kitchen center failed";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Kitchen center id", ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task DeleteKitchenCenterAsync(int kitchenCenterId)
        {
            try
            {
                KitchenCenter existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(kitchenCenterId);
                if (existedKitchenCenter == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistKitchenCenterId);
                }

                if (existedKitchenCenter.Stores != null && existedKitchenCenter.Stores.Count() > 0 && existedKitchenCenter.Stores.Any(x => x.Status == (int)StoreEnum.Status.ACTIVE))
                {
                    throw new BadRequestException(MessageConstant.KitchenCenterMessage.ExistedActiveStores_Delete);
                }

                if (existedKitchenCenter.Cashiers.Any())
                {
                    foreach (var cashier in existedKitchenCenter.Cashiers)
                    {
                        cashier.Account.Status = (int)AccountEnum.Status.DISABLE;
                    }
                }
                // Deactive kitchen center.
                existedKitchenCenter.Status = (int)KitchenCenterEnum.Status.DISABLE;
                // Deactive kitchen center manger.
                existedKitchenCenter.Manager.Status = (int)AccountEnum.Status.DISABLE;
                this._unitOfWork.KitchenCenterRepository.UpdateKitchenCenter(existedKitchenCenter);
                await this._unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.KitchenCenterMessage.ExistedActiveStores_Delete))
                {
                    fieldName = "Deleted kitchen center failed";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Kitchen center id", ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task<GetKitchenCenterResponse> GetKitchenCenterProfileAsync(IEnumerable<Claim> claims)
        {
            try
            {
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                string email = registeredEmailClaim.Value;

                KitchenCenter kitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(email);
                GetKitchenCenterResponse getKitchenCenterResponse = this._mapper.Map<GetKitchenCenterResponse>(kitchenCenter);
                return getKitchenCenterResponse;
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
    }
}
