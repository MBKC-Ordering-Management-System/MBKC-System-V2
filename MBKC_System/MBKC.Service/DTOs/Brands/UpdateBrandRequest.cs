using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Brands
{
    public class UpdateBrandRequest
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public IFormFile? Logo { get; set; }
        public string BrandManagerEmail { get; set; }
    }
}
