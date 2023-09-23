using MBKC.DAL.RedisModels;
using Redis.OM.Searching;
using Redis.OM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MBKC.DAL.Utils;

namespace MBKC.DAL.RedisDAOs
{
    public class KitchenCenterRedisDAO
    {
        private RedisConnectionProvider _redisConnectionProvider;
        private IRedisCollection<KitchenCenterRedisModel> _kitchenCentersCollection;
        public KitchenCenterRedisDAO(RedisConnectionProvider redisConnectionProvider)
        {
            this._redisConnectionProvider = redisConnectionProvider;
            this._kitchenCentersCollection = this._redisConnectionProvider.RedisCollection<KitchenCenterRedisModel>();
        }

        public async Task CreateKitchenCenterAsync(KitchenCenterRedisModel kitchenCenter)
        {
            try
            {
                await this._kitchenCentersCollection.InsertAsync(kitchenCenter);
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<KitchenCenterRedisModel> GetKitchenCenterAsync(string id)
        {
            try
            {
                return await this._kitchenCentersCollection.FindByIdAsync(id);
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IList<KitchenCenterRedisModel>> GetKitchenCentersAsync(string? searchValue, string? searchValueWithoutUnicode)
        {
            try
            {
                if(searchValue == null && searchValueWithoutUnicode == null)
                {
                    return await this._kitchenCentersCollection.ToListAsync();
                }
                IList<KitchenCenterRedisModel> kitchenCenterRedisModels = await this._kitchenCentersCollection.ToListAsync();
                return kitchenCenterRedisModels.Where(x => x.Name.ToLower().Contains(searchValue.ToLower()) || 
                                                           x.Name.ToLower().Contains(searchValueWithoutUnicode.ToLower()) ||
                                                           StringUtil.RemoveSign4VietnameseString(x.Name).ToLower().Contains(searchValue.ToLower()) ||
                                                           StringUtil.RemoveSign4VietnameseString(x.Name).ToLower().Contains(searchValueWithoutUnicode.ToLower())).ToList();
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
