using MBKC.Service.DTOs.KitchenCenters;
using System.Security.Claims;

namespace MBKC.Service.Services.Interfaces
{
    public interface IKitchenCenterService
    {
        public Task CreateKitchenCenterAsync(CreateKitchenCenterRequest newKitchenCenter);
        public Task<GetKitchenCenterResponse> GetKitchenCenterAsync(int kitchenCenterId);
        public Task<GetKitchenCentersResponse> GetKitchenCentersAsync(GetKitchenCentersRequest getKitchenCentersRequest);
        public Task UpdateKitchenCenterAsync(int kitchenCenterId, UpdateKitchenCenterRequest updatedKitchenCenter);
        public Task DeleteKitchenCenterAsync(int kitchenCenterId);
        public Task UpdateKitchenCenterStatusAsync(int kitchenCenterId, UpdateKitchenCenterStatusRequest updatedKitchenCenterStatus);
        public Task<GetKitchenCenterResponse> GetKitchenCenterProfileAsync(IEnumerable<Claim> claims);
    }
}
