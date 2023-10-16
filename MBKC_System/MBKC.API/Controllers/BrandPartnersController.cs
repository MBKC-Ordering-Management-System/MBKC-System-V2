using MBKC.API.Constants;
using MBKC.Repository.GrabFood.Models;
using MBKC.Service.DTOs.BrandPartners.Requests;
using MBKC.Service.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [ApiController]
    public class BrandPartnersController : ControllerBase
    {
        private IBrandPartnerService _brandPartnerService;
        public BrandPartnersController(IBrandPartnerService brandPartnerService)
        {
            this._brandPartnerService = brandPartnerService;
        }

        [HttpGet(APIEndPointConstant.BrandPartner.BrandPartnersEndPoint)]
        public async Task<IActionResult> GetBrandPartnersAsync([FromQuery] GetBrandPartnersRequest getBrandPartnersRequest)
        {
            return Ok();
        }

        [HttpPost(APIEndPointConstant.BrandPartner.BrandPartnersEndPoint)]
        public async Task<IActionResult> PostCreateNewBrandPartner([FromBody] PostBrandPartnerRequest postBrandPartnerRequest)
        {
            List<GrabFoodAuthenticationResponse> grabFoodAuthenticationResponses = await this._brandPartnerService.CreateBrandPartnerAsync(postBrandPartnerRequest);
            return Ok(grabFoodAuthenticationResponses);
        }
    }
}
