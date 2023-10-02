using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Brands
{
    public class StoreBrandResponse
    {
        public int BrandId { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public string BrandManagerEmail { get; set; }
    }
}
