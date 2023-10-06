using MBKC.Repository.DBContext;
using MBKC.Repository.Enums;
using MBKC.Repository.Models;
using MBKC.Repository.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Repositories
{
    public class StorePartnerRepository
    {
        private MBKCDbContext _dbContext;
        public StorePartnerRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<StorePartner> GetStorePartnerByPartnerIdAndStoreIdAsync(int partnerId, int storeId)
        {
            try
            {
                return await this._dbContext.StorePartners.SingleOrDefaultAsync(s => s.PartnerId == partnerId && s.StoreId == storeId && s.Status != (int)StorePartnerEnum.Status.DEACTIVE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<StorePartner>> GetStorePartnersByUserNameAndStoreIdAsync(string userName, int storeId, int partnerId)
        {
            try
            {
                return await this._dbContext.StorePartners.Where(s => s.StoreId != storeId && s.UserName.Equals(userName) && s.PartnerId == partnerId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetNumberStorePartnersAsync(string? searchName, string? searchValueWithoutUnicode, int? brandId)
        {
            try
            {
                if (searchName == null && searchValueWithoutUnicode != null)
                {
                    return this._dbContext.StorePartners.Include(x => x.Partner)
                                                         .Where(x => x.Status != (int)StorePartnerEnum.Status.DEACTIVE &&

                                                                     (brandId != null
                                                                     ? x.Store.Brand.BrandId == brandId
                                                                     : true)
                                                                   )
                                                         .Where(delegate (StorePartner storePartner)
                                                         {
                                                             if (searchValueWithoutUnicode.ToLower().Contains(StringUtil.RemoveSign4VietnameseString(storePartner.Partner.Name).ToLower()))
                                                             {
                                                                 return true;
                                                             }
                                                             return false;
                                                         }).AsQueryable().Count();
                }
                else if (searchName != null && searchValueWithoutUnicode == null)
                {
                    return await this._dbContext.StorePartners.Include(x => x.Partner)
                                                         .Where(x => x.Status != (int)StorePartnerEnum.Status.DEACTIVE &&
                                                                     x.Partner.Name.ToLower().Contains(searchName.ToLower()) &&

                                                                     (brandId != null
                                                                     ? x.Store.Brand.BrandId == brandId
                                                                     : true)).CountAsync();

                }
                return await this._dbContext.StorePartners.Include(x => x.Partner)
                                                         .Where(x => x.Status != (int)StorePartnerEnum.Status.DEACTIVE &&
                                                                     (brandId != null
                                                                     ? x.Store.Brand.BrandId == brandId
                                                                     : true)).CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CreateStorePartnerAsync(StorePartner storePartner)
        {
            try
            {
                await this._dbContext.AddAsync(storePartner);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateStorePartner(StorePartner storePartner)
        {
            try
            {
                this._dbContext.StorePartners.Update(storePartner);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<StorePartner>> GetStorePartnersAsync(string? searchName, string? searchValueWithoutUnicode, int? brandId, int? currentPage, int? itemsPerPage)
        {
            try
            {

                if (searchName == null && searchValueWithoutUnicode != null)
                {
                    return this._dbContext.StorePartners.Include(x => x.Partner)
                                                         .Where(x => x.Status != (int)StorePartnerEnum.Status.DEACTIVE &&
                                                                     (brandId != null
                                                                     ? x.Store.Brand.BrandId == brandId
                                                                     : true))

                                                         .Where(delegate (StorePartner storePartner)
                                                         {
                                                             if (searchValueWithoutUnicode.ToLower().Contains(StringUtil.RemoveSign4VietnameseString(storePartner.Partner.Name).ToLower()))
                                                             {
                                                                 return true;
                                                             }
                                                             return false;
                                                         }).Skip(itemsPerPage.Value * (currentPage.Value - 1)).Take(itemsPerPage.Value).AsQueryable().ToList();
                }
                else if (searchName != null && searchValueWithoutUnicode == null)
                {
                    return await this._dbContext.StorePartners.Include(x => x.Partner)
                                                         .Where(x => x.Status != (int)StorePartnerEnum.Status.DEACTIVE &&
                                                                      x.Partner.Name.ToLower().Contains(searchName.ToLower()) &&

                                                                     (brandId != null
                                                                     ? x.Store.Brand.BrandId == brandId
                                                                     : true)).Skip(itemsPerPage.Value * (currentPage.Value - 1)).Take(itemsPerPage.Value).ToListAsync();
                }
                return await this._dbContext.StorePartners.Include(x => x.Partner)
                                                         .Where(x => x.Status != (int)StorePartnerEnum.Status.DEACTIVE &&

                                                                     (brandId != null
                                                                     ? x.Store.Brand.BrandId == brandId
                                                                     : true)).Skip(itemsPerPage.Value * (currentPage.Value - 1)).Take(itemsPerPage.Value).ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
