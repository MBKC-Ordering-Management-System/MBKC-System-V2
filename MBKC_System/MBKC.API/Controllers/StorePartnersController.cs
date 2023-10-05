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
        public StorePartnersController(IStorePartnerService storePartnerService,
             IValidator<UpdateStorePartnerRequest> updateStorePartnerRequest,
            IValidator<PostStorePartnerRequest> postStorePartnerRequest)
        {
            this._storePartnerService = storePartnerService;
            this._postStorePartnerRequest = postStorePartnerRequest;
            this._updateStorePartnerRequest = updateStorePartnerRequest;
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
        /// <response code="200">Created new brand successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Brand_Manager)]
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
        ///         searchValue = Beamin
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
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Brand_Manager)]
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
        ///         id = 1
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
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Brand_Manager)]
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
        /// <param name="updateProductRequest">The object contains updated product information.</param>
        /// <returns>
        /// A success message about updating specific  store partner information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         PUT 
        ///         Name = Bún đậu mắm tôm
        ///         Description = Bún đậu mắm tôm thơn ngon
        ///         SellingPrice = 50000
        ///         DiscountPrice = 0
        ///         HistoricalPrice = 0
        ///         Image = [File Image]
        ///         DisplayOrder = 1
        ///         ParentProductId = 1
        ///         CategoryId = 1
        ///         Status = ACTIVE | INACTIVE
        /// </remarks>
        /// <response code="200">Updated product successfully.</response>
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
        [Consumes(MediaTypeConstant.Multipart_Form_Data)]
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Brand_Manager)]
        [HttpPut(APIEndPointConstant.StorePartner.StorePartnerEndpoint)]
        public async Task<IActionResult> PutUpdateProductAsync([FromRoute] int storeId, [FromRoute] int partnerId, [FromForm] UpdateStorePartnerRequest updateStorePartnerRequest)
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
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Brand_Manager)]
        [HttpDelete(APIEndPointConstant.StorePartner.StorePartnerEndpoint)]
        public async Task<IActionResult> DeleteProductAsync([FromRoute] int storeId, [FromRoute] int partnerId)
        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._storePartnerService.DeleteStorePartnerAsync(storeId, partnerId, claims);
            return Ok(new
            {
                Message = MessageConstant.StorePartnerMessage.DeletedStorePartnerSuccessfully
            });
        }
        #endregion
    }
}
