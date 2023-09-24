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
    public class ExtraCategoryRedisDAO
    {
        private RedisConnectionProvider _redisConnectionProvider;
        private IRedisCollection<ExtraCategoryRedisModel> _extraCategoryCollection;
        public ExtraCategoryRedisDAO(RedisConnectionProvider redisConnectionProvider)
        {
            this._redisConnectionProvider = redisConnectionProvider;
            this._extraCategoryCollection = this._redisConnectionProvider.RedisCollection<ExtraCategoryRedisModel>();
        }

        public async Task UpdateExtraCategoryAsync(ExtraCategoryRedisModel extraCategory)
        {
            try
            {
                await this._extraCategoryCollection.UpdateAsync(extraCategory);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ExtraCategoryRedisModel>> GetExtraCategoryByCategoriesIdAsync(string categoryId)
        {
            try
            {
                return (List<ExtraCategoryRedisModel>)await this._extraCategoryCollection.Where(x => x.CategoryId == categoryId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
