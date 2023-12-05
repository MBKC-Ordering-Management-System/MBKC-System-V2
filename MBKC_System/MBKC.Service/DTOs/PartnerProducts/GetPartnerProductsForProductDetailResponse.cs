using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.PartnerProducts
{
    public class GetPartnerProductsForProductDetailResponse
    {
        public string PartnerName { get; set; }
        public string ProductCode { get; set; }
        public string StoreName { get; set; }
    }
}
