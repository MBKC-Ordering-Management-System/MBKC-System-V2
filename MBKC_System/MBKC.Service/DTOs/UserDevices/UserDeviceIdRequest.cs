using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.UserDevices
{
    public class UserDeviceIdRequest
    {
        [FromRoute(Name = "id")]
        public int UserDeviceId { get; set; }
    }
}
