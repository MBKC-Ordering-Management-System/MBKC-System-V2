using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.BrandPartners.Requests
{
    public class BrandPartnerRequest
    {
        public int PartnerId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool GetProducts { get; set; }
    }
}
