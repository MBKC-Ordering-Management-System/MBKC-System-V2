using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.KitchenCenters
{
    public class GetKitchenCentersResponse
    {
        public int TotalPages { get; set; }
        public int NumberItems { get; set; }
        public IEnumerable<GetKitchenCenterResponse> KitchenCenters { get; set; }
    }
}
