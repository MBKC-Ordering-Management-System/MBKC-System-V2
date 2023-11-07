using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.MoneyExchanges
{
    public class GetMoneyExchangeRequest
    {
        public int ItemsPerPage { get; set; } = 5;
        public int CurrentPage { get; set; } = 1;
        public string? SearchDateFrom { get; set; }
        public string? SearchDateTo { get; set; }
        public string? SortBy { get; set; }
        public string? ExchangeType { get; set; }
        public bool? IsGetAll { get; set; }
    }
}
