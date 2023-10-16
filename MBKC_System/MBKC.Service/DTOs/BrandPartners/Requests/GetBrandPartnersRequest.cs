using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.BrandPartners.Requests
{
    public class GetBrandPartnersRequest
    {
        public int IdBrand { get; set; }
        public int? IdPartner { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? PartnerName { get; set; }
        public string? SortBy { get; set; }
    }
}
