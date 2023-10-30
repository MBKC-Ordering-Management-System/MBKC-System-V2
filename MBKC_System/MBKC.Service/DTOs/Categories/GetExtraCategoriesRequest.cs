using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Categories
{
    public class GetExtraCategoriesRequest
    {
        public string? SearchValue { get; set; }
        public int ItemsPerPage { get; set; } = 5;
        public int CurrentPage { get; set; } = 1;
        public string? SortBy { get; set; }
        public bool? isGetAll { get; set; }
    }
}
