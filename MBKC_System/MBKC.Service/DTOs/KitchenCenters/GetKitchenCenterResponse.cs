using MBKC.Service.DTOs.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.KitchenCenters
{
    public class GetKitchenCenterResponse
    {
        public int KitchenCenterId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string Logo { get; set; }
        public string KitchenCenterManagerEmail { get; set; }
    }
}
