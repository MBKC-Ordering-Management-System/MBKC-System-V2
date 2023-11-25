using MBKC.Service.DTOs.PartnerProducts;
using MBKC.Service.DTOs.Partners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.StorePartners
{
    public class GetStorePartnerForPrivateAPI
    {
        public int StoreId { get; set; }
        public int PartnerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public float Commission { get; set; }
        public GetPartnerForPrivateAPI Partner { get; set; }
        public List<GetPartnerProductForPrivateAPI> PartnerProducts { get; set; }
    }
}
