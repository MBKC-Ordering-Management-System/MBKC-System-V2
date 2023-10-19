using Redis.OM.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Redis.Models
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] { "AccountToken" }, IndexName = "account_tokens")]
    public class AccountToken
    {
        [RedisIdField]
        [Indexed]
        public string AccountId { get; set; }
        [Indexed]
        public string JWTId { get; set; }
        [Indexed]
        public string RefreshToken { get; set; }
        [Indexed]
        public DateTime ExpiredDate { get; set; }
    }
}
