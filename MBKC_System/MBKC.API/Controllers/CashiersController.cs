using FluentValidation;
using FluentValidation.Results;
using MBKC.API.Constants;
using MBKC.API.Validators.Cashiers;
using MBKC.Service.Authorization;
using MBKC.Service.DTOs.Cashiers;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MBKC.API.Controllers
{
    [ApiController]
    public class CashiersController : ControllerBase
    {
        private ICashierService _cashierService;
        private IValidator<CreateCashierRequest> _createCashierValidator;
        private IValidator<UpdateCashierRequest> _updateCashierValidator;
        private IValidator<UpdateCashierStatusRequest> _updateCashierStatusValidator;
        private IValidator<GetCashiersRequest> _getCashiersValidator;
        public CashiersController(ICashierService cashierService, IValidator<CreateCashierRequest> createCashierValidator,
            IValidator<UpdateCashierRequest> updateCashierValidator, IValidator<UpdateCashierStatusRequest> updateCashierStatusValidator,
            IValidator<GetCashiersRequest> getCashiersValidator)
        {
            this._cashierService = cashierService;
            this._createCashierValidator = createCashierValidator;
            this._updateCashierValidator = updateCashierValidator;
            this._updateCashierStatusValidator = updateCashierStatusValidator;
            this._getCashiersValidator = getCashiersValidator;
        }

        [Produces(MediaTypeConstant.Application_Json)]
        //[PermissionAuthorize(PermissionAuthorizeConstant.Kitchen_Center_Manager)]
        [HttpGet(APIEndPointConstant.Cashier.CashiersEndpoint)]
        public async Task<IActionResult> GetCashiersAsync([FromQuery]GetCashiersRequest getCashiersRequest)
        {
            ValidationResult validationResult = await this._getCashiersValidator.ValidateAsync(getCashiersRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            GetCashiersResponse getCashiersResponse = await this._cashierService.GetCashiersAsync(getCashiersRequest, claims);
            return Ok(getCashiersResponse);
        }

        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Kitchen_Center_Manager)]
        [HttpGet(APIEndPointConstant.Cashier.CashierEndpoint)]
        public async Task<IActionResult> GetCashierAsync([FromRoute]int id)
        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            return Ok();
        }


        [Consumes(MediaTypeConstant.Multipart_Form_Data)]
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Kitchen_Center_Manager)]
        [HttpPost(APIEndPointConstant.Cashier.CashiersEndpoint)]
        public async Task<IActionResult> PostCreateCashierAsync([FromForm]CreateCashierRequest createCashierRequest)
        {
            ValidationResult validationResult = await this._createCashierValidator.ValidateAsync(createCashierRequest);
            if(validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._cashierService.CreateCashierAsync(createCashierRequest, claims);
            return Ok(new
            {
                Message = MessageConstant.CashierMessage.CreatedCashierSuccessfully
            });
        }

        [Consumes(MediaTypeConstant.Multipart_Form_Data)]
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Kitchen_Center_Manager)]
        [HttpPut(APIEndPointConstant.Cashier.CashierEndpoint)]
        public async Task<IActionResult> UpdateCashierAsync([FromRoute]int id, [FromForm]UpdateCashierRequest updateCashierRequest)
        {
            ValidationResult validationResult = await this._updateCashierValidator.ValidateAsync(updateCashierRequest);
            if(validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            return Ok(new
            {
                Message = MessageConstant.CashierMessage.UpdatedCashierSuccessfully
            });
        }

        [Consumes(MediaTypeConstant.Application_Json)]
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Kitchen_Center_Manager)]
        [HttpPut(APIEndPointConstant.Cashier.UpdatingCashierStatusEndpoint)]
        public async Task<IActionResult> UpdateCashierStatusAsync([FromRoute]int id, [FromBody]UpdateCashierStatusRequest updateCashierStatusRequest)
        {
            ValidationResult validationResult = await this._updateCashierStatusValidator.ValidateAsync(updateCashierStatusRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            return Ok(new
            {
                Message = MessageConstant.CashierMessage.UpdatedCashierStatusSuccessfully
            });
        }

        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Kitchen_Center_Manager)]
        [HttpDelete(APIEndPointConstant.Cashier.CashierEndpoint)]
        public async Task<IActionResult> DeleteCashierAsync([FromRoute]int id)
        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            return Ok(new
            {
                Message = MessageConstant.CashierMessage.DeletedCashierSuccessfully
            });
        }
    }
}
