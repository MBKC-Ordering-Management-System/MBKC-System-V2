using MBKC.Service.DTOs.Products;
using System.Security.Claims;

namespace MBKC.Service.Services.Interfaces
{
    public interface IProductService
    {
        public Task CreateProductAsync(CreateProductRequest createProductRequest, IEnumerable<Claim> claims);
        public Task<GetProductsResponse> GetProductsAsync(GetProductsRequest getProductsRequest, IEnumerable<Claim> claims);
        public Task<GetProductsResponse> GetProductsWithNumberOfProductSoldAsync(GetProductWithNumberSoldRequest getProductsRequest, IEnumerable<Claim> claims);
        public Task<GetProductResponse> GetProductAsync(int idProduct, IEnumerable<Claim> claims);
        public Task DeleteProductAsync(int idProduct, IEnumerable<Claim> claims);
        public Task UpdateProductStatusAsync(int idProduct, UpdateProductStatusRequest updateProductStatusRequest, IEnumerable<Claim> claims);
        public Task UpdateProductAsync(int idProduct, UpdateProductRequest updateProductRequest, IEnumerable<Claim> claims);
    }
}
