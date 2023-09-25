using AutoMapper;
using MBKC.BAL.DTOs.Categories;
using MBKC.BAL.DTOs.FireBase;
using MBKC.BAL.DTOs.Products;
using MBKC.BAL.DTOs.SplitIdCategories;
using MBKC.BAL.Exceptions;
using MBKC.BAL.Repositories.Interfaces;
using MBKC.BAL.Utils;
using MBKC.DAL.Enums;
using MBKC.DAL.Infrastructures;
using MBKC.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MBKC.BAL.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public CategoryRepository(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        public async Task<GetCategoryResponse> CreateCategoryAsync(PostCategoryRequest postCategoryRequest, FireBaseImage fireBaseImage)
        {
            string imageId = "";
            bool uploaded = false;
            try
            {
                var checkCategoryCode = await _unitOfWork.CategoryDAO.GetCategoryByCodeAsync(postCategoryRequest.Code);
                var checkCategoryName = await _unitOfWork.CategoryDAO.GetCategoryByNameAsync(postCategoryRequest.Name);
                var checkBrandId = await _unitOfWork.BrandDAO.GetBrandByIdAsync(postCategoryRequest.BrandId);

                if (checkBrandId == null)
                {
                    throw new NotFoundException("Brand does not exist in the system");
                }

                if (checkCategoryCode != null)
                {
                    throw new ConflictException("Category code already exist in the system");
                }
                if (checkCategoryName != null)
                {
                    throw new ConflictException("Category name already exist in the system");
                }
                string s = CategoryEnum.Type.EXTRA.ToString();
                if (!postCategoryRequest.Type.Equals(CategoryEnum.Type.EXTRA.ToString()) && !postCategoryRequest.Type.Equals(CategoryEnum.Type.NORMAL.ToString()))
                {
                    throw new BadRequestException("Category type are NORMAL and EXTRA");
                }
                // Upload image to firebase
                FileStream fileStream = Utils.FileUtil.ConvertFormFileToStream(postCategoryRequest.ImageUrl);
                FileUtil.SetCredentials(fireBaseImage);
                Guid guild = Guid.NewGuid();
                imageId = guild.ToString();
                var urlImage = await Utils.FileUtil.UploadImage(fileStream, "Category", imageId);
                uploaded = true;

                //Create category
                var category = new Category()
                {
                    Code = postCategoryRequest.Code,
                    Name = postCategoryRequest.Name,
                    Type = postCategoryRequest.Type,
                    DisplayOrder = postCategoryRequest.DisplayOrder,
                    ImageUrl = urlImage + $"&imageUrl={imageId}",
                    Description = postCategoryRequest.Description,
                    Brand = checkBrandId,
                    Status = (int)(CategoryEnum.Status.ACTIVE)
                };
                await _unitOfWork.CategoryDAO.CreateCategoryAsyncAsync(category);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<GetCategoryResponse>(category);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Category type are NORMAL and EXTRA"))
                {
                    fieldName = "Type";
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
            catch (ConflictException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Category code already exist in the system"))
                {
                    fieldName = "Code";
                }
                else if (ex.Message.Equals("Category name already exist in the system"))
                {
                    fieldName = "Name";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new ConflictException(error);
            }
            catch (Exception ex)
            {
                if (uploaded)
                {
                    await FileUtil.DeleteImageAsync(imageId, "Category");
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }

        }

        public async Task<GetCategoryResponse> UpdateCategoryAsync(int categoryId, UpdateCategoryRequest updateCategoryRequest, FireBaseImage fireBaseImage)
        {
            string imageId = "";
            bool uploaded = false;
            try
            {
                var category = await this._unitOfWork.CategoryDAO.GetCategoryByIdAsync(categoryId);
                var categories = await this._unitOfWork.CategoryDAO.GetCategoriesAsync();
                var checkDupplicatedName = categories.SingleOrDefault(c => c.Name == updateCategoryRequest.Name && c.CategoryId != categoryId);
                if (category == null)
                {
                    throw new NotFoundException("Category does not exist in the system");
                }

                if (checkDupplicatedName != null)
                {
                    throw new ConflictException("Category name already exist in the system");
                }
                if (updateCategoryRequest.ImageUrl != null)
                {
                    //Delete image from database
                    FileUtil.SetCredentials(fireBaseImage);
                    Uri uri = new Uri(category.ImageUrl);
                    imageId = HttpUtility.ParseQueryString(uri.Query).Get("imageUrl");
                    await FileUtil.DeleteImageAsync(imageId, "Category");

                    // Upload image to firebase
                    FileStream fileStream = Utils.FileUtil.ConvertFormFileToStream(updateCategoryRequest.ImageUrl);
                    Guid guild = Guid.NewGuid();
                    imageId = guild.ToString();
                    var urlImage = await Utils.FileUtil.UploadImage(fileStream, "Category", imageId);
                    category.ImageUrl = urlImage + $"&imageUrl={imageId}";
                    uploaded = true;
                }
                category.Name = updateCategoryRequest.Name;
                category.Description = updateCategoryRequest.Description;
                category.DisplayOrder = updateCategoryRequest.DisplayOrder;

                _unitOfWork.CategoryDAO.UpdateCategory(category);
                _unitOfWork.Commit();

                return _mapper.Map<GetCategoryResponse>(category);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Category does not exist in the system"))
                {
                    fieldName = "CategoryId";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (ConflictException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Category name already exist in the system"))
                {
                    fieldName = "Name";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new ConflictException(error);
            }
            catch (Exception ex)
            {
                if (uploaded)
                {
                    await FileUtil.DeleteImageAsync(imageId, "Category");
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new ConflictException(error);
            }


        }

        public async Task<Tuple<List<GetCategoryResponse>, int, int?, int?>> GetCategoriesAsync(string type, SearchCategoryRequest? searchCategoryRequest, int? PAGE_NUMBER, int? PAGE_SIZE)
        {
            try
            {
                var categoryResponse = new List<GetCategoryResponse>();
                var categories = await this._unitOfWork.CategoryDAO.GetCategoriesAsync();

                if (!type.Equals("EXTRA") && !type.Equals("NORMAL"))
                {
                    throw new BadRequestException("Type are EXTRA and NORMAL");
                }
                if (type.Equals("EXTRA"))
                {
                    categories = categories.Where(c => c.Type.Equals("EXTRA")).ToList();
                }
                else if (type.Equals("NORMAL"))
                {
                    categories = categories.Where(c => c.Type.Equals("NORMAL")).ToList();
                }


                _mapper.Map(categories, categoryResponse);

                if (PAGE_SIZE == null)
                {
                    PAGE_SIZE = 10;
                }

                if (PAGE_NUMBER == null)
                {
                    PAGE_NUMBER = 1;
                }
                //Search
                if (searchCategoryRequest.KeySearchName != "" && searchCategoryRequest.KeySearchName != null)
                {
                    categoryResponse = categoryResponse.Where(b => b.Name.ToLower().Contains(searchCategoryRequest.KeySearchName.Trim().ToLower())).ToList();
                }
                // Count total page
                int totalRecords = categoryResponse.Count();
                int totalPages = (int)Math.Ceiling((double)((double)totalRecords / PAGE_SIZE));

                // Paing
                categoryResponse = categoryResponse.Skip((int)((PAGE_NUMBER - 1) * PAGE_SIZE)).Take((int)PAGE_SIZE).ToList();

                return Tuple.Create(categoryResponse, totalPages, PAGE_NUMBER, PAGE_SIZE);
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

        public async Task<GetCategoryResponse> GetCategoryByIdAsync(int id)
        {
            try
            {
                var category = await this._unitOfWork.CategoryDAO.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    throw new NotFoundException("Category does not exist in the system");
                }

                return _mapper.Map<GetCategoryResponse>(category);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Category does not exist in the system"))
                {
                    fieldName = "CategoryId";
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

        public async Task DeActiveCategoryByIdAsync(int id)
        {
            try
            {
                var category = await _unitOfWork.CategoryDAO.GetCategoryByIdAsync(id);

                if (category == null)
                {
                    throw new NotFoundException("Category does not exist in the system");
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
                _unitOfWork.CategoryDAO.UpdateCategory(category);
                _unitOfWork.Commit();
            }
            catch (NotFoundException ex)
            {
                string fileName = "";
                if (ex.Message.Equals("Category does not exist in the system"))
                {
                    fileName = "CategoryId";
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

        public async Task<Tuple<List<GetProductResponse>, int, int?, int?>> GetProductsInCategory(int categoryId, SearchProductsInCategory searchProductsInCategory, int? PAGE_NUMBER, int? PAGE_SIZE)
        {
            try
            {
                var productResponse = new List<GetProductResponse>();
                var category = await this._unitOfWork.CategoryDAO.GetCategoryByIdAsync(categoryId);
                if (category == null)
                {
                    throw new NotFoundException("Category does not exist in the system.");
                }
                var products = await this._unitOfWork.ProductDAO.GetProductsByCategoryIdAsync(categoryId);
                _mapper.Map(products, productResponse);

                if (PAGE_SIZE == null)
                {
                    PAGE_SIZE = 10;
                }

                if (PAGE_NUMBER == null)
                {
                    PAGE_NUMBER = 1;
                }

                //Search
                if (searchProductsInCategory.KeySearchName != "" && searchProductsInCategory.KeySearchName != null)
                {
                    productResponse = productResponse.Where(b => b.Name.ToLower().Contains(searchProductsInCategory.KeySearchName.Trim().ToLower())).ToList();
                }

                // Count total page
                int totalRecords = productResponse.Count();
                int totalPages = (int)Math.Ceiling((double)((double)totalRecords / PAGE_SIZE));

                // Paing
                productResponse = productResponse.Skip((int)((PAGE_NUMBER - 1) * PAGE_SIZE)).Take((int)PAGE_SIZE).ToList();

                return Tuple.Create(productResponse, totalPages, PAGE_NUMBER, PAGE_SIZE);

            }
            catch (NotFoundException ex)
            {

                string fileName = "";
                if (ex.Message.Equals("Category does not exist in the system."))
                {
                    fileName = "CategoryId";
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

        public async Task<Tuple<List<GetCategoryResponse>, int, int?, int?>> GetExtraCategoriesByCategoryId(int categoryId, SearchCategoryRequest? searchCategoryRequest, int? PAGE_NUMBER, int? PAGE_SIZE)
        {
            try
            {
                var categoryResponse = new List<GetCategoryResponse>();
                var listExtraCategoriesInNormalCategory = new List<Category>();
                var category = await this._unitOfWork.CategoryDAO.GetCategoryByIdAsync(categoryId);
                if (category == null)
                {
                    throw new NotFoundException("Category does not exist in the system.");
                }
                var extraCategories = await this._unitOfWork.ExtraCategoryDAO.GetExtraCategoriesByCategoryIdAsync(categoryId);

                foreach (var extraCategory in extraCategories)
                {
                    var extraCategoryInNormalCategory = await this._unitOfWork.CategoryDAO.GetCategoryByIdAsync(extraCategory.ExtraCategoryId);
                    listExtraCategoriesInNormalCategory.Add(extraCategoryInNormalCategory);
                }

                _mapper.Map(listExtraCategoriesInNormalCategory, categoryResponse);

                if (PAGE_SIZE == null)
                {
                    PAGE_SIZE = 10;
                }

                if (PAGE_NUMBER == null)
                {
                    PAGE_NUMBER = 1;
                }

                //Search
                if (searchCategoryRequest.KeySearchName != "" && searchCategoryRequest.KeySearchName != null)
                {
                    categoryResponse = categoryResponse.Where(b => b.Name.ToLower().Contains(searchCategoryRequest.KeySearchName.Trim().ToLower())).ToList();
                }

                // Count total page
                int totalRecords = categoryResponse.Count();
                int totalPages = (int)Math.Ceiling((double)((double)totalRecords / PAGE_SIZE));

                // Paing
                categoryResponse = categoryResponse.Skip((int)((PAGE_NUMBER - 1) * PAGE_SIZE)).Take((int)PAGE_SIZE).ToList();

                return Tuple.Create(categoryResponse, totalPages, PAGE_NUMBER, PAGE_SIZE);

            }
            catch (NotFoundException ex)
            {

                string fileName = "";
                if (ex.Message.Equals("Category does not exist in the system."))
                {
                    fileName = "CategoryId";
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

        public async Task AddExtraCategoriesToNormalCategory(int categoryId, List<int> request)
        {
            var currentExtraCategoriesId = await this._unitOfWork.ExtraCategoryDAO.GetExtraCategoriesByCategoryIdAsync(categoryId);
            var listExtraCategoryInNomalCategory = currentExtraCategoriesId.Select(e => e.ExtraCategoryId).ToList();
            SplitIdCategoryResponse splittedExtraCategoriesIds = CustomListUtil.splitIdsToAddAndRemove(listExtraCategoryInNomalCategory, request);

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
                _unitOfWork.ExtraCategoryDAO._dbContext.AddRange(extraCategoriesToInsert);
            }

            if (splittedExtraCategoriesIds.idsToRemove.Count > 0)
            {
                var listExtraCategoriesToDelete = new List<ExtraCategory>();
                List<ExtraCategory> extraCategoriesToDelete = new List<ExtraCategory>();
                extraCategoriesToDelete = (List<ExtraCategory>)await _unitOfWork.ExtraCategoryDAO.GetExtraCategoriesByCategoryIdAsync((int)categoryId);
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
                _unitOfWork.ExtraCategoryDAO._dbContext.RemoveRange((IEnumerable<ExtraCategory>)listExtraCategoriesToDelete);
            }
            await this._unitOfWork.CommitAsync();
        }
    }
}