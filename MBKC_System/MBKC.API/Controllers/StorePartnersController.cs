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

        #region Create StorePartner
        /// <summary>
        ///  Create new store partner.
        /// </summary>
        /// <param name="postBrandRequest">
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
        ///         
        ///         }
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
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Brand_Manager)]
        [HttpPost(APIEndPointConstant.StorePartner.StorePartnersEndpoint)]
        public async Task<IActionResult> PostCreateStorePartnerAsync([FromBody] PostStorePartnerRequest postStorePartnerRequest)
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
    }
}
