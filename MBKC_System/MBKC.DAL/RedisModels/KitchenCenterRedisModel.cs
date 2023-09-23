using Redis.OM.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.RedisModels
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] { "KitchenCenter" }, IndexName = "kitchencenters")]
    public class KitchenCenterRedisModel
    {
        [RedisIdField]
        [Indexed]
        public string KitchenCenterId { get; set; }
        [Searchable]
        public string Name { get; set; }
        [Indexed]
        public string Address { get; set; }
        [Indexed]
        public int Status { get; set; }
        [Indexed]
        public string Logo { get; set; }
        [Indexed]
        public int WalletId { get; set; }
        [Indexed]
        public int ManagerId { get; set; }
    }
}
