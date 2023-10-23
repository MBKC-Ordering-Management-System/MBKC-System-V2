using Redis.OM.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Redis.Models
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] { "EmailVerification" }, IndexName = "emailverifications")]
    public class EmailVerification
    {
        [RedisIdField]
        [Indexed]
        public string Email { get; set; }
        [Indexed]
        public string OTPCode { get; set; }
        [Indexed]
        public DateTime CreatedDate { get; set; }
        [Indexed]
        public bool IsVerified { get; set; }
    }
}
