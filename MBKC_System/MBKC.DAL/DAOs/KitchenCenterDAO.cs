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
                return await this._dbContext.KitchenCenters.Include(x => x.Manager)
                                                           .Include(x => x.Stores)
                                                           .FirstOrDefaultAsync(x => x.KitchenCenterId == id);
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetNumberKitchenCentersAsync(string? searchValue, string? searchValueWithoutUnicode)
        {
            try
            {
                if (searchValue == null && searchValueWithoutUnicode != null)
                {
                    return this._dbContext.KitchenCenters.Where(delegate (KitchenCenter kitchenCenter)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(kitchenCenter.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).AsQueryable().Count();
                } else if(searchValue != null && searchValueWithoutUnicode == null)
                {
                    return await this._dbContext.KitchenCenters.Where(x => x.Name.ToLower().Contains(searchValue.ToLower())).CountAsync();
                }
                return await this._dbContext.KitchenCenters.CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public async Task<List<KitchenCenter>> GetKitchenCentersAsync(string? searchValue, string? searchValueWithoutUnicode, int numberItems, int itemsPerPage, int currentPage)
        {
            try
            {
                if (searchValue == null && searchValueWithoutUnicode != null)
                {
                    return this._dbContext.KitchenCenters.Include(x => x.Manager).AsQueryable().Where(delegate (KitchenCenter kitchenCenter)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(kitchenCenter.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null)
                {
                    return await this._dbContext.KitchenCenters.Include(x => x.Manager)
                        .Where(x => x.Name.ToLower().Contains(searchValue.ToLower()))
                        .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
                }
                return await this._dbContext.KitchenCenters.Include(x => x.Manager)
                    .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
