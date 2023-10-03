using FluentValidation;
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
    [Route("api/[controller]")]
    [ApiController]
    public class PartnersController : ControllerBase
    {
        private IPartnerService _partnerService;
        private IValidator<PostPartnerRequest> _postPartnerRequest;
        private IValidator<UpdatePartnerRequest> _updatePartnerRequest;
        public PartnersController(IPartnerService partnerService, IValidator<PostPartnerRequest> postPartnerRequest, IValidator<UpdatePartnerRequest> updatePartnerRequest)
        {
            this._partnerService = partnerService;
            this._postPartnerRequest = postPartnerRequest;
            this._updatePartnerRequest = updatePartnerRequest;
        }
        #region Create Partner
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
        [Consumes(MediaTypeConstant.Multipart_Form_Data)]
        [Produces(MediaTypeConstant.Application_Json)]
        /*[PermissionAuthorize(PermissionAuthorizeConstant.MBKC_Admin)]*/
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
                Message = MessageConstant.BrandMessage.CreatedNewBrandSuccessfully
            });
        }
        #endregion

        #region Update Existed Partner
        /// <summary>
        ///  Update an existed partner information.
        /// </summary>
        /// <param name="id">
        /// Partner's id for update partner.
        /// </param>
        ///  <param name="updateBrandRequest">
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
        public async Task<IActionResult> UpdatePartnerAsync([FromRoute] int id, [FromForm] UpdatePartnerRequest updatePartnerRequest)
        {
            ValidationResult validationResult = await _updatePartnerRequest.ValidateAsync(updatePartnerRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            await this._partnerService.(id, updatePartnerRequest);
            return Ok(new
            {
                Message = MessageConstant.BrandMessage.UpdatedBrandSuccessfully
            });
        }
        #endregion
    }
}
