
using MBKC.Service.DTOs.Categories;
using MBKC.Service.DTOs.Products;

namespace MBKC.Service.Services.Interfaces
{
    public interface ICategoryService
    {
        public Task CreateCategoryAsync(PostCategoryRequest postCategoryRequest);
        public Task UpdateCategoryAsync(int categoryId, UpdateCategoryRequest updateCategoryRequest);
        public Task<GetCategoriesResponse> GetCategoriesAsync(string type, string? keySearchName, int? pageNumber, int? pageSize);
        public Task<GetCategoryResponse> GetCategoryByIdAsync(int id);
        public Task DeActiveCategoryByIdAsync(int id);
        public Task<GetProductsResponse> GetProductsInCategory(int categoryId, string? keySearchName, int? pageNumber, int? pageSize);
        public Task<GetCategoriesResponse> GetExtraCategoriesByCategoryId(int categoryId, string? keySearchName, int? pageNumber, int? pageSize);
        public Task AddExtraCategoriesToNormalCategory(int categoryId, List<int> request);
    }
}
