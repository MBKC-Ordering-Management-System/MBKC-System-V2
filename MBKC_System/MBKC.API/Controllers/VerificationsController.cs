using FluentValidation;
using FluentValidation.Results;
using MBKC.API.Constants;
using MBKC.Service.DTOs.Verifications;
using MBKC.Service.Errors;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [ApiController]
    public class VerificationsController : ControllerBase
    {
        private IVerificationService _verificationService;
        private IValidator<EmailVerificationRequest> _emailVerificationValidator;
        private IValidator<OTPCodeVerificationRequest> _otpCodeVerificationValidator;
        public VerificationsController(IVerificationService verificationService,
            IValidator<EmailVerificationRequest> emailVerificationValidator, IValidator<OTPCodeVerificationRequest> otpCodeVerificationValidator)
        {
            this._verificationService = verificationService;
            this._emailVerificationValidator = emailVerificationValidator;
            this._otpCodeVerificationValidator = otpCodeVerificationValidator;
        }

        #region Verify Email
        /// <summary>
        /// Verify email before resetting password.
        /// </summary>
        /// <param name="emailVerificationRequest">
        /// EmailVerificationRequest object contains Email property.
        /// </param>
        /// <returns>
        /// A success message about the sentting OTP code to Email.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         POST 
        ///         {
        ///             "email": "abc@gmail.com"
        ///         }
        /// </remarks>
        /// <response code="200">Sent OTP Code to Email Successfully.</response>
        /// <response code="404">Some Error about request data that are not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.ApplicationJson)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [HttpPost(APIEndPointConstant.Verification.EmailVerificationEndpoint)]
        public async Task<IActionResult> PostVerifyEmail([FromBody]EmailVerificationRequest emailVerificationRequest)
        {
            ValidationResult validationResult = await this._emailVerificationValidator.ValidateAsync(emailVerificationRequest);
            if(validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            await this._verificationService.VerifyEmailToResetPasswordAsync(emailVerificationRequest);
            return Ok(new
            {
                Message = MessageConstant.VerificationMessage.SentEmailConfirmationSuccessfully
            });
        }
        #endregion

        #region Verify OTP Code
        /// <summary>
        /// Compare sent OTP Code in the system with receiver's OTP Code. 
        /// </summary>
        /// <param name="otpCodeVerificationRequest">
        /// OTPCodeVerificationRequest object contains Email property and OTPCode property.
        /// </param>
        /// <returns>
        /// A success message when the OTP Code in the system matchs to receiver's OTP Code.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         POST 
        ///         {
        ///             "email": "abc@gmail.com",
        ///             "otpCode": "000000"
        ///         }
        /// </remarks>
        /// <response code="200">Sent OTP Code to Email Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data that are not found.</response>
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
        [HttpPost(APIEndPointConstant.Verification.OTPVerificationEndpoint)]
        public async Task<IActionResult> PostConfirmOTPCode([FromBody]OTPCodeVerificationRequest otpCodeVerificationRequest)
        {
            ValidationResult validationResult = await this._otpCodeVerificationValidator.ValidateAsync(otpCodeVerificationRequest);
            if(validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            await this._verificationService.ConfirmOTPCodeToResetPasswordAsync(otpCodeVerificationRequest);
            return Ok(new
            {
                Message = MessageConstant.VerificationMessage.ConfirmedOTPCodeSuccessfully
            });
        }
        #endregion 
    }
}
