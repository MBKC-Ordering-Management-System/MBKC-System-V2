using FluentValidation;
using FluentValidation.Results;
using MBKC.BAL.Authorization;
using MBKC.BAL.DTOs;
using MBKC.BAL.DTOs.Brands;
using MBKC.BAL.DTOs.FireBase;
using MBKC.BAL.DTOs.Verifications;
using MBKC.BAL.Errors;
using MBKC.BAL.Exceptions;
using MBKC.BAL.Services.Interfaces;
using MBKC.BAL.Utils;
using MBKC.BAL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private IBrandService _brandService;
        private IOptions<FireBaseImage> _firebaseImageOptions;
        private IValidator<PostBrandRequest> _postBrandRequest;
        private IValidator<UpdateBrandRequest> _updateBrandRequest;
        private IOptions<Email> _emailOption;
        public BrandsController(IBrandService brandService,
            IOptions<FireBaseImage> firebaseImageOptions,
            IValidator<PostBrandRequest> postBrandRequest,
            IOptions<Email> emailOption,
            IValidator<UpdateBrandRequest> updateBrandRequest)
        {
            this._brandService = brandService;
            this._firebaseImageOptions = firebaseImageOptions;
            this._postBrandRequest = postBrandRequest;
            this._updateBrandRequest = updateBrandRequest;
            this._emailOption = emailOption;
        }
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
        ///         {
        ///             "Name": "MyBrand"
        ///             "Address": "123 Main St"
        ///             "ManagerEmail": "manager@gmail.com"
        ///             Logo: [Upload a logo file] 
        ///         }
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
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [HttpPost]
        [PermissionAuthorize("MBKC Admin")]
        public async Task<IActionResult> PostCreateBrandAsync([FromForm] PostBrandRequest postBrandRequest)
        {
            ValidationResult validationResult = await _postBrandRequest.ValidateAsync(postBrandRequest);
            if (!validationResult.IsValid)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            await this._brandService.CreateBrandAsync(postBrandRequest, _firebaseImageOptions.Value, _emailOption.Value);
            return Ok(new
            {
                Message = "Created New Brand Successfully."
            });
        }
        #endregion

        #region Update Brand
        /// <summary>
        ///  Update brand.
        /// </summary>
        /// <param name="id">
        /// Brand's id for update brand.
        /// </param>
        ///  <param name="updateBrandRequest">
        /// Object include information for update brand.
        ///  </param>
        /// <returns>
        /// An Object will return BrandId, Name, Address, Logo and Status.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         PUT
        ///         {
        ///             "id": 3
        ///             "Name": "MyBrand"
        ///             "Address": "123 Main St"
        ///             "Status": INACTIVE
        ///             "Logo": [Upload a logo file] 
        ///         }
        /// </remarks>
        /// <response code="200">Update Brand Successfully.</response>
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
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [HttpPut("{id}")]
        [PermissionAuthorize("MBKC Admin")]
        public async Task<IActionResult> UpdateBrandAsync([FromRoute] int id, [FromForm] UpdateBrandRequest updateBrandRequest)
        {
            ValidationResult validationResult = await _updateBrandRequest.ValidateAsync(updateBrandRequest);
            if (!validationResult.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(error);
            }
            await this._brandService.UpdateBrandAsync(id, updateBrandRequest, _firebaseImageOptions.Value);
            return Ok(new
            {
                Message = "Update Brand Successfully."
            });
        }
        #endregion

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
        /// <param name="pageNumber">
        /// The current page the user wants to get next items.
        /// </param>
        /// <param name="pageSize">
        /// number of elements on a page.
        /// </param>
        /// <returns>
        /// A list of brands contains TotalItems, TotalPages, Brands' information
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         {    
        ///             "keySearchName": "HighLand Coffee"
        ///             "keyStatusFilter": "ACTIVE"
        ///             "pageSize": 5
        ///             "pageNumber": 1
        ///         }
        /// </remarks>
        /// <response code="200">Get brands Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetBrandsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [HttpGet]
        [PermissionAuthorize("MBKC Admin")]
        public async Task<IActionResult> GetBrandsAsync([FromQuery] string? keySearchName, [FromQuery] string? keyStatusFilter, [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var data = await this._brandService.GetBrandsAsync(keySearchName, keyStatusFilter, pageNumber, pageSize);

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
        ///         {
        ///             "id": 3
        ///         }
        ///         
        /// </remarks>
        /// <response code="200">Get brand Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetBrandResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [HttpGet("{id}")]
        [PermissionAuthorize("MBKC Admin")]
        public async Task<IActionResult> GetBrandByIdAsync([FromRoute] int id)
        {
            var data = await this._brandService.GetBrandByIdAsync(id);
            return Ok(data);
        }
        #endregion

        #region Deactive Brand By Id
        /// <summary>
        /// Deactive brand by id.
        /// </summary>
        /// <param name="id">
        ///  Id of brand.
        /// </param>
        /// <returns>
        /// An object will return message "Deactive brand successfully".
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
        [Produces("application/json")]
        [HttpDelete("{id}")]
        [PermissionAuthorize("MBKC Admin")]
        public async Task<IActionResult> DeActiveBrandByIdAsync([FromRoute] int id)
        {
            await this._brandService.DeActiveBrandByIdAsync(id);
            return Ok(new
            {
                Message = "Deactive brand successfully."
            });
        }
        #endregion

    }
}
