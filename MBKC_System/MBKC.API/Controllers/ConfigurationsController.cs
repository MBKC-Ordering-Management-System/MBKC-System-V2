using FluentValidation;
using FluentValidation.Results;
using MBKC.API.Constants;
using MBKC.Service.Authorization;
using MBKC.Service.DTOs.Configurations;
using MBKC.Service.Errors;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [ApiController]
    public class ConfigurationsController : ControllerBase
    {
        private IConfigurationService _configurationService;
        private IValidator<PutConfigurationRequest> _putConfigurationValidator;
        public ConfigurationsController(IConfigurationService configurationService, IValidator<PutConfigurationRequest> putConfigurationValidator)
        {
            this._putConfigurationValidator = putConfigurationValidator;
            this._configurationService = configurationService;
        }

        #region Get System Configuration
        /// <summary>
        /// Get Configuration of System.
        /// </summary>
        /// <returns>
        /// An Object contains some configuration.
        /// </returns>
        /// <response code="200">Get System configuration successfully.</response>
        /// <response code="500">Some Error about the system.</response>
        [ProducesResponseType(typeof(GetConfigurationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.MBKCAdmin)]
        [HttpGet(APIEndPointConstant.Configuration.ConfigurationsEndpoint)]
        public async Task<IActionResult> GetConfigurationAsync()
        {
            List<GetConfigurationResponse> configurationResponses = await this._configurationService.GetConfigurationsAsync();
            return Ok(configurationResponses.First());
        }
        #endregion

        #region Update System Configuration
        /// <summary>
        /// Update configuration of system.
        /// </summary>
        /// <param name="putConfigurationRequest">An Object constains some configurate properties.</param>
        /// <returns>
        /// A success message about updating configuration.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         PUT
        ///         {
        ///             "scrawlingOrderStartTime": "HH:mm:ss",
        ///             "scrawlingOrderEndTime": "HH:mm:ss",
        ///             "scrawlingMoneyExchangeToKitchenCenter": "HH:mm:ss",
        ///             "scrawlingMoneyExchangeToStore": "HH:mm:ss"
        ///         }
        /// </remarks>
        /// <response code="200">Update system configuration successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="500">Some Error about the system.</response>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.MBKCAdmin)]
        [HttpPut(APIEndPointConstant.Configuration.ConfigurationsEndpoint)]
        public async Task<IActionResult> PutConfigurationsAsync([FromBody] PutConfigurationRequest putConfigurationRequest)
        {
            ValidationResult validationResult = await this._putConfigurationValidator.ValidateAsync(putConfigurationRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            await this._configurationService.UpdateConfigurationAsync(putConfigurationRequest);
            return Ok(new
            {
                Message = MessageConstant.Configuration.UpdatedConfigurationSuccessfully
            });
        }
        #endregion
    }
}
