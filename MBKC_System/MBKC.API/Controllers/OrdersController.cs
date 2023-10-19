using FluentValidation;
using FluentValidation.Results;
using MBKC.API.Constants;
using MBKC.Service.Authorization;
using MBKC.Service.DTOs.Brands;
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
    public class OrdersController : ControllerBase
    {
        private IOrderService _orderService;
        private IValidator<ConfirmOrderToCompletedRequest> _confirmOrderToCompletedRequest;
        public OrdersController
        (
            IOrderService orderService, 
            IValidator<ConfirmOrderToCompletedRequest> confirmOrderToCompletedRequest
        )
        {
            this._orderService = orderService;
            this._confirmOrderToCompletedRequest = confirmOrderToCompletedRequest;
        }

        #region confirm order to completed
        /// <summary>
        ///  Confirm order status to completed.
        /// </summary>
        /// <param name="confirmOrderToCompletedRequest">
        /// An object includes information about change order status.  
        /// </param>
        /// <returns>
        /// A success message about changed order status.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         PUT
        ///         {
        ///           "orderPartnerId": "GRABFOOD",
        ///           "shipperPhone": "0987654321",
        ///           "bankingAccountId": 1
        ///         }
        /// </remarks>
        /// <response code="200">Changed order status successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.Application_Json)]
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Cashier)]
        [HttpPut(APIEndPointConstant.Order.ConfirmOrderToCompletedEndpoint)]
        public async Task<IActionResult> ConfirmOrderToCompletedAsync([FromBody] ConfirmOrderToCompletedRequest confirmOrderToCompletedRequest)
        {
            ValidationResult validationResult = await this._confirmOrderToCompletedRequest.ValidateAsync(confirmOrderToCompletedRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._orderService.ConfirmOrderToCompletedAsync(confirmOrderToCompletedRequest, claims);
            return Ok(new
            {
                Message = MessageConstant.OrderMessage.UpdateOrderSuccessfully
            });
        }
        #endregion

    }
}
