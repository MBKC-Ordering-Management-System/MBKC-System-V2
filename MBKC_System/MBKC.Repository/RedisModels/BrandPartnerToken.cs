using Redis.OM.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.RedisModels
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] { "BrandPartnerToken" }, IndexName = "brand_partner_tokens")]
    public class BrandPartnerToken
    {
        [RedisIdField]
        [Indexed]
        public string Id { get; set; }
        [Indexed]
        public int BrandId { get; set; }
        [Indexed]
        public int PartnerId { get; set; }
        [Indexed]
        public DateTime CreatedDate { get; set; }
        [Indexed]
        public string  JWT { get; set; }
    }
}
