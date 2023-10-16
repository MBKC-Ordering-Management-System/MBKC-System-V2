using AutoMapper;
using MBKC.Repository.Infrastructures;
using MBKC.Repository.Models;
using MBKC.Service.Constants;
using MBKC.Service.DTOs.Accounts;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using System.Security.Claims;

namespace MBKC.Service.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public AccountService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        public async Task<bool> IsActiveAccountAsync(string email)
        {
            try
            {
                Account existedAccount = await this._unitOfWork.AccountRepository.GetActiveAccountAsync(email);
                if (existedAccount == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task<GetAccountResponse> GetAccountAsync(int idAccount, IEnumerable<Claim> claims)
        {
            try
            {
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                string email = registeredEmailClaim.Value;

                Account existedAccount = await this._unitOfWork.AccountRepository.GetAccountAsync(idAccount);
                if (existedAccount is null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistAccountId);
                }
                if (existedAccount.Email.Equals(email) == false)
                {
                    throw new BadRequestException(MessageConstant.AccountMessage.AccountIdNotBelongYourAccount);
                }
                GetAccountResponse getAccountResponse = this._mapper.Map<GetAccountResponse>(existedAccount);
                return getAccountResponse;
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Account id", ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string error = ErrorUtil.GetErrorString("Account id", ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task UpdateAccountAsync(int idAccount, UpdateAccountRequest updateAccountRequest, IEnumerable<Claim> claims)
        {
            try
            {
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                string email = registeredEmailClaim.Value;

                Account existedAccount = await this._unitOfWork.AccountRepository.GetAccountAsync(idAccount);
                if (existedAccount is null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistAccountId);
                }
                if (existedAccount.Email.Equals(email) == false)
                {
                    throw new BadRequestException(MessageConstant.AccountMessage.AccountIdNotBelongYourAccount);
                }

                existedAccount.Password = updateAccountRequest.NewPassword;
                existedAccount.IsConfirmed = true;
                this._unitOfWork.AccountRepository.UpdateAccount(existedAccount);
                await this._unitOfWork.CommitAsync();
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Account id", ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string error = ErrorUtil.GetErrorString("Account id", ex.Message);
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
