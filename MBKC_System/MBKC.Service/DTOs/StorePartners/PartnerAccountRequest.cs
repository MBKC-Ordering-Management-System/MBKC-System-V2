using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.StorePartners
{
    public class PartnerAccountRequest
    {
        public int PartnerId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public float Commission { get; set; }
    }
}
