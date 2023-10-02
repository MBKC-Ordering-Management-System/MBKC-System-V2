using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Brands
{
    public class GetBrandResponse
    {
        public int BrandId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Logo { get; set; }
        public string Status { get; set; }
        public string BrandManagerEmail { get; set; }
    }
}
