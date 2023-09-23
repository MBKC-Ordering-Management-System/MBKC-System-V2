using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.DTOs.KitchenCenters
{
    public class GetKitchenCentersResponse
    {
        public int TotalPage { get; set; }
        public int NumberItems { get; set; }
        public IEnumerable<GetKitchenCenterResponse> KitchenCenters { get; set; }
    }
}
