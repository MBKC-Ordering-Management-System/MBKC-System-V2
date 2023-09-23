using MBKC.DAL.RedisModels;
using Redis.OM;
using Redis.OM.Searching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.RedisDAOs
{
    public class StoreRedisDAO
    {
        private RedisConnectionProvider _redisConnectionProvider;
        private IRedisCollection<StoreRedisModel> _storeCollection;
        public StoreRedisDAO(RedisConnectionProvider redisConnectionProvider)
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

        public async Task<StoreRedisModel> GetStoreByBrandIdAsync(int brandId)
        {
            try
            {
                return await this._storeCollection.SingleOrDefaultAsync(x => x.BrandId == brandId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
