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
    public class ProductRedisDAO
    {
        private RedisConnectionProvider _redisConnectionProvider;
        private IRedisCollection<ProductRedisModel> _productCollection;
        public ProductRedisDAO(RedisConnectionProvider redisConnectionProvider)
        {
            this._redisConnectionProvider = redisConnectionProvider;
            this._productCollection = this._redisConnectionProvider.RedisCollection<ProductRedisModel>();
        }
        public async Task UpdateProductAsync(ProductRedisModel product)
        {
            try
            {
                await this._productCollection.UpdateAsync(product);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ProductRedisModel> GetProductByBrandIdAsync(int brandId)
        {
            try
            {
                return await this._productCollection.SingleOrDefaultAsync(x => x.BrandId == brandId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
