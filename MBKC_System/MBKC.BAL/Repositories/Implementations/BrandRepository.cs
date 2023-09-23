using AutoMapper;
using MBKC.BAL.DTOs.Brands;
using MBKC.BAL.DTOs.FireBase;
using MBKC.BAL.DTOs.Verifications;
using MBKC.BAL.Exceptions;
using MBKC.BAL.Repositories.Interfaces;
using MBKC.BAL.Utils;
using MBKC.DAL.DBContext;
using MBKC.DAL.Enums;
using MBKC.DAL.Infrastructures;
using MBKC.DAL.Models;
using MBKC.DAL.RedisModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MBKC.BAL.Repositories.Implementations
{
    public class BrandRepository : IBrandRepository
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public BrandRepository(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }
        #region CreateBrand
        public async Task<GetBrandResponse> CreateBrandAsync(PostBrandRequest postBrandRequest, FireBaseImage fireBaseImage, Email emailSystem)
        {
            string logoId = "";
            bool uploaded = false;
            try
            {
                var checkDupplicatedEmail = await _unitOfWork.AccountDAO.GetAccountByEmailAsync(postBrandRequest.ManagerEmail);
                if (checkDupplicatedEmail != null)
                {
                    throw new ConflictException("Email already exists in the system");
                }
                var brandManagerRole = await _unitOfWork.RoleDAO.GetRoleById((int)RoleEnum.Role.BRAND_MANAGER);

                // Upload image to firebase
                FileStream fileStream = Utils.FileUtil.ConvertFormFileToStream(postBrandRequest.Logo);
                FileUtil.SetCredentials(fireBaseImage);
                Guid guild = Guid.NewGuid();
                logoId = guild.ToString();
                string urlImage = await Utils.FileUtil.UploadImage(fileStream, "Brand", logoId);
                uploaded = true;

                // Create account
                string unEncryptedPassword = RandomNumberUtil.GenerateEightDigitNumber().ToString();
                var account = new Account
                {
                    Email = postBrandRequest.ManagerEmail,
                    Password = StringUtil.EncryptData(unEncryptedPassword),
                    Status = Convert.ToBoolean(AccountEnum.Status.ACTIVE),
                    Role = brandManagerRole
                };
                await _unitOfWork.AccountDAO.CreateAccountAsync(account);

                // Create brand
                Brand brand = new Brand
                {
                    Name = postBrandRequest.Name,
                    Address = postBrandRequest.Address,
                    Logo = urlImage + $"&logoId={logoId}",
                    Status = (int)BrandEnum.Status.ACTIVE,
                };
                await _unitOfWork.BrandDAO.CreateBrand(brand);

                // Create brand account
                BrandAccount brandAccount = new BrandAccount
                {
                    Account = account,
                    Brand = brand,
                };
                await _unitOfWork.BrandAccountDAO.CreateBrandAccount(brandAccount);
                await _unitOfWork.CommitAsync();

                //Send password to email of Brand Manager
                await EmailUtil.SendEmailAndPasswordToEmail(emailSystem, account.Email, EmailUtil.MessageRegisterAccount(emailSystem.SystemName, account.Email, unEncryptedPassword), "Brand Manager");
                // add to Redis
                var brandRedisModel = new BrandRedisModel();
                var accountRedisModel = new AccountRedisModel();
                var brandAcocuntRedisModel = new BrandAccountRedisModel();
                _mapper.Map(brand, brandRedisModel);
                _mapper.Map(account, accountRedisModel);
                _mapper.Map(brandAccount, brandAcocuntRedisModel);
                await this._unitOfWork.AccountRedisDAO.AddAccountAsync(accountRedisModel);
                await this._unitOfWork.BrandRedisDAO.AddBrandAsync(brandRedisModel);
                await this._unitOfWork.BrandAccountRedisDAO.AddBrandAccountAsync(brandAcocuntRedisModel);
                return _mapper.Map<GetBrandResponse>(brand);
            }
            catch (ConflictException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Email already exists in the system"))
                {
                    fieldName = "Email";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new ConflictException(error);
            }
            catch (Exception ex)
            {
                if (uploaded)
                {
                    await FileUtil.DeleteImageAsync(logoId, "Brand");
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region UpdateBrand
        public async Task<GetBrandResponse> UpdateBrandAsync(int brandId, UpdateBrandRequest updateBrandRequest, FireBaseImage fireBaseImage)
        {
            string logoId = "";
            bool uploaded = false;
            try
            {
                var brand = await _unitOfWork.BrandDAO.GetBrandById(brandId);
                if (brand == null)
                {
                    throw new NotFoundException("Brand does not exist in the system");
                }

                if (brand.Status == (int)BrandEnum.Status.DEACTIVE)
                {
                    throw new BadRequestException("Can't update Brand DEACTIVED");
                }
                if (updateBrandRequest.Status != (int)BrandEnum.Status.ACTIVE &&
                    updateBrandRequest.Status != (int)BrandEnum.Status.INACTIVE)
                {
                    throw new BadRequestException("Status of Brand are 0(INACTIVE), 1(ACTIVE)");
                }
                if (updateBrandRequest.Logo != null)
                {
                    //Delete image from database
                    FileUtil.SetCredentials(fireBaseImage);
                    Uri uri = new Uri(brand.Logo);
                    logoId = HttpUtility.ParseQueryString(uri.Query).Get("logoId");
                    await FileUtil.DeleteImageAsync(logoId, "Brand");

                    // Upload image to firebase
                    FileStream fileStream = Utils.FileUtil.ConvertFormFileToStream(updateBrandRequest.Logo);
                    Guid guild = Guid.NewGuid();
                    logoId = guild.ToString();
                    var urlImage = await Utils.FileUtil.UploadImage(fileStream, "Brand", logoId);
                    brand.Logo = urlImage;
                    uploaded = true;
                }

                brand.Address = updateBrandRequest.Address;
                brand.Name = updateBrandRequest.Name;
                brand.Status = updateBrandRequest.Status;
                _unitOfWork.BrandDAO.UpdateBrand(brand);
                _unitOfWork.Commit();

                //Get brand from Redis
                var brandRedis = await _unitOfWork.BrandRedisDAO.GetBrandByIdAsync(brandId.ToString());
                _mapper.Map(brand, brandRedis);
                //Update brand to Redis
                await this._unitOfWork.BrandRedisDAO.UpdateBrandAsync(brandRedis);
                return _mapper.Map<GetBrandResponse>(brand);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Can't update Brand DEACTIVED") ||
                    ex.Message.Equals("Status of Brand are 0(INACTIVE), 1(ACTIVE)"))
                {
                    fieldName = "Status";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Brand does not exist in the system"))
                {
                    fieldName = "BrandId";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                if (uploaded)
                {
                    await FileUtil.DeleteImageAsync(logoId, "Brand");
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region Get Brands
        public async Task<Tuple<List<GetBrandResponse>, int, int?, int?>> GetBrandsAsync(SearchBrandRequest? searchBrandRequest, int? PAGE_NUMBER, int? PAGE_SIZE)
        {
            try
            {
                var brandResponse = new List<GetBrandResponse>();
                var brandsRedis = await this._unitOfWork.BrandRedisDAO.GetBrandsAsync();

                _mapper.Map(brandsRedis, brandResponse);
                if (PAGE_SIZE == null)
                {
                    PAGE_SIZE = 10;
                }

                if (PAGE_NUMBER == null)
                {
                    PAGE_NUMBER = 1;
                }
                // Search
                if (searchBrandRequest.KeySearchName != "" && searchBrandRequest.KeySearchName != null)
                {
                    brandResponse = brandResponse.Where(b => b.Name.ToLower().Contains(searchBrandRequest.KeySearchName.Trim().ToLower())).ToList();
                }
                if (searchBrandRequest.KeyStatusFilter != "" && searchBrandRequest.KeyStatusFilter != null)
                {
                    if (Enum.IsDefined(typeof(BrandEnum.StatusFilter), searchBrandRequest.KeyStatusFilter))
                    {
                        switch (searchBrandRequest.KeyStatusFilter)
                        {
                            case nameof(BrandEnum.StatusFilter.INACTIVE):
                                brandResponse = brandResponse.Where(b => b.Status == (int)BrandEnum.StatusFilter.INACTIVE).ToList();
                                break;
                            case nameof(BrandEnum.StatusFilter.ACTIVE):
                                brandResponse = brandResponse.Where(b => b.Status == (int)BrandEnum.StatusFilter.ACTIVE).ToList();
                                break;
                            case nameof(BrandEnum.StatusFilter.DEACTIVE):
                                brandResponse = brandResponse.Where(b => b.Status == (int)BrandEnum.StatusFilter.DEACTIVE).ToList();
                                break;
                        }
                    }
                    else
                    {
                        throw new BadRequestException("Key Filter not valid");
                    }
                }

                // Count total page
                int totalRecords = brandResponse.Count();
                int totalPages = (int)Math.Ceiling((double)((double)totalRecords / PAGE_SIZE));

                // Paing
                brandResponse = brandResponse.Skip((int)((PAGE_NUMBER - 1) * PAGE_SIZE)).Take((int)PAGE_SIZE).ToList();

                return Tuple.Create(brandResponse, totalPages, PAGE_NUMBER, PAGE_SIZE);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Key Filter not valid"))
                {
                    fieldName = "KeyStatusFilter";
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
        public async Task<GetBrandResponse> GetBrandByIdAsync(int id)
        {
            try
            {
                var brandRedis = await this._unitOfWork.BrandRedisDAO.GetBrandByIdAsync(id.ToString());
                var brand = new Brand();
                if (brandRedis == null)
                {
                    brand = await _unitOfWork.BrandDAO.GetBrandById(id);
                    if (brand == null)
                    {
                        throw new NotFoundException("Brand does not exist in the system");
                    }
                }
                return _mapper.Map<GetBrandResponse>(brandRedis);
            }
            catch (NotFoundException ex)
            {
                string fileName = "";
                if (ex.Message.Equals("Brand does not exist in the system"))
                {
                    fileName = "BrandId";
                }
                string error = ErrorUtil.GetErrorString(fileName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
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
                var brand = await _unitOfWork.BrandDAO.GetBrandById(id);

                if (brand == null)
                {
                    throw new NotFoundException("Brand does not exist in the system");
                }



                // Deactive status of brand
                brand.Status = (int)BrandEnum.Status.DEACTIVE;

                // Inactive Manager Account of brand
                foreach (var brandAccount in brand.BrandAccounts)
                {
                    brandAccount.Account.Status = Convert.ToBoolean(AccountEnum.Status.INACTIVE);
                }

                // Inactive products belong to brand
                if (brand.Products.Any())
                {
                    foreach (var product in brand.Products)
                    {
                        product.Status = (int)ProductEnum.Status.DEACTIVE;
                    }
                }


                // Inactive categories belong to brand
                if (brand.Categories.Any())
                {
                    foreach (var category in brand.Categories)
                    {
                        category.Status = (int)CategoryEnum.Status.DEACTIVE;
                    }
                    // Inactive extra categories belong to brand
                    foreach (var category in brand.Categories)
                    {
                        foreach (var extra in category.ExtraCategoryProductCategories)
                        {
                            extra.Status = (int)ExtraCategoryEnum.Status.DEACTIVE;
                        }
                    }
                }

                // Inactive brand's store
                if (brand.Stores.Any())
                {
                    foreach (var store in brand.Stores)
                    {
                        store.Status = (int)StoreEnum.Status.NOT_RENT;
                    }
                }
                _unitOfWork.BrandDAO.UpdateBrand(brand);
                _unitOfWork.Commit();

                var brandRedis = await _unitOfWork.BrandRedisDAO.GetBrandByIdAsync(id.ToString());
                if (brandRedis == null)
                {
                    throw new NotFoundException("Brand does not exist in the system");
                }

            }
            catch (NotFoundException ex)
            {
                string fileName = "";
                if (ex.Message.Equals("Brand does not exist in the system"))
                {
                    fileName = "BrandId";
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
    }
}
