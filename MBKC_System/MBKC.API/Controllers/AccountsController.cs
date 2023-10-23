using FluentValidation;
using FluentValidation.Results;
using MBKC.API.Constants;
using MBKC.Service.Authorization;
using MBKC.Service.DTOs.Accounts;
using MBKC.Service.DTOs.Cashiers.Responses;
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
    public class AccountsController : ControllerBase
    {
        private IAccountService _accountService;
        private IValidator<AccountIdRequest> _accountIdValidator;
        private IValidator<UpdateAccountRequest> _updateAccountValidator;
        public AccountsController(IAccountService accountService, IValidator<AccountIdRequest> accountIdValidator,
            IValidator<UpdateAccountRequest> updateAccountValidator)
        {
            this._accountService = accountService;
            this._accountIdValidator = accountIdValidator;
            this._updateAccountValidator = updateAccountValidator;
        }

        #region Get Account Profile
        /// <summary>
        /// Get a specific Account by id.
        /// </summary>
        /// <param name="accountIdRequest">An object contains the account's id.</param>
        /// <returns>
        /// An object contains the Account information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET 
        ///         id = 1
        /// </remarks>
        /// <response code="200">Get a specific account successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetAccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Cashier, PermissionAuthorizeConstant.KitchenCenterManager, 
                             PermissionAuthorizeConstant.BrandManager, PermissionAuthorizeConstant.StoreManager)]
        [HttpGet(APIEndPointConstant.Account.AccountEndpoint)]
        public async Task<IActionResult> GetAccountAsync([FromRoute]AccountIdRequest accountIdRequest)
        {
            ValidationResult validationResult = await this._accountIdValidator.ValidateAsync(accountIdRequest);
            if(validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            GetAccountResponse GetAccountResponse = await this._accountService.GetAccountAsync(accountIdRequest.Id, claims);
            return Ok(GetAccountResponse);
        }
        #endregion

        #region Update Account Profile
        /// <summary>
        /// Update a specific account information.
        /// </summary>
        /// <param name="accountIdRequest">An object contains the account's id.</param>
        /// <param name="updateAccountRequest">The object contains updated cashier information.</param>
        /// <returns>
        /// A success message about updating specific acccount information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         PUT 
        ///         id = 1
        /// </remarks>
        /// <response code="200">Updated account successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [Consumes(MediaTypeConstant.ApplicationJson)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Cashier, PermissionAuthorizeConstant.KitchenCenterManager,
                             PermissionAuthorizeConstant.BrandManager, PermissionAuthorizeConstant.StoreManager)]
        [HttpPut(APIEndPointConstant.Account.AccountEndpoint)]
        public async Task<IActionResult> UpdateAccountAsync([FromRoute] AccountIdRequest accountIdRequest, [FromBody]UpdateAccountRequest updateAccountRequest)
        {
            ValidationResult validationResult = await this._accountIdValidator.ValidateAsync(accountIdRequest);
            ValidationResult validationResultUpdateAccount = await this._updateAccountValidator.ValidateAsync(updateAccountRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            if (validationResultUpdateAccount.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResultUpdateAccount);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._accountService.UpdateAccountAsync(accountIdRequest.Id, updateAccountRequest, claims);
            return Ok(new
            {
                Message = MessageConstant.AccountMessage.UpdateAccountSuccessfully
            });
        }
        #endregion
    }
}
