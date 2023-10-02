using MBKC.Service.DTOs.Brands;
using MBKC.Service.DTOs.KitchenCenters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Stores
{
    public class GetStoreResponse
    {
        public int StoreId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Logo { get; set; }
        public string StoreManagerEmail { get; set; }
        public string? RejectedReason { get; set; }
        public GetKitchenCenterResponse KitchenCenter { get; set; }
        public GetBrandResponse Brand { get; set; }
    }
}
