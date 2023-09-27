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
                if (category == null)
                {
                    throw new NotFoundException("Category Id does not exist in the system");
                }
                if (categoryCode != null && !category.Code.ToLower().Equals(updateCategoryRequest.Code.ToLower()))
                {
                    throw new BadRequestException("Category code already exist in the system.");
                }


                if (updateCategoryRequest.ImageUrl != null)
                {
                    //Delete image from database
                    FileUtil.SetCredentials(fireBaseImage);
                    await FileUtil.DeleteImageAsync(FileUtil.GetImageIdFromUrlImage(category.ImageUrl, "imageUrl"), "Categories");

                    // Upload image to firebase
                    FileStream fileStream = Utils.FileUtil.ConvertFormFileToStream(updateCategoryRequest.ImageUrl);
                    Guid guild = Guid.NewGuid();
                    imageId = guild.ToString();
                    var urlImage = await Utils.FileUtil.UploadImageAsync(fileStream, "Categories", imageId);
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

                if (!updateCategoryRequest.Status.Equals(CategoryEnum.Status.ACTIVE.ToString().ToLower()) &&
                    !updateCategoryRequest.Status.Equals(CategoryEnum.Status.INACTIVE.ToString().ToLower()))
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
                    throw new BadRequestException("Category Id is not suitable for the system.");
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
                if (ex.Message.Equals("Category Id is not suitable for the system."))
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
                    throw new BadRequestException("Category Id is not suitable for the system.");
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
                if (ex.Message.Equals("Category Id is not suitable for the system."))
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
                    numberItems = await this._unitOfWork.CategoryRepository.GetNumberCategoriesAsync(keySearchName, null, type);
                    categories = await this._unitOfWork.CategoryRepository.GetCategoriesAsync(keySearchName, null, type, pageSize.Value, pageNumber.Value);
                }
                else if (keySearchName != null && StringUtil.IsUnicode(keySearchName) == false)
                {
                    numberItems = await this._unitOfWork.CategoryRepository.GetNumberCategoriesAsync(null, keySearchName, type);
                    categories = await this._unitOfWork.CategoryRepository.GetCategoriesAsync(null, keySearchName, type, pageSize.Value, pageNumber.Value);
                }
                else if (keySearchName == null)
                {
                    numberItems = await this._unitOfWork.CategoryRepository.GetNumberCategoriesAsync(null, null, type);
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
        public async Task<GetProductsResponse> GetProductsInCategory(int categoryId, string? keySearchName, int? pageNumber, int? pageSize)
        {
            try
            {
                if (categoryId <= 0)
                {
                    throw new BadRequestException("Category Id is not suitable for the system.");
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

                string fieldName = "";
                if (ex.Message.Equals("Category Id is not suitable for the system."))
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
                    throw new BadRequestException("Category Id is not suitable for the system.");
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

                foreach (var extraCategoryId in extraCategories)
                {
                    var extraCategoryInNormalCategory = await this._unitOfWork.CategoryRepository.GetCategoryByIdAsync(extraCategoryId);
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
                string fieldName = "";
                if (ex.Message.Equals("Category Id is not suitable for the system."))
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
        public async Task AddExtraCategoriesToNormalCategory(int categoryId, List<int> listExtraCategoryId)
        {
            try
            {
                if (categoryId < 0)
                {
                    throw new BadRequestException("Category Id is not suitable for the system.");
                }

                if (listExtraCategoryId.Any(item => item <= 0))
                {
                    throw new BadRequestException("Extra category id must be greater than 0");
                }
                var checkListExtraCategoryId = this._unitOfWork.CategoryRepository.CheckListExtraCategoryId(listExtraCategoryId);
                if (!checkListExtraCategoryId)
                {
                    throw new BadRequestException("There is an Extra Category Id in the List that does not exist in the system.");
                }

                var category = await this._unitOfWork.CategoryRepository.GetCategoryByIdAsync(categoryId);
                if (category == null)
                {
                    throw new BadRequestException("Category Id does not exist in the system.");
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
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Category Id does not exist in the system."))
                {
                    fieldName = "Category Id";
                }
                if (ex.Message.Equals("There is an Extra Category Id in the List that does not exist in the system."))
                {
                    fieldName = "List Extra Category Id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }

            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Category Id is not suitable for the system."))
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
