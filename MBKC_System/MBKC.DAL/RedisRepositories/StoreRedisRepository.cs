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
    public class StoreRedisRepository
    {
        private RedisConnectionProvider _redisConnectionProvider;
        private IRedisCollection<StoreRedisModel> _storeCollection;
        public StoreRedisRepository(RedisConnectionProvider redisConnectionProvider)
        {
            this._redisConnectionProvider = redisConnectionProvider;
            this._storeCollection = this._redisConnectionProvider.RedisCollection<StoreRedisModel>();
        }

        public async Task UpdateStoreAsync(StoreRedisModel store)
        {
            try
            {
                await this._storeCollection.UpdateAsync(store);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<StoreRedisModel>> GetStoresByBrandIdAsync(string brandId)
        {
            try
            {
                return (List<StoreRedisModel>)await this._storeCollection.Where(x => x.BrandId == brandId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
