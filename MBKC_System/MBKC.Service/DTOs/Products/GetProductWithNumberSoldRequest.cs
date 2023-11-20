using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Products
{
    public class GetProductWithNumberSoldRequest
    {
        public string? SearchValue { get; set; }
        public string? ProductSearchDateFrom { get; set; }
        public string? ProductSearchDateTo { get; set; }
        public int ItemsPerPage { get; set; } = 5;
        public int CurrentPage { get; set; } = 1;
        public string? SortBy { get; set; }
        public string? ProductType { get; set; }
        public int? IdCategory { get; set; }
        public bool? IsGetAll { get; set; }
    }
}
