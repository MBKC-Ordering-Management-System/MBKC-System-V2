using FluentValidation;
using FluentValidation.Results;
using MBKC.PrivateAPI.Constants;
using MBKC.Service.DTOs.Orders;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.PrivateAPI.Controllers
{
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private IOrderService _orderService;
        private IValidator<GetOrderRequest> _getOrderValidator;
        private IValidator<PostOrderRequest> _postOrderValidator;
        private IValidator<PutOrderIdRequest> _putOrderIdValidator;
        private IValidator<PutOrderRequest> _putOrderValidator;
        public OrdersController(IValidator<GetOrderRequest> getOrderValidator, IValidator<PostOrderRequest> postOrderValidator, 
            IValidator<PutOrderIdRequest> putOrderIdValidator, IValidator<PutOrderRequest> putOrderValidator, IOrderService orderService)
        {
            this._getOrderValidator = getOrderValidator;
            this._postOrderValidator = postOrderValidator;
            this._putOrderIdValidator = putOrderIdValidator;
            this._putOrderValidator = putOrderValidator;
            this._orderService = orderService;
        }

        [HttpGet(APIEndPointConstant.Order.OrderEndPoint)]
        public async Task<IActionResult> GetOrderAsync([FromRoute]GetOrderRequest getOrderRequest)
        {
            ValidationResult validationResult = await this._getOrderValidator.ValidateAsync(getOrderRequest);
            if(validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            GetOrderResponse getOrderResponse = await this._orderService.GetOrderAsync(getOrderRequest.Id);
            return Ok(getOrderResponse);
        }

        [HttpPost(APIEndPointConstant.Order.OrdersEndPoint)]
        public async Task<IActionResult> PostOrderAsync([FromBody]PostOrderRequest postOrderRequest)
        {
            ValidationResult validationResult = await this._postOrderValidator.ValidateAsync(postOrderRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            GetOrderResponse getOrderResponse = await this._orderService.CreateOrderAsync(postOrderRequest);
            return Ok(getOrderResponse);
        }

        [HttpPut(APIEndPointConstant.Order.OrderEndPoint)]
        public async Task<IActionResult> PutOrderAsync([FromRoute]PutOrderIdRequest putOrderIdRequest,[FromBody]PutOrderRequest putOrderRequest)
        {
            ValidationResult validationResult = await this._putOrderIdValidator.ValidateAsync(putOrderIdRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            
            ValidationResult validationResultPutOrderRequest = await this._putOrderValidator.ValidateAsync(putOrderRequest);
            if (validationResultPutOrderRequest.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResultPutOrderRequest);
                throw new BadRequestException(errors);
            }
            GetOrderResponse getOrderResponse = await this._orderService.UpdateOrderAsync(putOrderIdRequest, putOrderRequest);
            return Ok(getOrderResponse);
        }

    }
}
