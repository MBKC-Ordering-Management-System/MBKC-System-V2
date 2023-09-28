
using MBKC.BAL.DTOs.Categories;
using MBKC.BAL.DTOs.FireBase;
using MBKC.BAL.DTOs.Products;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Services.Interfaces
{
    public interface ICategoryService
    {
        public Task CreateCategoryAsync(PostCategoryRequest postCategoryRequest, FireBaseImage fireBaseImage, HttpContext httpContext);
        public Task UpdateCategoryAsync(int categoryId, UpdateCategoryRequest updateCategoryRequest, FireBaseImage fireBaseImage, HttpContext httpContext);
        public Task<GetCategoriesResponse> GetCategoriesAsync(string type, string? keySearchName, int? pageNumber, int? pageSize, HttpContext httpContext);
        public Task<GetCategoryResponse> GetCategoryByIdAsync(int id, HttpContext httpContext);
        public Task DeActiveCategoryByIdAsync(int id, HttpContext httpContext);
        public Task<GetProductsResponse> GetProductsInCategory(int categoryId, string? keySearchName, int? pageNumber, int? pageSize, HttpContext httpContext);
        public Task<GetCategoriesResponse> GetExtraCategoriesByCategoryId(int categoryId, string? keySearchName, int? pageNumber, int? pageSize, HttpContext httpContext);
        public Task AddExtraCategoriesToNormalCategory(int categoryId, List<int> request, HttpContext httpContext);
    }
}
