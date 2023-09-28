using MBKC.BAL.DTOs.FireBase;
using MBKC.BAL.DTOs.Stores;
using MBKC.BAL.DTOs.Verifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Services.Interfaces
{
    public interface IStoreService
    {
        public Task<GetStoresResponse> GetStoresAsync(string? searchValue, int? currentPage, int? itemsPerPage, int? brandId, IEnumerable<Claim>? claims);
        public Task<GetStoreResponse> GetStoreAsync(int id, int? brandId, IEnumerable<Claim>? claims);
        public Task CreateStoreAsync(CreateStoreRequest createStoreRequest, FireBaseImage fireBaseImageOption, Email emailOption);
        public Task UpdateStoreAsync(int brandId, int storeId, UpdateStoreRequest updateStoreRequest, FireBaseImage fireBaseImageOption, Email emailOption, IEnumerable<Claim> claims);
        public Task DeleteStoreAsync(int brandId, int storeId);
    }
}
