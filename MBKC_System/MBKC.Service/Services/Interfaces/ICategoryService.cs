
using MBKC.Service.DTOs.Categories;
using MBKC.Service.DTOs.Products;
using Microsoft.AspNetCore.Http;

namespace MBKC.Service.Services.Interfaces
{
    public interface ICategoryService
    {
        public Task CreateCategoryAsync(PostCategoryRequest postCategoryRequest, HttpContext httpContext);
        public Task UpdateCategoryAsync(int categoryId, UpdateCategoryRequest updateCategoryRequest, HttpContext httpContext);
        public Task<GetCategoriesResponse> GetCategoriesAsync(GetCategoriesRequest getCategoriesRequest, HttpContext httpContext);
        public Task<GetCategoryResponse> GetCategoryByIdAsync(int id, HttpContext httpContext);
        public Task DeActiveCategoryByIdAsync(int id, HttpContext httpContext);
        public Task<GetCategoriesResponse> GetExtraCategoriesByCategoryId(int categoryId, GetExtraCategoriesRequest getExtraCategoriesRequest, HttpContext httpContext);
        public Task AddExtraCategoriesToNormalCategoryAsync(int categoryId, ExtraCategoryRequest extraCategoryRequest, HttpContext httpContext);
    }
}
