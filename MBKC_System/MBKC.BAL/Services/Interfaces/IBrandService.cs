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
        public Task<GetBrandResponse> CreateBrandAsync(PostBrandRequest postBrandRequest, FireBaseImage fireBaseImage, Email emailSystem);
        public Task<GetBrandResponse> UpdateBrandAsync( int brandId, UpdateBrandRequest updateBrandRequest, FireBaseImage fireBaseImage);
        public Task<Tuple<List<GetBrandResponse>, int, int?, int?>> GetBrandsAsync(SearchBrandRequest? searchBrandRequest, int? PAGE_NUMBER, int? PAGE_SIZE);
        public Task<GetBrandResponse> GetBrandByIdAsync(int id);
        public Task DeActiveBrandByIdAsync(int id);
    }
}
