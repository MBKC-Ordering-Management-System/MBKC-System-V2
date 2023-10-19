using MBKC.Service.DTOs.PartnerProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Services.Interfaces
{
    public interface IPartnerProductService
    {
        public Task<GetPartnerProductResponse> GetPartnerProduct(int productId, int partnerId, int storeId, IEnumerable<Claim> claims);
        public Task<GetPartnerProductsResponse> GetPartnerProducts(string? searchName, int? currentPage, int? itemsPerPage, IEnumerable<Claim> claims);
        public Task CreatePartnerProduct(PostPartnerProductRequest postPartnerProductRequest, IEnumerable<Claim> claims);
        public Task UpdatePartnerProduct(int productId, int partnerId, int storeId, UpdatePartnerProductRequest updatePartnerProductRequest, IEnumerable<Claim> claims);
        public Task DeletePartnerProductByIdAsync(int productId, int partnerId, int storeId, IEnumerable<Claim> claims);

    }
}
