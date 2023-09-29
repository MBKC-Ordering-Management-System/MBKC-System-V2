using AutoMapper;
using MBKC.BAL.DTOs.Categories;
using MBKC.BAL.DTOs.FireBase;
using MBKC.BAL.DTOs.Products;
using MBKC.BAL.DTOs.SplitIdCategories;
using MBKC.BAL.Exceptions;
using MBKC.BAL.Services.Interfaces;
using MBKC.BAL.Utils;
using MBKC.DAL.Enums;
using MBKC.DAL.Infrastructures;
using MBKC.DAL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Services.Implementations
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
        public async Task CreateCategoryAsync(PostCategoryRequest postCategoryRequest, FireBaseImage fireBaseImage, HttpContext httpContext)
        {
            string imageId = "";
            bool uploaded = false;
            try
            {
                // Get brand from JWT
                JwtSecurityToken jwtSecurityToken = TokenHelper.ReadToken(httpContext);
                string accountId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid).Value;
                var brandAccount = await _unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId));
                var brand = brandAccount.Brand;

                var existedCategoryCode = await _unitOfWork.CategoryRepository.GetCategoryByCodeAsync(postCategoryRequest.Code);
                if (existedCategoryCode != null)
                {
                    throw new BadRequestException("Category code already exist in the system.");
                }
                // Upload image to firebase
                FileStream fileStream = Utils.FileUtil.ConvertFormFileToStream(postCategoryRequest.ImageUrl);
                FileUtil.SetCredentials(fireBaseImage);
                Guid guild = Guid.NewGuid();
                imageId = guild.ToString();
                var urlImage = await Utils.FileUtil.UploadImageAsync(fileStream, "Categories", imageId);
                if (urlImage != null)
                {
                    uploaded = true;
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
                if (ex.Message.Equals("Category code already exist in the system."))
                {
                    fieldName = "Category Code";
                }
                if (ex.Message.Equals("Upload image to firebase failed."))
                {
                    fieldName = "Upload file";
                }
                else if (ex.Message.Equals("Delete image failed."))
                {
                    fieldName = "Delete file";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                if (uploaded)
                {
                    await FileUtil.DeleteImageAsync(imageId, "Categories");
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region Update Category
        public async Task UpdateCategoryAsync(int categoryId, UpdateCategoryRequest updateCategoryRequest, FireBaseImage fireBaseImage, HttpContext httpContext)
        {
            string imageId = "";
            bool isUploaded = false;
            bool isDeleted = false;
            try
            {
                if (categoryId <= 0)
                {
                    throw new BadRequestException("Category Id is not suitable for the system.");
                }

                // Get brandId from JWT
                JwtSecurityToken jwtSecurityToken = TokenHelper.ReadToken(httpContext);
                string accountId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid).Value;
                var brandAccount = await _unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId));
                var brandId = brandAccount.BrandId;
                // Check category belong to brand or not.
                var checkCategoryIdExisted = brandAccount.Brand.Categories.SingleOrDefault(c => c.CategoryId == categoryId);

                if (checkCategoryIdExisted == null)
                {
                    throw new BadRequestException("This Category Id is not part of the brand's Category Id.");
                }
                // get category 
                var category = await this._unitOfWork.CategoryRepository.GetCategoryByIdAsync(categoryId);
                var categoryCode = await this._unitOfWork.CategoryRepository.GetCategoryByCodeAsync(updateCategoryRequest.Code);
                if (categoryCode != null && !category.Code.ToLower().Equals(updateCategoryRequest.Code.ToLower()))
                {
                    throw new BadRequestException("Category code already exist in the system.");
                }

                string oldImageUrl = category.ImageUrl;
                if (updateCategoryRequest.ImageUrl != null)
                {
                    // Upload image to firebase
                    FileStream fileStream = Utils.FileUtil.ConvertFormFileToStream(updateCategoryRequest.ImageUrl);
                    FileUtil.SetCredentials(fireBaseImage);
                    Guid guild = Guid.NewGuid();
                    imageId = guild.ToString();
                    var urlImage = await Utils.FileUtil.UploadImageAsync(fileStream, "Categories", imageId);
                    if (urlImage != null)
                    {
                        isUploaded = true;
                    }
                    category.ImageUrl = urlImage + $"&imageUrl={imageId}";

                    //Delete image from database
                    FileUtil.SetCredentials(fireBaseImage);
                    await FileUtil.DeleteImageAsync(FileUtil.GetImageIdFromUrlImage(oldImageUrl, "imageUrl"), "Categories");
                    isDeleted = true;
                }
                category.Name = updateCategoryRequest.Name;
                category.Description = updateCategoryRequest.Description;
                category.DisplayOrder = updateCategoryRequest.DisplayOrder;
                category.Code = updateCategoryRequest.Code;

                if (!updateCategoryRequest.Status.ToUpper().Equals(CategoryEnum.Status.ACTIVE.ToString()) &&
                    !updateCategoryRequest.Status.ToUpper().Equals(CategoryEnum.Status.INACTIVE.ToString()))
                {
                    throw new BadRequestException("Status is ACTIVE or INACTIVE.");
                }
                if (updateCategoryRequest.Status.ToLower().Equals(CategoryEnum.Status.ACTIVE.ToString().ToLower()))
                {
                    category.Status = (int)CategoryEnum.Status.ACTIVE;
                }
                else if (updateCategoryRequest.Status.ToLower().Equals(CategoryEnum.Status.INACTIVE.ToString().ToLower()))
                {
                    category.Status = (int)CategoryEnum.Status.INACTIVE;
                }
                _unitOfWork.CategoryRepository.UpdateCategory(category);
                _unitOfWork.Commit();
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";

                if (ex.Message.Equals("Category Id is not suitable for the system."))
                {
                    fieldName = "Category Id";
                }
                if (ex.Message.Equals("This Category Id is not part of the brand's Category Id."))
                {
                    fieldName = "Category Id";
                }
                else if (ex.Message.Equals("Category code already exist in the system."))
                {
                    fieldName = "Category Code";
                }
                else if (ex.Message.Equals("Status is ACTIVE or INACTIVE."))
                {
                    fieldName = "Status";
                }
                else if (ex.Message.Equals("Upload image to firebase failed."))
                {
                    fieldName = "Upload file";
                }
                else if (ex.Message.Equals("Delete image failed."))
                {
                    fieldName = "Delete file";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                if (isUploaded && isDeleted == false)
                {
                    await FileUtil.DeleteImageAsync(imageId, "Categories");
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
                if (id <= 0)
                {
                    throw new BadRequestException("Category Id is not suitable for the system.");
                }
                // Get brand from JWT
                JwtSecurityToken jwtSecurityToken = TokenHelper.ReadToken(httpContext);
                string accountId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid).Value;
                var brandAccount = await _unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId));
                var brandId = brandAccount.BrandId;
                var checkCategoryIdExisted = brandAccount.Brand.Categories.SingleOrDefault(c => c.CategoryId == id);
                if (checkCategoryIdExisted == null)
                {
                    throw new BadRequestException("This Category Id is not part of the brand's Category Id.");
                }
                var category = await _unitOfWork.CategoryRepository.GetCategoryByIdAsync(id);
                var categoryResponse = new GetCategoryResponse();
                _mapper.Map(category, categoryResponse);
                return categoryResponse;
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Category Id is not suitable for the system."))
                {
                    fieldName = "Category Id";
                }
                if (ex.Message.Equals("This Category Id is not part of the brand's Category Id."))
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
                    throw new BadRequestException("Category Id is not suitable for the system.");
                }
                // Get brand from JWT
                JwtSecurityToken jwtSecurityToken = TokenHelper.ReadToken(httpContext);
                string accountId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid).Value;
                var brandAccount = await _unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId));
                var category = brandAccount.Brand.Categories.SingleOrDefault(c => c.CategoryId == id);
                if (category == null)
                {
                    throw new BadRequestException("This Category Id is not part of the brand's Category Id.");
                }
                // Deactive category 
                category.Status = (int)CategoryEnum.Status.DEACTIVE;

                // Deactive category's extra category
                if (category.ExtraCategoryProductCategories.Any())
                {
                    foreach (var extraCategory in category.ExtraCategoryProductCategories)
                    {
                        extraCategory.Status = (int)CategoryEnum.Status.DEACTIVE;
                    }
                }

                //Deactive category's product
                if (category.Products != null)
                {
                    foreach (var product in category.Products)
                    {
                        product.Status = (int)CategoryEnum.Status.DEACTIVE;
                    }
                }
                _unitOfWork.CategoryRepository.UpdateCategory(category);
                _unitOfWork.Commit();
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Category Id is not suitable for the system."))
                {
                    fieldName = "Category Id";
                }
                else if (ex.Message.Equals("This Category Id is not part of the brand's Category Id."))
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
        public async Task<GetCategoriesResponse> GetCategoriesAsync(string type, string? keySearchName, int? pageNumber, int? pageSize, HttpContext httpContext)
        {
            try
            {
                // Get Brand Id from JWT
                JwtSecurityToken jwtSecurityToken = TokenHelper.ReadToken(httpContext);
                string accountId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid).Value;
                var brandAccount = await _unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId));
                var brandId = brandAccount.BrandId;

                var categories = new List<Category>();
                var categoryResponse = new List<GetCategoryResponse>();
                if (string.IsNullOrEmpty(type))
                {
                    throw new BadRequestException("Type is not suitable for the system.");
                }
                if (!type.ToLower().Equals(CategoryEnum.Type.EXTRA.ToString().ToLower())
                    && !type.ToLower().Equals(CategoryEnum.Type.NORMAL.ToString().ToLower()))
                {
                    throw new BadRequestException("Type are EXTRA or NORMAL.");
                }
                if (pageNumber != null && pageNumber <= 0)
                {
                    throw new BadRequestException("Page number is required greater than 0.");
                }
                else if (pageNumber == null)
                {
                    pageNumber = 1;
                }
                if (pageSize != null && pageSize <= 0)
                {
                    throw new BadRequestException("Page size is required greater than 0.");
                }
                else if (pageSize == null)
                {
                    pageSize = 5;
                }

                int numberItems = 0;
                if (keySearchName != null && StringUtil.IsUnicode(keySearchName))
                {
                    numberItems = await this._unitOfWork.CategoryRepository.GetNumberCategoriesAsync(keySearchName, null, type, brandId);
                    categories = await this._unitOfWork.CategoryRepository.GetCategoriesAsync(keySearchName, null, type, pageSize.Value, pageNumber.Value, brandId);
                }
                else if (keySearchName != null && StringUtil.IsUnicode(keySearchName) == false)
                {
                    numberItems = await this._unitOfWork.CategoryRepository.GetNumberCategoriesAsync(null, keySearchName, type, brandId);
                    categories = await this._unitOfWork.CategoryRepository.GetCategoriesAsync(null, keySearchName, type, pageSize.Value, pageNumber.Value, brandId);
                }
                else if (keySearchName == null)
                {
                    numberItems = await this._unitOfWork.CategoryRepository.GetNumberCategoriesAsync(null, null, type, brandId);
                    categories = await this._unitOfWork.CategoryRepository.GetCategoriesAsync(null, null, type, pageSize.Value, pageNumber.Value, brandId);
                }
                _mapper.Map(categories, categoryResponse);

                int totalPages = (int)((numberItems + pageSize) / pageSize);
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
                if (ex.Message.Equals("Type is not suitable for the system."))
                {
                    fieldName = "Type";
                }
                else if (ex.Message.Equals("Type are EXTRA or NORMAL."))
                {
                    fieldName = "Type";
                }
                else if (ex.Message.Equals("Page number is required greater than 0."))
                {
                    fieldName = "Page Number";
                }
                else if (ex.Message.Equals("Page size is required greater than 0."))
                {
                    fieldName = "Page Size";
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

        #region Get Products In Category
        public async Task<GetProductsResponse> GetProductsInCategory(int categoryId, string? keySearchName, int? pageNumber, int? pageSize, HttpContext httpContext)
        {
            try
            {
                if (categoryId <= 0)
                {
                    throw new BadRequestException("Category Id is not suitable for the system.");
                }
                JwtSecurityToken jwtSecurityToken = TokenHelper.ReadToken(httpContext);
                string accountId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid).Value;
                var brandAccount = await _unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId));
                var brandId = brandAccount.BrandId;
                var category = brandAccount.Brand.Categories.SingleOrDefault(c => c.CategoryId == categoryId);
                if (category == null || category.Status == (int)CategoryEnum.Status.INACTIVE)
                {
                    throw new BadRequestException("This Category Id is not part of the brand's Category Id.");
                }
                var products = new List<Product>();
                var productResponse = new List<GetProductResponse>();
                if (pageNumber != null && pageNumber <= 0)
                {
                    throw new BadRequestException("Page number is required greater than 0.");
                }
                else if (pageNumber == null)
                {
                    pageNumber = 1;
                }
                if (pageSize != null && pageSize <= 0)
                {
                    throw new BadRequestException("Page size is required greater than 0.");
                }
                else if (pageSize == null)
                {
                    pageSize = 5;
                }

                int numberItems = 0;
                if (keySearchName != null && StringUtil.IsUnicode(keySearchName))
                {
                    numberItems = await this._unitOfWork.ProductRepository.GetNumberProductsAsync(keySearchName, null, brandId, categoryId);
                    products = await this._unitOfWork.ProductRepository.GetProductsByCategoryIdAsync(categoryId, brandId, keySearchName, null, pageSize.Value, pageNumber.Value);
                }
                else if (keySearchName != null && StringUtil.IsUnicode(keySearchName) == false)
                {
                    numberItems = await this._unitOfWork.ProductRepository.GetNumberProductsAsync(null, keySearchName, brandId, categoryId);
                    products = await this._unitOfWork.ProductRepository.GetProductsByCategoryIdAsync(categoryId, brandId, null, keySearchName, pageSize.Value, pageNumber.Value);
                }
                else if (keySearchName == null)
                {
                    numberItems = await this._unitOfWork.ProductRepository.GetNumberProductsAsync(null, null, brandId, categoryId);
                    products = await this._unitOfWork.ProductRepository.GetProductsByCategoryIdAsync(categoryId, brandId, null, null, pageSize.Value, pageNumber.Value);
                }
                _mapper.Map(products, productResponse);
                int totalPages = (int)((numberItems + pageSize) / pageSize);
                if (numberItems == 0)
                {
                    totalPages = 0;
                }
                return new GetProductsResponse()
                {
                    Products = productResponse,
                    TotalItems = numberItems,
                    TotalPages = totalPages,
                };
            }
            catch (BadRequestException ex)
            {

                string fieldName = "";
                if (ex.Message.Equals("Category Id is not suitable for the system."))
                {
                    fieldName = "Category Id";
                }
                if (ex.Message.Equals("This Category Id is not part of the brand's Category Id."))
                {
                    fieldName = "Category Id";
                }
                else if (ex.Message.Equals("Page number is required greater than 0."))
                {
                    fieldName = "Page Number";
                }
                else if (ex.Message.Equals("Page size is required greater than 0."))
                {
                    fieldName = "Page Size";
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
        public async Task<GetCategoriesResponse> GetExtraCategoriesByCategoryId(int categoryId, string? keySearchName, int? pageNumber, int? pageSize, HttpContext httpContext)
        {
            try
            {
                if (categoryId <= 0)
                {
                    throw new BadRequestException("Category Id is not suitable for the system.");
                }

                // Get Brand Id from JWT
                JwtSecurityToken jwtSecurityToken = TokenHelper.ReadToken(httpContext);
                string accountId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid).Value;
                var brandAccount = await _unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId));
                var brandId = brandAccount.BrandId;
                var category = brandAccount.Brand.Categories.SingleOrDefault(c => c.CategoryId == categoryId);
                if (category == null || category.Status == (int)CategoryEnum.Status.INACTIVE)
                {
                    throw new BadRequestException("This Category Id is not part of the brand's Category Id.");
                }
                if (category.Type.Equals(CategoryEnum.Type.EXTRA.ToString()))
                {
                    throw new BadRequestException("Category type must be NORMAL.");
                }
                var categoryResponse = new List<GetCategoryResponse>();
                var listExtraCategoriesInNormalCategory = new List<Category>();

                if (pageNumber != null && pageNumber <= 0)
                {
                    throw new BadRequestException("Page number is required greater than 0.");
                }
                else if (pageNumber == null)
                {
                    pageNumber = 1;
                }
                if (pageSize != null && pageSize <= 0)
                {
                    throw new BadRequestException("Page size is required greater than 0.");
                }
                else if (pageSize == null)
                {
                    pageSize = 5;
                }

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
                    var extraCategoryInNormalCategory = await this._unitOfWork.CategoryRepository.GetCategoryByIdAsync(extraId);
                    listExtraCategoriesInNormalCategory.Add(extraCategoryInNormalCategory);
                }
               
                int numberItems = 0;
                if (keySearchName != null && StringUtil.IsUnicode(keySearchName))
                {
                    numberItems = this._unitOfWork.CategoryRepository.GetNumberExtraCategories(listExtraCategoriesInNormalCategory, keySearchName, null, brandId);
                    listExtraCategoriesInNormalCategory = this._unitOfWork.CategoryRepository.SearchAndPagingExtraCategory(listExtraCategoriesInNormalCategory, keySearchName, null, pageSize.Value, pageNumber.Value, brandId);
                }
                else if (keySearchName != null && StringUtil.IsUnicode(keySearchName) == false)
                {
                    numberItems = this._unitOfWork.CategoryRepository.GetNumberExtraCategories(listExtraCategoriesInNormalCategory, null, keySearchName, brandId);
                    listExtraCategoriesInNormalCategory = this._unitOfWork.CategoryRepository.SearchAndPagingExtraCategory(listExtraCategoriesInNormalCategory, null, keySearchName, pageSize.Value, pageNumber.Value, brandId);
                }
                else if (keySearchName == null)
                {
                    numberItems = this._unitOfWork.CategoryRepository.GetNumberExtraCategories(listExtraCategoriesInNormalCategory, null, null, brandId);
                    listExtraCategoriesInNormalCategory = this._unitOfWork.CategoryRepository.SearchAndPagingExtraCategory(listExtraCategoriesInNormalCategory, null, null, pageSize.Value, pageNumber.Value, brandId);
                }

                _mapper.Map(listExtraCategoriesInNormalCategory, categoryResponse);

                int totalPages = (int)((numberItems + pageSize) / pageSize);
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
                if (ex.Message.Equals("Category Id is not suitable for the system."))
                {
                    fieldName = "Category Id";
                }
                else if (ex.Message.Equals("This Category Id is not part of the brand's Category Id."))
                {
                    fieldName = "Category Id";
                }
                else if (ex.Message.Equals("Category type must be NORMAL."))
                {
                    fieldName = "Category Id";
                }
                else if (ex.Message.Equals("Page number is required greater than 0."))
                {
                    fieldName = "Page Number";
                }
                else if (ex.Message.Equals("Page size is required greater than 0."))
                {
                    fieldName = "Page Size";
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
        public async Task AddExtraCategoriesToNormalCategory(int categoryId, List<int> listExtraCategoryId, HttpContext httpContext)
        {
            try
            {
                if (categoryId < 0)
                {
                    throw new BadRequestException("Category Id is not suitable for the system.");
                }
                // Get brandId from JWT
                JwtSecurityToken jwtSecurityToken = TokenHelper.ReadToken(httpContext);
                string accountId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid).Value;
                var brandAccount = await _unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId));
                var brandId = brandAccount.BrandId;
                var category = brandAccount.Brand.Categories.SingleOrDefault(c => c.CategoryId == categoryId);
                if (category == null)
                {
                    throw new BadRequestException("This Category Id is not part of the brand's Category Id.");
                }
                else if (category.Type.Equals(CategoryEnum.Type.EXTRA.ToString()))
                {
                    throw new BadRequestException("CategoryId must be a NORMAL type.");
                }
                if (listExtraCategoryId.Any(item => item <= 0))
                {
                    throw new BadRequestException("Extra category Id must be greater than 0.");
                }
                foreach (var id in listExtraCategoryId)
                {
                    var extraCategory = await this._unitOfWork.CategoryRepository.GetCategoryByIdAsync(id);
                    if (extraCategory != null)
                    {
                        if (extraCategory.Type.Equals(CategoryEnum.Type.NORMAL.ToString()))
                        {
                            throw new BadRequestException("List extra category Id need to be a EXTRA type.");
                        }
                        else if (extraCategory.Status == (int)CategoryEnum.Status.INACTIVE)
                        {
                            throw new BadRequestException("List extra category Id need status is ACTIVE.");
                        }
                        else if (extraCategory.Brand.BrandId != brandId)
                        {
                            throw new BadRequestException("Extra category Id does not belong to brand.");
                        }
                    }
                    else
                    {
                        throw new BadRequestException("Extra category Id does not belong to brand.");
                    }
                }
                SplitIdCategoryResponse splittedExtraCategoriesIds = CustomListUtil
                                                                                   .splitIdsToAddAndRemove(category.ExtraCategoryProductCategories
                                                                                   .Select(e => e.ExtraCategoryId)
                                                                                   .ToList(), listExtraCategoryId);
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
                this._unitOfWork.Commit();
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Category Id is not suitable for the system."))
                {
                    fieldName = "Category Id";
                }
                else if (ex.Message.Equals("This Category Id is not part of the brand's Category Id."))
                {
                    fieldName = "Category Id";
                }
                else if (ex.Message.Equals("CategoryId must be a NORMAL type."))
                {
                    fieldName = "Category Id";
                }
                else if (ex.Message.Equals("Extra category id must be greater than 0."))
                {
                    fieldName = "List Extra Category Id";
                }
                else if (ex.Message.Equals("List extra category Id need to be a EXTRA type."))
                {
                    fieldName = "List Extra Category Id";
                }
                else if (ex.Message.Equals("List extra category Id need status is ACTIVE."))
                {
                    fieldName = "List Extra Category Id";
                }
                else if (ex.Message.Equals("Extra category Id does not belong to brand."))
                {
                    fieldName = "List Extra Category Id";
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
    }
}
