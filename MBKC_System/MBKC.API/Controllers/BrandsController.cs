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
        private IValidator<PostBrandRequest> _postBrandRequest;
        private IValidator<UpdateBrandRequest> _updateBrandRequest;
        private IValidator<UpdateBrandStatusRequest> _updateBrandStatusRequest;
        private IValidator<UpdateBrandProfileRequest> _updateBrandProfileRequest;
        public BrandsController(IBrandService brandService,
            IValidator<PostBrandRequest> postBrandRequest,
            IValidator<UpdateBrandRequest> updateBrandRequest, 
            IValidator<UpdateBrandStatusRequest> updateBrandStatusRequest,
            IValidator<UpdateBrandProfileRequest> updateBrandProfileRequest)
        {
            this._brandService = brandService;
            this._postBrandRequest = postBrandRequest;
            this._updateBrandRequest = updateBrandRequest;
            this._updateBrandStatusRequest = updateBrandStatusRequest;
            this._updateBrandProfileRequest = updateBrandProfileRequest;
        }

        #region Get Brands
        /// <summary>
        ///  Get a list of brands from the system with condition paging, searchByName and filterByStatus.
        /// </summary>
        /// <param name="keySearchName">
        ///  The brand name that the user wants to search.
        /// </param>
        /// <param name="keyStatusFilter">
        /// The status of the brand that the user wants to filter.
        /// </param>
        /// <param name="currentPage">
        /// The current page the user wants to get next items.
        /// </param>
        /// <param name="itemsPerPage">
        /// number of elements on a page.
        /// </param>
        /// <param name="isGetAll">
        /// Input TRUE if you want to get all brands, ignoring pageNumber and pageSize, otherwise Input FALSE
        /// </param>
        /// <returns>
        /// A list of brands contains TotalItems, TotalPages, Brands' information
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         keySearchName = HighLand Coffee
        ///         keyStatusFilter = ACTIVE | INACTIVE | DEACTIVE
        ///         pageSize = 5
        ///         pageNumber = 1
        /// </remarks>
        /// <response code="200">Get brands Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetBrandsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.MBKC_Admin)]
        [HttpGet(APIEndPointConstant.Brand.BrandsEndpoint)]
        public async Task<IActionResult> GetBrandsAsync([FromQuery] string? keySearchName, [FromQuery] string? keyStatusFilter, [FromQuery] int? currentPage, [FromQuery] int? itemsPerPage, [FromQuery] bool? isGetAll)
        {
            var data = await this._brandService.GetBrandsAsync(keySearchName, keyStatusFilter, currentPage, itemsPerPage, isGetAll);

            return Ok(data);
        }
        #endregion

        #region Get Brand By Id
        /// <summary>
        /// Get specific Brand By Brand Id.
        /// </summary>
        /// <param name="id">
        ///  Id of Brand.
        /// </param>
        /// <returns>
        /// An Object contains brand's information.
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
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.MBKC_Admin, PermissionAuthorizeConstant.Brand_Manager)]
        [HttpGet(APIEndPointConstant.Brand.BrandEndpoint)]
        public async Task<IActionResult> GetBrandByIdAsync([FromRoute] int id)
        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            var data = await this._brandService.GetBrandByIdAsync(id, claims);
            return Ok(data);
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
        [Consumes(MediaTypeConstant.Multipart_Form_Data)]
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.MBKC_Admin)]
        [HttpPost(APIEndPointConstant.Brand.BrandsEndpoint)]
        public async Task<IActionResult> PostCreateBrandAsync([FromForm] PostBrandRequest postBrandRequest)
        {
            ValidationResult validationResult = await _postBrandRequest.ValidateAsync(postBrandRequest);
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
        /// <param name="id">
        /// Brand's id for update brand.
        /// </param>
        ///  <param name="updateBrandRequest">
        /// A success message about updating brand information.
        ///  </param>
        /// <returns>
        /// An Object will return BrandId, Name, Address, Logo and Status.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         PUT
        ///         id = 3
        ///         Name = MyBrand
        ///         Address = 123 Main St
        ///         Status = INACTIVE | ACTIVE
        ///         Logo = [Image File]
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
        [Consumes(MediaTypeConstant.Multipart_Form_Data)]
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.MBKC_Admin)]
        [HttpPut(APIEndPointConstant.Brand.BrandEndpoint)]
        public async Task<IActionResult> UpdateBrandAsync([FromRoute] int id, [FromForm] UpdateBrandRequest updateBrandRequest)
        {
            ValidationResult validationResult = await _updateBrandRequest.ValidateAsync(updateBrandRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            await this._brandService.UpdateBrandAsync(id, updateBrandRequest);
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
        /// <param name="id">
        /// Brand's id for update brand.
        /// </param>
        ///  <param name="updateBrandStatusRequest">
        /// An Object includes status for updating brand.
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
        [Consumes(MediaTypeConstant.Application_Json)]
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.MBKC_Admin)]
        [HttpPut(APIEndPointConstant.Brand.UpdatingStatusBrand)]
        public async Task<IActionResult> UpdateBrandStatusAsync([FromRoute] int id, [FromBody] UpdateBrandStatusRequest updateBrandStatusRequest)
        {
            ValidationResult validationResult = await this._updateBrandStatusRequest.ValidateAsync(updateBrandStatusRequest);
            if(validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            await this._brandService.UpdateBrandStatusAsync(id, updateBrandStatusRequest);
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
        /// <param name="id">
        /// Brand's id for update brand.
        /// </param>
        ///  <param name="updateBrandProfileRequest">
        /// A success message about updating brand profile.
        ///  </param>
        /// <returns>
        /// An success message about updating brand's profile
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         PUT
        ///         id = 3
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
        [Consumes(MediaTypeConstant.Multipart_Form_Data)]
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Brand_Manager)]
        [HttpPut(APIEndPointConstant.Brand.UpdatingProfileBrand)]
        public async Task<IActionResult> UpdateBrandProfileAsync([FromRoute] int id, [FromForm] UpdateBrandProfileRequest updateBrandProfileRequest)
        {
            ValidationResult validationResult = await this._updateBrandProfileRequest.ValidateAsync(updateBrandProfileRequest);
            if(validationResult.IsValid == false)
            {
                var errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._brandService.UpdateBrandProfileAsync(id, updateBrandProfileRequest, claims);
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
        /// <param name="id">
        ///  Id of brand.
        /// </param>
        /// <returns>
        /// An object will return message "Deleted brand successfully".
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         DELETE
        ///         {
        ///             "id": 3
        ///         }
        /// </remarks>
        /// <response code="200">Deactive brand successfully.</response>
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
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.MBKC_Admin)]
        [HttpDelete(APIEndPointConstant.Brand.BrandEndpoint)]
        public async Task<IActionResult> DeActiveBrandByIdAsync([FromRoute] int id)
        {
            await this._brandService.DeActiveBrandByIdAsync(id);
            return Ok(new
            {
                Message = MessageConstant.BrandMessage.DeletedBrandSuccessfully
            });
        }
        #endregion

    }
}
