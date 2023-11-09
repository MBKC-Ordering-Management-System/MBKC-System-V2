using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.ShipperPayments
{
    public class GetShipperPaymentsRequest
    {
        public int ItemsPerPage { get; set; } = 5;
        public int CurrentPage { get; set; } = 1;
        public string? SearchDateFrom { get; set; }
        public string? SearchDateTo { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Status { get; set; }
        public string? SortBy { get; set; }
    }
}
