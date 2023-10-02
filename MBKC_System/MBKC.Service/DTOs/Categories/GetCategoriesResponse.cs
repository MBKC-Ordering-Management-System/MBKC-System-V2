using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Categories
{
    public class GetCategoriesResponse
    {
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public List<GetCategoryResponse> Categories { get; set; }
    }
}
