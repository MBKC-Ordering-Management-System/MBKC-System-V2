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
        public Task<GetPartnerProductResponse> GetPartnerProductAsync(int productId, int partnerId, int storeId, IEnumerable<Claim> claims);
        public Task<GetPartnerProductsResponse> GetPartnerProductsAsync(GetPartnerProductsRequest getPartnerProductsRequest, IEnumerable<Claim> claims);
        public Task CreatePartnerProductAsync(PostPartnerProductRequest postPartnerProductRequest, IEnumerable<Claim> claims);
        public Task UpdatePartnerProductAsync(int productId, int partnerId, int storeId, UpdatePartnerProductRequest updatePartnerProductRequest, IEnumerable<Claim> claims);
        public Task UpdatePartnerProductStatusAsync(int productId, int partnerId, int storeId, UpdatePartnerProductStatusRequest updatePartnerProductStatusRequest, IEnumerable<Claim> claims);
        public Task DeletePartnerProductByIdAsync(int productId, int partnerId, int storeId, IEnumerable<Claim> claims);

    }
}
