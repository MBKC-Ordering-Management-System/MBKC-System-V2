using MBKC.BAL.DTOs;
using MBKC.BAL.DTOs.Brands;
using MBKC.BAL.DTOs.FireBase;
using MBKC.BAL.DTOs.Verifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Services.Interfaces
{
    public interface IBrandService
    {
        public Task CreateBrandAsync(PostBrandRequest postBrandRequest, FireBaseImage fireBaseImage, Email emailSystem);
        public Task UpdateBrandAsync( int brandId, UpdateBrandRequest updateBrandRequest, FireBaseImage fireBaseImage, Email emailOption);
        public Task<GetBrandsResponse> GetBrandsAsync(string? keySearchName, string? keyStatusFilter, int? pageNumber, int? pageSize, bool? isGetAll);
        public Task<GetBrandResponse> GetBrandByIdAsync(int id);
        public Task DeActiveBrandByIdAsync(int id);
    }
}
