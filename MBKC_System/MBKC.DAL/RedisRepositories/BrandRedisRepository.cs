using MBKC.DAL.RedisModels;
using Redis.OM.Searching;
using Redis.OM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MBKC.DAL.RedisRepositories
{
    public class BrandRedisRepository
    {
        private RedisConnectionProvider _redisConnectionProvider;
        private IRedisCollection<BrandRedisModel> _brandCollection;
        public BrandRedisRepository(RedisConnectionProvider redisConnectionProvider)
        {
            this._redisConnectionProvider = redisConnectionProvider;
            this._brandCollection = this._redisConnectionProvider.RedisCollection<BrandRedisModel>();
        }

        public async Task AddBrandAsync(BrandRedisModel brand)
        {
            try
            {
                await this._brandCollection.InsertAsync(brand);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateBrandAsync(BrandRedisModel brand)
        {
            try
            {
                await this._brandCollection.UpdateAsync(brand);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<BrandRedisModel> GetBrandByIdAsync(string id)
        {
            try
            {
                var brands = await this._brandCollection.ToListAsync();
                var distinctBrands = brands
                    .Where(b => b.Name != null && b.Address != null && b.Logo != null && b.BrandId == id)
                    .FirstOrDefault();
                return distinctBrands;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BrandRedisModel>> GetBrandsAsync()
        {
            try
            {
                var brands = (List<BrandRedisModel>)await this._brandCollection.ToListAsync();

                // Loại bỏ các phần tử trùng lặp dựa trên BrandId
                var distinctBrands = brands
                    .Where(b => b.Name != null && b.Address != null && b.Logo != null)
                    .ToList();
                return distinctBrands;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
