using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.DashBoards.Brand
{
    public class GetStoreRevenueResponse
    {
        public int StoreId { get; set; }
        public string? StoreName { get; set; }
        public List<GetColumnChartResponse>? Revenues { get; set; }
    }
}
