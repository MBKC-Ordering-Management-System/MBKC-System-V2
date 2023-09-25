using FluentValidation;
using FluentValidation.Results;
using MBKC.BAL.DTOs.Accounts;
using MBKC.BAL.DTOs.Verifications;
using MBKC.BAL.Errors;
using MBKC.BAL.Exceptions;
using MBKC.BAL.Repositories.Interfaces;
using MBKC.BAL.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    public class VerificationsController : ControllerBase
    {
        private IVerificationRepository _verificationRepository;
        private IOptions<Email> _emailOption;
        private IValidator<EmailVerificationRequest> _emailVerificationValidator;
        private IValidator<OTPCodeVerificationRequest> _otpCodeVerificationValidator;
        public VerificationsController(IVerificationRepository verificationRepository, IOptions<Email> emailOption,
            IValidator<EmailVerificationRequest> emailVerificationValidator, IValidator<OTPCodeVerificationRequest> otpCodeVerificationValidator)
        {
            this._verificationRepository = verificationRepository;
            this._emailOption = emailOption;
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
        [Consumes("application/json")]
        [Produces("application/json")]
        [HttpPost("email-verification")]
        public async Task<IActionResult> PostVerifyEmail([FromBody]EmailVerificationRequest emailVerificationRequest)
        {
            ValidationResult validationResult = await this._emailVerificationValidator.ValidateAsync(emailVerificationRequest);
            if(validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            await this._verificationRepository.VerifyEmailToResetPasswordAsync(this._emailOption.Value, emailVerificationRequest);
            return Ok(new
            {
                Message = "Sent Email Confirmation Successfully."
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
        [Consumes("application/json")]
        [Produces("application/json")]
        [HttpPost("otp-verification")]
        public async Task<IActionResult> PostConfirmOTPCode([FromBody]OTPCodeVerificationRequest otpCodeVerificationRequest)
        {
            ValidationResult validationResult = await this._otpCodeVerificationValidator.ValidateAsync(otpCodeVerificationRequest);
            if(validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            await this._verificationRepository.ConfirmOTPCodeToResetPasswordAsync(otpCodeVerificationRequest);
            return Ok(new
            {
                Message = "Confirmed OTP Code Successfully."
            });
        }
        #endregion 
    }
}
