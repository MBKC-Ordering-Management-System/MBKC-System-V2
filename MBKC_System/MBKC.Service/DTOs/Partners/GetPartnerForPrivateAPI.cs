using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Partners
{
    public class GetPartnerForPrivateAPI
    {
        public int PartnerId { get; set; }
        public string Name { get; set; }
        public float TaxCommission { get; set; }
    }
}
