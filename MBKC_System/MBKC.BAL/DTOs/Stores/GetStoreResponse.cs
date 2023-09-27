using MBKC.BAL.DTOs.Brands;
using MBKC.BAL.DTOs.KitchenCenters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.DTOs.Stores
{
    public class GetStoreResponse
    {
        public int StoreId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Logo { get; set; }
        public string StoreManagerEmail { get; set; }
        public GetKitchenCenterResponse KitchenCenter { get; set; }
        public GetBrandResponse Brand { get; set; }
    }
}
