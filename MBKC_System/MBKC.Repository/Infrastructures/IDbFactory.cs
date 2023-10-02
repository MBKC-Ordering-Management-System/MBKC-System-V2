using MBKC.Repository.DBContext;
using Redis.OM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Infrastructures
{
    public interface IDbFactory : IDisposable
    {
        public MBKCDbContext InitDbContext();
        public Task<RedisConnectionProvider> InitRedisConnectionProvider();
    }
}
