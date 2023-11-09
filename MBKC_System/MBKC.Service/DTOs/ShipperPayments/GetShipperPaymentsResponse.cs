using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.ShipperPayments
{
    public class GetShipperPaymentsResponse
    {
        public int TotalPages { get; set; }
        public int NumberItems { get; set; }
        public List<GetShipperPaymentResponse> ShipperPayments { get; set; }
    }
}
