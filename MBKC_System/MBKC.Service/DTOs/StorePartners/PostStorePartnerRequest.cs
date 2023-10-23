using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.StorePartners
{
    public class PostStorePartnerRequest
    {
        public int StoreId { get; set; }
        public List<PartnerAccountRequest> partnerAccounts { get; set; }
        public bool IsMappingProducts { get; set; }
    }
}
