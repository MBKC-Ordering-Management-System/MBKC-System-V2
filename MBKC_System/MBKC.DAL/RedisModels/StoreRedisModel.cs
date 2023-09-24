using Redis.OM.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.RedisModels
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] { "Store" }, IndexName = "stores")]
    public class StoreRedisModel
    {
        [RedisIdField]
        [Indexed]
        public string StoreId { get; set; }
        [Indexed]
        public string Name { get; set; }
        [Indexed]
        public int Status { get; set; }
        [Indexed]
        public string Logo { get; set; }
        [Indexed]
        public int KitchenCenterId { get; set; }
        [Indexed]
        public string BrandId { get; set; }
        [Indexed]
        public int WalletId { get; set; }
    }
}
