﻿using FluentValidation;
using FluentValidation.Results;
using MBKC.API.Constants;
using MBKC.Service.Authorization;
using MBKC.Service.DTOs.Partners;
using MBKC.Service.Errors;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [ApiController]
    public class PartnersController : ControllerBase
    {
        private IPartnerService _partnerService;
        private IValidator<PostPartnerRequest> _postPartnerValidator;
        private IValidator<UpdatePartnerRequest> _updatePartnerValidator;
        private IValidator<GetPartnersRequest> _getPartnersValidator;
        private IValidator<PartnerRequest> _getPartnerValidator;
        public PartnersController(IPartnerService partnerService, 
            IValidator<PostPartnerRequest> postPartnerValidator, 
            IValidator<UpdatePartnerRequest> updatePartnerValidator,
            IValidator<PartnerRequest> getPartnerValidator,
            IValidator<GetPartnersRequest> getPartnersValidator)
        {
            this._partnerService = partnerService;
            this._postPartnerValidator = postPartnerValidator;
            this._updatePartnerValidator = updatePartnerValidator;
            this._getPartnersValidator = getPartnersValidator;
            this._getPartnerValidator = getPartnerValidator;
        }

        #region Get Partners
        /// <summary>
        ///  Get a list of partners from the system with condition paging, searchByName, sortByName, sortByStatus.
        /// </summary>
        /// <param name="keySearchName">
        ///  The brand name that the user wants to search.
        /// </param>
        /// <param name="keySortName">
        ///  Keywords when the user wants to sort by name ascending or descending(ASC or DESC).
        /// </param>
        /// <param name="keySortStatus">
        ///  Keywords when the user wants to sort by stasus ascending or descending(ASC or DESC).
        /// </param>
        /// <param name="currentPage">
        /// The current page the user wants to get next items.
        /// </param>
        /// <param name="itemsPerPage">
        /// number of elements on a page.
        /// </param>
        /// <param name="isGetAll">
        /// Input TRUE if you want to get all partners, ignoring pageNumber and pageSize, otherwise Input FALSE
        /// </param>
        /// <returns>
        /// A list of partners contains NumberItems, TotalPages, Partners' information
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         keySearchName = Shoppe Food
        ///         keySortName = ASC | DESC
        ///         keySortStatus = ASC | DESC
        ///         currentPage = 1
        ///         itemsPerPage = 5
        ///         isGetAll = true
        /// </remarks>
        /// <response code="200">Get brands Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetPartnersResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.MBKCAdmin, PermissionAuthorizeConstant.BrandManager)]
        [HttpGet(APIEndPointConstant.Partner.PartnersEndpoint)]
        public async Task<IActionResult> GetPartnersAsync([FromQuery] string? keySearchName, [FromQuery] string? keySortName, [FromQuery] string? keySortStatus, [FromQuery] int? currentPage, [FromQuery] int? itemsPerPage, [FromQuery] bool? isGetAll)
        {
            var data = await this._partnerService.GetPartnersAsync(keySearchName, keySortName, keySortStatus, currentPage, itemsPerPage, isGetAll);

            return Ok(data);
        }
        #endregion

        #region Get Partner By Id
        /// <summary>
        /// Get specific partner by partner id.
        /// </summary>
        /// <param name="id">
        ///  Id of parner.
        /// </param>
        /// <returns>
        /// An Object contains partner's information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         id = 3
        ///         
        /// </remarks>
        /// <response code="200">Get partner Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetPartnerResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.MBKCAdmin)]
        [HttpGet(APIEndPointConstant.Partner.PartnerEndpoint)]
        public async Task<IActionResult> GetPartnerByIdAsync([FromRoute] int id)
        {
            var data = await this._partnerService.GetPartnerByIdAsync(id);
            return Ok(data);
        }
        #endregion

        /*#region Create Partner
        /// <summary>
        ///  Create new partner.
        /// </summary>
        /// <param name="postPartnerRequest">
        /// An object includes information about partner. 
        /// </param>
        /// <returns>
        /// A success message about creating new partner.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         POST
        ///         Name = Grab
        ///         WebUrl = https://merchant.grab.com/portal
        ///         Logo =  [Image file]
        /// </remarks>
        /// <response code="200">Created new partner successfully.</response>
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
        [HttpPost(APIEndPointConstant.Partner.PartnersEndpoint)]
        public async Task<IActionResult> PostCreatePartnerAsync([FromForm] PostPartnerRequest postPartnerRequest)
        {
            ValidationResult validationResult = await _postPartnerRequest.ValidateAsync(postPartnerRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            await this._partnerService.CreatePartner(postPartnerRequest);
            return Ok(new
            {
                Message = MessageConstant.PartnerMessage.CreatedPartnerSuccessfully
            });
        }
        #endregion*/

        #region Get Partners
        /// <summary>
        ///  Get a list of partners from the system with condition paging, searchByName, sortByName, sortByStatus.
        /// </summary>
        /// <param name="getPartnersRequest">
        ///  An object include SearchValue, ItemsPerPage, CurrentPage, SortBy for search, sort, paging.
        /// </param>
        /// <returns>
        /// A list of partners contains NumberItems, TotalPages, Partners' information
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         searchValue = Grab Food
        ///         currentPage = 1
        ///         itemsPerPage = 5
        ///         sortBy = "propertyName_asc | propertyName_ASC | propertyName_desc | propertyName_DESC"
        /// </remarks>
        /// <response code="200">Get brands Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetPartnersResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.MBKCAdmin, PermissionAuthorizeConstant.BrandManager)]
        [HttpGet(APIEndPointConstant.Partner.PartnersEndpoint)]
        public async Task<IActionResult> GetPartnersAsync([FromQuery] GetPartnersRequest getPartnersRequest)
        {
            ValidationResult validationResult = await this._getPartnersValidator.ValidateAsync(getPartnersRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            var data = await this._partnerService.GetPartnersAsync(getPartnersRequest);

            return Ok(data);
        }
        #endregion

        #region Get Partner By Id
        /// <summary>
        /// Get specific partner by partner id.
        /// </summary>
        /// <param name="getPartnerRequest">
        ///  An object include partner id.
        /// </param>
        /// <returns>
        /// An Object contains partner's information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         id = 3
        ///         
        /// </remarks>
        /// <response code="200">Get partner Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetPartnerResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.MBKCAdmin)]
        [HttpGet(APIEndPointConstant.Partner.PartnerEndpoint)]
        public async Task<IActionResult> GetPartnerByIdAsync([FromQuery] PartnerRequest getPartnerRequest)
        {

            ValidationResult validationResult = await _getPartnerValidator.ValidateAsync(getPartnerRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            var data = await this._partnerService.GetPartnerByIdAsync(getPartnerRequest.Id);
            return Ok(data);
        }
        #endregion

        #region Update Existed Partner
        /// <summary>
        ///  Update an existed partner information.
        /// </summary>
        /// <param name="getPartnerRequest">
        /// An oject include partner id.
        /// </param>
        ///  <param name="updatePartnerRequest">
        /// A success message about updating partner information.
        ///  </param>
        /// <returns>
        /// Return message Update Partner Successfully.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         PUT
        ///         id = 3
        ///         Name = Grab
        ///         Logo = [Image File]
        ///         WebUrl = https://merchant.grab.com/portal
        ///         Status = INACTIVE | ACTIVE
        /// </remarks>
        /// <response code="200">Updated Existed Partner Successfully.</response>
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
        [HttpPut(APIEndPointConstant.Partner.PartnerEndpoint)]
        public async Task<IActionResult> UpdatePartnerAsync([FromRoute] PartnerRequest getPartnerRequest, [FromForm] UpdatePartnerRequest updatePartnerRequest)
        {
            ValidationResult validationResult = await _updatePartnerValidator.ValidateAsync(updatePartnerRequest);
            ValidationResult validationResultPartnerId = await _getPartnerValidator.ValidateAsync(getPartnerRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            if (validationResultPartnerId.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResultPartnerId);
                throw new BadRequestException(errors);
            }
            await this._partnerService.UpdatePartnerAsync(getPartnerRequest.Id, updatePartnerRequest);
            return Ok(new
            {
                Message = MessageConstant.PartnerMessage.UpdatedPartnerSuccessfully
            });
        }
        #endregion

        #region Delete existed Partner By Id
        /// <summary>
        /// Delete existed partner by id.
        /// </summary>
        /// <param name="getPartnerRequest">
        ///  An object include partner id.
        /// </param>
        /// <returns>
        /// A success message about deleting existed partner.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         DELETE
        ///             id = 3
        ///             "id": 3
        ///         }
        /// </remarks>
        /// <response code="200">Delete partner successfully.</response>
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
        [HttpDelete(APIEndPointConstant.Partner.PartnerEndpoint)]
        public async Task<IActionResult> DeActivePartnerByIdAsync([FromRoute] PartnerRequest getPartnerRequest)
        {
            ValidationResult validationResult = await _getPartnerValidator.ValidateAsync(getPartnerRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            await this._partnerService.DeActivePartnerByIdAsync(getPartnerRequest.Id);
            return Ok(new
            {
                Message = MessageConstant.PartnerMessage.DeletedPartnerSuccessfully
            });
        }
        #endregion
    }
}
