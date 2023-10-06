using MBKC.Service.DTOs.AccountTokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Accounts
{
    public class AccountResponse
    {
        public int AccountId { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public bool IsConfirmed { get; set; }
        public AccountTokenResponse Tokens { get; set; }
    }
}
