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
        private IHangfireService _hangfireService;
        private IMoneyExchangeService _moneyExchangeService;
        private IValidator<WithdrawMoneyRequest> _withdrawMoneyValidator;
        private IValidator<UpdateCronJobRequest> _updateCronJobValidator;
        public MoneyExchangeController
        (
            IHangfireService hangfireService,
            IMoneyExchangeService moneyExchangeService,
            IValidator<WithdrawMoneyRequest> withdrawMoneyValidator,
            IValidator<UpdateCronJobRequest> updateCronJobValidator

        )
        {
            this._hangfireService = hangfireService;
            this._moneyExchangeService = moneyExchangeService;
            this._withdrawMoneyValidator = withdrawMoneyValidator;
            this._updateCronJobValidator = updateCronJobValidator;
        }

        #region update cron of money exchange
        /// <summary>
        ///  Update time to run background job.
        /// </summary>
        /// <param name="updateCronJobRequest">
        /// An object includes information update time.  
        /// </param>
        /// <returns>
        /// A success message update time.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         PUT
        ///         {
        ///           "jobId": "job_money_exchange_to_kitchen_center || job_money_exchange_to_store",
        ///           "hour": 22,
        ///           "minute": 0
        ///         }
        /// </remarks>
        /// <response code="200">Update cron job successfully.</response>
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
        [PermissionAuthorize(PermissionAuthorizeConstant.MBKCAdmin)]
        [HttpPut(APIEndPointConstant.MoneyExchange.UpdateSchedulingTimeJob)]
        public async Task<IActionResult> UpdateCronBackgroundJob([FromBody] UpdateCronJobRequest updateCronJobRequest)
        {
            ValidationResult validationResult = await this._updateCronJobValidator.ValidateAsync(updateCronJobRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            await this._hangfireService.UpdateCronAsync(updateCronJobRequest);
            return Ok(new
            {
                Message = MessageConstant.MoneyExchangeMessage.UpdateSchedulingTimeJob
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
            ValidationResult validationResult = await this._withdrawMoneyValidator.ValidateAsync(withdrawMoneyRequest);
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
