using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.BankingAccounts
{
    public class UpdateBankingAccountRequest
    {
        public string BankName { get; set; }
        public IFormFile? BankLogo { get; set; }
        public string Status { get; set; }
    }
}
