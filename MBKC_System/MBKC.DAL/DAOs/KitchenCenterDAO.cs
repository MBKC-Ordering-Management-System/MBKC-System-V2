using MBKC.DAL.DBContext;
using MBKC.DAL.Models;
using MBKC.DAL.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.DAOs
{
    public class KitchenCenterDAO
    {
        private MBKCDbContext _dbContext;
        public KitchenCenterDAO(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task CreateKitchenCenterAsync(KitchenCenter kitchenCenter)
        {
            try
            {
                await this._dbContext.KitchenCenters.AddAsync(kitchenCenter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateKitchenCenter(KitchenCenter kitchenCenter)
        {
            try
            {
                this._dbContext.KitchenCenters.Update(kitchenCenter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<KitchenCenter> GetKitchenCenterAsync(int id)
        {
            try
            {
                return await this._dbContext.KitchenCenters.FirstOrDefaultAsync(x => x.KitchenCenterId == id);
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public async Task<List<KitchenCenter>> GetKitchenCentersAsync(string? searchValue, string? searchValueWithoutUnicode)
        {
            try
            {
                if(searchValue == null && searchValueWithoutUnicode == null)
                {
                    return await this._dbContext.KitchenCenters.ToListAsync();
                }
                return await this._dbContext.KitchenCenters.Where(x => x.Name.ToLower().Contains(searchValue.ToLower())||
                                                                       x.Name.ToLower().Contains(searchValueWithoutUnicode.ToLower()) ||
                                                                       StringUtil.RemoveSign4VietnameseString(x.Name).ToLower().Contains(searchValue.ToLower()) ||
                                                                       StringUtil.RemoveSign4VietnameseString(x.Name).ToLower().Contains(searchValueWithoutUnicode.ToLower())).ToListAsync();
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
