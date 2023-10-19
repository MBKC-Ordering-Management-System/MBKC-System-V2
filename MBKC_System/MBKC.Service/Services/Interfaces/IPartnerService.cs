using MBKC.Service.DTOs.Partners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Services.Interfaces
{
    public interface IPartnerService
    {
        public Task CreatePartner(PostPartnerRequest postPartnerRequest);
        public Task UpdatePartnerAsync(int partnerId, UpdatePartnerRequest updatePartnerRequest);
        public Task<GetPartnersResponse> GetPartnersAsync(string? keySearchName, string? keySortName, string? keySortStatus, int? pageNumber, int? pageSize, bool? isGetAll);
        public Task<GetPartnerResponse> GetPartnerByIdAsync(int id);
        public Task DeActivePartnerByIdAsync(int id);
    }
}
