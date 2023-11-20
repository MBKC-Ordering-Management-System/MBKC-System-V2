using FluentValidation;
using FluentValidation.Results;
using MBKC.PrivateAPI.Constants;
using MBKC.PrivateAPI.Validators.UserDevices;
using MBKC.Service.DTOs.UserDevices;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.PrivateAPI.Controllers
{
    [ApiController]
    public class UserDevicesController : ControllerBase
    {
        private IUserDevicceService _userDevicceService;
        private IValidator<UserDeviceIdRequest> _userDeviceIdValidator;
        public UserDevicesController(IUserDevicceService userDevicceService, IValidator<UserDeviceIdRequest> userDeviceIdValidator)
        {
            this._userDevicceService = userDevicceService;
            this._userDeviceIdValidator = userDeviceIdValidator;
        }

        [HttpDelete(APIEndPointConstant.UserDevice.UserDeviceEndPoint)]
        public async Task<IActionResult> DeleteUserDeviceAsync([FromRoute] UserDeviceIdRequest userDeviceIdRequest)
        {
            ValidationResult validationResult = await this._userDeviceIdValidator.ValidateAsync(userDeviceIdRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            await this._userDevicceService.DeleteUserDeviceAsync(userDeviceIdRequest.UserDeviceId);

            return Ok(new
            {
                Message = MessageConstant.UserDevice.DeletedUserDeviceSuccessfully
            });
        }
    }
}
