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
        private IValidator<UserDeviceIdRequest> _userDeviceIdValidator;
        public UserDevicesController(IUserDevicceService userDevicceService, IValidator<CreateUserDeviceRequest> createUserDeviceValidator, IValidator<UserDeviceIdRequest> userDeviceIdValidator)
        {
            this._userDevicceService = userDevicceService;
            this._createUserDeviceValidator = createUserDeviceValidator;
            this._userDeviceIdValidator = userDeviceIdValidator; 
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
            return Ok(new
            {
                Message = MessageConstant.UserDevice.CreatedUserDeviceSuccessfully
            });
        }
        #endregion

        #region Delete user device
        /// <summary>
        /// Delete an existed user device
        /// </summary>
        /// <param name="userDeviceIdRequest">An object contains user device id.</param>
        /// <returns>
        /// A success message about deleting user device
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         DELETE 
        ///         
        ///             UserDeviceId = 1
        ///         
        /// </remarks>
        /// <response code="200">Delete a user device successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
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
        #endregion
    }
}
