using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.BankingAccounts
{
    public class GetBankingAccountResponse
    {
        public int BankingAccountId { get; set; }
        public string NumberAccount { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
    }
}
