using FluentValidation;
using FluentValidation.Results;
using MBKC.BAL.DTOs.Brands;
using MBKC.BAL.DTOs.FireBase;
using MBKC.BAL.Exceptions;
using MBKC.BAL.Repositories.Interfaces;
using MBKC.BAL.Utils;
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
        private IOptions<FireBaseImage> _firebaseImageOptions;
        private IValidator<PostBrandRequest> _postBrandRequest;
        private IValidator<UpdateBrandRequest> _updateBrandRequest;

        public BrandsController(IBrandRepository brandRepository,
            IOptions<FireBaseImage> firebaseImageOptions,
            IValidator<PostBrandRequest> postBrandRequest, IValidator<UpdateBrandRequest> updateBrandRequest)
        {
            this._brandRepository = brandRepository;
            this._firebaseImageOptions = firebaseImageOptions;
            this._postBrandRequest = postBrandRequest;
            this._updateBrandRequest = updateBrandRequest;
        }
        #region Create Brand
        [HttpPost]
        public async Task<IActionResult> CreateBrand([FromForm] PostBrandRequest postBrandRequest)
        {
            ValidationResult validationResult = await _postBrandRequest.ValidateAsync(postBrandRequest);
            if (!validationResult.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(error);
            }
            var data = await this._brandRepository.CreateBrand(postBrandRequest, _firebaseImageOptions.Value);
            return Ok(new
            {
                Data = data
            });
        }
        #endregion

        #region Update Brand
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBrand([FromRoute] int id, [FromForm] UpdateBrandRequest updateBrandRequest)
        {
            ValidationResult validationResult = await _updateBrandRequest.ValidateAsync(updateBrandRequest);
            if (!validationResult.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(error);
            }
            var data = await this._brandRepository.UpdateBrand(id, updateBrandRequest, _firebaseImageOptions.Value);
            return Ok(new
            {
                Data = data
            });
        }
        #endregion

        #region Get Brands
        [HttpGet]
        public async Task<IActionResult> GetBrands()
        {
            var data = await this._brandRepository.GetBrands();
            return Ok(new
            {
                Data = data
            });
        }
        #endregion

        #region Get Brand By Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrandById([FromRoute] int id)
        {
            var data = await this._brandRepository.GetBrandById(id);
            return Ok(new
            {
                Data = data
            });
        }
        #endregion

    }
}
