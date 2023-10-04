using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Partners
{
    public class GetPartnersResponse
    {
        public int TotalPages { get; set; }
        public int NumberItems { get; set; }
        public IEnumerable<GetPartnerResponse> Partners { get; set; }
    }
}
