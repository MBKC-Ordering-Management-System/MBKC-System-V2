using MBKC.Service.DTOs.MappingProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Services.Interfaces
{
    public interface IMappingProductService
    {
        public Task<GetMappingProductResponse> GetMappingProduct(int productId, int partnerId, int storeId, IEnumerable<Claim> claims);
        public Task<GetMappingProductsResponse> GetMappingProducts(string? searchName, int? currentPage, int? itemsPerPage, IEnumerable<Claim> claims);
        public Task CreateMappingProduct(PostMappingProductRequest postMappingProductRequest, IEnumerable<Claim> claims);
        public Task UpdateMappingProduct(int productId, int partnerId, int storeId, UpdateMappingProductRequest updateMappingProductRequest, IEnumerable<Claim> claims);
    }
}
