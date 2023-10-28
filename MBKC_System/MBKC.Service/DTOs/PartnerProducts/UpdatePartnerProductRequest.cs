using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.PartnerProducts
{
    public class UpdatePartnerProductRequest
    {
        public string ProductCode { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
    }
}
