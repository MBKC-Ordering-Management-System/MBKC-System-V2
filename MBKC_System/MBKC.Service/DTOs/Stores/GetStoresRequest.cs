using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Stores
{
    public class GetStoresRequest
    {
        public string? SearchValue { get; set; }
        public int ItemsPerPage { get; set; } = 5;
        public int CurrentPage { get; set; } = 1;
        public string? SortBy { get; set; }
        public bool? IsGetAll { get; set; }
        public int? IdBrand { get; set; }
        public int? IdKitchenCenter { get; set; }
        public string? Status { get; set; }
    }
}
