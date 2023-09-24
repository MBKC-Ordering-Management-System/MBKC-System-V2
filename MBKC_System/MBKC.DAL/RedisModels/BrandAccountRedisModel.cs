using Redis.OM.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.RedisModels
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] { "BrandAccount" }, IndexName = "brandAccounts")]
    public class BrandAccountRedisModel
    {
        [RedisIdField]
        [Indexed]
        public string BrandId { get; set; }
        [Indexed]
        public string AccountId { get; set; }
    }
}
