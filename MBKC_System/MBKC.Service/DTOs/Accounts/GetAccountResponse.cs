using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Accounts
{
    public class GetAccountResponse
    {
        public int AccountId { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public string Status { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
