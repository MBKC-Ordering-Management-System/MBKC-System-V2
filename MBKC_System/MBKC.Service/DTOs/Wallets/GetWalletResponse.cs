using MBKC.Service.DTOs.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Wallets
{
    public class GetWalletResponse
    {
        public int WalletId { get; set; }
        public decimal Balance { get; set; }
        public virtual IEnumerable<GetTransactionWalletResponse> Transactions { get; set; }
    }
}
