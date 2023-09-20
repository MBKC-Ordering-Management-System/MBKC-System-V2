using Microsoft.EntityFrameworkCore;
using Redis.OM.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.RedisModels
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] { "Account" }, IndexName = "accounts")]
    public class AccountRedisModel
    {
        [RedisIdField]
        [Indexed]
        public string AccountId { get; set; }
        [Indexed]
        public string Email { get; set; }
        [Indexed]
        public string Password { get; set; }
        [Indexed]
        public bool Status { get; set; }
        [Indexed]
        public int RoleId { get; set; }
        [Indexed]
        public string RoleName { get; set; }
    }
}
