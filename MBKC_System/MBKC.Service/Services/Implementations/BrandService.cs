using AutoMapper;
using MBKC.Service.DTOs;
using MBKC.Service.DTOs.Brands;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Repository.Enums;
using MBKC.Repository.Infrastructures;
using MBKC.Repository.Models;
using System.Security.Claims;
using MBKC.Service.Utils;
using MBKC.Service.Constants;
using StackExchange.Redis;
using Role = MBKC.Repository.Models.Role;
using MBKC.Repository.Redis.Models;
using static MBKC.Service.Constants.EmailMessageConstant;
using Brand = MBKC.Repository.Models.Brand;

namespace MBKC.Service.Services.Implementations
{


    public class BrandService : IBrandService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public BrandService(IUnitOfWork unitOfWork, IMapper mapper)

        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        #region Get Brands
        public async Task<GetBrandsResponse> GetBrandsAsync(GetBrandsRequest getBrandsRequest)
        {
            try
            {
                var brands = new List<Brand>();
                var brandResponse = new List<GetBrandResponse>();

                int numberItems = 0;
                if (getBrandsRequest.SearchValue != null && StringUtil.IsUnicode(getBrandsRequest.SearchValue))
                {
                    numberItems = await this._unitOfWork.BrandRepository.GetNumberBrandsAsync(getBrandsRequest.SearchValue, null);
                    brands = await this._unitOfWork.BrandRepository.GetBrandsAsync(getBrandsRequest.SearchValue, null, getBrandsRequest.CurrentPage, getBrandsRequest.ItemsPerPage,
                                                                                                              getBrandsRequest.SortBy != null && getBrandsRequest.SortBy.ToLower().EndsWith("asc") ? getBrandsRequest.SortBy.Split("_")[0] : null,
                                                                                                              getBrandsRequest.SortBy != null && getBrandsRequest.SortBy.ToLower().EndsWith("desc") ? getBrandsRequest.SortBy.Split("_")[0] : null);
                }
                else if (getBrandsRequest.SearchValue != null && StringUtil.IsUnicode(getBrandsRequest.SearchValue) == false)
                {
                    numberItems = await this._unitOfWork.BrandRepository.GetNumberBrandsAsync(null, getBrandsRequest.SearchValue);
                    brands = await this._unitOfWork.BrandRepository.GetBrandsAsync(null, getBrandsRequest.SearchValue, getBrandsRequest.CurrentPage, getBrandsRequest.ItemsPerPage,
                                                                                                              getBrandsRequest.SortBy != null && getBrandsRequest.SortBy.ToLower().EndsWith("asc") ? getBrandsRequest.SortBy.Split("_")[0] : null,
                                                                                                              getBrandsRequest.SortBy != null && getBrandsRequest.SortBy.ToLower().EndsWith("desc") ? getBrandsRequest.SortBy.Split("_")[0] : null);
                }
                else if (getBrandsRequest.SearchValue == null)
                {
                    numberItems = await this._unitOfWork.BrandRepository.GetNumberBrandsAsync(null, null);
                    brands = await this._unitOfWork.BrandRepository.GetBrandsAsync(null, null, getBrandsRequest.CurrentPage, getBrandsRequest.ItemsPerPage,
                                                                                                              getBrandsRequest.SortBy != null && getBrandsRequest.SortBy.ToLower().EndsWith("asc") ? getBrandsRequest.SortBy.Split("_")[0] : null,
                                                                                                              getBrandsRequest.SortBy != null && getBrandsRequest.SortBy.ToLower().EndsWith("desc") ? getBrandsRequest.SortBy.Split("_")[0] : null);
                }

                int totalPages = (int)((numberItems + getBrandsRequest.ItemsPerPage) / getBrandsRequest.ItemsPerPage);
                if (numberItems == 0)
                {
                    totalPages = 0;
                }

                this._mapper.Map(brands, brandResponse);
                return new GetBrandsResponse()
                {
                    Brands = brandResponse,
                    NumberItems = numberItems,
                    TotalPages = totalPages,
                };
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }

        }
        #endregion

