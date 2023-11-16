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
        public int? TotalStore { get; set; }
        public int? TotalNormalCategory { get; set; }
        public int? TotalExtraCategory { get; set; }
        public int? TotalProduct { get; set; }
        public GetStoreRevenueResponse? StoreRevenues{ get; set; }
        public List<GetNumberOfProductsSoldResponse>? NumberOfProductSolds { get; set; }
        public List<GetStoreResponse>? Stores { get; set; }

    }
}
