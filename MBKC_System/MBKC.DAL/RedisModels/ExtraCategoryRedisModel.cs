using Redis.OM.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.RedisModels
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] { "ExtraCategory" }, IndexName = "extraCategories")]
    public class ExtraCategoryRedisModel
    {
        [RedisIdField]
        [Indexed]
        public string CategoryId { get; set; }
        [Indexed]
        public int Status { get; set; }
        [Indexed]
        public int ExtraCategoryId { get; set; }
        [Indexed]
        public int ProductCategoryId { get; set; }
    }
}
