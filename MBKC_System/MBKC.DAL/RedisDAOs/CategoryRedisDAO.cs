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
    public class CategoryRedisDAO
    {
        private RedisConnectionProvider _redisConnectionProvider;
        private IRedisCollection<CategoryRedisModel> _categoryCollection;
        public CategoryRedisDAO(RedisConnectionProvider redisConnectionProvider)
        {
            this._redisConnectionProvider = redisConnectionProvider;
            this._categoryCollection = this._redisConnectionProvider.RedisCollection<CategoryRedisModel>();
        }

        public async Task UpdateCategoryAsync(CategoryRedisModel categoryRedisModel)
        {
            try
            {
                await this._categoryCollection.UpdateAsync(categoryRedisModel);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<CategoryRedisModel> GetCategoryByBrandIdAsync(int brandId)
        {
            try
            {
                return await this._categoryCollection.SingleOrDefaultAsync(x => x.BrandId == brandId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
