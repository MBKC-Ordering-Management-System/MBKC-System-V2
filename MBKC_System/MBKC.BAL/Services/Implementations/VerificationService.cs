using AutoMapper;
using MBKC.BAL.DTOs.Verifications;
using MBKC.BAL.Exceptions;
using MBKC.BAL.Services.Interfaces;
using MBKC.BAL.Utils;
using MBKC.DAL.Enums;
using MBKC.DAL.Infrastructures;
using MBKC.DAL.Models;
using MBKC.DAL.RedisModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Services.Implementations
{
    public class VerificationService : IVerificationService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public VerificationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        public async Task VerifyEmailToResetPasswordAsync(Email email, EmailVerificationRequest emailVerificationRequest)
        {
            try
            {
                Account account = await this._unitOfWork.AccountRepository.GetAccountAsync(emailVerificationRequest.Email);
                if (account == null)
                {
                    throw new NotFoundException("Email does not exist in the system.");
                }
                EmailVerification emailVerification = EmailUtil.SendEmailToResetPassword(email, emailVerificationRequest);
                EmailVerificationRedisModel emailVerificationRedisModel = this._mapper.Map<EmailVerificationRedisModel>(emailVerification);
                await this._unitOfWork.EmailVerificationRedisRepository.AddEmailVerificationAsync(emailVerificationRedisModel);
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Email", ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Excception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task ConfirmOTPCodeToResetPasswordAsync(OTPCodeVerificationRequest otpCodeVerificationRequest)
        {
            try
            {
                Account account = await this._unitOfWork.AccountRepository.GetAccountAsync(otpCodeVerificationRequest.Email);
                if (account == null)
                {
                    throw new NotFoundException("Email does not exist in the system.");
                }
                EmailVerificationRedisModel emailVerificationRedisModel = await this._unitOfWork.EmailVerificationRedisRepository.GetEmailVerificationAsync(otpCodeVerificationRequest.Email);
                if (emailVerificationRedisModel == null)
                {
                    throw new BadRequestException("Email has not been previously authenticated.");
                }
                if (emailVerificationRedisModel.CreatedDate.AddMinutes(10) <= DateTime.Now)
                {
                    throw new BadRequestException("OTP code has expired.");
                }
                if (emailVerificationRedisModel.OTPCode.Equals(otpCodeVerificationRequest.OTPCode) == false)
                {
                    throw new BadRequestException("Your OTP code does not match with the previously sent OTP code.");
                }
                emailVerificationRedisModel.IsVerified = Convert.ToBoolean((int)EmailVerificationEnum.Status.VERIFIED);
                await this._unitOfWork.EmailVerificationRedisRepository.UpdateEmailVerificationAsync(emailVerificationRedisModel);
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Email", ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Email has not been previously authenticated."))
                {
                    fieldName = "Email";
                }
                else if (ex.Message.Equals("OTP code has expired.")
                    || ex.Message.Equals("Your OTP code does not match with the previously sent OTP code."))
                {
                    fieldName = "OTP Code";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
    }
}
