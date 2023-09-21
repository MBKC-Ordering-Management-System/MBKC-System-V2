using FluentValidation;
using FluentValidation.Results;
using MBKC.BAL.DTOs.Verifications;
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
    }
}
