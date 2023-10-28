using MBKC.Service.DTOs.Partners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.StorePartners
{
    public class GetStorePartnerResponse
    {
        public int StoreId { get; set; }
        public int PartnerId { get; set; }
        public string PartnerLogo { get; set; }
        public string PartnerName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public float Commission { get; set; }
    }
}
