using MBKC.Service.DTOs.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.MoneyExchanges
{
    public class GetMoneyExchangesResponse
    {
        public int NumberItems { get; set; }
        public int TotalPages { get; set; }
        public List<GetMoneyExchangeResponse> MoneyExchanges { get; set; }
    }
}
