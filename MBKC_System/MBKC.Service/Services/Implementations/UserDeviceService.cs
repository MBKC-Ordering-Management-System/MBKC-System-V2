using MBKC.Repository.Infrastructures;
using MBKC.Repository.Models;
using MBKC.Service.Constants;
using MBKC.Service.DTOs.UserDevices;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Services.Implementations
{
    public class UserDeviceService: IUserDevicceService
    {
        private UnitOfWork _unitOfWork;
        public UserDeviceService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
        }

        public async Task CreateUserDeviceAsync(CreateUserDeviceRequest userDeviceRequest, IEnumerable<Claim> claims)
        {
            try
            {
                Claim sidClaim = claims.First(x => x.Type.ToLower() == "sid");
                string idAccount = sidClaim.Value;
                UserDevice existedUserDevice = await this._unitOfWork.UserDeviceRepository.GetUserDeviceAsync(userDeviceRequest.FCMToken);
                if (existedUserDevice is null)
                {
                    Account existedAccount = await this._unitOfWork.AccountRepository.GetAccountAsync(int.Parse(idAccount));
                    UserDevice userDevice = new UserDevice()
                    {
                        Account = existedAccount,
                        FCMToken = userDeviceRequest.FCMToken
                    };
                    await this._unitOfWork.UserDeviceRepository.CreateUserDeviceAsync(userDevice);
                    await this._unitOfWork.CommitAsync();
                }
            } catch(Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task DeleteUserDeviceAsync(int userDeviceId)
        {
            try
            {
                UserDevice existedUserDevice = await this._unitOfWork.UserDeviceRepository.GetUserDeviceAsync(userDeviceId);
                if(existedUserDevice is null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.UserDeviceIdNotExist);
                }
                this._unitOfWork.UserDeviceRepository.DeleteUserDevice(existedUserDevice);
                await this._unitOfWork.CommitAsync();
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("User device id", ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
    }
}
