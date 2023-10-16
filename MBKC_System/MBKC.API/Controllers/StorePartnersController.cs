using FluentValidation;
using FluentValidation.Results;
using MBKC.API.Constants;
using MBKC.Service.Authorization;
using MBKC.Service.DTOs.StorePartners;
using MBKC.Service.Errors;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MBKC.API.Controllers
{
    [ApiController]
    public class StorePartnersController : ControllerBase
    {
        private IStorePartnerService _storePartnerService;
        private IValidator<PostStorePartnerRequest> _postStorePartnerRequest;
        private IValidator<UpdateStorePartnerRequest> _updateStorePartnerRequest;
        private IValidator<UpdateStorePartnerStatusRequest> _updateStorePartnerStatusRequest;
        public StorePartnersController(IStorePartnerService storePartnerService,
             IValidator<UpdateStorePartnerRequest> updateStorePartnerRequest,
             IValidator<UpdateStorePartnerStatusRequest> updateStorePartnerStatusRequest,
            IValidator<PostStorePartnerRequest> postStorePartnerRequest)
        {
            this._storePartnerService = storePartnerService;
            this._postStorePartnerRequest = postStorePartnerRequest;
            this._updateStorePartnerRequest = updateStorePartnerRequest;
            this._updateStorePartnerStatusRequest = updateStorePartnerStatusRequest;
        }

        #region Create store partner
        /// <summary>
        ///  Create new store partner.
        /// </summary>
        /// <param name="postStorePartnerRequest">
        /// An object includes information about store partner. 
        /// </param>
        /// <returns>
        /// A success message about creating new store partner.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         POST
        ///         {
        ///             "storeId" = 1,
        ///             "partnerId" = 1,
        ///             "userName" = "passsio_68"
        ///             "password" =  "12345678"
        ///         }
        /// </remarks>
        /// <response code="200">Created new store partner successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager)]
        [HttpPost(APIEndPointConstant.StorePartner.StorePartnersEndpoint)]
        public async Task<IActionResult> CreateStorePartnerAsync([FromBody] PostStorePartnerRequest postStorePartnerRequest)
        {
            ValidationResult validationResult = await _postStorePartnerRequest.ValidateAsync(postStorePartnerRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._storePartnerService.CreateStorePartnerAsync(postStorePartnerRequest, claims);
            return Ok(new
            {
                Message = MessageConstant.StorePartnerMessage.CreatedNewStorePartnerSuccessfully
            });
        }
        #endregion

        #region Get store partners
        /// <summary>
        /// Get StorePartners in the system.
        /// </summary>
        /// <param name="searchName">The name of product that user wants to find out.</param>
        /// <param name="currentPage">The number of page</param>
        /// <param name="itemsPerPage">The number of records that user wants to get.</param>
        /// <returns>
        /// A list of store partners with requested conditions.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET 
        ///         searchName = Beamin
        ///         currentPage = 1
        ///         itemsPerPage = 5
        /// </remarks>
        /// <response code="200">Get list of store partners successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetStorePartnersResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager)]
        [HttpGet(APIEndPointConstant.StorePartner.StorePartnersEndpoint)]
        public async Task<IActionResult> GetProductsAsync([FromQuery] string? searchName, [FromQuery] int? currentPage, [FromQuery] int? itemsPerPage)

        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            GetStorePartnersResponse getStorePartnersResponse = await this._storePartnerService.GetStorePartnersAsync(searchName, currentPage, itemsPerPage, claims);
            return Ok(getStorePartnersResponse);
        }
        #endregion

        #region Get a specific store partner
        /// <summary>
        /// Get a specific store partner by id.
        /// </summary>
        /// <param name="storeId">The store partner's id.</param>
        /// <param name="partnerId">The store partner's id.</param>
        /// <returns>
        /// An object contains the store partner information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET 
        ///         storeId = 1
        ///         partnerId = 2
        /// </remarks>
        /// <response code="200">Get a specific store partner successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetStorePartnerResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager)]
        [HttpGet(APIEndPointConstant.StorePartner.StorePartnerEndpoint)]
        public async Task<IActionResult> GetStorePartnerAsync([FromRoute] int storeId, [FromRoute] int partnerId)
        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            GetStorePartnerResponse getStorePartnerResponse = await this._storePartnerService.GetStorePartnerAsync(storeId, partnerId, claims);
            return Ok(getStorePartnerResponse);
        }
        #endregion

        #region Update store partner information
        /// <summary>
        /// Update a specific store partner information.
        /// </summary>
        /// <param name="storeId">The store partner's id.</param>
        /// <param name="partnerId">The store partner's id.</param>
        /// <param name="updateStorePartnerRequest">The object contains updating store partner information.</param>
        /// <returns>
        /// A success message about updating specific  store partner information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         PUT 
        ///         storeId = 1
        ///         partnerId = 1
        ///         UserName = passio_q9
        ///         Password = 12345678
        ///         Status = ACTIVE | INACTIVE
        /// </remarks>
        /// <response code="200">Updated store partner successfully.</response>
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
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager)]
        [HttpPut(APIEndPointConstant.StorePartner.StorePartnerEndpoint)]
        public async Task<IActionResult> PutUpdateStorePartnerAsync([FromRoute] int storeId, [FromRoute] int partnerId, [FromBody] UpdateStorePartnerRequest updateStorePartnerRequest)
        {
            ValidationResult validationResult = await this._updateStorePartnerRequest.ValidateAsync(updateStorePartnerRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._storePartnerService.UpdateStorePartnerRequestAsync(storeId, partnerId, updateStorePartnerRequest, claims);
            return Ok(new
            {
                Message = MessageConstant.StorePartnerMessage.UpdatedStorePartnerSuccessfully
            });
        }
        #endregion

        #region Update store partner status
        /// <summary>
        /// Update stasus of store partner
        /// </summary>
        /// <param name="storeId">The store partner's id.</param>
        /// <param name="partnerId">The store partner's id.</param>
        /// <param name="updateStorePartnerStatusRequest">The store partner's status.</param>
        /// <returns>
        /// A success message about updating specific  store partner information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         PUT 
        ///         Status = ACTIVE | INACTIVE
        /// </remarks>
        /// <response code="200">Updated store partner status successfully.</response>
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
        [Consumes(MediaTypeConstant.ApplicationJson)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager)]
        [HttpPut(APIEndPointConstant.StorePartner.UpdatingStatusStorePartnerEndpoint)]
        public async Task<IActionResult> PutUpdateStorePartnerStatusAsync([FromRoute] int storeId, [FromRoute] int partnerId, [FromBody] UpdateStorePartnerStatusRequest updateStorePartnerStatusRequest)
        {
            ValidationResult validationResult = await this._updateStorePartnerStatusRequest.ValidateAsync(updateStorePartnerStatusRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._storePartnerService.UpdateStatusStorePartnerAsync(storeId, partnerId, updateStorePartnerStatusRequest, claims);
            return Ok(new
            {
                Message = MessageConstant.StorePartnerMessage.UpdatedStatusStorePartnerSuccessfully
            });
        }
        #endregion

        #region Delete a store partner
        /// <summary>
        /// Delete a specific store partner.
        /// </summary>
        /// <param name="storeId">The store partner's id.</param>
        /// <param name="partnerId">The store partner's id.</param>
        /// <returns>
        /// A success message about deleting specific store partner.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         DELETE
        ///         id= 1
        /// </remarks>
        /// <response code="200">Deleted store partner successfully.</response>
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
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager)]
        [HttpDelete(APIEndPointConstant.StorePartner.StorePartnerEndpoint)]
        public async Task<IActionResult> DeleteStorePartnerAsync([FromRoute] int storeId, [FromRoute] int partnerId)
        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._storePartnerService.DeleteStorePartnerAsync(storeId, partnerId, claims);
            return Ok(new
            {
                Message = MessageConstant.StorePartnerMessage.DeletedStorePartnerSuccessfully
            });
        }
        #endregion

        #region Get store partner information by store Id
        /// <summary>
        /// Get a store partner information by store Id.
        /// </summary>
        /// <param name="storeId">The store's id.</param>
        /// <returns>
        /// An object contains the store partner, partner, kitchen center information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET 
        ///         storeId = 1
        /// </remarks>
        /// <response code="200">Get store partner by store Id successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetStorePartnerInformationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager)]
        [HttpGet(APIEndPointConstant.StorePartner.PartnerInformationEndpoint)]
        public async Task<IActionResult> GetStorePartnerInformationByIdAsync([FromRoute] int storeId)
        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            var getStorePartnerResponse = await this._storePartnerService.GetPartnerInformationAsync(storeId, claims);
            return Ok(getStorePartnerResponse);
        }
        #endregion
    }
}
