using AutoMapper;
using MBKC.Repository.Models;
using MBKC.Service.DTOs.UserDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Profiles.UserDevices
{
    public class UserDeviceProfile: Profile
    {
        public UserDeviceProfile()
        {
            CreateMap<UserDevice, GetUserDeviceResponse>();
        }
    }
}
