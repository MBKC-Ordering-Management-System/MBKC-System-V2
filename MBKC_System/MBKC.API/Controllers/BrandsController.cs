using FluentValidation;
using FluentValidation.Results;
using MBKC.BAL.DTOs.Brands;
using MBKC.BAL.DTOs.FireBase;
using MBKC.BAL.DTOs.Verifications;
using MBKC.BAL.Errors;
using MBKC.BAL.Exceptions;
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
        /// MBKC admin create new brand for system
        /// </summary>
        /// <param name="postBrandRequest">
        /// Include information about brand for create new brand 
        /// </param>
        /// <returns>
        /// An Object will return BrandId, Name, Address, Logo and Status
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         POST
        ///         Name: MyBrand
        ///         Address: 123 Main St
        ///         ManagerEmail: manager@gmail.com
        ///         Logo: [Upload a logo file] 
        /// </remarks>
        /// <response code="200">Create brand Successfully.</response>
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
        [Produces("application/json")]
        [HttpPost]
        public async Task<IActionResult> CreateBrandAsync([FromForm] PostBrandRequest postBrandRequest)
        {
            ValidationResult validationResult = await _postBrandRequest.ValidateAsync(postBrandRequest);
            if (!validationResult.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(error);
            }
            var data = await this._brandService.CreateBrandAsync(postBrandRequest, _firebaseImageOptions.Value, _emailOption.Value);
            return Ok(new
            {
                Data = data
            });
        }
        #endregion

        #region Update Brand
        /// <summary>
        /// MBKC admin update brand for system
        /// </summary>
        /// <param name="id">
        /// brand's id for update brand 
        /// </param>
        /// /// <param name="updateBrandRequest">
        /// Object include Name, Address, Status, Logo
        ///     </param>
        /// <returns>
        /// An Object will return BrandId, Name, Address, Logo and Status
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         PUT
        ///         id: 3
        ///         Name: MyBrand
        ///         Address: 123 Main St
        ///         Status: false
        ///         Logo: [Upload a logo file] 
        /// </remarks>
        /// <response code="200">Update brand Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBrandAsync([FromRoute] int id, [FromForm] UpdateBrandRequest updateBrandRequest)
        {
            ValidationResult validationResult = await _updateBrandRequest.ValidateAsync(updateBrandRequest);
            if (!validationResult.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(error);
            }
            var data = await this._brandService.UpdateBrandAsync(id, updateBrandRequest, _firebaseImageOptions.Value);
            return Ok(new
            {
                Data = data
            });
        }
        #endregion

        #region Get Brands
        /// <summary>
        /// MBKC admin get Brands from the system and also paging, searchByName and filterByStatus
        /// </summary>
        /// <param name="searchBrandRequest">
        ///  Include KeySearchName, KeyStatusFilter, PAGE_SIZE, PAGE_NUMBER
        /// </param>
        /// <returns>
        /// An Object will return BrandId, Name, Address, Logo and Status
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         KeySearchName: HighLand Coffee
        ///         KeyStatusFilter: ACTIVE
        ///         PAGE_SIZE: 5
        ///         PAGE_NUMBER: 1
        /// </remarks>
        /// <response code="200">Get brands Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [HttpGet]
        public async Task<IActionResult> GetBrandsAsync([FromQuery] SearchBrandRequest? searchBrandRequest, [FromQuery] int? PAGE_NUMBER, [FromQuery] int? PAGE_SIZE)
        {
            var data = await this._brandService.GetBrandsAsync(searchBrandRequest, PAGE_NUMBER, PAGE_SIZE);

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
        /// <summary>
        /// MBKC admin get Brand By Brand Id
        /// </summary>
        /// <param name="id">
        ///  id of Brand
        /// </param>
        /// <returns>
        /// An Object will return BrandId, Name, Address, Logo and Status
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         id: 3
        /// </remarks>
        /// <response code="200">Get brand Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrandByIdAsync([FromRoute] int id)
        {
            var data = await this._brandService.GetBrandByIdAsync(id);
            return Ok(data);
        }
        #endregion

        #region Deactive Brand By Id
        /// <summary>
        /// MBKC admin Deactive brand by id
        /// </summary>
        /// <param name="id">
        ///  id of Brand
        /// </param>
        /// <returns>
        /// An Object will return Message "Deactive brand successfully"
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         DELETE
        ///         id: 3
        /// </remarks>
        /// <response code="200">Deactive brand successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeActiveBrandByIdAsync([FromRoute] int id)
        {
            await this._brandService.DeActiveBrandByIdAsync(id);
            return Ok(new
            {
                Message = "Deactive brand successfully"
            });
        }
        #endregion

    }
}
