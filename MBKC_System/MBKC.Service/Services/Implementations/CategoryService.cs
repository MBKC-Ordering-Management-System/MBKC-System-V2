using AutoMapper;
using MBKC.Repository.Enums;
using MBKC.Repository.Infrastructures;
using MBKC.Repository.Models;
using MBKC.Service.Constants;
using MBKC.Service.DTOs.Categories;
using MBKC.Service.DTOs.Products;
using MBKC.Service.DTOs.SplitIdCategories;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;

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
        public async Task CreateCategoryAsync(PostCategoryRequest postCategoryRequest)
        {
            string folderName = "Categories";
            string imageId = "";
            bool isUploaded = false;
            try
            {
                var existedCategory = await _unitOfWork.CategoryRepository.GetCategoryByCodeAsync(postCategoryRequest.Code);
                if (existedCategory != null)
                {
                    throw new BadRequestException(MessageConstant.CategoryMessage.CategoryCodeExisted);
                }
                var brand = await _unitOfWork.BrandRepository.GetBrandByIdAsync(postCategoryRequest.BrandId);
                if (brand == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBrandId);
                }

                
                // Upload image to firebase
                FileStream fileStream = FileUtil.ConvertFormFileToStream(postCategoryRequest.ImageUrl);
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
                await this._unitOfWork.CategoryRepository.CreateCategoryAsyncAsync(category);
                await this._unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CategoryMessage.CategoryCodeExisted))
                {
                    fieldName = "Category code";
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
        public async Task UpdateCategoryAsync(int categoryId, UpdateCategoryRequest updateCategoryRequest)
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
                var category = await this._unitOfWork.CategoryRepository.GetCategoryByIdAsync(categoryId);
                if (category == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistCategoryId);
                }
                if(category.Status == (int)CategoryEnum.Status.DEACTIVE)
                {
                    throw new BadRequestException(MessageConstant.CategoryMessage.DeactiveCategory_Update);
                }

                string oldImageUrl = category.ImageUrl;
                if (updateCategoryRequest.ImageUrl != null)
                {
                    // Upload image to firebase
                    FileStream fileStream = FileUtil.ConvertFormFileToStream(updateCategoryRequest.ImageUrl);
                    Guid guild = Guid.NewGuid();
                    imageId = guild.ToString();
                    var urlImage = await this._unitOfWork.FirebaseStorageRepository.UploadImageAsync(fileStream, folderName, imageId);
                    if (urlImage != null)
                    {
                        isUploaded = true;
                    }
                    category.ImageUrl = urlImage + $"&imageUrl={imageId}";

                    //Delete image from database
                    await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(FileUtil.GetImageIdFromUrlImage(oldImageUrl, "imageUrl"), folderName);
                    isDeleted = true;
                }
                category.Name = updateCategoryRequest.Name;
                category.Description = updateCategoryRequest.Description;
                category.DisplayOrder = updateCategoryRequest.DisplayOrder;

                if (updateCategoryRequest.Status.Trim().ToLower().Equals(CategoryEnum.Status.ACTIVE.ToString().ToLower()))
                {
                    category.Status = (int)CategoryEnum.Status.ACTIVE;
                }
                else if (updateCategoryRequest.Status.Trim().ToLower().Equals(CategoryEnum.Status.INACTIVE.ToString().ToLower()))
                {
                    category.Status = (int)CategoryEnum.Status.INACTIVE;
                }
                this._unitOfWork.CategoryRepository.UpdateCategory(category);
                await this._unitOfWork.CommitAsync();
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
                } else if (ex.Message.Equals(MessageConstant.CategoryMessage.DeactiveCategory_Update))
                {
                    fieldName = "Updated category failed";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
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
        public async Task<GetCategoryResponse> GetCategoryByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidCategoryId);
                }
                var categoryResponse = new GetCategoryResponse();
                var category = await this._unitOfWork.CategoryRepository.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistCategoryId);
                }
                _mapper.Map(category, categoryResponse);
                return categoryResponse;
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidCategoryId))
                {
                    fieldName = "Category id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
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
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidCategoryId);
                }
                var category = await _unitOfWork.CategoryRepository.GetCategoryByIdAsync(id);

                if (category == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistCategoryId);
                }

                if(category.Status == (int)CategoryEnum.Status.DEACTIVE)
                {
                    throw new BadRequestException(MessageConstant.CategoryMessage.DeactiveCategory_Delete);
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
                this._unitOfWork.CategoryRepository.UpdateCategory(category);
                await this._unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidCategoryId))
                {
                    fieldName = "Category id";
                } else if (ex.Message.Equals(MessageConstant.CategoryMessage.DeactiveCategory_Delete))
                {
                    fieldName = "Deleted category failed";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string fileName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistCategoryId))
                {
                    fileName = "Category id";
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

        #region Get Categories
        public async Task<GetCategoriesResponse> GetCategoriesAsync(string type, string? keySearchName, int? pageNumber, int? pageSize)
        {
            try
            {
                var categories = new List<Category>();
                var categoryResponse = new List<GetCategoryResponse>();
                if (string.IsNullOrWhiteSpace(type))
                {
                    throw new BadRequestException(MessageConstant.CategoryMessage.InvalidCategoryType);
                }
                if (!type.ToLower().Trim().Equals(CategoryEnum.Type.EXTRA.ToString().ToLower())
                    && !type.ToLower().Trim().Equals(CategoryEnum.Type.NORMAL.ToString().ToLower()))
                {
                    throw new BadRequestException(MessageConstant.CategoryMessage.NotExistCategoryType);
                }
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

                int totalPages = (int)((numberItems + pageSize) / pageSize);
                if(numberItems == 0)
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
                else if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidCurrentPage))
                {
                    fieldName = "Current page";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidItemsPerPage))
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
                int totalPages = (int)((numberItems + pageSize) / pageSize);
                if(numberItems == 0)
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
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidCategoryId);
                }
                var category = await this._unitOfWork.CategoryRepository.GetCategoryByIdAsync(categoryId);
                if (category == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistCategoryId);
                }
                var categoryResponse = new List<GetCategoryResponse>();
                var listExtraCategoriesInNormalCategory = new List<Category>();

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

                int totalPages = (int)((numberItems + pageSize) / pageSize);
                if(numberItems == 0)
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
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidCategoryId))
                {
                    fieldName = "Category id";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidCurrentPage))
                {
                    fieldName = "Current page";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidItemsPerPage))
                {
                    fieldName = "Items per page";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string fileName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistCategoryId))
                {
                    fileName = "Category id";
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

        #region Add Extra Categories To Normal Category
        public async Task AddExtraCategoriesToNormalCategory(int categoryId, List<int> listExtraCategoryId)
        {
            try
            {
                if (categoryId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidCategoryId);
                }

                var category = await this._unitOfWork.CategoryRepository.GetCategoryByIdAsync(categoryId);

                if (category == null)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.NotExistCategoryId);
                }

                var extraCategory = await this._unitOfWork.ExtraCategoryRepository.GetExtraCategoriesByCategoryIdAsync(categoryId);
                var listIdExtraCategoriesInNomalCategory = extraCategory.Select(e => e.ExtraCategoryId).ToList();
                SplitIdCategoryResponse splittedExtraCategoriesIds = CustomListUtil.splitIdsToAddAndRemove(listIdExtraCategoriesInNomalCategory, listExtraCategoryId);

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
                   await this._unitOfWork.ExtraCategoryRepository.InsertRange(extraCategoriesToInsert);
                }

                if (splittedExtraCategoriesIds.idsToRemove.Count > 0)
                {
                    // Delete extra category from normal category
                    var listExtraCategoriesToDelete = new List<ExtraCategory>();
                    List<ExtraCategory> extraCategoriesToDelete = new List<ExtraCategory>();
                    extraCategoriesToDelete = await _unitOfWork.ExtraCategoryRepository.GetExtraCategoriesByCategoryIdAsync(categoryId);
                    foreach (var extra in extraCategoriesToDelete)
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
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistCategoryId))
                {
                    fieldName = "Category id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }

            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidCategoryId))
                {
                    fieldName = "Category id";
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
    }
}
