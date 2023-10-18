using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.PartnerProducts
{
    public class GetPartnerProductsResponse
    {
        public int TotalPages { get; set; }
        public int NumberItems { get; set; }
        public IEnumerable<GetPartnerProductResponse> PartnerProducts { get; set; }
    }
}
