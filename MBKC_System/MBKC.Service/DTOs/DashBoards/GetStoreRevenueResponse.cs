using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.DashBoards
{
    public class GetStoreRevenueResponse
    {
        public string? StoreName { get; set; }
        public decimal? Revenue { get; set; } 
    }
}
