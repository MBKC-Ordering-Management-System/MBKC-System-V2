using MBKC.Repository.DBContext;
using MBKC.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Repositories
{
    public class UserDeviceRepository
    {
        private MBKCDbContext _dbContext;
        public UserDeviceRepository(MBKCDbContext dbContext)
        {
                this._dbContext = dbContext;
        }

        public async Task CreateUserDeviceAsync(UserDevice userDevice)
        {
            try
            {
                await this._dbContext.UserDevices.AddAsync(userDevice);
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserDevice> GetUserDeviceAsync(int id)
        {
            try
            {
                return await this._dbContext.UserDevices.SingleOrDefaultAsync(x => x.UserDeviceId == id);
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public async Task<UserDevice> GetUserDeviceAsync(string fcmToken)
        {
            try
            {
                return await this._dbContext.UserDevices.SingleOrDefaultAsync(x => x.FCMToken.Equals(fcmToken));
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public void DeleteUserDevice(UserDevice userDevice)
        {
            try
            {
                this._dbContext.UserDevices.Remove(userDevice);
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
