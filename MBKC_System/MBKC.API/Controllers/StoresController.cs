using FluentValidation;
using FluentValidation.Results;
using MBKC.BAL.Authorization;
using MBKC.BAL.DTOs.Accounts;
using MBKC.BAL.DTOs.FireBase;
using MBKC.BAL.DTOs.JWTs;
using MBKC.BAL.DTOs.KitchenCenters;
using MBKC.BAL.DTOs.Stores;
using MBKC.BAL.DTOs.Verifications;
using MBKC.BAL.Errors;
using MBKC.BAL.Exceptions;
using MBKC.BAL.Services.Interfaces;
using MBKC.BAL.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoresController : ControllerBase
    {
        private IStoreService _storeService;
        private IOptions<FireBaseImage> _firebaseImageOption;
        private IOptions<Email> _emailOption;
        private IValidator<CreateStoreRequest> _createStoreValidator;
        private IValidator<UpdateStoreRequest> _updateStoreValidator;
        public StoresController(IStoreService storeService, IOptions<FireBaseImage> firebaseImageOption,
            IOptions<Email> emailOption, IValidator<CreateStoreRequest> createStoreValidator, 
            IValidator<UpdateStoreRequest> updateStoreValidator)
        {
            this._storeService = storeService;
            this._firebaseImageOption = firebaseImageOption;
            this._emailOption = emailOption;
            this._createStoreValidator = createStoreValidator;
            this._updateStoreValidator = updateStoreValidator;
        }

        #region Get Stores
        /// <summary>
        /// Get stores in the system.
        /// </summary>
        /// <param name="itemsPerPage">The number of items that will display on a page.</param>
        /// <param name="currentPage">The position of the page.</param>
        /// <param name="searchValue">The search value about store's name.</param>
        /// <returns>
        /// An object contains NumberItems, TotalPage, a list of stores.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET 
        ///         itemsPerPage = 5
        ///         currentPage = 1
        ///         searchValue = KFC Bình Thạnh
        /// </remarks>
        /// <response code="200">Get a list of stores Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetStoresResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [PermissionAuthorize("MBKC Admin")]
        [HttpGet]
        public async Task<IActionResult> GetStoresAync([FromQuery] int? itemsPerPage, [FromQuery] int? currentPage, [FromQuery] string? searchValue)
        {
            GetStoresResponse stores = await this._storeService.GetStoresAsync(searchValue, currentPage, itemsPerPage, null, null);
            return Ok(stores);
        }
        #endregion

        #region Get Store
        /// <summary>
        /// Get a specific store by store id.
        /// </summary>
        /// <param name="id">The store's id.</param>
        /// <returns>
        /// An object contains the store's information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET 
        ///         id = 1
        /// </remarks>
        /// <response code="200">Get a specific store by id Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetStoreResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [PermissionAuthorize("MBKC Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStoreAsync([FromRoute]int id)
        {

            GetStoreResponse store = await this._storeService.GetStoreAsync(id, null, null);
            return Ok(store);
        }
        #endregion

        #region Get Brand's Stores
        /// <summary>
        /// Get Brand's stores in the system.
        /// </summary>
        /// <param name="idBrand">The brand's id.</param>
        /// <param name="itemsPerPage">The number of items that will display on a page.</param>
        /// <param name="currentPage">The position of the page.</param>
        /// <param name="searchValue">The search value about store's name.</param>
        /// <returns>
        /// An object contains NumberItems, TotalPage, a list of stores.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET 
        ///         brandId = 1
        ///         itemsPerPage = 5
        ///         currentPage = 1
        ///         searchValue = KFC Bình Thạnh
        /// </remarks>
        /// <response code="200">Get a list of stores Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetStoresResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [PermissionAuthorize("Brand Manager", "MBKC Admin")]
        [HttpGet("/api/brand/{idBrand}/[controller]")]
        public async Task<IActionResult> GetBrandStoresAsync([FromRoute] int idBrand, [FromQuery] int? itemsPerPage, [FromQuery] int? currentPage, [FromQuery] string? searchValue)
        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            GetStoresResponse stores = await this._storeService.GetStoresAsync(searchValue, currentPage, itemsPerPage, idBrand, claims);
            return Ok(stores);
        }
        #endregion

        #region Get Brand's Store
        /// <summary>
        /// Get a specific store of a brand by store id and brand id.
        /// </summary>
        /// <param name="idBrand">The brand's id.</param>
        /// <param name="idStore">The store's id.</param>
        /// <returns>
        /// An object contains the store's information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET 
        ///         idBrand = 1
        ///         idStore = 1
        /// </remarks>
        /// <response code="200">Get a specific store of a brand by id Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetStoreResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [PermissionAuthorize("Brand Manager")]
        [HttpGet("/api/brand/{idBrand}/[controller]/{idStore}")]
        public async Task<IActionResult> GetBrandStoreAsync([FromRoute] int idBrand, [FromRoute] int idStore)
        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            GetStoreResponse store = await this._storeService.GetStoreAsync(idStore, idBrand, claims);
            return Ok(store);
        }
        #endregion

        #region Create New Store
        /// <summary>
        /// Create new store.
        /// </summary>
        /// <param name="storeRequest">A store object contains created information.</param>
        /// <returns>
        /// A success message about creating store information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         POST 
        ///         {
        ///             "Name": "Kitchen Center Example"
        ///             "Logo": [Imgage File]
        ///             "KitchenCenterId": 1
        ///             "BrandId": 1
        ///             "StoreManagerEmail": "abc@example.com"
        ///         }
        /// </remarks>
        /// <response code="200">Created new store successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [PermissionAuthorize("MBKC Admin")]
        [HttpPost]
        public async Task<IActionResult> PostCreateStoreAsync([FromForm] CreateStoreRequest storeRequest)
        {
            ValidationResult validationResult = await this._createStoreValidator.ValidateAsync(storeRequest);
            if(validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            await this._storeService.CreateStoreAsync(storeRequest, this._firebaseImageOption.Value, this._emailOption.Value);
            return Ok(new
            {
                Message = "Created New Store Successfully."
            });
        }
        #endregion

        #region Update Existed Store of A Brand
        /// <summary>
        /// Update information of an existed store of a brand.
        /// </summary>
        /// <param name="idBrand">The brand's id.</param>
        /// <param name="idStore">The store's id.</param>
        /// <param name="updateStoreRequest">An store object contains updated information.</param>
        /// <returns>
        /// A success message about updating store information.  
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         PUT 
        ///         idBrand = 1
        ///         idStore = 1
        ///         
        ///         {
        ///             "Name": "Kitchen Center Example"
        ///             "Status": "Active | Inactive"
        ///             "Logo": [Imgage File]
        ///             "StoreManagerEmail": "abc@example.com"
        ///         }
        /// </remarks>
        /// <response code="200">Updated a store of a brand successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [PermissionAuthorize("MBKC Admin", "Brand Manager")]
        [HttpPut("/api/brand/{idBrand}/[controller]/{idStore}")]
        public async Task<IActionResult> PutUpdateStoreAsync([FromRoute]int idBrand, [FromRoute]int idStore, [FromForm]UpdateStoreRequest updateStoreRequest)
        {
            ValidationResult validationResult = await this._updateStoreValidator.ValidateAsync(updateStoreRequest);
            if(validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._storeService.UpdateStoreAsync(idBrand, idStore, updateStoreRequest, this._firebaseImageOption.Value, this._emailOption.Value, claims);
            return Ok(new
            {
                Message = "Updated Store Information Successfully."
            });
        }
        #endregion

        #region Delete Existed Store of A Brand
        /// <summary>
        /// Delete an existed store of a brand.
        /// </summary>
        /// <param name="idBrand">The brand's id</param>
        /// <param name="idStore">The store's id</param>
        /// <returns>
        /// A success message about deleting exsited store of a brand.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         DELETE 
        ///         idBrand = 1
        ///         idStore = 1
        /// </remarks>
        /// <response code="200">Deleted a store of a brand successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [PermissionAuthorize("MBKC Admin")]
        [HttpDelete("/api/brand/{idBrand}/[controller]/{idStore}")]
        public async Task<IActionResult> DeleteStoreAsync([FromRoute] int idBrand, [FromRoute] int idStore)
        {
            await this._storeService.DeleteStoreAsync(idBrand, idStore);
            return Ok(new
            {
                Message = "Deleted Store Successfully."
            });
        }
        #endregion
    }
}
