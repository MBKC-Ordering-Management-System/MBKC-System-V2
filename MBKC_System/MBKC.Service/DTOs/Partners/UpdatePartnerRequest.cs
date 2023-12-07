using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Partners
{
    public class UpdatePartnerRequest
    {
        public IFormFile? Logo { get; set; }
        public string WebUrl { get; set; }
        public string Status { get; set; }
        public float TaxCommission { get; set; }
    }
}
