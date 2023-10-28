using MBKC.Service.DTOs;
using MBKC.Service.DTOs.Brands;
using System.Security.Claims;

namespace MBKC.Service.Services.Interfaces
{
    public interface IBrandService
    {
        public Task CreateBrandAsync(PostBrandRequest postBrandRequest);
        public Task UpdateBrandAsync( int brandId, UpdateBrandRequest updateBrandRequest);
        public Task<GetBrandsResponse> GetBrandsAsync(GetBrandsRequest getBrandsRequest);
        public Task<GetBrandResponse> GetBrandByIdAsync(int id, IEnumerable<Claim> claims);
        public Task DeActiveBrandByIdAsync(int id);
        public Task UpdateBrandStatusAsync(int brandId, UpdateBrandStatusRequest updateBrandStatusRequest);
        public Task UpdateBrandProfileAsync(int brandId, UpdateBrandProfileRequest updateBrandProfileRequest, IEnumerable<Claim> claims);
        public Task<GetBrandResponse> GetBrandProfileAsync(IEnumerable<Claim> claims);
    }
}
