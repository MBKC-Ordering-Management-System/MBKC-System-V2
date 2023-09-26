using MBKC.BAL.DTOs.FireBase;
using MBKC.BAL.DTOs.Stores;
using MBKC.BAL.DTOs.Verifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Services.Interfaces
{
    public interface IStoreService
    {
        public Task<GetStoresResponse> GetStoresAsync(string? searchValue, int? currentPage, int? itemsPerPage, int? brandId);
        public Task<GetStoreResponse> GetStoreAsync(int id, int? brandId);
        public Task CreateStoreAsync(CreateStoreRequest createStoreRequest, FireBaseImage fireBaseImageOption, Email emailOption);
        public Task UpdateStoreAsync(int brandId, int storeId, UpdateStoreRequest updateStoreRequest, FireBaseImage fireBaseImageOption, Email emailOption);
        public Task DeleteStoreAsync(int brandId, int storeId);
    }
}
