using MBKC.BAL.DTOs.FireBase;
using MBKC.BAL.DTOs.KitchenCenters;
using MBKC.BAL.DTOs.Verifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Services.Interfaces
{
    public interface IKitchenCenterService
    {
        public Task CreateKitchenCenterAsync(CreateKitchenCenterRequest newKitchenCenter, Email emailOption, FireBaseImage firebaseImageOption);
        public Task<GetKitchenCenterResponse> GetKitchenCenterAsync(int kitchenCenterId);
        public Task<GetKitchenCentersResponse> GetKitchenCentersAsync(int? itemsPerPage, int? currentPage, string? searchValue, bool? isGetAll);
        public Task UpdateKitchenCenterAsync(int kitchenCenterId, UpdateKitchenCenterRequest updatedKitchenCenter, Email emailOption, FireBaseImage firebaseImageOption);
        public Task DeleteKitchenCenterAsync(int kitchenCenterId);
    }
}
