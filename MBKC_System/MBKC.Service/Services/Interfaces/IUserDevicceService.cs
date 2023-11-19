using MBKC.Service.DTOs.UserDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Services.Interfaces
{
    public interface IUserDevicceService
    {
        public Task CreateUserDeviceAsync(CreateUserDeviceRequest userDeviceRequest, IEnumerable<Claim> claims);
        public Task DeleteUserDeviceAsync(int userDeviceId);
    }
}
