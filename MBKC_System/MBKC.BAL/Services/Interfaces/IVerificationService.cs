using MBKC.BAL.DTOs.Verifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Services.Interfaces
{
    public interface IVerificationService
    {
        public Task VerifyEmailToResetPasswordAsync(Email email, EmailVerificationRequest emailVerificationRequest);
        public Task ConfirmOTPCodeToResetPasswordAsync(OTPCodeVerificationRequest otpCodeVerificationRequest);
    }
}
