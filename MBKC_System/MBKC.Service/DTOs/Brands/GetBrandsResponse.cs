using MBKC.Service.DTOs.Brands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs
{
    public class GetBrandsResponse
    {
        public int TotalPages { get; set; }
        public int NumberItems { get; set; }
        public List<GetBrandResponse> Brands { get; set; }
    }
}
