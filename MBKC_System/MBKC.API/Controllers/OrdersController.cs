using FluentValidation;
using FluentValidation.Results;
using MBKC.API.Constants;
using MBKC.Service.Authorization;
using MBKC.Service.DTOs.Brands;
using MBKC.Service.DTOs.Orders;
using MBKC.Service.DTOs.Orders.MBKC.Service.DTOs.Orders;
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
        private IValidator<ConfirmOrderToCompletedRequest> _confirmOrderToCompletedValidator;
        private IValidator<GetOrdersRequest> _getOrdersValidator;
        private IValidator<OrderRequest> _getOrderValidator;
        private IValidator<PutCancelOrderRequest> _putCancelOrderValidator;
        public OrdersController
        (
            IOrderService orderService,
            IValidator<ConfirmOrderToCompletedRequest> confirmOrderToCompletedValidator,
            IValidator<GetOrdersRequest> getOrdersValidator,
            IValidator<OrderRequest> getOrderValidator,
            IValidator<PutCancelOrderRequest> putCancelOrderValidator
        )
        {
            this._orderService = orderService;
            this._confirmOrderToCompletedValidator = confirmOrderToCompletedValidator;
            this._getOrdersValidator = getOrdersValidator;
            this._getOrderValidator = getOrderValidator;
            this._putCancelOrderValidator = putCancelOrderValidator;
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
        ///           OrderPartnerId: "GRABFOOD",
        ///           BankingAccountId: 1,
        ///           Image: ...
        ///         }
        /// </remarks>
        /// <response code="200">Changed order status successfully.</response>
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
        [PermissionAuthorize(PermissionAuthorizeConstant.Cashier)]
        [HttpPut(APIEndPointConstant.Order.ConfirmOrderToCompletedEndpoint)]
        public async Task<IActionResult> ConfirmOrderToCompletedAsync([FromForm] ConfirmOrderToCompletedRequest confirmOrderToCompletedRequest)
        {
            ValidationResult validationResult = await this._confirmOrderToCompletedValidator.ValidateAsync(confirmOrderToCompletedRequest);
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

        #region Get orders
        /// <summary>
        ///  Get all orders for a specified store or kitchen center.
        /// </summary>
        /// <param name="getOrdersRequest">
        /// An object include SearchValue,
        /// ItemsPerPage, CurrentPage, SortBy for sort, search and paging. 
        /// </param>
        /// <returns>
        ///List of orders for a specified store or kitchen center.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         SearchValue = Grab
        ///         CurrentPage = 1
        ///         ItemsPerPage = 5
        ///         SortBy = "propertyName_asc | propertyName_ASC | propertyName_desc | propertyName_DESC"
        /// </remarks>
        /// <response code="200">Get list order successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetOrdersResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.ApplicationJson)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Cashier, PermissionAuthorizeConstant.KitchenCenterManager, PermissionAuthorizeConstant.StoreManager)]
        [HttpGet(APIEndPointConstant.Order.OrdersEndpoint)]
        public async Task<IActionResult> GetOrdersAsync([FromQuery] GetOrdersRequest getOrdersRequest)
        {
            ValidationResult validationResult = await this._getOrdersValidator.ValidateAsync(getOrdersRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            var getOrderResponse = await this._orderService.GetOrdersAsync(getOrdersRequest, claims);
            return Ok(getOrderResponse);
        }
        #endregion

        #region Get order
        /// <summary>
        ///  Get order by id.
        /// </summary>
        /// <param name="getOrderRequest">
        /// An object include id of order.
        /// </param>
        /// <returns>
        /// An object include information about order.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         id = 1
        /// </remarks>
        /// <response code="200">Get order successfully.</response>
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
        [PermissionAuthorize(PermissionAuthorizeConstant.Cashier, PermissionAuthorizeConstant.KitchenCenterManager, PermissionAuthorizeConstant.StoreManager)]
        [HttpGet(APIEndPointConstant.Order.OrderEndpoint)]
        public async Task<IActionResult> GetOrderAsync([FromRoute] OrderRequest getOrderRequest)
        {
            ValidationResult validationResult = await this._getOrderValidator.ValidateAsync(getOrderRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            var getOrderResponse = await this._orderService.GetOrderAsync(getOrderRequest, claims);
            return Ok(getOrderResponse);
        }
        #endregion

        #region Change order status to Ready
        /// <summary>
        /// Change order status to ready.
        /// </summary>
        /// <param name="orderRequest">
        /// An object include id of order.
        /// </param>
        /// <returns>
        /// Success message about update order status to ready.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         id = 1
        /// </remarks>
        /// <response code="200">Update status to ready success.</response>
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
        [PermissionAuthorize(PermissionAuthorizeConstant.StoreManager)]
        [HttpPut(APIEndPointConstant.Order.ChangeOrderToReadyEndpoint)]
        public async Task<IActionResult> ChangeOrderStatusToReadyAsync([FromRoute] OrderRequest orderRequest)
        {
            ValidationResult validationResult = await this._getOrderValidator.ValidateAsync(orderRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._orderService.ChangeOrderStatusToReadyAsync(orderRequest, claims);
            return Ok(new
            {
                Message = MessageConstant.OrderMessage.UpdateOrderSuccessfully
            });
        }
        #endregion

        #region Change order status to ready delivery.
        /// <summary>
        /// Change order status to ready delivery.
        /// </summary>
        /// <param name="orderRequest">
        /// An object include id of order.
        /// </param>
        /// <returns>
        /// Success message about update order status to ready delivery.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         id = 1
        /// </remarks>
        /// <response code="200">Update status to ready delivery success.</response>
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
        [PermissionAuthorize(PermissionAuthorizeConstant.Cashier)]
        [HttpPut(APIEndPointConstant.Order.ChangeOrderToReadyDeliveryEndpoint)]
        public async Task<IActionResult> ChangeOrderStatusToReadyDeliveryAsync([FromRoute] OrderRequest orderRequest)
        {
            ValidationResult validationResult = await this._getOrderValidator.ValidateAsync(orderRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._orderService.ChangeOrderStatusToReadyDeliveryAsync(orderRequest, claims);
            return Ok(new
            {
                Message = MessageConstant.OrderMessage.UpdateOrderSuccessfully
            });
        }
        #endregion

        #region Cancel order
        /// <summary>
        /// Cancel order when order status is UPCOMING or PREPARING
        /// </summary>
        /// <param name="getOrderRequest">
        /// An object include id of order.
        /// </param>
        /// <returns>
        /// Success message about update order status to cancelled.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         id = 1
        /// </remarks>
        /// <response code="200">Update status to cancelled success.</response>
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
        [PermissionAuthorize(PermissionAuthorizeConstant.StoreManager)]
        [HttpPut(APIEndPointConstant.Order.CancelOrderEndpoint)]
        public async Task<IActionResult> CancelOrderAsync([FromRoute] OrderRequest getOrderRequest, [FromBody] PutCancelOrderRequest cancelOrderRequest)
        {
            ValidationResult validationResult = await this._getOrderValidator.ValidateAsync(getOrderRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            ValidationResult cancelOrderValidationResult = await this._putCancelOrderValidator.ValidateAsync(cancelOrderRequest);
            if (cancelOrderValidationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(cancelOrderValidationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._orderService.CancelOrderAsync(getOrderRequest, cancelOrderRequest, claims);
            return Ok(new
            {
                Message = MessageConstant.OrderMessage.CancelOrderSuccessfully
            });
        }
        #endregion
    }
}
