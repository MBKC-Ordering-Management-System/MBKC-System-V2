﻿using AutoMapper;
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
        public async Task<GetBrandsResponse> GetBrandsAsync(string? keySearchName, string? keyStatusFilter, int? pageNumber, int? pageSize, bool? isGetAll)
        {
            try
            {
                var brands = new List<Brand>();
                var brandResponse = new List<GetBrandResponse>();
                if (isGetAll != null && isGetAll.Value == true)
                {
                    pageNumber = null;
                    pageSize = null;
                }
                else
                {
                    if (pageNumber != null && pageNumber <= 0)
                    {
                        throw new BadRequestException(MessageConstant.CommonMessage.InvalidCurrentPage);
                    }
                    else if (pageNumber == null)
                    {
                        pageNumber = 1;
                    }

                    if (pageSize != null && pageSize <= 0)
                    {
                        throw new BadRequestException(MessageConstant.CommonMessage.InvalidItemsPerPage);
                    }
                    else if (pageSize == null)
                    {
                        pageSize = 5;
                    }
                }

                int? keyStatus = null;
                if (keyStatusFilter != null)
                {
                    if (BrandEnum.Status.ACTIVE.ToString().ToLower().Equals(keyStatusFilter.Trim().ToLower()))
                    {
                        keyStatus = (int)BrandEnum.Status.ACTIVE;
                    }
                    else if (BrandEnum.Status.INACTIVE.ToString().ToLower().Equals(keyStatusFilter.Trim().ToLower()))
                    {
                        keyStatus = (int)BrandEnum.Status.INACTIVE;
                    }
                    else
                    {
                        throw new BadRequestException(MessageConstant.BrandMessage.InvalidStatusFilter);
                    }
                }

                int numberItems = 0;
                if (keySearchName != null && StringUtil.IsUnicode(keySearchName))
                {
                    numberItems = await this._unitOfWork.BrandRepository.GetNumberBrandsAsync(keySearchName, null, keyStatus);
                    brands = await this._unitOfWork.BrandRepository.GetBrandsAsync(keySearchName, null, keyStatus, pageSize, pageNumber);
                }
                else if (keySearchName != null && StringUtil.IsUnicode(keySearchName) == false)
                {
                    numberItems = await this._unitOfWork.BrandRepository.GetNumberBrandsAsync(null, keySearchName, keyStatus);
                    brands = await this._unitOfWork.BrandRepository.GetBrandsAsync(null, keySearchName, keyStatus, pageSize, pageNumber);
                }
                else if (keySearchName == null)
                {
                    numberItems = await this._unitOfWork.BrandRepository.GetNumberBrandsAsync(null, null, keyStatus);
                    brands = await this._unitOfWork.BrandRepository.GetBrandsAsync(null, null, keyStatus, pageSize, pageNumber);
                }

                this._mapper.Map(brands, brandResponse);

                int totalPages = 0;
                if (numberItems > 0 && isGetAll == null || numberItems > 0 && isGetAll != null && isGetAll == false)
                {
                    totalPages = (int)((numberItems + pageSize.Value) / pageSize.Value);
                }

                if (numberItems == 0)
                {
                    totalPages = 0;
                }
                return new GetBrandsResponse()
                {
                    Brands = brandResponse,
                    NumberItems = numberItems,
                    TotalPages = totalPages,
                };
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.BrandMessage.InvalidStatusFilter))
                {
                    fieldName = "Key status filter";
                }
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidCurrentPage))
                {
                    fieldName = "Current page";
                }
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidItemsPerPage))
                {
                    fieldName = "Items per page";
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
        #endregion

        #region Get Brand By Id
        public async Task<GetBrandResponse> GetBrandByIdAsync(int id, IEnumerable<Claim> claims)
        {
            try
            {
                if (id <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidBrandId);
                }
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
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidBrandId) ||
                    ex.Message.Equals(MessageConstant.BrandMessage.NotBelongToBrand))
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
                string unEncryptedPassword = RandomPasswordUtil.CreateRandomPassword();
                var account = new Account
                {
                    Email = postBrandRequest.ManagerEmail,
                    Password = StringUtil.EncryptData(unEncryptedPassword),
                    Status = (int)AccountEnum.Status.ACTIVE,
                    Role = brandManagerRole
                };
                await _unitOfWork.AccountRepository.CreateAccountAsync(account);

                // Create brand
                Brand brand = new Brand
                {
                    Name = postBrandRequest.Name,
                    Address = postBrandRequest.Address,
                    Logo = urlImage + $"&logoId={logoId}",
                    Status = (int)BrandEnum.Status.ACTIVE,
                    BrandManagerEmail = postBrandRequest.ManagerEmail,
                };
                await _unitOfWork.BrandRepository.CreateBrandAsync(brand);

                // Create brand account
                BrandAccount brandAccount = new BrandAccount
                {
                    Account = account,
                    Brand = brand,
                };
                await _unitOfWork.BrandAccountRepository.CreateBrandAccount(brandAccount);
                await _unitOfWork.CommitAsync();

                //Send password to email of Brand Manager
                string messageBody = EmailMessageConstant.Brand.Message + $" \"{brand.Name}\" " + EmailMessageConstant.CommonMessage.Message;
                string message = this._unitOfWork.EmailRepository.GetMessageToRegisterAccount(account.Email, unEncryptedPassword, messageBody);
                await this._unitOfWork.EmailRepository.SendEmailAndPasswordToEmail(account.Email, message);
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
                if (brandId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidBrandId);
                }
                var brand = await _unitOfWork.BrandRepository.GetBrandByIdAsync(brandId);
                if (brand == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBrandId);
                }
                if (brand.Status == (int)BrandEnum.Status.DEACTIVE)
                {
                    throw new BadRequestException(MessageConstant.BrandMessage.DeactiveBrand_Update);
                }

                string password = "";
                if (brand.BrandAccounts.FirstOrDefault(x => x.Account.Role.RoleId == (int)RoleEnum.Role.BRAND_MANAGER
                                                        && x.Account.Status == (int)AccountEnum.Status.ACTIVE).Account.Email.Equals(updateBrandRequest.BrandManagerEmail) == false)
                {
                    Account existedAccount = await this._unitOfWork.AccountRepository.GetAccountAsync(updateBrandRequest.BrandManagerEmail);
                    if (existedAccount != null)
                    {
                        throw new BadRequestException(MessageConstant.BrandMessage.ManagerEmailExisted);
                    }
                    brand.BrandAccounts.FirstOrDefault(x => x.Account.Role.RoleId == (int)RoleEnum.Role.BRAND_MANAGER
                                                        && x.Account.Status == (int)AccountEnum.Status.ACTIVE).Account.Status = (int)AccountEnum.Status.DEACTIVE;

                    Role brandManagerRole = await this._unitOfWork.RoleRepository.GetRoleById((int)RoleEnum.Role.BRAND_MANAGER);
                    password = RandomPasswordUtil.CreateRandomPassword();
                    Account newBrandManagerAccount = new Account()
                    {
                        Email = updateBrandRequest.BrandManagerEmail,
                        Password = StringUtil.EncryptData(password),
                        Role = brandManagerRole,
                        Status = (int)AccountEnum.Status.ACTIVE
                    };

                    BrandAccount newBrandAccount = new BrandAccount()
                    {
                        Account = newBrandManagerAccount,
                        Brand = brand
                    };

                    brand.BrandAccounts.ToList().Add(newBrandAccount);
                    isNewManager = true;
                    brand.BrandManagerEmail = updateBrandRequest.BrandManagerEmail;
                }

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
                    await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(FileUtil.GetImageIdFromUrlImage(brand.Logo, "logoId"), "Brands");
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
                    await this._unitOfWork.EmailRepository.SendEmailAndPasswordToEmail(updateBrandRequest.BrandManagerEmail, message);
                }
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidBrandId))
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

                if (brand.Status == (int)BrandEnum.Status.DEACTIVE)
                {
                    throw new BadRequestException(MessageConstant.BrandMessage.DeactiveBrand_Delete);
                }

                // Deactive status of brand
                brand.Status = (int)BrandEnum.Status.DEACTIVE;

                // Deactive Manager Account of brand
                foreach (var brandAccount in brand.BrandAccounts)
                {
                    brandAccount.Account.Status = (int)AccountEnum.Status.DEACTIVE;
                }

                // Deactive products belong to brand
                if (brand.Products.Any())
                {
                    foreach (var product in brand.Products)
                    {
                        product.Status = (int)ProductEnum.Status.DEACTIVE;
                    }
                }

                // Deactive categories belong to brand
                if (brand.Categories.Any())
                {
                    foreach (var category in brand.Categories)
                    {
                        category.Status = (int)CategoryEnum.Status.DEACTIVE;
                    }
                    // Deactive extra categories belong to brand
                    foreach (var category in brand.Categories)
                    {
                        foreach (var extra in category.ExtraCategoryProductCategories)
                        {
                            extra.Status = (int)ExtraCategoryEnum.Status.DEACTIVE;
                        }
                    }
                }

                // Change status brand's store to NOT_RENT
                if (brand.Stores.Any())
                {
                    foreach (var store in brand.Stores)
                    {
                        store.Status = (int)StoreEnum.Status.INACTIVE;
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
                if (brandId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidBrandId);
                }
                var brand = await _unitOfWork.BrandRepository.GetBrandByIdAsync(brandId);
                if (brand == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBrandId);
                }
                if (brand.Status == (int)BrandEnum.Status.DEACTIVE)
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
                else if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidBrandId) ||
                    ex.Message.Equals(MessageConstant.BrandMessage.NotBelongToBrand))
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
                if (brandId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidBrandId);
                }
                var existedBrand = await _unitOfWork.BrandRepository.GetBrandByIdAsync(brandId);
                if (existedBrand == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBrandId);
                }
                if (existedBrand.Status == (int)BrandEnum.Status.DEACTIVE)
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
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidBrandId) ||
                    ex.Message.Equals(MessageConstant.BrandMessage.NotBelongToBrand))
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
    }
}