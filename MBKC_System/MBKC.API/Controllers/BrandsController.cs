using FluentValidation;
using FluentValidation.Results;
using MBKC.API.Constants;
using MBKC.Service.Authorization;
using MBKC.Service.DTOs;
using MBKC.Service.DTOs.Brands;
using MBKC.Service.Errors;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace MBKC.API.Controllers
{
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private IBrandService _brandService;
        private IValidator<PostBrandRequest> _postBrandValidator;
        private IValidator<UpdateBrandRequest> _updateBrandValidator;
        private IValidator<UpdateBrandStatusRequest> _updateBrandStatusValidator;
        private IValidator<UpdateBrandProfileRequest> _updateBrandProfileValidator;
        private IValidator<GetBrandsRequest> _getBrandsValidator;
        private IValidator<BrandRequest> _getBrandValidator;
        public BrandsController(IBrandService brandService,
            IValidator<PostBrandRequest> postBrandValidator,
            IValidator<UpdateBrandRequest> updateBrandValidator,
            IValidator<UpdateBrandStatusRequest> updateBrandStatusValidator,
            IValidator<GetBrandsRequest> getBrandsValidator,
            IValidator<BrandRequest> getBrandValidator,
            IValidator<UpdateBrandProfileRequest> updateBrandProfileValidator)
        {
            this._brandService = brandService;
            this._postBrandValidator = postBrandValidator;
            this._updateBrandValidator = updateBrandValidator;
            this._updateBrandStatusValidator = updateBrandStatusValidator;
            this._updateBrandProfileValidator = updateBrandProfileValidator;
            this._getBrandsValidator = getBrandsValidator;
            this._getBrandValidator = getBrandValidator;
        }

        #region Get Brands
        /// <summary>
        ///  Get a list of brands from the system.
        /// </summary>
        /// <param name="getBrandsRequest">
        ///  An object include SearchValue, ItemsPerPage, CurrentPage, SortBy for search, sort, paging.
        /// </param>
        /// <returns>
        /// A list of brands contains NumberItems, TotalPages, Brands' information
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         searchValue = HighLand Coffee
        ///         currentPage = 1
        ///         itemsPerPage = 5
        ///         sortBy = "propertyName_asc | propertyName_ASC | propertyName_desc | propertyName_DESC"
        /// </remarks>
        /// <response code="200">Get brands Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetBrandsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.MBKCAdmin)]
        [HttpGet(APIEndPointConstant.Brand.BrandsEndpoint)]
        public async Task<IActionResult> GetBrandsAsync([FromQuery] GetBrandsRequest getBrandsRequest)
        {
            ValidationResult validationResult = await this._getBrandsValidator.ValidateAsync(getBrandsRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            var data = await this._brandService.GetBrandsAsync(getBrandsRequest);

            return Ok(data);
        }
        #endregion

        #region Get Brand By Id
        /// <summary>
        /// Get specific brand by brand id.
        /// </summary>
        /// <param name="getBrandRequest">
        /// An object include id of brand.
        /// </param>
        /// <returns>
        /// An object contains brand's information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         id = 3
        ///         
        /// </remarks>
        /// <response code="200">Get brand Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetBrandResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.MBKCAdmin, PermissionAuthorizeConstant.BrandManager)]
        [HttpGet(APIEndPointConstant.Brand.BrandEndpoint)]
        public async Task<IActionResult> GetBrandByIdAsync([FromRoute] BrandRequest getBrandRequest)
        {
            ValidationResult validationResult = await this._getBrandValidator.ValidateAsync(getBrandRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            var data = await this._brandService.GetBrandByIdAsync(getBrandRequest.Id, claims);
            return Ok(data);
        }
        #endregion

        #region Ger Brand Profile
        /// <summary>
        /// Get brand profile.
        /// </summary>
        /// <returns>
        /// An object contains brand's information.
        /// </returns>
        /// <response code="200">Get brand profile Successfully.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetBrandResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager)]
        [HttpGet(APIEndPointConstant.Brand.BrandProfileEndpoint)]
        public async Task<IActionResult> GetBrandProfileAsync()
        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            GetBrandResponse getBrandResponse = await this._brandService.GetBrandProfileAsync(claims);
            return Ok(getBrandResponse);
        }
        #endregion

        #region Create Brand
        /// <summary>
        ///  Create new brand.
        /// </summary>
        /// <param name="postBrandRequest">
        /// An object includes information about brand. 
        /// </param>
        /// <returns>
        /// A success message about creating new brand.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         POST
        ///         Name = MyBrand
        ///         Address = 123 Main St
        ///         ManagerEmail = manager@gmail.com
        ///         Logo =  [Image file]
        /// </remarks>
        /// <response code="200">Created new brand successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.MultipartFormData)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.MBKCAdmin)]
        [HttpPost(APIEndPointConstant.Brand.BrandsEndpoint)]
        public async Task<IActionResult> PostCreateBrandAsync([FromForm] PostBrandRequest postBrandRequest)
        {
            ValidationResult validationResult = await _postBrandValidator.ValidateAsync(postBrandRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            await this._brandService.CreateBrandAsync(postBrandRequest);
            return Ok(new
            {
                Message = MessageConstant.BrandMessage.CreatedNewBrandSuccessfully
            });
        }
        #endregion

        #region Update Existed Brand
        /// <summary>
        ///  Update an existed brand information.
        /// </summary>
        /// <param name="brandRequest">
        /// An object include id of brand.
        /// </param>
        ///  <param name="updateBrandRequest">
        /// An object include information for update brand.
        ///  </param>
        /// <returns>
        /// A success message about updating brand.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         PUT
        ///         Id = 3
        ///         Name = MyBrand 
        ///         Address = 123 Main St
        ///         Status = INACTIVE | ACTIVE
        ///         Logo = [Image File]
        ///         BrandManagerEmail = example@gmail.com
        /// </remarks>
        /// <response code="200">Updated Existed Brand Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.MultipartFormData)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.MBKCAdmin)]
        [HttpPut(APIEndPointConstant.Brand.BrandEndpoint)]
        public async Task<IActionResult> UpdateBrandAsync([FromRoute] BrandRequest brandRequest, [FromForm] UpdateBrandRequest updateBrandRequest)
        {
            ValidationResult validationResultBrandId = await _getBrandValidator.ValidateAsync(brandRequest);
            ValidationResult validationResult = await _updateBrandValidator.ValidateAsync(updateBrandRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            if (validationResultBrandId.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResultBrandId);
                throw new BadRequestException(errors);
            }
            await this._brandService.UpdateBrandAsync(brandRequest.Id, updateBrandRequest);
            return Ok(new
            {
                Message = MessageConstant.BrandMessage.UpdatedBrandSuccessfully
            });
        }
        #endregion

        #region Update Existed Brand's Status
        /// <summary>
        ///  Update an existed brand status.
        /// </summary>
        /// <param name="getBrandRequest">
        /// An object includes id of brand for updating brand.
        /// </param>
        ///  <param name="updateBrandStatusRequest">
        /// An object includes status for updating brand.
        ///  </param>
        /// <returns>
        /// A success message about updating brand status.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         PUT
        ///         id = 3
        ///         
        ///         { 
        ///             "status": "ACTIVE | INACTIVE"
        ///         }
        /// </remarks>
        /// <response code="200">Updated Existed Brand Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.ApplicationJson)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.MBKCAdmin)]
        [HttpPut(APIEndPointConstant.Brand.UpdatingStatusBrand)]
        public async Task<IActionResult> UpdateBrandStatusAsync([FromRoute] BrandRequest getBrandRequest, [FromBody] UpdateBrandStatusRequest updateBrandStatusRequest)
        {
            ValidationResult validationResult = await this._updateBrandStatusValidator.ValidateAsync(updateBrandStatusRequest);
            ValidationResult validationResultBrandId = await this._getBrandValidator.ValidateAsync(getBrandRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            if (validationResultBrandId.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResultBrandId);
                throw new BadRequestException(errors);
            }
            await this._brandService.UpdateBrandStatusAsync(getBrandRequest.Id, updateBrandStatusRequest);
            return Ok(new
            {
                Message = MessageConstant.BrandMessage.UpdatedBrandStatusSuccessfully
            });
        }
        #endregion

        #region Update Brand Profile
        /// <summary>
        ///  Update an existed brand profile.
        /// </summary>
        /// <param name="getBrandRequest">
        /// An object include id of brand.
        /// </param>
        ///  <param name="updateBrandProfileRequest">
        /// An object include information about brand for update brand's profile.
        ///  </param>
        /// <returns>
        /// An success message about updating brand's profile
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         PUT
        ///         Id = 3
        ///         Name = MyBrand
        ///         Address = 123 Main St
        ///         Logo = [Image File]
        /// </remarks>
        /// <response code="200">Updated Existed Brand Profile Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.MultipartFormData)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager)]
        [HttpPut(APIEndPointConstant.Brand.UpdatingProfileBrand)]
        public async Task<IActionResult> UpdateBrandProfileAsync([FromRoute] BrandRequest getBrandRequest, [FromForm] UpdateBrandProfileRequest updateBrandProfileRequest)
        {
            ValidationResult validationResult = await this._updateBrandProfileValidator.ValidateAsync(updateBrandProfileRequest);
            ValidationResult validationResultBrandId = await this._getBrandValidator.ValidateAsync(getBrandRequest);
            if (validationResult.IsValid == false)
            {
                var errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            if (validationResultBrandId.IsValid == false)
            {
                var errors = ErrorUtil.GetErrorsString(validationResultBrandId);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._brandService.UpdateBrandProfileAsync(getBrandRequest.Id, updateBrandProfileRequest, claims);
            return Ok(new
            {
                Message = MessageConstant.BrandMessage.UpdatedBrandProfileSuccessfully
            });
        }
        #endregion

        #region Delete existed Brand By Id
        /// <summary>
        /// Delete existed brand by id.
        /// </summary>
        /// <param name="getBrandRequest">
        ///  An object include id of brand.
        /// </param>
        /// <returns>
        /// An object will return message "Deleted brand successfully".
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///        DELETE
        ///         id  = 3
        /// </remarks>
        /// <response code="200">Delete brand successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.MBKCAdmin)]
        [HttpDelete(APIEndPointConstant.Brand.BrandEndpoint)]
        public async Task<IActionResult> DeActiveBrandByIdAsync([FromRoute] BrandRequest getBrandRequest)
        {
            ValidationResult validationResultBrandId = await this._getBrandValidator.ValidateAsync(getBrandRequest);
            if (validationResultBrandId.IsValid == false)
            {
                var errors = ErrorUtil.GetErrorsString(validationResultBrandId);
                throw new BadRequestException(errors);
            }
            await this._brandService.DeActiveBrandByIdAsync(getBrandRequest.Id);
            return Ok(new
            {
                Message = MessageConstant.BrandMessage.DeletedBrandSuccessfully
            });
        }
        #endregion

    }
}
