using AutoMapper;
using MBKC.Repository.Enums;
using MBKC.Repository.Infrastructures;
using MBKC.Repository.Models;
using MBKC.Service.Constants;
using MBKC.Service.DTOs.Categories;
using MBKC.Service.DTOs.SplitIdCategories;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace MBKC.Service.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }
        #region Create Category
        public async Task CreateCategoryAsync(PostCategoryRequest postCategoryRequest, HttpContext httpContext)
        {
            string folderName = "Categories";
            string imageId = "";
            bool isUploaded = false;
            try
            {
                // Get brand from JWT
                JwtSecurityToken jwtSecurityToken = TokenUtil.ReadToken(httpContext);
                string accountId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid).Value;
                var brandAccount = await _unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId));
                var brand = brandAccount.Brand;

                var existedCategoryCode = await _unitOfWork.CategoryRepository.GetCategoryByCodeAsync(postCategoryRequest.Code, brand.BrandId);
                if (existedCategoryCode != null)
                {
                    throw new BadRequestException(MessageConstant.CategoryMessage.CategoryCodeExistedInBrand);
                }

                var existedCategoryName = await _unitOfWork.CategoryRepository.GetCategoryByNameAsync(postCategoryRequest.Name, brand.BrandId);
                if (existedCategoryName != null)
                {
                    throw new BadRequestException(MessageConstant.CategoryMessage.CategoryNameExistedInBrand);
                }
                // Upload image to firebase
                FileStream fileStream = Utils.FileUtil.ConvertFormFileToStream(postCategoryRequest.ImageUrl);
                Guid guild = Guid.NewGuid();
                imageId = guild.ToString();
                var urlImage = await this._unitOfWork.FirebaseStorageRepository.UploadImageAsync(fileStream, folderName, imageId);
                if (urlImage != null)
                {
                    isUploaded = true;
                }
                //Create category
                var category = new Category()
                {
                    Code = postCategoryRequest.Code,
                    Name = postCategoryRequest.Name,
                    Type = postCategoryRequest.Type.ToUpper(),
                    DisplayOrder = postCategoryRequest.DisplayOrder,
                    ImageUrl = urlImage + $"&imageUrl={imageId}",
                    Description = postCategoryRequest.Description,
                    Brand = brand,
                    Status = (int)CategoryEnum.Status.ACTIVE
                };
                await _unitOfWork.CategoryRepository.CreateCategoryAsyncAsync(category);
                await _unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CategoryMessage.CategoryCodeExistedInBrand))
                {
                    fieldName = "Category code";
                }
                else if (ex.Message.Equals(MessageConstant.CategoryMessage.CategoryNameExistedInBrand))
                {
                    fieldName = "Category name";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                if (isUploaded)
                {
                    await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(imageId, folderName);
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region Update Category
        public async Task UpdateCategoryAsync(int categoryId, UpdateCategoryRequest updateCategoryRequest, HttpContext httpContext)
        {
            string folderName = "Categories";
            string imageId = "";
            bool isUploaded = false;
            bool isDeleted = false;
            try
            {
                if (categoryId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidCategoryId);
                }

                // Get brandId from JWT
                JwtSecurityToken jwtSecurityToken = TokenUtil.ReadToken(httpContext);
                string accountId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid).Value;
                var brandAccount = await _unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId));
                var brandId = brandAccount.BrandId;
                // Check category belong to brand or not.
                var checkCategoryIdExisted = brandAccount.Brand.Categories.SingleOrDefault(c => c.CategoryId == categoryId);

                if (checkCategoryIdExisted == null)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.CategoryIdNotBelongToBrand);
                }

                var existedCategoryName = await _unitOfWork.CategoryRepository.GetCategoryByNameAsync(updateCategoryRequest.Name, brandId);
                if (existedCategoryName != null && existedCategoryName.CategoryId != categoryId)
                {
                    throw new BadRequestException(MessageConstant.CategoryMessage.CategoryNameExistedInBrand);
                }
                // get category 
                var category = await this._unitOfWork.CategoryRepository.GetCategoryByIdAsync(categoryId);

                string oldImageUrl = checkCategoryIdExisted.ImageUrl;
                if (updateCategoryRequest.ImageUrl != null)
                {
                    // Upload image to firebase
                    FileStream fileStream = Utils.FileUtil.ConvertFormFileToStream(updateCategoryRequest.ImageUrl);
                    Guid guild = Guid.NewGuid();
                    imageId = guild.ToString();
                    var urlImage = await this._unitOfWork.FirebaseStorageRepository.UploadImageAsync(fileStream, folderName, imageId);
                    if (urlImage != null)
                    {
                        isUploaded = true;
                    }
                    checkCategoryIdExisted.ImageUrl = urlImage + $"&imageUrl={imageId}";

                    //Delete image from database
                    await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(FileUtil.GetImageIdFromUrlImage(oldImageUrl, "imageUrl"), folderName);
                    isDeleted = true;
                }
                checkCategoryIdExisted.Name = updateCategoryRequest.Name;
                checkCategoryIdExisted.Description = updateCategoryRequest.Description;
                checkCategoryIdExisted.DisplayOrder = updateCategoryRequest.DisplayOrder;

                if (!updateCategoryRequest.Status.ToUpper().Equals(CategoryEnum.Status.ACTIVE.ToString()) &&
                    !updateCategoryRequest.Status.ToUpper().Equals(CategoryEnum.Status.INACTIVE.ToString()))
                {
                    throw new BadRequestException(MessageConstant.CategoryMessage.StatusInvalid);
                }
                if (updateCategoryRequest.Status.ToUpper().Equals(CategoryEnum.Status.ACTIVE.ToString()))
                {
                    checkCategoryIdExisted.Status = (int)CategoryEnum.Status.ACTIVE;
                }
                else if (updateCategoryRequest.Status.ToUpper().Equals(CategoryEnum.Status.INACTIVE.ToString()))
                {
                    category.Status = (int)CategoryEnum.Status.INACTIVE;
                    foreach (var extraCategory in category.ExtraCategoryProductCategories)
                    {
                        extraCategory.Status = (int)CategoryEnum.Status.INACTIVE;
                    }
                }
                _unitOfWork.CategoryRepository.UpdateCategory(checkCategoryIdExisted);
                _unitOfWork.Commit();
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";

                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidCategoryId))
                {
                    fieldName = "Category id";
                }
                else if (ex.Message.Equals(MessageConstant.CategoryMessage.CategoryCodeExisted))
                {
                    fieldName = "Category code";
                }
                else if (ex.Message.Equals(MessageConstant.CategoryMessage.CategoryNameExistedInBrand))
                {
                    fieldName = "Category name";
                }
                else if (ex.Message.Equals(MessageConstant.CategoryMessage.DeactiveCategory_Update))
                {
                    fieldName = "Updated category failed";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                if (isUploaded && isDeleted == false)
                {
                    await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(imageId, folderName);
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new ConflictException(error);
            }
        }
        #endregion

        #region Get Category By Id
        public async Task<GetCategoryResponse> GetCategoryByIdAsync(int id, HttpContext httpContext)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetCategoryByIdAsync(id);
                if (category is null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistCategoryId);
                }
                // Get brand from JWT
                JwtSecurityToken jwtSecurityToken = TokenUtil.ReadToken(httpContext);
                string accountId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid).Value;
                string role = jwtSecurityToken.Claims.First(x => x.Type.ToLower().Equals("role")).Value;
                string email = jwtSecurityToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Email).Value;
                if (role.ToLower().Equals(RoleConstant.Brand_Manager.ToLower()))
                {
                    var brandAccount = await _unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId));
                    var brandId = brandAccount.BrandId;
                    var checkCategoryIdExisted = brandAccount.Brand.Categories.SingleOrDefault(c => c.CategoryId == id);
                    if (checkCategoryIdExisted == null)
                    {
                        throw new BadRequestException(MessageConstant.CommonMessage.CategoryIdNotBelongToBrand);
                    }
                }
                else if (role.ToLower().Equals(RoleConstant.Store_Manager.ToLower()))
                {
                    Store existedStore = await this._unitOfWork.StoreRepository.GetStoreAsync(email);
                    if (existedStore.Brand.Categories.Any(x => x.CategoryId == id) == false)
                    {
                        throw new BadRequestException(MessageConstant.CommonMessage.CategoryIdNotBelongToStore);
                    }
                }
                var categoryResponse = new GetCategoryResponse();
                categoryResponse = this._mapper.Map<GetCategoryResponse>(category);
                categoryResponse.ExtraCategories = categoryResponse.ExtraCategories.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToList();
                categoryResponse.ExtraCategories.ForEach(x => x.ExtraProducts = x.ExtraProducts.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToList());
                return categoryResponse;
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistCategoryId))
                {
                    fieldName = "Category id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.CategoryIdNotBelongToBrand) ||
                    ex.Message.Equals(MessageConstant.CommonMessage.CategoryIdNotBelongToStore) ||
                    ex.Message.Equals(MessageConstant.CommonMessage.InvalidCategoryId))
                {
                    fieldName = "Category Id";
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

        #region Deactive Category By Id
        public async Task DeActiveCategoryByIdAsync(int id, HttpContext httpContext)
        {
            try
            {
                if (id <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidCategoryId);
                }
                // Get brand from JWT
                JwtSecurityToken jwtSecurityToken = TokenUtil.ReadToken(httpContext);
                string accountId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid).Value;
                var brandAccount = await _unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId));
                var category = brandAccount.Brand.Categories.SingleOrDefault(c => c.CategoryId == id);
                if (category == null)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.CategoryIdNotBelongToBrand);
                }
                // Deactive category 
                category.Status = (int)CategoryEnum.Status.DISABLE;

                // Deactive category's extra category
                if (category.ExtraCategoryProductCategories.Any())
                {
                    foreach (var extraCategory in category.ExtraCategoryProductCategories)
                    {
                        extraCategory.Status = (int)CategoryEnum.Status.DISABLE;
                    }
                }

                //Deactive category's product
                if (category.Products != null && category.Products.Count() > 0)
                {
                    foreach (var product in category.Products)
                    {
                        product.Status = (int)CategoryEnum.Status.DISABLE;
                    }
                }
                _unitOfWork.CategoryRepository.UpdateCategory(category);
                _unitOfWork.Commit();
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidCategoryId))
                {
                    fieldName = "Category id";
                }
                if (ex.Message.Equals(MessageConstant.CommonMessage.CategoryIdNotBelongToBrand))
                {
                    fieldName = "Category Id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Error", ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region Get Categories
        public async Task<GetCategoriesResponse> GetCategoriesAsync(GetCategoriesRequest getCategoriesRequest, HttpContext httpContext)
        {
            try
            {
                // Get Brand Id from JWT
                JwtSecurityToken jwtSecurityToken = TokenUtil.ReadToken(httpContext);
                string accountId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid).Value;
                string role = jwtSecurityToken.Claims.First(x => x.Type.ToLower().Equals("role")).Value;
                string email = jwtSecurityToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Email).Value;
                int? brandId = null;
                if (role.ToLower().Equals(RoleConstant.Brand_Manager.ToLower()))
                {
                    var brandAccount = await _unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId));
                    brandId = brandAccount.BrandId;
                }
                else if (role.ToLower().Equals(RoleConstant.Store_Manager.ToLower()))
                {
                    Store existedStore = await this._unitOfWork.StoreRepository.GetStoreAsync(email);
                    brandId = existedStore.Brand.BrandId;
                }
                var categories = new List<Category>();
                var categoryResponse = new List<GetCategoryResponse>();

                int numberItems = 0;
                if (getCategoriesRequest.SearchValue != null && StringUtil.IsUnicode(getCategoriesRequest.SearchValue))
                {
                    numberItems = await this._unitOfWork.CategoryRepository.GetNumberCategoriesAsync(getCategoriesRequest.SearchValue, null, getCategoriesRequest.Type, brandId.Value);
                    categories = await this._unitOfWork.CategoryRepository.GetCategoriesAsync(getCategoriesRequest.SearchValue, null, getCategoriesRequest.CurrentPage, getCategoriesRequest.ItemsPerPage,
                                                                                                              getCategoriesRequest.SortBy != null && getCategoriesRequest.SortBy.ToLower().EndsWith("asc") ? getCategoriesRequest.SortBy.Split("_")[0] : null,
                                                                                                              getCategoriesRequest.SortBy != null && getCategoriesRequest.SortBy.ToLower().EndsWith("desc") ? getCategoriesRequest.SortBy.Split("_")[0] : null,
                                                                                                              getCategoriesRequest.Type, brandId.Value, getCategoriesRequest.IsGetAll);
                }
                else if (getCategoriesRequest.SearchValue != null && StringUtil.IsUnicode(getCategoriesRequest.SearchValue) == false)
                {
                    numberItems = await this._unitOfWork.CategoryRepository.GetNumberCategoriesAsync(null, getCategoriesRequest.SearchValue, getCategoriesRequest.Type, brandId.Value);
                    categories = await this._unitOfWork.CategoryRepository.GetCategoriesAsync(null, getCategoriesRequest.SearchValue, getCategoriesRequest.CurrentPage, getCategoriesRequest.ItemsPerPage,
                                                                                                              getCategoriesRequest.SortBy != null && getCategoriesRequest.SortBy.ToLower().EndsWith("asc") ? getCategoriesRequest.SortBy.Split("_")[0] : null,
                                                                                                              getCategoriesRequest.SortBy != null && getCategoriesRequest.SortBy.ToLower().EndsWith("desc") ? getCategoriesRequest.SortBy.Split("_")[0] : null,
                                                                                                              getCategoriesRequest.Type, brandId.Value, getCategoriesRequest.IsGetAll);
                }
                else if (getCategoriesRequest.SearchValue == null)
                {
                    numberItems = await this._unitOfWork.CategoryRepository.GetNumberCategoriesAsync(null, null, getCategoriesRequest.Type, brandId.Value);
                    categories = await this._unitOfWork.CategoryRepository.GetCategoriesAsync(null, null, getCategoriesRequest.CurrentPage, getCategoriesRequest.ItemsPerPage,
                                                                                                              getCategoriesRequest.SortBy != null && getCategoriesRequest.SortBy.ToLower().EndsWith("asc") ? getCategoriesRequest.SortBy.Split("_")[0] : null,
                                                                                                              getCategoriesRequest.SortBy != null && getCategoriesRequest.SortBy.ToLower().EndsWith("desc") ? getCategoriesRequest.SortBy.Split("_")[0] : null,
                                                                                                              getCategoriesRequest.Type, brandId.Value, getCategoriesRequest.IsGetAll);
                }

                _mapper.Map(categories, categoryResponse);

                int totalPages = 0;
                if (numberItems > 0 && getCategoriesRequest.IsGetAll == null || numberItems > 0 && getCategoriesRequest.IsGetAll != null && getCategoriesRequest.IsGetAll == false)
                {
                    totalPages = (int)((numberItems + getCategoriesRequest.ItemsPerPage) / getCategoriesRequest.ItemsPerPage);
                }

                if (numberItems == 0)
                {
                    totalPages = 0;
                }
                return new GetCategoriesResponse()
                {
                    Categories = categoryResponse,
                    TotalItems = numberItems,
                    TotalPages = totalPages
                };
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CategoryMessage.InvalidCategoryType))
                {
                    fieldName = "Type";
                }
                else if (ex.Message.Equals(MessageConstant.CategoryMessage.NotExistCategoryType))
                {
                    fieldName = "Type";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Error", ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region Get Extra Categories From Normal Category
        public async Task<GetCategoriesResponse> GetExtraCategoriesByCategoryId(int categoryId, GetExtraCategoriesRequest getExtraCategoriesRequest, HttpContext httpContext)
        {
            try
            {
                // Get Brand Id from JWT
                JwtSecurityToken jwtSecurityToken = TokenUtil.ReadToken(httpContext);
                string accountId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid).Value;
                var brandAccount = await _unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId));
                var brandId = brandAccount.BrandId;
                var category = brandAccount.Brand.Categories.SingleOrDefault(c => c.CategoryId == categoryId);
                if (category == null)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.CategoryIdNotBelongToBrand);
                }
                if (category.Type.Equals(CategoryEnum.Type.EXTRA.ToString()))
                {
                    throw new BadRequestException(MessageConstant.CategoryMessage.CategoryMustBeNormal);
                }
                var categoryResponse = new List<GetCategoryResponse>();
                var listExtraCategoriesInNormalCategory = new List<Category>();

                // Get list extra category id
                var listExtraCategoryId = await this._unitOfWork.ExtraCategoryRepository.GetExtraCategoriesByCategoryIdAsync(categoryId);
                if (!listExtraCategoryId.Any())
                {
                    return new GetCategoriesResponse()
                    {
                        Categories = categoryResponse,
                        TotalItems = 0,
                        TotalPages = 0
                    };
                }
                foreach (var extraId in listExtraCategoryId)
                {
                    var extraCategoryInNormalCategory = await this._unitOfWork.CategoryRepository.GetCategoryByIdAsync(extraId.ExtraCategoryId);
                    listExtraCategoriesInNormalCategory.Add(extraCategoryInNormalCategory);
                }

                int numberItems = 0;
                if (getExtraCategoriesRequest.SearchValue != null && StringUtil.IsUnicode(getExtraCategoriesRequest.SearchValue))
                {
                    numberItems = this._unitOfWork.CategoryRepository.GetNumberExtraCategories(listExtraCategoriesInNormalCategory, getExtraCategoriesRequest.SearchValue, null, brandId);
                    listExtraCategoriesInNormalCategory = this._unitOfWork.CategoryRepository.SearchAndPagingExtraCategory(listExtraCategoriesInNormalCategory, getExtraCategoriesRequest.SearchValue, null, getExtraCategoriesRequest.CurrentPage, getExtraCategoriesRequest.ItemsPerPage,
                                                                                                              getExtraCategoriesRequest.SortBy != null && getExtraCategoriesRequest.SortBy.ToLower().EndsWith("asc") ? getExtraCategoriesRequest.SortBy.Split("_")[0] : null,
                                                                                                              getExtraCategoriesRequest.SortBy != null && getExtraCategoriesRequest.SortBy.ToLower().EndsWith("desc") ? getExtraCategoriesRequest.SortBy.Split("_")[0] : null, brandId, getExtraCategoriesRequest.isGetAll);
                }
                else if (getExtraCategoriesRequest.SearchValue != null && StringUtil.IsUnicode(getExtraCategoriesRequest.SearchValue) == false)
                {
                    numberItems = this._unitOfWork.CategoryRepository.GetNumberExtraCategories(listExtraCategoriesInNormalCategory, null, getExtraCategoriesRequest.SearchValue, brandId);
                    listExtraCategoriesInNormalCategory = this._unitOfWork.CategoryRepository.SearchAndPagingExtraCategory(listExtraCategoriesInNormalCategory, null, getExtraCategoriesRequest.SearchValue, getExtraCategoriesRequest.CurrentPage, getExtraCategoriesRequest.ItemsPerPage,
                                                                                                              getExtraCategoriesRequest.SortBy != null && getExtraCategoriesRequest.SortBy.ToLower().EndsWith("asc") ? getExtraCategoriesRequest.SortBy.Split("_")[0] : null,
                                                                                                              getExtraCategoriesRequest.SortBy != null && getExtraCategoriesRequest.SortBy.ToLower().EndsWith("desc") ? getExtraCategoriesRequest.SortBy.Split("_")[0] : null, brandId, getExtraCategoriesRequest.isGetAll);
                }
                else if (getExtraCategoriesRequest.SearchValue == null)
                {
                    numberItems = this._unitOfWork.CategoryRepository.GetNumberExtraCategories(listExtraCategoriesInNormalCategory, null, null, brandId);
                    listExtraCategoriesInNormalCategory = this._unitOfWork.CategoryRepository.SearchAndPagingExtraCategory(listExtraCategoriesInNormalCategory, null, null, getExtraCategoriesRequest.CurrentPage, getExtraCategoriesRequest.ItemsPerPage,
                                                                                                              getExtraCategoriesRequest.SortBy != null && getExtraCategoriesRequest.SortBy.ToLower().EndsWith("asc") ? getExtraCategoriesRequest.SortBy.Split("_")[0] : null,
                                                                                                              getExtraCategoriesRequest.SortBy != null && getExtraCategoriesRequest.SortBy.ToLower().EndsWith("desc") ? getExtraCategoriesRequest.SortBy.Split("_")[0] : null, brandId, getExtraCategoriesRequest.isGetAll);
                }


                _mapper.Map(listExtraCategoriesInNormalCategory, categoryResponse);
                int totalPages = 0;
                if (numberItems > 0 && getExtraCategoriesRequest.isGetAll == null || numberItems > 0 && getExtraCategoriesRequest.isGetAll != null && getExtraCategoriesRequest.isGetAll == false)
                {
                    totalPages = (int)((numberItems + getExtraCategoriesRequest.ItemsPerPage) / getExtraCategoriesRequest.ItemsPerPage);
                }
                if (numberItems == 0)
                {
                    totalPages = 0;
                }
                return new GetCategoriesResponse()
                {
                    Categories = categoryResponse,
                    TotalItems = numberItems,
                    TotalPages = totalPages
                };
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.CategoryIdNotBelongToBrand))
                {
                    fieldName = "Category id";
                }
                else if (ex.Message.Equals(MessageConstant.CategoryMessage.CategoryMustBeNormal))
                {
                    fieldName = "Category id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Error", ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region Add Extra Categories To Normal Category
        public async Task AddExtraCategoriesToNormalCategoryAsync(int categoryId, ExtraCategoryRequest extraCategoryRequest, HttpContext httpContext)
        {
            try
            {
                var existedCategory = await _unitOfWork.CategoryRepository.GetCategoryByIdAsync(categoryId);
                if (existedCategory is null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistCategoryId);
                }
                // Get brandId from JWT
                JwtSecurityToken jwtSecurityToken = TokenUtil.ReadToken(httpContext);
                string accountId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid).Value;
                var brandAccount = await _unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId));
                var brandId = brandAccount.BrandId;
                var category = brandAccount.Brand.Categories.SingleOrDefault(c => c.CategoryId == categoryId);
                if (category == null)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.CategoryIdNotBelongToBrand);
                }
                else if (category.Type.Equals(CategoryEnum.Type.EXTRA.ToString()))
                {
                    throw new BadRequestException(MessageConstant.CategoryMessage.CategoryMustBeNormal);
                }
                if (extraCategoryRequest.ExtraCategoryIds.Any(item => item <= 0))
                {
                    throw new BadRequestException(MessageConstant.CategoryMessage.ExtraCategoryGreaterThan0);
                }
                foreach (var id in extraCategoryRequest.ExtraCategoryIds)
                {
                    var extraCategory = await this._unitOfWork.CategoryRepository.GetCategoryByIdAsync(id);
                    if (extraCategory != null)
                    {
                        if (extraCategory.Type.Equals(CategoryEnum.Type.NORMAL.ToString()))
                        {
                            throw new BadRequestException(MessageConstant.CategoryMessage.ListExtraCategoryIdIsExtraType);
                        }
                        else if (extraCategory.Status == (int)CategoryEnum.Status.INACTIVE)
                        {
                            throw new BadRequestException(MessageConstant.CategoryMessage.ListExtraCategoryIdIsActive);
                        }
                        else if (extraCategory.Brand.BrandId != brandId)
                        {
                            throw new BadRequestException(MessageConstant.CategoryMessage.ExtraCategoryIdNotBelongToBrand);
                        }
                    }
                    else
                    {
                        throw new NotFoundException(MessageConstant.CategoryMessage.ExtraCategoryIdDoesNotExist);
                    }
                }
                SplitIdCategoryResponse splittedExtraCategoriesIds = CustomListUtil
                                                                                   .splitIdsToAddAndRemove(category.ExtraCategoryProductCategories
                                                                                   .Select(e => e.ExtraCategoryId)
                                                                                   .ToList(), extraCategoryRequest.ExtraCategoryIds);
                //Handle add and remove to database
                if (splittedExtraCategoriesIds.idsToAdd.Count > 0)
                {
                    // Add new extra category to normal category
                    List<ExtraCategory> extraCategoriesToInsert = new List<ExtraCategory>();
                    splittedExtraCategoriesIds.idsToAdd.ForEach(id => extraCategoriesToInsert.Add(new ExtraCategory
                    {
                        ProductCategoryId = categoryId,
                        ExtraCategoryId = id,
                        Status = (int)CategoryEnum.Status.ACTIVE
                    }));
                    await this._unitOfWork.ExtraCategoryRepository.InsertRangeAsync(extraCategoriesToInsert);
                }

                if (splittedExtraCategoriesIds.idsToRemove.Count > 0)
                {
                    // Delete extra category from normal category
                    var listExtraCategoriesToDelete = new List<ExtraCategory>();
                    foreach (var extra in category.ExtraCategoryProductCategories)
                    {
                        foreach (var id in splittedExtraCategoriesIds.idsToRemove)
                        {
                            if (id == extra.ExtraCategoryId)
                            {
                                listExtraCategoriesToDelete.Add(extra);
                            }
                        }
                    }
                    _unitOfWork.ExtraCategoryRepository.DeleteRange(listExtraCategoriesToDelete);
                }
                await this._unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidCategoryId))
                {
                    fieldName = "Category id";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.CategoryIdNotBelongToBrand))
                {
                    fieldName = "Category id";
                }

                else if (ex.Message.Equals(MessageConstant.CategoryMessage.CategoryMustBeNormal))
                {
                    fieldName = "Category id";
                }
                else if (ex.Message.Equals(MessageConstant.CategoryMessage.ExtraCategoryGreaterThan0))
                {
                    fieldName = "List Extra Category id";
                }
                else if (ex.Message.Equals(MessageConstant.CategoryMessage.ListExtraCategoryIdIsExtraType))
                {
                    fieldName = "List Extra Category id";
                }
                else if (ex.Message.Equals(MessageConstant.CategoryMessage.ListExtraCategoryIdIsActive))
                {
                    fieldName = "List Extra Category id";
                }
                else if (ex.Message.Equals(MessageConstant.CategoryMessage.ExtraCategoryIdNotBelongToBrand))
                {
                    fieldName = "List Extra Category id";
                }

                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CategoryMessage.ExtraCategoryIdDoesNotExist))
                {
                    fieldName = "List Extra Category id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
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
