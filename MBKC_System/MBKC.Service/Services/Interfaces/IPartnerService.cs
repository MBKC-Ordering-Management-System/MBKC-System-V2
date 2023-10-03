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
    }
}
