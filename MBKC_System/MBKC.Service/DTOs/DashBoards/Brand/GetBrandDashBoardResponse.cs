using MBKC.Service.DTOs.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.DashBoards.Brand
{
    public class GetBrandDashBoardResponse
    {
        public int? TotalStores { get; set; }
        public int? TotalNormalCategories { get; set; }
        public int? TotalExtraCategories { get; set; }
        public int? TotalProducts { get; set; }
        public GetStoreRevenueResponse? StoreRevenues{ get; set; }
        public List<GetStoreResponse>? Stores { get; set; }

    }
}
