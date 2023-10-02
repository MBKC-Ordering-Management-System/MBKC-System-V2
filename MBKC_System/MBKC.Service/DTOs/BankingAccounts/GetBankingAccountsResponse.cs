using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.BankingAccounts
{
    public class GetBankingAccountsResponse
    {
        public int TotalPages { get; set; }
        public int NumberItems { get; set; }
        public IEnumerable<GetBankingAccountResponse> BankingAccounts { get; set; }
    }
}
