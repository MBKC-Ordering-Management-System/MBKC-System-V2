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
    public class CategoryRedisRepository
    {
        private RedisConnectionProvider _redisConnectionProvider;
        private IRedisCollection<CategoryRedisModel> _categoryCollection;
        public CategoryRedisRepository(RedisConnectionProvider redisConnectionProvider)
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

        public async Task<List<CategoryRedisModel>> GetCategoriesByBrandIdAsync(string brandId)
        {
            try
            {
                return (List<CategoryRedisModel>)await this._categoryCollection.Where(x => x.BrandId == brandId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
