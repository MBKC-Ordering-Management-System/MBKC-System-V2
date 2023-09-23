using Redis.OM.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.RedisModels
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] { "Product" }, IndexName = "products")]

    public class ProductRedisModel
    {
        [RedisIdField]
        [Indexed]
        public string ProductId { get; set; }
        [Indexed]
        public string Code { get; set; }
        [Indexed]
        public string Name { get; set; }
        [Indexed]
        public string Description { get; set; }
        [Indexed]
        public decimal SellingPrice { get; set; }
        [Indexed]
        public decimal DiscountPrice { get; set; }
        [Indexed]
        public string Size { get; set; }
        [Indexed]
        public string Type { get; set; }
        [Indexed]
        public int Status { get; set; }
        [Indexed]
        public string Image { get; set; }
        [Indexed]
        public int DisplayOrder { get; set; }
        [Indexed]
        public int ParentProductId { get; set; }
        [Indexed]
        public decimal HistoricalPrice { get; set; }
        [Indexed]
        public int CategoryId { get; set; }
        [Indexed]
        public int BrandId { get; set; }
    }
}
