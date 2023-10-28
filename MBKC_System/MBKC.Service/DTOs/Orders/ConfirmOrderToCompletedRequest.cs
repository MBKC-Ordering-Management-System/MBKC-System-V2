using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Orders
{
    public class ConfirmOrderToCompletedRequest
    {
        public string OrderPartnerId { get; set; }
        public string ShipperPhone { get; set; }
        public int? BankingAccountId { get; set; }
    }
}
