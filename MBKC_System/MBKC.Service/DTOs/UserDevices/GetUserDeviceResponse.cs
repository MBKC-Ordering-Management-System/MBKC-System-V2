using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.UserDevices
{
    public class GetUserDeviceResponse
    {
        public int UserDeviceId { get; set; }
        public string FCMToken { get; set; }
    }
}
