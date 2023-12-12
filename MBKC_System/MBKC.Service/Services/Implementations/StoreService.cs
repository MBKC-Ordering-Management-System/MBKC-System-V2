using AutoMapper;
using MBKC.Repository.Enums;
using MBKC.Repository.Infrastructures;
using MBKC.Repository.Models;
using MBKC.Repository.Redis.Models;
using MBKC.Service.Constants;
using MBKC.Service.DTOs.Stores;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using System.Security.Claims;
using static MBKC.Service.Constants.EmailMessageConstant;
using Brand = MBKC.Repository.Models.Brand;
using KitchenCenter = MBKC.Repository.Models.KitchenCenter;
using Store = MBKC.Repository.Models.Store;

namespace MBKC.Service.Services.Implementations
{
    public class StoreService : IStoreService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public StoreService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        public async Task<GetStoresResponse> GetStoresAsync(GetStoresRequest getStoresRequest, IEnumerable<Claim> claims)
        {
            try
            {

                int? statusParam = null;
                if (getStoresRequest.Status != null)
                {
                    statusParam = StatusUtil.ChangeStoreStatus(getStoresRequest.Status);
                }
                Brand existedBrand = null;
                if (getStoresRequest.IdBrand != null)
                {
                    existedBrand = await this._unitOfWork.BrandRepository.GetBrandByIdAsync(getStoresRequest.IdBrand.Value);
                    if (existedBrand == null)
                    {
                        throw new NotFoundException(MessageConstant.CommonMessage.NotExistBrandId);
                    }
                }
                KitchenCenter existedKitchenCenter = null;
                if (getStoresRequest.IdKitchenCenter != null)
                {
                    existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(getStoresRequest.IdKitchenCenter.Value);
                    if (existedKitchenCenter == null)
                    {
                        throw new NotFoundException(MessageConstant.CommonMessage.NotExistKitchenCenterId);
                    }
                }

                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                Claim registeredRoleClaim = claims.First(x => x.Type.ToLower().Equals("role"));
                string email = registeredEmailClaim.Value;
                string role = registeredRoleClaim.Value;
                if (claims != null && getStoresRequest.IdBrand != null)
                {
                    if (role.ToLower().Equals(RoleConstant.Brand_Manager.ToLower()))
                    {
                        if (existedBrand.BrandManagerEmail.Equals(email) == false)
                        {
                            throw new BadRequestException(MessageConstant.BrandMessage.NotBelongToBrand);
                        }
                        if (getStoresRequest.IdKitchenCenter != null && existedBrand.Stores.Any(x => x.KitchenCenter.KitchenCenterId == getStoresRequest.IdKitchenCenter) == false)
                        {
                            throw new BadRequestException(MessageConstant.StoreMessage.BrandNotJoinKitchenCenter);
                        }
                    }
                }
                else if (claims != null && getStoresRequest.IdBrand == null)
                {
                    if (role.ToLower().Equals(RoleConstant.Brand_Manager.ToLower()))
                    {
                        existedBrand = await this._unitOfWork.BrandRepository.GetBrandAsync(email);
                        getStoresRequest.IdBrand = existedBrand.BrandId;
                    }
                }

                if (claims != null && getStoresRequest.IdKitchenCenter != null)
                {
                    if (role.ToLower().Equals(RoleConstant.Kitchen_Center_Manager.ToLower()))
                    {
                        if (existedKitchenCenter.Manager.Email.Equals(email) == false)
                        {
                            throw new BadRequestException(MessageConstant.KitchenCenterMessage.NotBelongToKitchenCenter);
                        }
                        if (getStoresRequest.IdBrand != null && existedKitchenCenter.Stores.Any(x => x.Brand.BrandId == getStoresRequest.IdBrand) == false)
                        {
                            throw new BadRequestException(MessageConstant.StoreMessage.KitchenCenterNotHaveBrand);
                        }
                    }
                }
                else if (claims != null && getStoresRequest.IdKitchenCenter == null)
                {
                    if (role.ToLower().Equals(RoleConstant.Kitchen_Center_Manager.ToLower()))
                    {
                        existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(email);
                        getStoresRequest.IdKitchenCenter = existedKitchenCenter.KitchenCenterId;
                    }
                }

                int numberItems = 0;
                List<Store> stores = null;
                if (getStoresRequest.SearchValue != null && StringUtil.IsUnicode(getStoresRequest.SearchValue))
                {
                    numberItems = await this._unitOfWork.StoreRepository.GetNumberStoresAsync(getStoresRequest.SearchValue, null, getStoresRequest.IdBrand, getStoresRequest.IdKitchenCenter, statusParam);
                    stores = await this._unitOfWork.StoreRepository.GetStoresAsync(getStoresRequest.SearchValue, null, getStoresRequest.CurrentPage, getStoresRequest.ItemsPerPage,
                                                                                                              getStoresRequest.SortBy != null && getStoresRequest.SortBy.ToLower().EndsWith("asc") ? getStoresRequest.SortBy.Split("_")[0] : null,
                                                                                                              getStoresRequest.SortBy != null && getStoresRequest.SortBy.ToLower().EndsWith("desc") ? getStoresRequest.SortBy.Split("_")[0] : null,
                                                                                                              getStoresRequest.IdBrand, getStoresRequest.IdKitchenCenter, statusParam, getStoresRequest.IsGetAll);
                }
                else if (getStoresRequest.SearchValue != null && StringUtil.IsUnicode(getStoresRequest.SearchValue) == false)
                {
                    numberItems = await this._unitOfWork.StoreRepository.GetNumberStoresAsync(null, getStoresRequest.SearchValue, getStoresRequest.IdBrand, getStoresRequest.IdKitchenCenter, statusParam);
                    stores = await this._unitOfWork.StoreRepository.GetStoresAsync(null, getStoresRequest.SearchValue, getStoresRequest.CurrentPage, getStoresRequest.ItemsPerPage,
                                                                                                              getStoresRequest.SortBy != null && getStoresRequest.SortBy.ToLower().EndsWith("asc") ? getStoresRequest.SortBy.Split("_")[0] : null,
                                                                                                              getStoresRequest.SortBy != null && getStoresRequest.SortBy.ToLower().EndsWith("desc") ? getStoresRequest.SortBy.Split("_")[0] : null,
                                                                                                              getStoresRequest.IdBrand, getStoresRequest.IdKitchenCenter, statusParam, getStoresRequest.IsGetAll);
                }
                else if (getStoresRequest.SearchValue == null)
                {
                    numberItems = await this._unitOfWork.StoreRepository.GetNumberStoresAsync(null, null, getStoresRequest.IdBrand, getStoresRequest.IdKitchenCenter, statusParam);
                    stores = await this._unitOfWork.StoreRepository.GetStoresAsync(null, null, getStoresRequest.CurrentPage, getStoresRequest.ItemsPerPage,
                                                                                                              getStoresRequest.SortBy != null && getStoresRequest.SortBy.ToLower().EndsWith("asc") ? getStoresRequest.SortBy.Split("_")[0] : null,
                                                                                                              getStoresRequest.SortBy != null && getStoresRequest.SortBy.ToLower().EndsWith("desc") ? getStoresRequest.SortBy.Split("_")[0] : null,
                                                                                                              getStoresRequest.IdBrand, getStoresRequest.IdKitchenCenter, statusParam, getStoresRequest.IsGetAll);
                }

                int totalPages = 0;
                if (numberItems > 0 && getStoresRequest.IsGetAll == null || numberItems > 0 && getStoresRequest.IsGetAll != null && getStoresRequest.IsGetAll == false)
                {
                    totalPages = (int)((numberItems + getStoresRequest.ItemsPerPage) / getStoresRequest.ItemsPerPage);
                }

                if (numberItems == 0)
                {
                    totalPages = 0;
                }
                List<GetStoreResponse> getStoreResponses = this._mapper.Map<List<GetStoreResponse>>(stores);
                GetStoresResponse getStores = new GetStoresResponse()
                {
                    NumberItems = numberItems,
                    TotalPages = totalPages,
                    Stores = getStoreResponses
                };
                return getStores;
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistBrandId) ||
                    ex.Message.Equals(MessageConstant.StoreMessage.BrandNotJoinKitchenCenter))
                {
                    fieldName = "Brand id";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistKitchenCenterId) ||
                    ex.Message.Equals(MessageConstant.StoreMessage.KitchenCenterNotHaveBrand))
                {
                    fieldName = "Kitchen center id";
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

        public async Task<GetStoreResponse> GetStoreAsync(int id, IEnumerable<Claim> claims)
        {
            try
            {
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                Claim registeredRoleClaim = claims.First(x => x.Type.ToLower().Equals("role"));
                string email = registeredEmailClaim.Value;
                string role = registeredRoleClaim.Value;

                Store existedStore = await this._unitOfWork.StoreRepository.GetStoreAsync(id);
                if (existedStore == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistStoreId);
                }

                if (role.ToLower().Equals(RoleConstant.Brand_Manager.ToLower()))
                {
                    Brand existedBrand = await this._unitOfWork.BrandRepository.GetBrandAsync(email);
                    if (existedBrand.Stores.FirstOrDefault(x => x.StoreId == id) == null)
                    {
                        throw new BadRequestException(MessageConstant.StoreMessage.BrandNotHaveStore);
                    }
                }

                if (role.ToLower().Equals(RoleConstant.Kitchen_Center_Manager.ToLower()))
                {
                    KitchenCenter existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(email);
                    if (existedKitchenCenter.Stores.FirstOrDefault(x => x.StoreId == id) == null)
                    {
                        throw new BadRequestException(MessageConstant.StoreMessage.KitchenCenterNotHaveStore);
                    }
                }
                GetStoreResponse store = this._mapper.Map<GetStoreResponse>(existedStore);
                return store;
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistStoreId))
                {
                    fieldName = "Store id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.StoreMessage.BrandNotHaveStore))
                {
                    fieldName = "Get Store failed";
                }
                else if (ex.Message.Equals(MessageConstant.StoreMessage.KitchenCenterNotHaveStore))
                {
                    fieldName = "Get store failed";
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

        public async Task<GetStoreResponse> GetStoreAsync(IEnumerable<Claim> claims)
        {
            try
            {
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                string email = registeredEmailClaim.Value;

                Store existedStore = await this._unitOfWork.StoreRepository.GetStoreAsync(email);

                GetStoreResponse getStoreResponse = this._mapper.Map<GetStoreResponse>(existedStore);
                return getStoreResponse;
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task CreateStoreAsync(RegisterStoreRequest registerStoreRequest, IEnumerable<Claim> claims)
        {
            bool isUploaded = false;
            string folderName = "Stores";
            string logoId = "";
            try
            {
                KitchenCenter existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(registerStoreRequest.KitchenCenterId);
                if (existedKitchenCenter == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistKitchenCenterId);
                }

                Brand existedBrand = await this._unitOfWork.BrandRepository.GetBrandByIdAsync(registerStoreRequest.BrandId);
                if (existedBrand == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBrandId);
                }

                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                string email = registeredEmailClaim.Value;
                if (existedBrand.BrandManagerEmail.Equals(email) == false)
                {
                    throw new BadRequestException(MessageConstant.BrandMessage.NotBelongToBrand);
                }

                Account existedAccount = await this._unitOfWork.AccountRepository.GetAccountAsync(registerStoreRequest.StoreManagerEmail);
                if (existedAccount != null)
                {
                    throw new BadRequestException(MessageConstant.StoreMessage.ManageremailExisted);
                }

                FileStream logoStream = FileUtil.ConvertFormFileToStream(registerStoreRequest.Logo);
                Guid guid = Guid.NewGuid();
                logoId = guid.ToString();
                string logoLink = await this._unitOfWork.FirebaseStorageRepository.UploadImageAsync(logoStream, folderName, logoId);
                if (logoLink != null && logoLink.Length > 0)
                {
                    isUploaded = true;
                }
                logoLink += $"&logoId={logoId}";

                Role storeManagerRole = await this._unitOfWork.RoleRepository.GetRoleAsync((int)RoleEnum.Role.STORE_MANAGER);
                string password = "";
                Account managerAccount = new Account()
                {
                    Email = registerStoreRequest.StoreManagerEmail,
                    Password = password,
                    Status = (int)AccountEnum.Status.INACTIVE,
                    Role = storeManagerRole,
                };



                Store newStore = new Store()
                {
                    Name = registerStoreRequest.Name,
                    Logo = logoLink,
                    Status = (int)StoreEnum.Status.CONFIRMING,
                    Brand = existedBrand,
                    KitchenCenter = existedKitchenCenter,
                    Wallet = null,
                    StoreManagerEmail = registerStoreRequest.StoreManagerEmail
                };

                StoreAccount storeAccount = new StoreAccount()
                {
                    Account = managerAccount,
                    Store = newStore
                };

                newStore.StoreAccounts = new List<StoreAccount>() { storeAccount };

                await this._unitOfWork.StoreRepository.CreateStoreAsync(newStore);
                await this._unitOfWork.CommitAsync();
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistKitchenCenterId))
                {
                    fieldName = "Kitchen center id";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistBrandId))
                {
                    fieldName = "Brand id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.BrandMessage.NotBelongToBrand))
                {
                    fieldName = "Brand id";
                }
                else if (ex.Message.Equals(MessageConstant.StoreMessage.ManageremailExisted))
                {
                    fieldName = "Store manager email";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
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

        public async Task UpdateStoreAsync(int storeId, UpdateStoreRequest updateStoreRequest, IEnumerable<Claim> claims)
        {
            bool isUploaded = false;
            bool isDeleted = false;
            string folderName = "Stores";
            string logoId = "";
            bool isNewManager = false;
            try
            {
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                Claim registeredRoleClaim = claims.First(x => x.Type.ToLower().Equals("role"));
                string email = registeredEmailClaim.Value;
                string role = registeredRoleClaim.Value;
                Store existedStore = await this._unitOfWork.StoreRepository.GetStoreAsync(storeId);
                if (existedStore == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistStoreId);
                }

                if (existedStore.Status == (int)StoreEnum.Status.DISABLE)
                {
                    throw new BadRequestException(MessageConstant.StoreMessage.DeactiveStore_Update);
                }
                if (role.ToLower().Equals(RoleConstant.Brand_Manager.ToLower()))
                {
                    Brand existedBrand = await this._unitOfWork.BrandRepository.GetBrandAsync(email);
                    if (existedBrand.Stores.FirstOrDefault(x => x.StoreId == storeId) == null)
                    {
                        throw new BadRequestException(MessageConstant.StoreMessage.BrandNotHaveStore);
                    }
                }

                string password = "";
                if (existedStore.StoreManagerEmail.Equals(updateStoreRequest.StoreManagerEmail) == false)
                {
                    Account existedStoreManagerAccount = await this._unitOfWork.AccountRepository.GetAccountAsync(updateStoreRequest.StoreManagerEmail);
                    if (existedStoreManagerAccount != null)
                    {
                        throw new BadRequestException(MessageConstant.StoreMessage.ManageremailExisted);
                    }

                    //Account oldStoreManagerAccount = await this._unitOfWork.AccountRepository.GetAccountAsync(existedStore.StoreManagerEmail);

                    Account oldStoreManagerAccount = existedStore.StoreAccounts.Where(x => x.Account.Email.Equals(existedStore.StoreManagerEmail)).Single().Account;

                    oldStoreManagerAccount.Status = (int)AccountEnum.Status.DISABLE;
                    this._unitOfWork.AccountRepository.UpdateAccount(oldStoreManagerAccount);

                    Role storeManagerRole = await this._unitOfWork.RoleRepository.GetRoleAsync((int)RoleEnum.Role.STORE_MANAGER);
                    password = PasswordUtil.CreateRandomPassword();
                    Account newStoreManagerAccount = new Account()
                    {
                        Email = updateStoreRequest.StoreManagerEmail,
                        Password = StringUtil.EncryptData(password),
                        Role = storeManagerRole,
                        Status = (int)AccountEnum.Status.ACTIVE
                    };

                    await this._unitOfWork.AccountRepository.CreateAccountAsync(newStoreManagerAccount);

                    StoreAccount newStoreAccount = new StoreAccount()
                    {
                        Account = newStoreManagerAccount,
                        Store = existedStore
                    };

                    await this._unitOfWork.StoreAccountRepository.AddStoreAccountAsync(newStoreAccount);

                    isNewManager = true;
                    existedStore.StoreManagerEmail = updateStoreRequest.StoreManagerEmail;
                }

                string oldLogo = existedStore.Logo;
                if (updateStoreRequest.Logo != null)
                {
                    Guid guid = Guid.NewGuid();
                    logoId = guid.ToString();
                    FileStream logoFileStream = FileUtil.ConvertFormFileToStream(updateStoreRequest.Logo);
                    string logoLink = await this._unitOfWork.FirebaseStorageRepository.UploadImageAsync(logoFileStream, folderName, logoId);
                    if (logoLink != null && logoLink.Length > 0)
                    {
                        isUploaded = true;
                    }
                    logoLink += $"&logoId={logoId}";
                    existedStore.Logo = logoLink;
                    await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(FileUtil.GetImageIdFromUrlImage(oldLogo, "logoId"), folderName);
                    isDeleted = true;
                }

                existedStore.Name = updateStoreRequest.Name;
                if (updateStoreRequest.Status.Trim().ToLower().Equals(StoreEnum.Status.ACTIVE.ToString().ToLower()))
                {
                    existedStore.Status = (int)StoreEnum.Status.ACTIVE;
                }
                else if (updateStoreRequest.Status.Trim().ToLower().Equals(StoreEnum.Status.INACTIVE.ToString().ToLower()))
                {
                    existedStore.Status = (int)StoreEnum.Status.INACTIVE;
                }

                this._unitOfWork.StoreRepository.UpdateStore(existedStore);
                await this._unitOfWork.CommitAsync();

                if (isNewManager)
                {
                    string messageBody = EmailMessageConstant.Store.Message + $" {updateStoreRequest.Name}. " + EmailMessageConstant.CommonMessage.Message;
                    string message = this._unitOfWork.EmailRepository.GetMessageToRegisterAccount(updateStoreRequest.StoreManagerEmail, password, messageBody);
                    await this._unitOfWork.EmailRepository.SendAccountToEmailAsync(updateStoreRequest.StoreManagerEmail, message);
                }
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistStoreId))
                {
                    fieldName = "Store id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.StoreMessage.ManageremailExisted))
                {
                    fieldName = "Store manager email";
                }
                else if (ex.Message.Equals(MessageConstant.StoreMessage.DeactiveStore_Update))
                {
                    fieldName = "Updated store failed";
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

        public async Task DeleteStoreAsync(int storeId, IEnumerable<Claim> claims)
        {
            try
            {
                if (storeId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidStoreId);
                }

                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                Claim registeredRoleClaim = claims.First(x => x.Type.ToLower().Equals("role"));
                string email = registeredEmailClaim.Value;
                string role = registeredRoleClaim.Value;
                Store existedStore = await this._unitOfWork.StoreRepository.GetStoreAsync(storeId);
                if (existedStore == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistStoreId);
                }

                if (existedStore.Status == (int)StoreEnum.Status.DISABLE)
                {
                    throw new BadRequestException(MessageConstant.StoreMessage.DeactiveStore_Delete);
                }
                if (role.ToLower().Equals(RoleConstant.Brand_Manager.ToLower()))
                {
                    Brand existedBrand = await this._unitOfWork.BrandRepository.GetBrandAsync(email);
                    if (existedBrand.Stores.FirstOrDefault(x => x.StoreId == storeId) == null)
                    {
                        throw new BadRequestException(MessageConstant.StoreMessage.BrandNotHaveStore);
                    }
                }

                existedStore.Status = (int)StoreEnum.Status.DISABLE;
                foreach (var storeAccount in existedStore.StoreAccounts)
                {
                    if (storeAccount.Account.Status == (int)AccountEnum.Status.ACTIVE)
                    {
                        storeAccount.Account.Status = (int)AccountEnum.Status.DISABLE;
                    }
                }



                if (existedStore.StorePartners.Any())
                {
                    foreach (var storePartner in existedStore.StorePartners)
                    {
                        storePartner.Status = (int)StorePartnerEnum.Status.DISABLE;
                        if (storePartner.PartnerProducts.Any())
                        {
                            foreach (var partnerProduct in storePartner.PartnerProducts)
                            {
                                partnerProduct.Status = (int)PartnerProductEnum.Status.DISABLE;
                            }
                        }
                    }
                }

                this._unitOfWork.StoreRepository.UpdateStore(existedStore);
                await this._unitOfWork.CommitAsync();
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistStoreId))
                {
                    fieldName = "Store id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidStoreId))
                {
                    fieldName = "Store id";
                }
                else if (ex.Message.Equals(MessageConstant.StoreMessage.DeactiveStore_Delete) ||
                    ex.Message.Equals(MessageConstant.StoreMessage.BrandNotHaveStore))
                {
                    fieldName = "Deleted store failed";
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

        public async Task UpdateStoreStatusAsync(int storeId, UpdateStoreStatusRequest updateStoreStatusRequest, IEnumerable<Claim> claims)
        {
            try
            {
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                Claim registeredRoleClaim = claims.First(x => x.Type.ToLower().Equals("role"));
                string email = registeredEmailClaim.Value;
                string role = registeredRoleClaim.Value;
                Store existedStore = await this._unitOfWork.StoreRepository.GetStoreAsync(storeId);
                if (existedStore == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistStoreId);
                }

                if (existedStore.Status == (int)StoreEnum.Status.DISABLE)
                {
                    throw new BadRequestException(MessageConstant.StoreMessage.DeactiveStore_Update);
                }
                if (role.ToLower().Equals(RoleConstant.Brand_Manager.ToLower()))
                {
                    Brand existedBrand = await this._unitOfWork.BrandRepository.GetBrandAsync(email);
                    if (existedBrand.Stores.FirstOrDefault(x => x.StoreId == storeId) == null)
                    {
                        throw new BadRequestException(MessageConstant.StoreMessage.BrandNotHaveStore);
                    }
                }

                if (updateStoreStatusRequest.Status.Trim().ToLower().Equals(StoreEnum.Status.ACTIVE.ToString().ToLower()))
                {
                    existedStore.Status = (int)StoreEnum.Status.ACTIVE;
                }
                else if (updateStoreStatusRequest.Status.Trim().ToLower().Equals(StoreEnum.Status.INACTIVE.ToString().ToLower()))
                {
                    existedStore.Status = (int)StoreEnum.Status.INACTIVE;
                }
                this._unitOfWork.StoreRepository.UpdateStore(existedStore);
                await this._unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.StoreMessage.DeactiveStore_Update) ||
                   ex.Message.Equals(MessageConstant.StoreMessage.BrandNotHaveStore))
                {
                    fieldName = "Updated store failed";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistStoreId))
                {
                    fieldName = "Store id";
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

        public async Task ConfirmStoreRegistrationAsync(int storeId, ConfirmStoreRegistrationRequest confirmStoreRegistrationRequest)
        {
            try
            {
                Store existedStore = await this._unitOfWork.StoreRepository.GetStoreAsync(storeId);
                if (existedStore == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistStoreId);
                }
                if (existedStore.Status != (int)StoreEnum.Status.CONFIRMING)
                {
                    throw new BadRequestException(MessageConstant.StoreMessage.NotConfirmingStore);
                }

                bool isActiveStore = false;
                string password = "";
                if (confirmStoreRegistrationRequest.Status.Trim().ToLower().Equals(StoreEnum.Status.ACTIVE.ToString().ToLower()))
                {
                    confirmStoreRegistrationRequest.RejectedReason = null;
                    existedStore.Status = (int)StoreEnum.Status.ACTIVE;
                    isActiveStore = true;
                    Wallet storeWallet = new Wallet()
                    {
                        Balance = 0,
                    };
                    existedStore.Wallet = storeWallet;
                    StoreAccount existStoreAccount = existedStore.StoreAccounts.FirstOrDefault(x => x.Account.Email.Equals(existedStore.StoreManagerEmail));
                    password = PasswordUtil.CreateRandomPassword();
                    existStoreAccount.Account.Password = StringUtil.EncryptData(password);
                    existStoreAccount.Account.IsConfirmed = false;
                    existStoreAccount.Account.IsConfirmed = false;
                    existStoreAccount.Account.Status = (int)AccountEnum.Status.ACTIVE;
                }
                else if (confirmStoreRegistrationRequest.Status.Trim().ToLower().Equals(StoreEnum.Status.REJECTED.ToString().ToLower()) &&
                    confirmStoreRegistrationRequest.RejectedReason == null)
                {
                    throw new BadRequestException(MessageConstant.StoreMessage.NotRejectedResonForNewStore);
                }
                else
                {
                    existedStore.Status = (int)StoreEnum.Status.REJECTED;
                    existedStore.RejectedReason = confirmStoreRegistrationRequest.RejectedReason;
                    isActiveStore = false;
                }
                this._unitOfWork.StoreRepository.UpdateStore(existedStore);
                await this._unitOfWork.CommitAsync();

                if (isActiveStore)
                {
                    string messageBody = EmailMessageConstant.Store.Message + $" {existedStore.Name}. " + EmailMessageConstant.CommonMessage.Message;
                    string message = this._unitOfWork.EmailRepository.GetMessageToRegisterAccount(existedStore.StoreManagerEmail, password, messageBody);
                    await this._unitOfWork.EmailRepository.SendAccountToEmailAsync(existedStore.StoreManagerEmail, message);
                }
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.StoreMessage.NotConfirmingStore))
                {
                    fieldName = "Status";
                }
                else if (ex.Message.Equals(MessageConstant.StoreMessage.NotRejectedResonForNewStore))
                {
                    fieldName = "Rejected reason";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);

            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Store id", ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Eception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task<List<GetStoreResponseForPrivateAPI>> GetStoresAsync()
        {
            try
            {
                List<Store> stores = await this._unitOfWork.StoreRepository.GetStoresAsync();
                List<GetStoreResponseForPrivateAPI> storeResponseForPrivateAPIs = this._mapper.Map<List<GetStoreResponseForPrivateAPI>>(stores);
                return storeResponseForPrivateAPIs;
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task<GetStoresResponse> GetStoresWithInactiveAndActiveStatusAsync(GetStoresRequest getStoresRequest, IEnumerable<Claim> claims)
        {
            try
            {

                int? statusParam = null;
                if (getStoresRequest.Status != null)
                {
                    statusParam = StatusUtil.ChangeStoreStatus(getStoresRequest.Status);
                }
                Brand existedBrand = null;
                if (getStoresRequest.IdBrand != null)
                {
                    existedBrand = await this._unitOfWork.BrandRepository.GetBrandByIdAsync(getStoresRequest.IdBrand.Value);
                    if (existedBrand == null)
                    {
                        throw new NotFoundException(MessageConstant.CommonMessage.NotExistBrandId);
                    }
                }
                KitchenCenter existedKitchenCenter = null;
                if (getStoresRequest.IdKitchenCenter != null)
                {
                    existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(getStoresRequest.IdKitchenCenter.Value);
                    if (existedKitchenCenter == null)
                    {
                        throw new NotFoundException(MessageConstant.CommonMessage.NotExistKitchenCenterId);
                    }
                }

                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                Claim registeredRoleClaim = claims.First(x => x.Type.ToLower().Equals("role"));
                string email = registeredEmailClaim.Value;
                string role = registeredRoleClaim.Value;
                if (claims != null && getStoresRequest.IdBrand != null)
                {
                    if (role.ToLower().Equals(RoleConstant.Brand_Manager.ToLower()))
                    {
                        if (existedBrand.BrandManagerEmail.Equals(email) == false)
                        {
                            throw new BadRequestException(MessageConstant.BrandMessage.NotBelongToBrand);
                        }
                        if (getStoresRequest.IdKitchenCenter != null && existedBrand.Stores.Any(x => x.KitchenCenter.KitchenCenterId == getStoresRequest.IdKitchenCenter) == false)
                        {
                            throw new BadRequestException(MessageConstant.StoreMessage.BrandNotJoinKitchenCenter);
                        }
                    }
                }
                else if (claims != null && getStoresRequest.IdBrand == null)
                {
                    if (role.ToLower().Equals(RoleConstant.Brand_Manager.ToLower()))
                    {
                        existedBrand = await this._unitOfWork.BrandRepository.GetBrandAsync(email);
                        getStoresRequest.IdBrand = existedBrand.BrandId;
                    }
                }

                if (claims != null && getStoresRequest.IdKitchenCenter != null)
                {
                    if (role.ToLower().Equals(RoleConstant.Kitchen_Center_Manager.ToLower()))
                    {
                        if (existedKitchenCenter.Manager.Email.Equals(email) == false)
                        {
                            throw new BadRequestException(MessageConstant.KitchenCenterMessage.NotBelongToKitchenCenter);
                        }
                        if (getStoresRequest.IdBrand != null && existedKitchenCenter.Stores.Any(x => x.Brand.BrandId == getStoresRequest.IdBrand) == false)
                        {
                            throw new BadRequestException(MessageConstant.StoreMessage.KitchenCenterNotHaveBrand);
                        }
                    }
                }
                else if (claims != null && getStoresRequest.IdKitchenCenter == null)
                {
                    if (role.ToLower().Equals(RoleConstant.Kitchen_Center_Manager.ToLower()))
                    {
                        existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(email);
                        getStoresRequest.IdKitchenCenter = existedKitchenCenter.KitchenCenterId;
                    }
                }

                int numberItems = 0;
                List<Store> stores = null;
                if (getStoresRequest.SearchValue != null && StringUtil.IsUnicode(getStoresRequest.SearchValue))
                {
                    numberItems = await this._unitOfWork.StoreRepository.GetNumberStoresActiveAndInactiveAsync(getStoresRequest.SearchValue, null, getStoresRequest.IdBrand, getStoresRequest.IdKitchenCenter, statusParam);
                    stores = await this._unitOfWork.StoreRepository.GetStoresActiveAndInactiveAsync(getStoresRequest.SearchValue, null, getStoresRequest.CurrentPage, getStoresRequest.ItemsPerPage,
                                                                                                              getStoresRequest.SortBy != null && getStoresRequest.SortBy.ToLower().EndsWith("asc") ? getStoresRequest.SortBy.Split("_")[0] : null,
                                                                                                              getStoresRequest.SortBy != null && getStoresRequest.SortBy.ToLower().EndsWith("desc") ? getStoresRequest.SortBy.Split("_")[0] : null,
                                                                                                              getStoresRequest.IdBrand, getStoresRequest.IdKitchenCenter, statusParam, getStoresRequest.IsGetAll);
                }
                else if (getStoresRequest.SearchValue != null && StringUtil.IsUnicode(getStoresRequest.SearchValue) == false)
                {
                    numberItems = await this._unitOfWork.StoreRepository.GetNumberStoresActiveAndInactiveAsync(null, getStoresRequest.SearchValue, getStoresRequest.IdBrand, getStoresRequest.IdKitchenCenter, statusParam);
                    stores = await this._unitOfWork.StoreRepository.GetStoresActiveAndInactiveAsync(null, getStoresRequest.SearchValue, getStoresRequest.CurrentPage, getStoresRequest.ItemsPerPage,
                                                                                                              getStoresRequest.SortBy != null && getStoresRequest.SortBy.ToLower().EndsWith("asc") ? getStoresRequest.SortBy.Split("_")[0] : null,
                                                                                                              getStoresRequest.SortBy != null && getStoresRequest.SortBy.ToLower().EndsWith("desc") ? getStoresRequest.SortBy.Split("_")[0] : null,
                                                                                                              getStoresRequest.IdBrand, getStoresRequest.IdKitchenCenter, statusParam, getStoresRequest.IsGetAll);
                }
                else if (getStoresRequest.SearchValue == null)
                {
                    numberItems = await this._unitOfWork.StoreRepository.GetNumberStoresActiveAndInactiveAsync(null, null, getStoresRequest.IdBrand, getStoresRequest.IdKitchenCenter, statusParam);
                    stores = await this._unitOfWork.StoreRepository.GetStoresActiveAndInactiveAsync(null, null, getStoresRequest.CurrentPage, getStoresRequest.ItemsPerPage,
                                                                                                              getStoresRequest.SortBy != null && getStoresRequest.SortBy.ToLower().EndsWith("asc") ? getStoresRequest.SortBy.Split("_")[0] : null,
                                                                                                              getStoresRequest.SortBy != null && getStoresRequest.SortBy.ToLower().EndsWith("desc") ? getStoresRequest.SortBy.Split("_")[0] : null,
                                                                                                              getStoresRequest.IdBrand, getStoresRequest.IdKitchenCenter, statusParam, getStoresRequest.IsGetAll);
                }

                int totalPages = 0;
                if (numberItems > 0 && getStoresRequest.IsGetAll == null || numberItems > 0 && getStoresRequest.IsGetAll != null && getStoresRequest.IsGetAll == false)
                {
                    totalPages = (int)((numberItems + getStoresRequest.ItemsPerPage) / getStoresRequest.ItemsPerPage);
                }

                if (numberItems == 0)
                {
                    totalPages = 0;
                }
                List<GetStoreResponse> getStoreResponses = this._mapper.Map<List<GetStoreResponse>>(stores);
                GetStoresResponse getStores = new GetStoresResponse()
                {
                    NumberItems = numberItems,
                    TotalPages = totalPages,
                    Stores = getStoreResponses
                };
                return getStores;
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistBrandId) ||
                    ex.Message.Equals(MessageConstant.StoreMessage.BrandNotJoinKitchenCenter))
                {
                    fieldName = "Brand id";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistKitchenCenterId) ||
                    ex.Message.Equals(MessageConstant.StoreMessage.KitchenCenterNotHaveBrand))
                {
                    fieldName = "Kitchen center id";
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
    }
}
