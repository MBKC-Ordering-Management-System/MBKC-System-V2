using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Orders
{
    public class ConfirmOrderToCompletedRequest
    {
        public String OrderPartnerId { get; set; }
        public int? BankingAccountId { get; set; }
    }
}
