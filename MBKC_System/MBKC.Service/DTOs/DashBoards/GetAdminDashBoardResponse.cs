using MBKC.Service.DTOs.Brands;
using MBKC.Service.DTOs.KitchenCenters;
using MBKC.Service.DTOs.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.DashBoards
{
    public class GetAdminDashBoardResponse
    {
        public int TotalKitchenCenters { get; set; }
        public int TotalBrands {  get; set; }
        public int TotalStores {  get; set; }
        public int TotalPartners { get; set; }
        public List<GetKitchenCenterResponse>? KitchenCenters { get; set; }
        public List<GetBrandResponse>? Brands { get; set; }
        public List<GetStoreResponse>? Stores { get; set; }
    }
}
