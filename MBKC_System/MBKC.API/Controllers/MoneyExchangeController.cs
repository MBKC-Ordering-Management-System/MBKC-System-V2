using FluentValidation;
using MBKC.API.Constants;
using MBKC.Service.Authorization;
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
        public MoneyExchangeController
        (
            IMoneyExchangeService moneyExchangeService
        )
        {
            this._moneyExchangeService = moneyExchangeService;

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
        [Produces(MediaTypeConstant.Application_Json)]
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

        #region money exchange to store
        [PermissionAuthorize(PermissionAuthorizeConstant.Kitchen_Center_Manager)]
        [HttpPut(APIEndPointConstant.MoneyExchange.MoneyExchangeToStore)]
        public async Task<IActionResult> MoneyExchangeToStoreAsync()
        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._moneyExchangeService.MoneyExchangeToStoreAsync(claims);
            return Ok(new
            {
                Message = "Success"
            });
        }
        #endregion
    }
}
