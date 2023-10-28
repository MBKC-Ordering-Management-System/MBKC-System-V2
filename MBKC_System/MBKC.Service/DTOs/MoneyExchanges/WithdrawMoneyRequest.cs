using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.MoneyExchanges
{
    public class WithdrawMoneyRequest
    {
        public int StoreId { get; set; }
        public decimal Amount { get; set; }
        public IFormFile Image { get; set; }
    }
}
