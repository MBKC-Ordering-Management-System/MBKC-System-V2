using Redis.OM.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.RedisModels
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] { "Brand" }, IndexName = "brands")]
    public class BrandRedisModel
    {
        [RedisIdField]
        [Indexed]
        public string BrandId { get; set; }
        [Indexed]
        public string Name { get; set; }
        [Indexed]
        public string Address { get; set; }
        [Indexed]
        public string Logo { get; set; }
        [Indexed]
        public int Status { get; set; }
    }
}
