using AutoMapper;
using MBKC.BAL.DTOs.Brands;
using MBKC.BAL.DTOs.FireBase;
using MBKC.BAL.DTOs.KitchenCenters;
using MBKC.BAL.DTOs.Stores;
using MBKC.BAL.DTOs.Verifications;
using MBKC.BAL.Exceptions;
using MBKC.BAL.Services.Interfaces;
using MBKC.BAL.Utils;
using MBKC.DAL.Enums;
using MBKC.DAL.Infrastructures;
using MBKC.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Services.Implementations
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

        public async Task<GetStoresResponse> GetStoresAsync(string? searchValue, int? currentPage, int? itemsPerPage, int? brandId, IEnumerable<Claim>? claims)
        {
            try
            {
                if (claims != null && brandId != null)
                {
                    Claim registeredEmailClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
                    Claim registeredRoleClaim = claims.FirstOrDefault(x => x.Type.ToLower().Equals("role"));
                    string email = registeredEmailClaim.Value;
                    string role = registeredRoleClaim.Value;
                    string[] roleNameParts = RoleEnum.Role.BRAND_MANAGER.ToString().ToLower().Split("_");
                    string roleName = "";
                    foreach (var roleNamePart in roleNameParts)
                    {
                        roleName += $"{roleNamePart} ";
                    }
                    roleName = roleName.Trim();
                    if (role.ToLower().Equals(roleName))
                    {
                        Brand existedBrand = await this._unitOfWork.BrandRepository.GetBrandByIdAsync(brandId.Value);
                        BrandAccount brandManagerAccount = existedBrand.BrandAccounts.FirstOrDefault(x => x.Account.Role.RoleId == (int)RoleEnum.Role.BRAND_MANAGER
                                                                                                       && x.Account.Status == (int)AccountEnum.Status.ACTIVE);
                        if (brandManagerAccount.Account.Email.Equals(email))
                        {
                            throw new BadRequestException("Brand id does not belong to your brand.");
                        }
                    }
                }
                if (itemsPerPage != null && itemsPerPage <= 0)
                {
                    throw new BadRequestException("Items per page number is required more than 0.");
                }
                else if (itemsPerPage == null)
                {
                    itemsPerPage = 5;
                }
                if (currentPage != null && currentPage <= 0)
                {
                    throw new BadRequestException("Current page number is required more than 0.");
                }
                else if (currentPage == null)
                {
                    currentPage = 1;
                }
                int numberItems = 0;
                List<Store> stores = null;
                if (searchValue != null && StringUtil.IsUnicode(searchValue))
                {
                    numberItems = await this._unitOfWork.StoreRepository.GetNumberStoresAsync(searchValue, null, brandId);
                    stores = await this._unitOfWork.StoreRepository.GetStoresAsync(searchValue, null, itemsPerPage.Value, currentPage.Value, brandId);
                }
                else if (searchValue != null && StringUtil.IsUnicode(searchValue) == false)
                {
                    numberItems = await this._unitOfWork.StoreRepository.GetNumberStoresAsync(null, searchValue, brandId);
                    stores = await this._unitOfWork.StoreRepository.GetStoresAsync(null, searchValue, itemsPerPage.Value, currentPage.Value, brandId);
                }
                else if (searchValue == null)
                {
                    numberItems = await this._unitOfWork.StoreRepository.GetNumberStoresAsync(null, null, brandId);
                    stores = await this._unitOfWork.StoreRepository.GetStoresAsync(null, null, itemsPerPage.Value, currentPage.Value, brandId);
                }
                int totalPage = (int)((numberItems + itemsPerPage) / itemsPerPage);
                if (numberItems == 0)
                {
                    totalPage = 0;
                }
                List<GetStoreResponse> getStoreResponses = this._mapper.Map<List<GetStoreResponse>>(stores);
                GetStoresResponse getStores = new GetStoresResponse()
                {
                    NumberItems = numberItems,
                    TotalPages = totalPage,
                    Stores = getStoreResponses
                };
                return getStores;
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Items per page number is required more than 0."))
                {
                    fieldName = "Items Per Page";
                }
                else if (ex.Message.Equals("Current page number is required more than 0."))
                {
                    fieldName = "Current Page";
                }
                else if (ex.Message.Equals("Brand id does not belong to your brand."))
                {
                    fieldName = "Brand Id";
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

        public async Task<GetStoreResponse> GetStoreAsync(int id, int? brandId, IEnumerable<Claim>? claims)
        {
            try
            {
                Brand existedBrand = null;
                if (claims != null && brandId != null)
                {
                    if (brandId <= 0)
                    {
                        throw new BadRequestException("Brand id is not suiltable id in the system.");
                    }
                    Claim registeredEmailClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
                    Claim registeredRoleClaim = claims.FirstOrDefault(x => x.Type.ToLower().Equals("role"));
                    string email = registeredEmailClaim.Value;
                    string role = registeredRoleClaim.Value;
                    string[] roleNameParts = RoleEnum.Role.BRAND_MANAGER.ToString().ToLower().Split("_");
                    string roleName = "";
                    foreach (var roleNamePart in roleNameParts)
                    {
                        roleName += $"{roleNamePart} ";
                    }
                    roleName = roleName.Trim();
                    if (role.ToLower().Equals(roleName))
                    {
                        existedBrand = await this._unitOfWork.BrandRepository.GetBrandByIdAsync(brandId.Value);
                        if (existedBrand == null)
                        {
                            throw new NotFoundException("Brand does not exist in the system.");
                        }
                        BrandAccount brandManagerAccount = existedBrand.BrandAccounts.FirstOrDefault(x => x.Account.Role.RoleId == (int)RoleEnum.Role.BRAND_MANAGER
                                                                                                       && x.Account.Status == (int)AccountEnum.Status.ACTIVE);
                        if (brandManagerAccount.Account.Email.Equals(email))
                        {
                            throw new BadRequestException("Brand id does not belong to your brand.");
                        }
                    }
                }
                if (id <= 0)
                {
                    throw new BadRequestException("Store id is not suiltable id in the system.");
                }

                Store existedStore = await this._unitOfWork.StoreRepository.GetStoreAsync(id, null);
                if (existedStore == null)
                {
                    throw new NotFoundException("Store id does not exist in the system.");
                }
                if (brandId != null && existedStore.Brand.BrandId != brandId)
                {
                    throw new NotFoundException($"[Brand-{brandId}] does not have the [Store-{id}] in the system.");
                }
                GetStoreResponse store = this._mapper.Map<GetStoreResponse>(existedStore);
                return store;
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Store does not exist in the system.") ||
                    ex.Message.Contains(" does not have"))
                {
                    fieldName = "Store id";
                }
                else if (ex.Message.Equals("Brand does not exist in the system."))
                {
                    fieldName = "Brand id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Store id is not suiltable id in the system."))
                {
                    fieldName = "Store id";
                }
                else if (ex.Message.Equals("Brand id is not suiltable id in the system.") ||
                    ex.Message.Equals("Brand id does not belong to your brand."))
                {
                    fieldName = "Brand id";
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

        public async Task CreateStoreAsync(CreateStoreRequest createStoreRequest, FireBaseImage fireBaseImageOption, Email emailOption)
        {
            bool isUploaded = false;
            string folderName = "Stores";
            string logoId = "";
            try
            {
                KitchenCenter existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(createStoreRequest.KitchenCenterId);
                if (existedKitchenCenter == null)
                {
                    throw new NotFoundException("Kitchen center id does not exist in the system.");
                }

                Brand existedBrand = await this._unitOfWork.BrandRepository.GetBrandByIdAsync(createStoreRequest.BrandId);
                if (existedBrand == null)
                {
                    throw new NotFoundException("Brand id does not exist in the system.");
                }

                Account existedAccount = await this._unitOfWork.AccountRepository.GetAccountAsync(createStoreRequest.StoreManagerEmail);
                if (existedAccount != null)
                {
                    throw new BadRequestException("Email already existed in the system.");
                }

                FileStream logoStream = FileUtil.ConvertFormFileToStream(createStoreRequest.Logo);
                Guid guid = Guid.NewGuid();
                logoId = guid.ToString();
                FileUtil.SetCredentials(fireBaseImageOption);
                string logoLink = await FileUtil.UploadImageAsync(logoStream, folderName, logoId);
                if (logoLink != null && logoLink.Length > 0)
                {
                    isUploaded = true;
                }
                logoLink += $"&logoId={logoId}";

                Role storeManagerRole = await this._unitOfWork.RoleRepository.GetRoleAsync((int)RoleEnum.Role.STORE_MANAGER);
                string password = RandomPasswordUtil.CreateRandomPassword();
                Account managerAccount = new Account()
                {
                    Email = createStoreRequest.StoreManagerEmail,
                    Password = password,
                    Status = (int)AccountEnum.Status.ACTIVE,
                    Role = storeManagerRole,
                };

                Wallet storeWallet = new Wallet()
                {
                    Balance = 0,
                };

                Store newStore = new Store()
                {
                    Name = createStoreRequest.Name,
                    Logo = logoLink,
                    Status = (int)StoreEnum.Status.ACTIVE,
                    Brand = existedBrand,
                    KitchenCenter = existedKitchenCenter,
                    Wallet = storeWallet
                };

                StoreAccount storeAccount = new StoreAccount()
                {
                    Account = managerAccount,
                    Store = newStore
                };

                newStore.StoreAccounts = new List<StoreAccount>() { storeAccount };

                await this._unitOfWork.StoreRepository.CreateStoreAsync(newStore);
                await this._unitOfWork.CommitAsync();

                string message = EmailUtil.MessageRegisterAccountForStore(emailOption.SystemName, createStoreRequest.StoreManagerEmail, password, createStoreRequest.Name);
                await EmailUtil.SendEmailAndPasswordToEmail(emailOption, createStoreRequest.StoreManagerEmail, message);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Kitchen center id does not exist in the system."))
                {
                    fieldName = "Kitchen center id";
                }
                else if (ex.Message.Equals("Brand id does not exist in the system."))
                {
                    fieldName = "Brand id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string error = ErrorUtil.GetErrorString("Email", ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                if (isUploaded)
                {
                    FileUtil.SetCredentials(fireBaseImageOption);
                    await FileUtil.DeleteImageAsync(logoId, folderName);
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task UpdateStoreAsync(int brandId, int storeId, UpdateStoreRequest updateStoreRequest, FireBaseImage fireBaseImageOption, Email emailOption, IEnumerable<Claim> claims)
        {
            bool isUploaded = false;
            bool isDeleted = false;
            string folderName = "Stores";
            string logoId = "";
            bool isNewManager = false;
            try
            {
                Brand existedBrand = await this._unitOfWork.BrandRepository.GetBrandByIdAsync(brandId);
                if (existedBrand == null)
                {
                    throw new NotFoundException("Brand id does not exist in the system.");
                }
                Claim registeredEmailClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
                Claim registeredRoleClaim = claims.FirstOrDefault(x => x.Type.ToLower().Equals("role"));
                string email = registeredEmailClaim.Value;
                string role = registeredRoleClaim.Value;
                string[] roleNameParts = RoleEnum.Role.BRAND_MANAGER.ToString().ToLower().Split("_");
                string roleName = "";
                foreach (var roleNamePart in roleNameParts)
                {
                    roleName += $"{roleNamePart} ";
                }
                roleName = roleName.Trim();
                if (role.ToLower().Equals(roleName))
                {
                    BrandAccount brandManagerAccount = existedBrand.BrandAccounts.FirstOrDefault(x => x.Account.Role.RoleId == (int)RoleEnum.Role.BRAND_MANAGER
                                                                                                   && x.Account.Status == (int)AccountEnum.Status.ACTIVE);
                    if (brandManagerAccount.Account.Email.Equals(email))
                    {
                        throw new BadRequestException("Brand id does not belong to your brand.");
                    }
                }

                Store existedStore = await this._unitOfWork.StoreRepository.GetStoreAsync(storeId, null);
                if (existedStore == null)
                {
                    throw new NotFoundException("Store id does not exist in the system.");
                }

                if (existedStore.Brand.BrandId != brandId)
                {
                    throw new NotFoundException($"[Brand-{brandId}] does not have [store-{storeId}] in the system.");
                }

                string password = "";
                if (existedStore.StoreAccounts.FirstOrDefault(x => x.Account.Role.RoleId == (int)RoleEnum.Role.STORE_MANAGER)
                                                        .Account.Email.Equals(updateStoreRequest.StoreManagerEmail) == false)
                {
                    Account existedStoreManagerAccount = await this._unitOfWork.AccountRepository.GetAccountAsync(updateStoreRequest.StoreManagerEmail);
                    if (existedStoreManagerAccount != null)
                    {
                        throw new BadRequestException("Store Manager Email already existed in the system.");
                    }

                    Account oldStoreManagerAccount = existedStore.StoreAccounts.FirstOrDefault(x => x.Account.Role.RoleId == (int)RoleEnum.Role.STORE_MANAGER
                                                                                               && x.Account.Status == (int)AccountEnum.Status.ACTIVE)
                                                                                            .Account;

                    oldStoreManagerAccount.Status = (int)AccountEnum.Status.DEACTIVE;
                    this._unitOfWork.AccountRepository.UpdateAccount(oldStoreManagerAccount);

                    Role storeManagerRole = await this._unitOfWork.RoleRepository.GetRoleAsync((int)RoleEnum.Role.STORE_MANAGER);
                    password = RandomPasswordUtil.CreateRandomPassword();
                    Account newStoreManagerAccount = new Account()
                    {
                        Email = updateStoreRequest.StoreManagerEmail,
                        Password = password,
                        Role = storeManagerRole,
                        Status = (int)AccountEnum.Status.ACTIVE
                    };

                    StoreAccount newStoreAccount = new StoreAccount()
                    {
                        Account = newStoreManagerAccount,
                        Store = existedStore
                    };

                    existedStore.StoreAccounts.ToList().Add(newStoreAccount);
                    isNewManager = true;
                }

                string oldLogo = existedStore.Logo;
                if (updateStoreRequest.Logo != null)
                {
                    Guid guid = Guid.NewGuid();
                    logoId = guid.ToString();
                    FileUtil.SetCredentials(fireBaseImageOption);
                    FileStream logoFileStream = FileUtil.ConvertFormFileToStream(updateStoreRequest.Logo);
                    string logoLink = await FileUtil.UploadImageAsync(logoFileStream, folderName, logoId);
                    if (logoLink != null && logoLink.Length > 0)
                    {
                        isUploaded = true;
                    }
                    logoLink += $"&logoId={logoId}";
                    existedStore.Logo = logoLink;
                    FileUtil.SetCredentials(fireBaseImageOption);
                    await FileUtil.DeleteImageAsync(FileUtil.GetImageIdFromUrlImage(oldLogo, "logoId"), folderName);
                    isDeleted = true;
                }

                existedStore.Name = updateStoreRequest.Name;
                if (updateStoreRequest.Status.ToLower().Equals(StoreEnum.Status.ACTIVE.ToString().ToLower()))
                {
                    existedStore.Status = (int)StoreEnum.Status.ACTIVE;
                }
                else if (updateStoreRequest.Status.ToLower().Equals(StoreEnum.Status.INACTIVE.ToString().ToLower()))
                {
                    existedStore.Status = (int)StoreEnum.Status.INACTIVE;
                }

                this._unitOfWork.StoreRepository.UpdateStore(existedStore);
                await this._unitOfWork.CommitAsync();

                if (isNewManager)
                {
                    string message = EmailUtil.MessageRegisterAccountForStore(emailOption.SystemName, updateStoreRequest.StoreManagerEmail, password, updateStoreRequest.Name);
                    await EmailUtil.SendEmailAndPasswordToEmail(emailOption, updateStoreRequest.StoreManagerEmail, message);
                }
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Brand id does not exist in the system."))
                {
                    fieldName = "Brand id";
                }
                else if (ex.Message.Equals("Store id does not exist in the system.") ||
                    ex.Message.Contains("does not have"))
                {
                    fieldName = "Store id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Brand id does not belong to your brand."))
                {
                    fieldName = "Brand id";
                }
                else if (ex.Message.Equals("Store Manager Email already existed in the system."))
                {
                    fieldName = "Store manager email";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                if (isUploaded && isDeleted == false)
                {
                    FileUtil.SetCredentials(fireBaseImageOption);
                    await FileUtil.DeleteImageAsync(logoId, folderName);
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task DeleteStoreAsync(int brandId, int storeId)
        {
            try
            {
                Brand existedBrand = await this._unitOfWork.BrandRepository.GetBrandByIdAsync(brandId);
                if (existedBrand == null)
                {
                    throw new NotFoundException("Brand id does not exist in the system.");
                }

                Store existedStore = await this._unitOfWork.StoreRepository.GetStoreAsync(storeId, null);
                if (existedStore == null)
                {
                    throw new NotFoundException("Store id does not exist in the system.");
                }

                if (existedStore.Brand.BrandId != brandId)
                {
                    throw new NotFoundException($"[Brand-{brandId}] does not have [store-{storeId}] in the system.");
                }

                existedStore.Status = (int)StoreEnum.Status.DEACTIVE;
                foreach (var storeAccount in existedStore.StoreAccounts)
                {
                    if (storeAccount.Account.Status == (int)AccountEnum.Status.ACTIVE)
                    {
                        storeAccount.Account.Status = (int)AccountEnum.Status.DEACTIVE;
                    }
                }

                this._unitOfWork.StoreRepository.UpdateStore(existedStore);
                await this._unitOfWork.CommitAsync();
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Brand id does not exist in the system."))
                {
                    fieldName = "Brand id";
                }
                else if (ex.Message.Equals("Store id does not exist in the system.") ||
                    ex.Message.Contains("does not have"))
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
    }
}
