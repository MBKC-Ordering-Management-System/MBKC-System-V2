using FluentValidation;
using FluentValidation.Results;
using MBKC.API.Constants;
using MBKC.API.Validators.UserDevices;
using MBKC.Service.Authorization;
using MBKC.Service.Constants;
using MBKC.Service.DTOs.UserDevices;
using MBKC.Service.Errors;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MessageConstant = MBKC.API.Constants.MessageConstant;

namespace MBKC.API.Controllers
{
    [ApiController]
    public class UserDevicesController : ControllerBase
    {
        private IUserDevicceService _userDevicceService;
        private IValidator<CreateUserDeviceRequest> _createUserDeviceValidator;
        public UserDevicesController(IUserDevicceService userDevicceService, IValidator<CreateUserDeviceRequest> createUserDeviceValidator)
        {
            this._userDevicceService = userDevicceService;
            this._createUserDeviceValidator = createUserDeviceValidator;
        }

        #region Create new user device
        /// <summary>
        /// Create a new user device
        /// </summary>
        /// <param name="userDeviceRequest">An object contains the FCM token from firebase.</param>
        /// <returns>
        /// A success message about creating new user device
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         POST 
        ///         {
        ///             "FCMToken": "Example"
        ///         }
        /// </remarks>
        /// <response code="200">Create a new user device successfully.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(RoleConstant.Store_Manager)]
        [HttpPost(APIEndPointConstant.UserDevice.UserDevicesEndpoint)]
        public async Task<IActionResult> PostCreateUserDeviceAsync([FromBody]CreateUserDeviceRequest userDeviceRequest)
        {
            ValidationResult validationResult = await this._createUserDeviceValidator.ValidateAsync(userDeviceRequest);
            if(validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._userDevicceService.CreateUserDeviceAsync(userDeviceRequest, claims);
            return Ok(MessageConstant.UserDevice.CreatedUserDeviceSuccessfully);
        }
        #endregion
    }
}
