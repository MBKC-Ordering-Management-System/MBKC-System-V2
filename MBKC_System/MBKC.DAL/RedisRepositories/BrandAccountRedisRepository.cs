using MBKC.DAL.RedisModels;
using Redis.OM;
using Redis.OM.Searching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.RedisRepositories
{
    public class BrandAccountRedisRepository
    {
        private RedisConnectionProvider _redisConnectionProvider;
        private IRedisCollection<BrandAccountRedisModel> _brandAccountCollection;
        public BrandAccountRedisRepository(RedisConnectionProvider redisConnectionProvider)
        {
            this._redisConnectionProvider = redisConnectionProvider;
            this._brandAccountCollection = this._redisConnectionProvider.RedisCollection<BrandAccountRedisModel>();
        }
        public async Task AddBrandAccountAsync(BrandAccountRedisModel brandAccount)
        {
            try
            {
                await this._brandAccountCollection.InsertAsync(brandAccount);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BrandAccountRedisModel>> GetBrandAccountsByBrandIdAsync(string brandId)
        {
            try
            {
                return (List<BrandAccountRedisModel>)await this._brandAccountCollection.Where(x => x.BrandId == brandId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
