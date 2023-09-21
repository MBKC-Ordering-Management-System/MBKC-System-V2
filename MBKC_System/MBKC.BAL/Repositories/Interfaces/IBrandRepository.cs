using MBKC.BAL.DTOs.Brands;
using MBKC.BAL.DTOs.FireBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Repositories.Interfaces
{
    public interface IBrandRepository
    {
        public Task<GetBrandResponse> CreateBrand(PostBrandRequest postBrandRequest, FireBaseImage fireBaseImage);
        public Task<GetBrandResponse> UpdateBrand( int brandId, UpdateBrandRequest updateBrandRequest, FireBaseImage fireBaseImage);
        public Task<List<GetBrandResponse>> GetBrands();
        public Task<GetBrandResponse> GetBrandById(int id);

    }
}
