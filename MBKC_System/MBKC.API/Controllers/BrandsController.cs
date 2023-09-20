using MBKC.BAL.DTOs.Brands;
using MBKC.BAL.DTOs.FireBase;
using MBKC.BAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private IBrandRepository _brandRepository;
        IOptions<FireBaseImage> _firebaseImageOptions;
        public BrandsController(IBrandRepository brandRepository, IOptions<FireBaseImage> firebaseImageOptions)
        {
            _brandRepository = brandRepository;
            _firebaseImageOptions = firebaseImageOptions;   
        }
        #region Create KitchenCenter

        [HttpPost]
        public async Task<IActionResult> CreateBrand([FromForm] PostBrandRequest postBrandRequest)
        {

            await this._brandRepository.CreateBrand(postBrandRequest, _firebaseImageOptions.Value);
            return Ok(new
            {
            });
        }
        #endregion

    }
}
