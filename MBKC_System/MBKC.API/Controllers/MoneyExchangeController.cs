using FluentValidation;
using FluentValidation.Results;
using MBKC.API.Constants;
using MBKC.Service.Authorization;
using MBKC.Service.DTOs.MoneyExchanges;
using MBKC.Service.DTOs.Orders;
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
    public class MoneyExchangeController : ControllerBase
    {
        private IMoneyExchangeService _moneyExchangeService;
        private IValidator<WithdrawMoneyRequest> _withdrawMoneyRequest;
        public MoneyExchangeController
        (
            IMoneyExchangeService moneyExchangeService,
            IValidator<WithdrawMoneyRequest> withdrawMoneyRequest

        )
        {
            this._moneyExchangeService = moneyExchangeService;
            this._withdrawMoneyRequest = withdrawMoneyRequest;
        }

        #region money exchange to kitchen center
        /// <summary>
        ///  Send money back to kitchen center.
        /// </summary>
        /// <returns>
        /// A success message about sent money.
        /// </returns>
        /// <response code="200">Sent money successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Cashier)]
        [HttpPut(APIEndPointConstant.MoneyExchange.MoneyExchangeToKitchenCenter)]
        public async Task<IActionResult> MoneyExchangeToKitchenCenterAsync()
        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._moneyExchangeService.MoneyExchangeToKitchenCenterAsync(claims);
            return Ok(new
            {
                Message = MessageConstant.MoneyExchangeMessage.MoneyExchangeToKitchenCenterSuccessfully
            });
        }
        #endregion

        #region withdraw money for store
        /// <summary>
        ///  Withdraw money for store.
        /// </summary>
        /// <param name="withdrawMoneyRequest">
        /// An object includes information withdraw money for store.  
        /// </param>
        /// <returns>
        /// A success message about withdraw money.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         POST
        ///           StoreId: 1,
        ///           Amount: 50000,
        ///           Image: ...
        /// </remarks>
        /// <response code="200">Withdraw money successfully.</response>
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
        [Consumes(MediaTypeConstant.MultipartFormData)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.KitchenCenterManager)]
        [HttpPost(APIEndPointConstant.MoneyExchange.WithdrawMoneyToStore)]
        public async Task<IActionResult> WithdrawMoneyToStoreAsync([FromForm] WithdrawMoneyRequest withdrawMoneyRequest)
        {
            ValidationResult validationResult = await this._withdrawMoneyRequest.ValidateAsync(withdrawMoneyRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._moneyExchangeService.WithdrawMoneyAsync(claims, withdrawMoneyRequest);
            return Ok(new
            {
                Message = MessageConstant.MoneyExchangeMessage.WithdrawMoneySuccessfully
            });
        }
        #endregion
    }
}
