using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.StorePartners
{
    public class GetStorePartnersResponse
    {
        public int TotalPages { get; set; }
        public int NumberItems { get; set; }
        public List<GetStorePartnerResponse> StorePartners { get; set; }
    }
}
