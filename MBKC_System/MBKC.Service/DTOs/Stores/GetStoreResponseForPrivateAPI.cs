using MBKC.Service.DTOs.StorePartners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Stores
{
    public class GetStoreResponseForPrivateAPI
    {
        public int StoreId { get; set; }
        public string StoreManagerEmail { get; set; }
        public List<GetStorePartnerForPrivateAPI> StorePartners { get; set; }
    }
}
