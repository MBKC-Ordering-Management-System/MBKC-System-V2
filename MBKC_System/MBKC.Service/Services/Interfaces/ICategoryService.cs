﻿
using MBKC.Service.DTOs.Categories;
using MBKC.Service.DTOs.Products;
using Microsoft.AspNetCore.Http;

namespace MBKC.Service.Services.Interfaces
{
    public interface ICategoryService
    {
        public Task CreateCategoryAsync(PostCategoryRequest postCategoryRequest, HttpContext httpContext);
        public Task UpdateCategoryAsync(int categoryId, UpdateCategoryRequest updateCategoryRequest, HttpContext httpContext);
        public Task<GetCategoriesResponse> GetCategoriesAsync(string type, string? keySearchName, int? pageNumber, int? pageSize, HttpContext httpContext);
        public Task<GetCategoryResponse> GetCategoryByIdAsync(int id, HttpContext httpContext);
        public Task DeActiveCategoryByIdAsync(int id, HttpContext httpContext);
        public Task<GetProductsResponse> GetProductsInCategory(int categoryId, string? keySearchName, int? pageNumber, int? pageSize, HttpContext httpContext);
        public Task<GetCategoriesResponse> GetExtraCategoriesByCategoryId(int categoryId, string? keySearchName, int? pageNumber, int? pageSize, HttpContext httpContext);
        public Task AddExtraCategoriesToNormalCategory(int categoryId, ExtraCategoryRequest extraCategoryRequest, HttpContext httpContext);
    }
}