        #region Get Brand By Id
        public async Task<GetBrandResponse> GetBrandByIdAsync(int id, IEnumerable<Claim> claims)
        {
            try
            {
                var existedBrand = await _unitOfWork.BrandRepository.GetBrandByIdAsync(id);
                if (existedBrand == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBrandId);
                }
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                Claim registeredRoleClaim = claims.First(x => x.Type.ToLower().Equals("role"));
                string email = registeredEmailClaim.Value;
                string role = registeredRoleClaim.Value;
                if (role.ToLower().Equals(RoleConstant.Brand_Manager.ToLower()))
                {
                    if (existedBrand.BrandManagerEmail.Equals(email) == false)
                    {
                        throw new BadRequestException(MessageConstant.BrandMessage.NotBelongToBrand);
                    }
                }

                var brandResponse = this._mapper.Map<GetBrandResponse>(existedBrand);

                return brandResponse;
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.BrandMessage.NotBelongToBrand))
                {
                    fieldName = "Brand id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistBrandId))
                {
                    fieldName = "Brand id";
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
        #endregion

        #region CreateBrand
        public async Task CreateBrandAsync(PostBrandRequest postBrandRequest)
        {
            string folderName = "Brands";
            string logoId = "";
            bool uploaded = false;
            try
            {
                var checkDupplicatedEmail = await _unitOfWork.AccountRepository.GetAccountByEmailAsync(postBrandRequest.ManagerEmail);
                if (checkDupplicatedEmail != null)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.AlreadyExistEmail);
                }
                var brandManagerRole = await _unitOfWork.RoleRepository.GetRoleById((int)RoleEnum.Role.BRAND_MANAGER);

                // Upload image to firebase
                FileStream fileStream = FileUtil.ConvertFormFileToStream(postBrandRequest.Logo);
                Guid guild = Guid.NewGuid();
                logoId = guild.ToString();
                string urlImage = await this._unitOfWork.FirebaseStorageRepository.UploadImageAsync(fileStream, folderName, logoId);
                if (urlImage != null)
                {
                    uploaded = true;
                }
                // Create account
                string unEncryptedPassword = PasswordUtil.CreateRandomPassword();
                var account = new Account
                {
                    Email = postBrandRequest.ManagerEmail,
                    Password = StringUtil.EncryptData(unEncryptedPassword),
                    Status = (int)AccountEnum.Status.ACTIVE,
                    Role = brandManagerRole,
                    IsConfirmed = false
                };

                // Create brand
                Brand brand = new Brand
                {
                    Name = postBrandRequest.Name,
                    Address = postBrandRequest.Address,
                    Logo = urlImage + $"&logoId={logoId}",
                    Status = (int)BrandEnum.Status.ACTIVE,
                    BrandManagerEmail = postBrandRequest.ManagerEmail,
                };

                // Create brand account
                BrandAccount brandAccount = new BrandAccount
                {
                    Account = account,
                    Brand = brand,
                };

                brand.BrandAccounts = new List<BrandAccount>() { brandAccount };

                await _unitOfWork.BrandRepository.CreateBrandAsync(brand);

                await _unitOfWork.CommitAsync();

                //Send password to email of Brand Manager
                string messageBody = EmailMessageConstant.Brand.Message + $" \"{brand.Name}\" " + EmailMessageConstant.CommonMessage.Message;
                string message = this._unitOfWork.EmailRepository.GetMessageToRegisterAccount(account.Email, unEncryptedPassword, messageBody);
                await this._unitOfWork.EmailRepository.SendAccountToEmailAsync(account.Email, message);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.AlreadyExistEmail))
                {
                    fieldName = "Manager email";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                if (uploaded)
                {
                    await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(logoId, folderName);
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region Update Brand
        public async Task UpdateBrandAsync(int brandId, UpdateBrandRequest updateBrandRequest)
        {
            string folderName = "Brands";
            string logoId = "";
            bool isUploaded = false;
            bool isDeleted = false;
            bool isNewManager = false;
            try
            {
                var brand = await _unitOfWork.BrandRepository.GetBrandByIdAsync(brandId);
                if (brand == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBrandId);
                }
                if (brand.Status == (int)BrandEnum.Status.DISABLE)
                {
                    throw new BadRequestException(MessageConstant.BrandMessage.DeactiveBrand_Update);
                }

                string password = "";
                var checkAccountExisted = await this._unitOfWork.AccountRepository.GetAccountByEmailAsync(updateBrandRequest.BrandManagerEmail);
                if (checkAccountExisted != null && brand.BrandManagerEmail.Equals(updateBrandRequest.BrandManagerEmail) == false)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.AlreadyExistEmail);
                }
                if (checkAccountExisted == null)
                {
                    // Inactive old Brand Manager Email
                    var getBrandAccountById = await this._unitOfWork.BrandAccountRepository.GetBrandAccountByBrandIdAsync(brandId);
                    if (getBrandAccountById != null)
                    {
                        getBrandAccountById.Account.Status = (int)AccountEnum.Status.DISABLE;
                        _unitOfWork.BrandAccountRepository.UpdateBrandAccount(getBrandAccountById);
                    }
                    Role brandManagerRole = await this._unitOfWork.RoleRepository.GetRoleById((int)RoleEnum.Role.BRAND_MANAGER);
                    password = PasswordUtil.CreateRandomPassword();
                    Account newBrandManagerAccount = new Account()
                    {
                        Email = updateBrandRequest.BrandManagerEmail,
                        Password = StringUtil.EncryptData(password),
                        Role = brandManagerRole,
                        Status = (int)AccountEnum.Status.ACTIVE
                    };

                    await this._unitOfWork.AccountRepository.CreateAccountAsync(newBrandManagerAccount);

                    BrandAccount newBrandAccount = new BrandAccount()
                    {
                        Account = newBrandManagerAccount,
                        Brand = brand
                    };

                    await this._unitOfWork.BrandAccountRepository.CreateBrandAccount(newBrandAccount);
                    isNewManager = true;
                    brand.BrandManagerEmail = updateBrandRequest.BrandManagerEmail;
                }

                string oldLogo = brand.Logo;
                if (updateBrandRequest.Logo != null)
                {
                    // Upload image to firebase
                    FileStream fileStream = FileUtil.ConvertFormFileToStream(updateBrandRequest.Logo);
                    Guid guild = Guid.NewGuid();
                    logoId = guild.ToString();
                    var urlImage = await this._unitOfWork.FirebaseStorageRepository.UploadImageAsync(fileStream, folderName, logoId);
                    if (urlImage != null)
                    {
                        isUploaded = true;
                    }
                    brand.Logo = urlImage + $"&logoId={logoId}";

                    //Delete image from database
                    await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(FileUtil.GetImageIdFromUrlImage(oldLogo, "logoId"), folderName);
                    isDeleted = true;
                }

                brand.Address = updateBrandRequest.Address;
                brand.Name = updateBrandRequest.Name;

                if (updateBrandRequest.Status.ToLower().Equals(CategoryEnum.Status.ACTIVE.ToString().ToLower()))
                {
                    brand.Status = (int)CategoryEnum.Status.ACTIVE;
                }
                else if (updateBrandRequest.Status.ToLower().Equals(CategoryEnum.Status.INACTIVE.ToString().ToLower()))
                {
                    brand.Status = (int)CategoryEnum.Status.INACTIVE;
                }

                _unitOfWork.BrandRepository.UpdateBrand(brand);
                await _unitOfWork.CommitAsync();

                if (isNewManager)
                {
                    string messageBody = EmailMessageConstant.Brand.Message + $" \"{brand.Name}\" " + EmailMessageConstant.CommonMessage.Message;
                    string message = this._unitOfWork.EmailRepository.GetMessageToRegisterAccount(updateBrandRequest.BrandManagerEmail, password, messageBody);
                    await this._unitOfWork.EmailRepository.SendAccountToEmailAsync(updateBrandRequest.BrandManagerEmail, message);
                }
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.BrandMessage.DeactiveBrand_Update))
                {
                    fieldName = "Updated brand failed";
                }
                else if (ex.Message.Equals(MessageConstant.BrandMessage.RoleNotSuitable))
                {
                    fieldName = "Role from account";
                }
                else if (ex.Message.Equals(MessageConstant.BrandMessage.ManagerEmailExisted) ||
                    ex.Message.Equals(MessageConstant.CommonMessage.AlreadyExistEmail))
                {
                    fieldName = "Manager Email";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistBrandId))
                {
                    fieldName = "Brand id";
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
        #endregion

        #region Deactive Brand By Id
        public async Task DeActiveBrandByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidBrandId);
                }
                var brand = await _unitOfWork.BrandRepository.GetBrandByIdAsync(id);

                if (brand == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBrandId);
                }

                if (brand.Status == (int)BrandEnum.Status.DISABLE)
                {
                    throw new BadRequestException(MessageConstant.BrandMessage.DeactiveBrand_Delete);
                }

                // Deactive status of brand
                brand.Status = (int)BrandEnum.Status.DISABLE;

                // Deactive Manager Account of brand
                foreach (var brandAccount in brand.BrandAccounts)
                {
                    brandAccount.Account.Status = (int)AccountEnum.Status.DISABLE;
                }

                // Deactive products belong to brand
                if (brand.Products.Any())
                {
                    foreach (var product in brand.Products)
                    {
                        product.Status = (int)ProductEnum.Status.DISABLE;
                    }
                }

                // Deactive categories belong to brand
                if (brand.Categories.Any())
                {
                    foreach (var category in brand.Categories)
                    {
                        category.Status = (int)CategoryEnum.Status.DISABLE;
                    }
                    // Deactive extra categories belong to brand
                    foreach (var category in brand.Categories)
                    {
                        foreach (var extra in category.ExtraCategoryProductCategories)
                        {
                            extra.Status = (int)ExtraCategoryEnum.Status.DISABLE;
                        }
                    }
                }

                // Change status brand's store to INACTIVE
                if (brand.Stores.Any())
                {
                    foreach (var store in brand.Stores)
                    {
                        store.Status = (int)StoreEnum.Status.DISABLE;
                        if (store.StorePartners.Any())
                        {
                            foreach (var storePartner in store.StorePartners)
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
                    }
                }
                this._unitOfWork.BrandRepository.UpdateBrand(brand);
                await this._unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidBrandId))
                {
                    fieldName = "Brand id";
                }
                else if (ex.Message.Equals(MessageConstant.BrandMessage.DeactiveBrand_Delete))
                {
                    fieldName = "Deleted brand failed";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (NotFoundException ex)
            {
                string fileName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistBrandId))
                {
                    fileName = "Brand id";
                }
                string error = ErrorUtil.GetErrorString(fileName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Error", ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region Update Brand Status
        public async Task UpdateBrandStatusAsync(int brandId, UpdateBrandStatusRequest updateBrandStatusRequest)
        {
            try
            {
                var brand = await _unitOfWork.BrandRepository.GetBrandByIdAsync(brandId);
                if (brand == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBrandId);
                }
                if (brand.Status == (int)BrandEnum.Status.DISABLE)
                {
                    throw new BadRequestException(MessageConstant.BrandMessage.DeactiveBrand_Update);
                }

                if (updateBrandStatusRequest.Status.Trim().ToLower().Equals(BrandEnum.Status.ACTIVE.ToString().ToLower()))
                {
                    brand.Status = (int)BrandEnum.Status.ACTIVE;
                }
                else if (updateBrandStatusRequest.Status.Trim().ToLower().Equals(BrandEnum.Status.INACTIVE.ToString().ToLower()))
                {
                    brand.Status = (int)BrandEnum.Status.INACTIVE;
                }
                this._unitOfWork.BrandRepository.UpdateBrand(brand);
                await this._unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.BrandMessage.DeactiveBrand_Update))
                {
                    fieldName = "Updated brand failed";
                }
                else if (ex.Message.Equals(MessageConstant.BrandMessage.NotBelongToBrand))
                {
                    fieldName = "Brand id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Brand id", ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region Update Brand Profile
        public async Task UpdateBrandProfileAsync(int brandId, UpdateBrandProfileRequest updateBrandProfileRequest, IEnumerable<Claim> claims)
        {
            string folderName = "Brands";
            string logoId = "";
            bool isUploaded = false;
            bool isDeleted = false;
            try
            {
                var existedBrand = await _unitOfWork.BrandRepository.GetBrandByIdAsync(brandId);
                if (existedBrand == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBrandId);
                }
                if (existedBrand.Status == (int)BrandEnum.Status.DISABLE)
                {
                    throw new BadRequestException(MessageConstant.BrandMessage.DeactiveBrand_Update);
                }

                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                Claim registeredRoleClaim = claims.First(x => x.Type.ToLower().Equals("role"));
                string email = registeredEmailClaim.Value;
                string role = registeredRoleClaim.Value;
                if (role.ToLower().Equals(RoleConstant.Brand_Manager.ToLower()))
                {
                    if (existedBrand.BrandManagerEmail.Equals(email) == false)
                    {
                        throw new BadRequestException(MessageConstant.BrandMessage.NotBelongToBrand);
                    }
                }

                if (updateBrandProfileRequest.Logo != null)
                {
                    // Upload image to firebase
                    FileStream fileStream = FileUtil.ConvertFormFileToStream(updateBrandProfileRequest.Logo);
                    Guid guild = Guid.NewGuid();
                    logoId = guild.ToString();
                    var urlImage = await this._unitOfWork.FirebaseStorageRepository.UploadImageAsync(fileStream, folderName, logoId);
                    if (urlImage != null)
                    {
                        isUploaded = true;
                    }
                    existedBrand.Logo = urlImage + $"&logoId={logoId}";

                    //Delete image from database
                    await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(FileUtil.GetImageIdFromUrlImage(existedBrand.Logo, "logoId"), folderName);
                    isDeleted = true;
                }

                existedBrand.Address = updateBrandProfileRequest.Address;
                existedBrand.Name = updateBrandProfileRequest.Name;
                this._unitOfWork.BrandRepository.UpdateBrand(existedBrand);
                await this._unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.BrandMessage.NotBelongToBrand))
                {
                    fieldName = "Brand id";
                }
                else if (ex.Message.Equals(MessageConstant.BrandMessage.DeactiveBrand_Update))
                {
                    fieldName = "Updated brand failed";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistBrandId))
                {
                    fieldName = "Brand id";
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
        #endregion

        public async Task<GetBrandResponse> GetBrandProfileAsync(IEnumerable<Claim> claims)
        {
            try
            {
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                string email = registeredEmailClaim.Value;

                Brand existedBrand = await this._unitOfWork.BrandRepository.GetBrandAsync(email);
                GetBrandResponse getBrandResponse = this._mapper.Map<GetBrandResponse>(existedBrand);
                return getBrandResponse;
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
    }
}
