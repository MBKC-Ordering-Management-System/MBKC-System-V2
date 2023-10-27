using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.StorePartners
{
    public class GetPartnerInformationRequest
    {
        public string? keySortName { get; set; }
        public string? keySortStatus { get; set; }
        public string? keySortCommission { get; set; }
    }
}
