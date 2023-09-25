
using MBKC.BAL.DTOs.Categories;
using MBKC.BAL.DTOs.FireBase;
using MBKC.BAL.DTOs.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        public Task<GetCategoryResponse> CreateCategoryAsync(PostCategoryRequest postCategoryRequest, FireBaseImage fireBaseImage);
        public Task<GetCategoryResponse> UpdateCategoryAsync(int categoryId, UpdateCategoryRequest updateCategoryRequest, FireBaseImage fireBaseImage);
        public Task<Tuple<List<GetCategoryResponse>, int, int?, int?>> GetCategoriesAsync(string type, SearchCategoryRequest? searchCategoryRequest, int? PAGE_NUMBER, int? PAGE_SIZE);
        public Task<GetCategoryResponse> GetCategoryByIdAsync(int id);
        public Task DeActiveCategoryByIdAsync(int id);
        public Task<Tuple<List<GetProductResponse>, int, int?, int?>> GetProductsInCategory(int categoryId, SearchProductsInCategory? searchProductsInCategory, int? PAGE_NUMBER, int? PAGE_SIZE);
        public Task<Tuple<List<GetCategoryResponse>, int, int?, int?>> GetExtraCategoriesByCategoryId(int categoryId, SearchCategoryRequest? searchCategoryRequest, int? PAGE_NUMBER, int? PAGE_SIZE);
        public Task AddExtraCategoriesToNormalCategory(int categoryId, List<int> request);
    }
}
