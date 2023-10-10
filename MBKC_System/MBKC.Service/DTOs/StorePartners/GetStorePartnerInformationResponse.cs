using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.StorePartners
{
    public class GetStorePartnerInformationResponse
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string KitchenCenterName { get; set; }
        public List<GetPartnerInformationResponse> StorePartners { get; set; }

    }
}
