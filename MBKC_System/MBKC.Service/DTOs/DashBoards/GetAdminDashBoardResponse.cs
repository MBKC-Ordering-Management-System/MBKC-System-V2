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
        public int TotalKitchenter { get; set; }
        public int TotalBrand {  get; set; }
        public int TotalStore {  get; set; }
        public int TotalPartner { get; set; }
        public List<GetKitchenCenterResponse>? KitchenCenters { get; set; }
        public List<GetBrandResponse>? Brands { get; set; }
        public List<GetStoreResponse>? Stores { get; set; }
    }
}
