using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Partners
{
    public class GetPartnerResponse
    {
        public int PartnerId { get; set; }
        public string Name { get; set; }
        public string? Logo { get; set; }
        public string? WebUrl { get; set; }
        public string Status { get; set; }
        public float TaxCommission { get; set; }
    }
}
