
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
    public interface ICategoryService
    {
        public Task CreateCategoryAsync(PostCategoryRequest postCategoryRequest, FireBaseImage fireBaseImage);
        public Task UpdateCategoryAsync(int categoryId, UpdateCategoryRequest updateCategoryRequest, FireBaseImage fireBaseImage);
        public Task<GetCategoriesResponse> GetCategoriesAsync(string type, string? keySearchName, int? pageNumber, int? pageSize);
        public Task<GetCategoryResponse> GetCategoryByIdAsync(int id);
        public Task DeActiveCategoryByIdAsync(int id);
        public Task<GetProductsResponse> GetProductsInCategory(int categoryId, string? keySearchName, int? pageNumber, int? pageSize);
        public Task<GetCategoriesResponse> GetExtraCategoriesByCategoryId(int categoryId, string? keySearchName, int? pageNumber, int? pageSize);
        public Task AddExtraCategoriesToNormalCategory(int categoryId, List<int> request);
    }
}
