using MBKC.Repository.GrabFood.Models;
using MBKC.Service.DTOs.BrandPartners.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Services.Interfaces
{
    public interface IBrandPartnerService
    {
        public Task<List<GrabFoodAuthenticationResponse>> CreateBrandPartnerAsync(PostBrandPartnerRequest postBrandPartnerRequest);
    }
}
