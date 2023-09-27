using AutoMapper;
using MBKC.BAL.DTOs.Categories;
using MBKC.BAL.DTOs.FireBase;
using MBKC.BAL.DTOs.Products;
using MBKC.BAL.DTOs.SplitIdCategories;
using MBKC.BAL.Exceptions;
using MBKC.BAL.Utils;
using MBKC.DAL.Enums;
using MBKC.BAL.Services.Interfaces;
using MBKC.DAL.Infrastructures;
using MBKC.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MBKC.BAL.Services.Implementations
{
<<<<<<<< HEAD:MBKC_System/MBKC.BAL/Services/Implementations/CashierService.cs
    public class CashierService : ICashierService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public CashierService(IUnitOfWork unitOfWork, IMapper mapper)
========
    public class CategoryService : ICategoryService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
>>>>>>>> main:MBKC_System/MBKC.BAL/Services/Implementations/CategoryService.cs
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        #region Create Category
        public async Task CreateCategoryAsync(PostCategoryRequest postCategoryRequest, FireBaseImage fireBaseImage)
        {
            string imageId = "";
            bool uploaded = false;
            try
            {
                var existedCategory = await _unitOfWork.CategoryRepository.GetCategoryByCodeAsync(postCategoryRequest.Code);
                var brand = await _unitOfWork.BrandRepository.GetBrandByIdAsync(postCategoryRequest.BrandId);
                if (brand == null)
                {
                    throw new NotFoundException("Brand Id does not exist in the system.");
                }

                if (existedCategory != null)
                {
                    throw new BadRequestException("Category code already exist in the system.");
                }
                // Upload image to firebase
                FileStream fileStream = Utils.FileUtil.ConvertFormFileToStream(postCategoryRequest.ImageUrl);
                FileUtil.SetCredentials(fireBaseImage);
                Guid guild = Guid.NewGuid();
                imageId = guild.ToString();
                var urlImage = await Utils.FileUtil.UploadImage(fileStream, "Categories", imageId);
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
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Brand Id does not exist in the system."))
                {
                    fieldName = "Brand Id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
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
        public async Task UpdateCategoryAsync(int categoryId, UpdateCategoryRequest updateCategoryRequest, FireBaseImage fireBaseImage)
        {
            string imageId = "";
            bool uploaded = false;
            try
            {
                if (categoryId <= 0)
                {
                    throw new BadRequestException("Category Id is not suitable for the system.");
                }
                var category = await this._unitOfWork.CategoryRepository.GetCategoryByIdAsync(categoryId);
                var categoryCode = await this._unitOfWork.CategoryRepository.GetCategoryByCodeAsync(updateCategoryRequest.Code);
                if (categoryCode != null && !category.Code.ToLower().Equals(updateCategoryRequest.Code.ToLower()))
                {
                    throw new BadRequestException("Category code already exist in the system.");
                }
                if (category == null)
                {
                    throw new NotFoundException("Category Id does not exist in the system");
                }

                if (updateCategoryRequest.ImageUrl != null)
                {
                    //Delete image from database
                    FileUtil.SetCredentials(fireBaseImage);
                    Uri uri = new Uri(category.ImageUrl);
                    imageId = HttpUtility.ParseQueryString(uri.Query).Get("imageUrl");
                    await FileUtil.DeleteImageAsync(imageId, "Categories");

                    // Upload image to firebase
                    FileStream fileStream = Utils.FileUtil.ConvertFormFileToStream(updateCategoryRequest.ImageUrl);
                    Guid guild = Guid.NewGuid();
                    imageId = guild.ToString();
                    var urlImage = await Utils.FileUtil.UploadImage(fileStream, "Categories", imageId);
                    if (urlImage != null)
                    {
                        uploaded = true;
                    }
                    category.ImageUrl = urlImage + $"&imageUrl={imageId}";
                }
                category.Name = updateCategoryRequest.Name;
                category.Description = updateCategoryRequest.Description;
                category.DisplayOrder = updateCategoryRequest.DisplayOrder;
                category.Code = updateCategoryRequest.Code;
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
                if (ex.Message.Equals("Display order already exist in the system"))
                {
                    fieldName = "Display Order";
                }
                else if (ex.Message.Equals("Category Id is not suitable for the system."))
                {
                    fieldName = "Category Id";
                }
                else if (ex.Message.Equals("Category code already exist in the system."))
                {
                    fieldName = "Category Code";
                }
                else if (ex.Message.Equals("Upload image to firebase failed."))
                {
                    fieldName = "Upload file";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Category Id does not exist in the system"))
                {
                    fieldName = "Category Id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }

            catch (Exception ex)

            {
                if (uploaded)
                {
                    await FileUtil.DeleteImageAsync(imageId, "Categories");
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new ConflictException(error);
            }
        }
        #endregion

        #region Get Category By Id
        public async Task<GetCategoryResponse> GetCategoryByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new BadRequestException("Category ID must be a non-negative number.");
                }
                var categoryResponse = new GetCategoryResponse();
                var category = await this._unitOfWork.CategoryRepository.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    throw new NotFoundException("Category Id does not exist in the system");
                }
                _mapper.Map(category, categoryResponse);

                if (categoryResponse.Status.Equals(((int)ExtraCategoryEnum.Status.ACTIVE).ToString()))
                {
                    categoryResponse.Status = ExtraCategoryEnum.Status.ACTIVE.ToString().ToUpper()[0] + ExtraCategoryEnum.Status.ACTIVE.ToString().ToLower().Substring(1);
                }
                else if (category.Status.Equals(((int)ExtraCategoryEnum.Status.INACTIVE).ToString()))
                {
                    categoryResponse.Status = ExtraCategoryEnum.Status.INACTIVE.ToString().ToUpper()[0] + ExtraCategoryEnum.Status.INACTIVE.ToString().ToLower().Substring(1);
                }
                return categoryResponse;
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Category ID must be a non-negative number."))
                {
                    fieldName = "Category Id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Category Id does not exist in the system"))
                {
                    fieldName = "Category Id";
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

        #region Deactive Category By Id
        public async Task DeActiveCategoryByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new BadRequestException("Category ID must be a non-negative number.");
                }
                var category = await _unitOfWork.CategoryRepository.GetCategoryByIdAsync(id);

                if (category == null)
                {
                    throw new NotFoundException("Category Id does not exist in the system.");
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
                if (category.ExtraCategoryProductCategories.Any())
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
                if (ex.Message.Equals("Category ID must be a non-negative number."))
                {
                    fieldName = "Category Id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string fileName = "";
                if (ex.Message.Equals("Category Id does not exist in the system."))
                {
                    fileName = "Category Id";
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

        #region Get Categories
        public async Task<GetCategoriesResponse> GetCategoriesAsync(string type, string? keySearchName, int? pageNumber, int? pageSize)
        {
            try
            {

                var categories = new List<Category>();
                var categoryResponse = new List<GetCategoryResponse>();

                if (!type.Equals("EXTRA") && !type.Equals("NORMAL"))
                {
                    throw new BadRequestException("Type are EXTRA and NORMAL");
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
                    numberItems = await this._unitOfWork.CategoryRepository.GetNumberCategoriesAsync(keySearchName, null);
                    categories = await this._unitOfWork.CategoryRepository.GetCategoriesAsync(keySearchName, null, type, pageSize.Value, pageNumber.Value);
                }
                else if (keySearchName != null && StringUtil.IsUnicode(keySearchName) == false)
                {
                    numberItems = await this._unitOfWork.CategoryRepository.GetNumberCategoriesAsync(null, keySearchName);
                    categories = await this._unitOfWork.CategoryRepository.GetCategoriesAsync(null, keySearchName, type, pageSize.Value, pageNumber.Value);
                }
                else if (keySearchName == null)
                {
                    numberItems = await this._unitOfWork.CategoryRepository.GetNumberCategoriesAsync(null, null);
                    categories = await this._unitOfWork.CategoryRepository.GetCategoriesAsync(null, null, type, pageSize.Value, pageNumber.Value);
                }
                _mapper.Map(categories, categoryResponse);

                foreach (var category in categoryResponse)
                {
                    if (category.Status.Equals(((int)CategoryEnum.Status.ACTIVE).ToString()))
                    {
                        category.Status = CategoryEnum.Status.ACTIVE.ToString().ToUpper()[0] + CategoryEnum.Status.ACTIVE.ToString().ToLower().Substring(1);
                    }
                    else if (category.Status.Equals(((int)CategoryEnum.Status.INACTIVE).ToString()))
                    {
                        category.Status = CategoryEnum.Status.INACTIVE.ToString().ToUpper()[0] + CategoryEnum.Status.INACTIVE.ToString().ToLower().Substring(1);
                    }
                }

                if (categoryResponse == null || categoryResponse.Count == 0)
                {
                    return new GetCategoriesResponse()
                    {
                        Categories = categoryResponse,
                        TotalItems = 0,
                        TotalPages = 0,
                    };
                }

                int totalPages = (int)((numberItems + pageSize) / pageSize); ;
                return new GetCategoriesResponse()
                {
                    Categories = categoryResponse,
                    TotalItems = numberItems,
                    TotalPages = totalPages,
                };
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Type are EXTRA and NORMAL"))
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

        #region Get Products
        public async Task<GetProductsResponse> GetProductsInCategory(int categoryId, string? keySearchName, int? pageNumber, int? pageSize)
        {
            try
            {
                if (categoryId <= 0)
                {
                    throw new BadRequestException("Category ID must be a non-negative number.");
                }
                var products = new List<Product>();
                var productResponse = new List<GetProductResponse>();
                var category = await this._unitOfWork.CategoryRepository.GetCategoryByIdAsync(categoryId);
                if (category == null)
                {
                    throw new NotFoundException("Category Id does not exist in the system.");
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
                    numberItems = await this._unitOfWork.ProductRepository.GetNumberProductsAsync(keySearchName, null);
                    products = await this._unitOfWork.ProductRepository.GetProductsByCategoryIdAsync(categoryId, keySearchName, null, pageSize.Value, pageNumber.Value);
                }
                else if (keySearchName != null && StringUtil.IsUnicode(keySearchName) == false)
                {
                    numberItems = await this._unitOfWork.ProductRepository.GetNumberProductsAsync(null, keySearchName);
                    products = await this._unitOfWork.ProductRepository.GetProductsByCategoryIdAsync(categoryId, null, keySearchName, pageSize.Value, pageNumber.Value);
                }
                else if (keySearchName == null)
                {
                    numberItems = await this._unitOfWork.ProductRepository.GetNumberProductsAsync(null, null);
                    products = await this._unitOfWork.ProductRepository.GetProductsByCategoryIdAsync(categoryId, null, null, pageSize.Value, pageNumber.Value);
                }
                _mapper.Map(products, productResponse);

                if (productResponse == null || productResponse.Count == 0)
                {
                    return new GetProductsResponse()
                    {
                        Products = productResponse,
                        TotalItems = 0,
                        TotalPages = 0,
                    };
                }

                int totalPages = (int)((numberItems + pageSize) / pageSize);

                return new GetProductsResponse()
                {
                    Products = productResponse,
                    TotalItems = numberItems,
                    TotalPages = totalPages,
                };
            }
            catch (BadRequestException ex)
            {

                string fileName = "";
                if (ex.Message.Equals("Category ID must be a non-negative number."))
                {
                    fileName = "Category Id";
                }
                string error = ErrorUtil.GetErrorString(fileName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {

                string fileName = "";
                if (ex.Message.Equals("Category Id does not exist in the system."))
                {
                    fileName = "Category Id";
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

        #region Get Extra Categories To Normal Category
        public async Task<GetCategoriesResponse> GetExtraCategoriesByCategoryId(int categoryId, string? keySearchName, int? pageNumber, int? pageSize)
        {
            try
            {
                if (categoryId <= 0)
                {
                    throw new BadRequestException("Category ID must be a non-negative number.");
                }
                var categoryResponse = new List<GetCategoryResponse>();
                var listExtraCategoriesInNormalCategory = new List<Category>();
                var category = await this._unitOfWork.CategoryRepository.GetCategoryByIdAsync(categoryId);
                if (category == null)
                {
                    throw new NotFoundException("Category Id does not exist in the system.");
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

                var extraCategories = await this._unitOfWork.ExtraCategoryRepository.GetExtraCategoriesByCategoryIdAsync(categoryId);

                foreach (var extraCategory in extraCategories)
                {
                    var extraCategoryInNormalCategory = await this._unitOfWork.CategoryRepository.GetCategoryByIdAsync(extraCategory.ExtraCategoryId);
                    listExtraCategoriesInNormalCategory.Add(extraCategoryInNormalCategory);
                }

                int numberItems = 0;
                if (keySearchName != null && StringUtil.IsUnicode(keySearchName))
                {
                    numberItems = this._unitOfWork.CategoryRepository.GetNumberExtraCategories(listExtraCategoriesInNormalCategory, keySearchName, null);
                    listExtraCategoriesInNormalCategory = this._unitOfWork.CategoryRepository.SearchAndPagingExtraCategory(listExtraCategoriesInNormalCategory, keySearchName, null, pageSize.Value, pageNumber.Value);
                }
                else if (keySearchName != null && StringUtil.IsUnicode(keySearchName) == false)
                {
                    numberItems = this._unitOfWork.CategoryRepository.GetNumberExtraCategories(listExtraCategoriesInNormalCategory, null, keySearchName);
                    listExtraCategoriesInNormalCategory = this._unitOfWork.CategoryRepository.SearchAndPagingExtraCategory(listExtraCategoriesInNormalCategory, null, keySearchName, pageSize.Value, pageNumber.Value);
                }
                else if (keySearchName == null)
                {
                    numberItems = this._unitOfWork.CategoryRepository.GetNumberExtraCategories(listExtraCategoriesInNormalCategory, null, null);
                    listExtraCategoriesInNormalCategory = this._unitOfWork.CategoryRepository.SearchAndPagingExtraCategory(listExtraCategoriesInNormalCategory, null, null, pageSize.Value, pageNumber.Value);
                }

                _mapper.Map(listExtraCategoriesInNormalCategory, categoryResponse);

                foreach (var extra in categoryResponse)
                {
                    if (extra.Status.Equals(((int)ExtraCategoryEnum.Status.ACTIVE).ToString()))
                    {
                        extra.Status = ExtraCategoryEnum.Status.ACTIVE.ToString().ToUpper()[0] + ExtraCategoryEnum.Status.ACTIVE.ToString().ToLower().Substring(1);
                    }
                    else if (extra.Status.Equals(((int)ExtraCategoryEnum.Status.INACTIVE).ToString()))
                    {
                        extra.Status = ExtraCategoryEnum.Status.INACTIVE.ToString().ToUpper()[0] + ExtraCategoryEnum.Status.INACTIVE.ToString().ToLower().Substring(1);
                    }
                }

                if (categoryResponse == null || categoryResponse.Count == 0)
                {
                    return new GetCategoriesResponse()
                    {
                        Categories = categoryResponse,
                        TotalItems = 0,
                        TotalPages = 0,
                    };
                }

                int totalPages = (int)((numberItems + pageSize) / pageSize);

                return new GetCategoriesResponse()
                {
                    Categories = categoryResponse,
                    TotalItems = numberItems,
                    TotalPages = totalPages
                };
            }
            catch (BadRequestException ex)
            {
                string fileName = "";
                if (ex.Message.Equals("Category ID must be a non-negative number."))
                {
                    fileName = "Category Id";
                }
                string error = ErrorUtil.GetErrorString(fileName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string fileName = "";
                if (ex.Message.Equals("Category Id does not exist in the system."))
                {
                    fileName = "Category Id";
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

        #region Add Extra Categories To Normal Category
        public async Task AddExtraCategoriesToNormalCategory(int categoryId, List<int> request)
        {
            try
            {
                if (categoryId < 0)
                {
                    throw new BadRequestException("Category ID must be a non-negative number.");
                }

                var checkCategoryId = await this._unitOfWork.CategoryRepository.GetCategoryByIdAsync(categoryId);
                var currentExtraCategoriesId = await this._unitOfWork.ExtraCategoryRepository.GetExtraCategoriesByCategoryIdAsync(categoryId);
                var listExtraCategoryInNomalCategory = currentExtraCategoriesId.Select(e => e.ExtraCategoryId).ToList();
                SplitIdCategoryResponse splittedExtraCategoriesIds = CustomListUtil.splitIdsToAddAndRemove(listExtraCategoryInNomalCategory, request);

                if (checkCategoryId == null)
                {
                    throw new BadRequestException("Category Id does not exist in the system.");
                }

                //Handle add and remove to database
                if (splittedExtraCategoriesIds.idsToAdd.Count > 0)
                {
                    List<ExtraCategory> extraCategoriesToInsert = new List<ExtraCategory>();
                    splittedExtraCategoriesIds.idsToAdd.ForEach(id => extraCategoriesToInsert.Add(new ExtraCategory
                    {
                        ProductCategoryId = categoryId,
                        ExtraCategoryId = id,
                        Status = (int)CategoryEnum.Status.ACTIVE
                    }));
                    _unitOfWork.ExtraCategoryRepository._dbContext.AddRange(extraCategoriesToInsert);
                }

                if (splittedExtraCategoriesIds.idsToRemove.Count > 0)
                {
                    var listExtraCategoriesToDelete = new List<ExtraCategory>();
                    List<ExtraCategory> extraCategoriesToDelete = new List<ExtraCategory>();
                    extraCategoriesToDelete = await _unitOfWork.ExtraCategoryRepository.GetExtraCategoriesByCategoryIdAsync(categoryId);
                    foreach (var extraCategory in extraCategoriesToDelete)
                    {
                        foreach (var id in splittedExtraCategoriesIds.idsToRemove)
                        {
                            if (id == extraCategory.ExtraCategoryId)
                            {
                                listExtraCategoriesToDelete.Add(extraCategory);
                            }
                        }
                    }
                    _unitOfWork.ExtraCategoryRepository._dbContext.RemoveRange(listExtraCategoriesToDelete);
                }
                await this._unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Category ID must be a non-negative number."))
                {
                    fieldName = "Category Id";
                }
                else if (ex.Message.Equals("Category Id does not exist in the system."))
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
    }
}
