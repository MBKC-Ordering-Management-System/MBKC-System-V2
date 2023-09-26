using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.DTOs.KitchenCenters
{
    public class UpdateKitchenCenterRequest
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public IFormFile? NewLogo { get; set; }
        public string? DeletedLogo { get; set; }
        public string ManagerEmail { get; set; }
    }
}
