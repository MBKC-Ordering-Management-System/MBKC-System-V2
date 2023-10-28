using MBKC.Service.DTOs.StorePartners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Services.Interfaces
{
    public interface IStorePartnerService
    {
        public Task CreateStorePartnerAsync(PostStorePartnerRequest postStorePartnerRequest, IEnumerable<Claim> claims);
        public Task UpdateStorePartnerRequestAsync(int storeId, int partnerId, UpdateStorePartnerRequest updateStorePartnerRequest, IEnumerable<Claim> claims);
        public Task UpdateStatusStorePartnerAsync(int storeId, int partnerId, UpdateStorePartnerStatusRequest updateStorePartnerStatusRequest, IEnumerable<Claim> claims);
        public Task<GetStorePartnersResponse> GetStorePartnersAsync(GetStorePartnersRequest getStorePartnersRequest, IEnumerable<Claim> claims);
        public Task<GetStorePartnerResponse> GetStorePartnerAsync(int storeId, int partnerId, IEnumerable<Claim> claims);
        public Task DeleteStorePartnerAsync(int storeId, int partnerId, IEnumerable<Claim> claims);
        public Task<GetStorePartnerInformationResponse> GetPartnerInformationAsync(int storeId, GetPartnerInformationRequest getPartnerInformationRequest, IEnumerable<Claim> claims);
    }
}
