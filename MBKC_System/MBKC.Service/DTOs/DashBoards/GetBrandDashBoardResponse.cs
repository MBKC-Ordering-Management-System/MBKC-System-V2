using MBKC.Service.DTOs.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.DashBoards
{
    public class GetBrandDashBoardResponse
    {
        public int? TotalStore { get; set; }
        public int? TotalNormalCategory { get; set; }
        public int? TotalExtraCategory { get; set; }
        public int? TotalProduct { get; set; }
        public GetStoreRevenueResponse? GetStoreRevenueResponses { get; set; }
        public List<GetStoreResponse>? GetStoreResponse { get; set; }
        public List<NumberOfProductsSoldResponse>? NumberOfProductsSoldResponses { get; set; }
    }
}
