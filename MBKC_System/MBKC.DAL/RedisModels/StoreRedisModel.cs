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
        public int StoreId { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public string Logo { get; set; }
        public int KitchenCenterId { get; set; }
        public int BrandId { get; set; }
        public int WalletId { get; set; }
    }
}
