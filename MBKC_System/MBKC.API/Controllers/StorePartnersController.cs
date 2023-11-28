using FluentValidation;
using FluentValidation.Results;
using MBKC.API.Constants;
using MBKC.Service.Authorization;
using MBKC.Service.DTOs.StorePartners;
using MBKC.Service.DTOs.Stores;
using MBKC.Service.Errors;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MBKC.API.Controllers
{
    [ApiController]
    public class StorePartnersController : ControllerBase
    {
        private IStorePartnerService _storePartnerService;
        private IValidator<PostStorePartnerRequest> _postStorePartnerValidator;
        private IValidator<UpdateStorePartnerRequest> _updateStorePartnerValidator;
        private IValidator<UpdateStorePartnerStatusRequest> _updateStorePartnerStatusValidator;
        private IValidator<GetStorePartnersRequest> _getStorePartnersValidator;
        private IValidator<StorePartnerRequest> _getStorePartnerValidator;
        private IValidator<StoreRequest> _getStoreValidator;
        private IValidator<GetPartnerInformationRequest> _getPartnerInformationsValidator;
        public StorePartnersController(IStorePartnerService storePartnerService,
             IValidator<UpdateStorePartnerRequest> updateStorePartnerValidator,
             IValidator<UpdateStorePartnerStatusRequest> updateStorePartnerStatusValidator,
             IValidator<GetStorePartnersRequest> getStorePartnersValidator,
             IValidator<StorePartnerRequest> getStorePartnerValidator,
             IValidator<StoreRequest> getStoreValidator,
             IValidator<GetPartnerInformationRequest> getPartnerInformationsValidator,
             IValidator<PostStorePartnerRequest> postStorePartnerValidator)
        {
            this._storePartnerService = storePartnerService;
            this._postStorePartnerValidator = postStorePartnerValidator;
            this._updateStorePartnerValidator = updateStorePartnerValidator;
            this._updateStorePartnerStatusValidator = updateStorePartnerStatusValidator;
            this._getStorePartnersValidator = getStorePartnersValidator;
            this._getStorePartnerValidator = getStorePartnerValidator;
            this._getStoreValidator = getStoreValidator;
            this._getPartnerInformationsValidator = getPartnerInformationsValidator;
        }
        #region Get store partners
        /// <summary>
        /// Get StorePartners in the system.
        /// </summary>
        /// <param name="getStorePartnersRequest">An object incluce SearchValue, ItemsPerpage, CurrentPage, SortBy, IsGetAll for search, sort and paging.</param>
        /// <returns>
        /// A list of store partners.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET 
        ///         SearchValue = Grab Food
        ///         ItemsPerPage = 1
        ///         CurrentPage = 5
        ///         SortBy = "propertyName_asc | propertyName_ASC | propertyName_desc | propertyName_DESC"
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
        public async Task<IActionResult> GetStorePartnersAsync([FromQuery] GetStorePartnersRequest getStorePartnersRequest)
        {
            ValidationResult validationResult = await this._getStorePartnersValidator.ValidateAsync(getStorePartnersRequest);
            if (!validationResult.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(error);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            GetStorePartnersResponse getStorePartnersResponse = await this._storePartnerService.GetStorePartnersAsync(getStorePartnersRequest, claims);
            return Ok(getStorePartnersResponse);
        }
        #endregion

        #region Get store partner information by store Id
        /// <summary>
        /// Get a store partner information by store Id, sortByName, sortByStatus.
        /// </summary>
        /// <param name="getStoreRequest">An object include store id.</param>
        /// <param name="getPartnerInformationRequest">An object incluce KeySortName, KeySortStatus, KeySortCommission.</param>
        /// <returns>
        /// An object contains the store partner, partner, kitchen center information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET 
        ///         storeId = 1
        ///         keySortName = ASC | DESC
        ///         keySortStatus = ASC | DESC
        ///         keySortCommission = ASC | DESC
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
        public async Task<IActionResult> GetStorePartnerInformationByIdAsync([FromRoute] StoreRequest getStoreRequest, [FromQuery] GetPartnerInformationRequest getPartnerInformationRequest)
        {
            ValidationResult validationResultStoreId = await this._getStoreValidator.ValidateAsync(getStoreRequest);
            ValidationResult validationResult = await this._getPartnerInformationsValidator.ValidateAsync(getPartnerInformationRequest);
            if (!validationResultStoreId.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResultStoreId);
                throw new BadRequestException(error);
            }

            if (!validationResult.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(error);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            var getStorePartnerResponse = await this._storePartnerService.GetPartnerInformationAsync(getStoreRequest.Id, getPartnerInformationRequest, claims);
            return Ok(getStorePartnerResponse);
        }
        #endregion

        #region Create store partner
        /// <summary>
        ///  Create new store partners.
        /// </summary>
        /// <param name="postStorePartnerRequest">
        /// An object includes information about store partner. 
        /// </param>
        /// <returns>
        /// A success message about creating new store partners.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         POST
        ///         {
        ///             "storeId": 1,
        ///             "partnerAccounts":[
        ///                 {
        ///                     "PartnerId": 1,
        ///                     "UserName": "example",
        ///                     "Password": "********",
        ///                     "Commission": 0-100
        ///                 }
        ///             ],
        ///             "isMappingProducts": true|false
        ///         }
        /// </remarks>
        /// <response code="200">Created new store partners successfully.</response>
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
            if (postStorePartnerRequest == null)
            {
                string error = ErrorUtil.GetErrorString("Exception", "Request message body is not null.");
                throw new BadRequestException(error);
            }
            ValidationResult validationResult = await _postStorePartnerValidator.ValidateAsync(postStorePartnerRequest);
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

        #region Update store partner information
        /// <summary>
        /// Update a specific store partner information.
        /// </summary>
        /// <param name="getStorePartnerRequest">An object include partner id and store id.</param>
        /// <returns>
        /// A success message about updating specific  store partner information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         PUT
        ///         storeId = 1
        ///         partnerId = 1
        ///         {
        ///           "userName": "highlandq9",
        ///           "password": "12345678",
        ///           "status": "ACTIVE",
        ///           "commission": 10.5
        ///         }
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
        public async Task<IActionResult> PutUpdateStorePartnerAsync([FromRoute] StorePartnerRequest getStorePartnerRequest, [FromBody] UpdateStorePartnerRequest updateStorePartnerRequest)
        {

            ValidationResult validationResultStorePartnerId = await this._getStorePartnerValidator.ValidateAsync(getStorePartnerRequest);
            ValidationResult validationResult = await this._updateStorePartnerValidator.ValidateAsync(updateStorePartnerRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            if (!validationResultStorePartnerId.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResultStorePartnerId);
                throw new BadRequestException(error);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._storePartnerService.UpdateStorePartnerRequestAsync(getStorePartnerRequest.StoreId, getStorePartnerRequest.PartnerId, updateStorePartnerRequest, claims);
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
        /// <param name="getStorePartnerRequest">An object include partner id and store id.</param>
        /// <param name="updateStorePartnerStatusRequest">The store partner's status.</param>
        /// <returns>
        /// A success message about updating specific  store partner information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         PUT 
        ///         storeId = 1
        ///         partnerId = 1
        ///         {
        ///             status = ACTIVE | INACTIVE
        ///         }
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
        public async Task<IActionResult> PutUpdateStorePartnerStatusAsync([FromRoute] StorePartnerRequest getStorePartnerRequest, [FromBody] UpdateStorePartnerStatusRequest updateStorePartnerStatusRequest)
        {
            ValidationResult validationResultStorePartnerId = await this._getStorePartnerValidator.ValidateAsync(getStorePartnerRequest);
            ValidationResult validationResult = await this._updateStorePartnerStatusValidator.ValidateAsync(updateStorePartnerStatusRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            if (!validationResultStorePartnerId.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResultStorePartnerId);
                throw new BadRequestException(error);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._storePartnerService.UpdateStatusStorePartnerAsync(getStorePartnerRequest.StoreId, getStorePartnerRequest.PartnerId, updateStorePartnerStatusRequest, claims);
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
        /// <param name="getStorePartnerRequest">An object include store id and partner id.</param>
        /// <returns>
        /// A success message about deleting specific store partner.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         DELETE
        ///         storeId = 1
        ///         partnerId = 1
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
        public async Task<IActionResult> DeleteStorePartnerAsync([FromRoute] StorePartnerRequest getStorePartnerRequest)
        {
            ValidationResult validationResult = await this._getStorePartnerValidator.ValidateAsync(getStorePartnerRequest);
            if (!validationResult.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(error);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._storePartnerService.DeleteStorePartnerAsync(getStorePartnerRequest.StoreId, getStorePartnerRequest.PartnerId, claims);
            return Ok(new
            {
                Message = MessageConstant.StorePartnerMessage.DeletedStorePartnerSuccessfully
            });
        }
        #endregion
    }
}
