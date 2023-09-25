using MBKC.BAL.DTOs.Brands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.DTOs
{
    public class GetBrandsResponse
    {
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public List<GetBrandResponse> Brands { get; set; }
    }
}
