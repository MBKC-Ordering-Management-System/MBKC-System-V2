using MBKC.DAL.DBContext;
using MBKC.DAL.Enums;
using MBKC.DAL.Models;
using MBKC.DAL.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.Repositories
{
    public class StoreRepository
    {
        private MBKCDbContext _dbContext;
        public StoreRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<int> GetNumberStoresAsync(string? searchValue, string? searchValueWithoutUnicode)
        {
            try
            {
                if (searchValue == null && searchValueWithoutUnicode != null)
                {
                    return this._dbContext.Stores.Where(delegate (Store store)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).AsQueryable().Count();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null)
                {
                    return await this._dbContext.Stores.Where(x => x.Name.ToLower().Contains(searchValue.ToLower())).CountAsync();
                }
                return await this._dbContext.Stores.CountAsync();
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Store>> GetStoresAsync(string? searchValue, string? searchValueWithoutUnicode, int itemsPerPage, int currentPage)
        {
            try
            {
                if(searchValue == null && searchValueWithoutUnicode != null)
                {
                    return this._dbContext.Stores.Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.Brand.BrandAccounts.Any(x => x.Account.Role.RoleId == (int)RoleEnum.Role.BRAND_MANAGER))
                                                 .Where(delegate (Store store) {
                                                        if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                        {
                                                            return true;
                                                        } else
                                                        {
                                                            return false;
                                                        }
                                                 }).Take(itemsPerPage).Skip(itemsPerPage * (currentPage - 1)).ToList();
                } else if(searchValue != null && searchValueWithoutUnicode == null)
                {
                    return await this._dbContext.Stores.Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                             .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                             .Where(x => x.Brand.BrandAccounts.Any(x => x.Account.Role.RoleId == (int)RoleEnum.Role.BRAND_MANAGER)
                                                    && x.Name.ToLower().Contains(searchValue))
                                             .Take(itemsPerPage).Skip(itemsPerPage * (currentPage - 1)).ToListAsync();
                }
                return await this._dbContext.Stores.Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                             .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                             .Where(x => x.Brand.BrandAccounts.Any(x => x.Account.Role.RoleId == (int)RoleEnum.Role.BRAND_MANAGER))
                                             .Take(itemsPerPage).Skip(itemsPerPage * (currentPage - 1)).ToListAsync();
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
