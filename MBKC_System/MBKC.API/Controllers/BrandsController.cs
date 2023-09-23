using FluentValidation;
using FluentValidation.Results;
using MBKC.BAL.DTOs.Brands;
using MBKC.BAL.DTOs.FireBase;
using MBKC.BAL.DTOs.Verifications;
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
        private IOptions<Email> _emailOption;
        public BrandsController(IBrandRepository brandRepository,
            IOptions<FireBaseImage> firebaseImageOptions,
            IValidator<PostBrandRequest> postBrandRequest,
            IOptions<Email> emailOption,
            IValidator<UpdateBrandRequest> updateBrandRequest)
        {
            this._brandRepository = brandRepository;
            this._firebaseImageOptions = firebaseImageOptions;
            this._postBrandRequest = postBrandRequest;
            this._updateBrandRequest = updateBrandRequest;
            this._emailOption = emailOption;
        }
        #region Create Brand
        [HttpPost]
        public async Task<IActionResult> CreateBrandAsync([FromForm] PostBrandRequest postBrandRequest)
        {
            ValidationResult validationResult = await _postBrandRequest.ValidateAsync(postBrandRequest);
            if (!validationResult.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(error);
            }
            var data = await this._brandRepository.CreateBrandAsync(postBrandRequest, _firebaseImageOptions.Value, _emailOption.Value);
            return Ok(new
            {
                Data = data
            });
        }
        #endregion

        #region Update Brand
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBrandAsync([FromRoute] int id, [FromForm] UpdateBrandRequest updateBrandRequest)
        {
            ValidationResult validationResult = await _updateBrandRequest.ValidateAsync(updateBrandRequest);
            if (!validationResult.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(error);
            }
            var data = await this._brandRepository.UpdateBrandAsync(id, updateBrandRequest, _firebaseImageOptions.Value);
            return Ok(new
            {
                Data = data
            });
        }
        #endregion

        #region Get Brands
        [HttpGet]
        public async Task<IActionResult> GetBrandsAsync([FromQuery] SearchBrandRequest? searchBrandRequest, [FromQuery] int? PAGE_NUMBER, [FromQuery] int? PAGE_SIZE)
        {
            var data = await this._brandRepository.GetBrandsAsync(searchBrandRequest, PAGE_NUMBER, PAGE_SIZE);

            return Ok(new
            {
                brands = data.Item1,
                totalPage = data.Item2,
                pageNumber = data.Item3,
                pageSize = data.Item4
            });
        }
        #endregion

        #region Get Brand By Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrandByIdAsync([FromRoute] int id)
        {
            var data = await this._brandRepository.GetBrandByIdAsync(id);
            return Ok(new
            {
                Data = data
            });
        }
        #endregion

        #region Deactive Brand By Id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeActiveBrandByIdAsync([FromRoute] int id)
        {
            await this._brandRepository.DeActiveBrandByIdAsync(id);
            return Ok(new
            {
                Message = "Deactive brand successfully"
            });
        }
        #endregion

    }
}
