using MBKC.Service.DTOs.Verifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Services.Interfaces
{
    public interface IVerificationService
    {
        public Task VerifyEmailToResetPasswordAsync(EmailVerificationRequest emailVerificationRequest);
        public Task ConfirmOTPCodeToResetPasswordAsync(OTPCodeVerificationRequest otpCodeVerificationRequest);
    }
}
