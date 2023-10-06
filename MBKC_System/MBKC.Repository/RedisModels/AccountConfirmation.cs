using Redis.OM.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.RedisModels
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] { "AccountConfirmation" }, IndexName = "account_confirmations")]
    public class AccountConfirmation
    {
        [RedisIdField]
        [Indexed]
        public string AccountId { get; set; }
        [Indexed]
        public bool IsConfirmationLogin { get; set; }
    }
}
