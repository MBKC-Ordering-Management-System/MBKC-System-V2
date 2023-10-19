using AutoMapper;
using MBKC.Service.DTOs.Verifications;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;

using MBKC.Repository.Enums;
using MBKC.Repository.Infrastructures;
using MBKC.Repository.Models;
using MBKC.Repository.Redis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBKC.Service.Utils;
using MBKC.Service.Constants;

namespace MBKC.Service.Services.Implementations
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
        
        public async Task VerifyEmailToResetPasswordAsync(EmailVerificationRequest emailVerificationRequest)
        {
            try
            {
                Account account = await this._unitOfWork.AccountRepository.GetAccountAsync(emailVerificationRequest.Email);
                if (account == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistEmail);
                }
                Repository.Redis.Models.EmailVerification emailVerificationRedisModel = this._unitOfWork.EmailRepository.SendEmailToResetPassword(emailVerificationRequest.Email);
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
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistEmail);
                }
                Repository.Redis.Models.EmailVerification emailVerificationRedisModel = await this._unitOfWork.EmailVerificationRedisRepository.GetEmailVerificationAsync(otpCodeVerificationRequest.Email);
                if (emailVerificationRedisModel == null)
                {
                    throw new BadRequestException(MessageConstant.VerificationMessage.NotAuthenticatedEmailBefore);
                }
                if (emailVerificationRedisModel.CreatedDate.AddMinutes(10) <= DateTime.Now)
                {
                    throw new BadRequestException(MessageConstant.VerificationMessage.ExpiredOTPCode);
                }
                if (emailVerificationRedisModel.OTPCode.Equals(otpCodeVerificationRequest.OTPCode) == false)
                {
                    throw new BadRequestException(MessageConstant.VerificationMessage.NotMatchOTPCode);
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
                if (ex.Message.Equals(MessageConstant.VerificationMessage.NotAuthenticatedEmailBefore))
                {
                    fieldName = "Email";
                }
                else if (ex.Message.Equals(MessageConstant.VerificationMessage.ExpiredOTPCode)
                    || ex.Message.Equals(MessageConstant.VerificationMessage.NotMatchOTPCode))
                {
                    fieldName = "OTP code";
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
