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
        public Task UpdateStorePartnerRequestAsync(UpdateStorePartnerRequest updateStorePartnerRequest, IEnumerable<Claim> claims);
        public Task GetStorePartnersAsync(string? searchValue, int? currentPage, int? itemsPerPage, IEnumerable<Claim> claims);
        public Task<GetStorePartnerResponse> GetStorePartnerAsync(int storeId, int partnerId, IEnumerable<Claim> claims);
        public Task DeleteStorePartnerAsync(int storeId, int partnerId, IEnumerable<Claim> claims);
    }
}
