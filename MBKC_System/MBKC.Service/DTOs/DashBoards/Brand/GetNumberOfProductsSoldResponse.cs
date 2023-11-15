using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.DashBoards.Brand
{
    public class GetNumberOfProductsSoldResponse
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public string? StoreSearchDateFrom { get; set; }
        public string? StoreSearchDateTo { get; set; }
        public string? ProductSearchDateFrom { get; set; }
        public string? ProductSearchDateTo { get; set; }
    }
}
