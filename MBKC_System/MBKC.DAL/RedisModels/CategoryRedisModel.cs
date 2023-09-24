using Redis.OM.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.RedisModels
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] { "Category" }, IndexName = "categories")]

    public class CategoryRedisModel
    {
        [RedisIdField]
        [Indexed]
        public string CategoryId { get; set; }
        [Indexed]
        public string Code { get; set; }
        [Indexed]
        public string Name { get; set; }
        [Indexed]
        public string Type { get; set; }
        [Indexed]
        public int DisplayOrder { get; set; }
        [Indexed]
        public string Description { get; set; }
        [Indexed]
        public string ImageUrl { get; set; }
        [Indexed]
        public int Status { get; set; }
        [Indexed]
        public string BrandId { get; set; }
    }
}
