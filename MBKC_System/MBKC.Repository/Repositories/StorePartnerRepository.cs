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
        protected readonly DbSet<StorePartner> _dbSet;
        public StorePartnerRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
            this._dbSet = dbContext.Set<StorePartner>();
        }

        public async Task<StorePartner> GetStorePartnerByPartnerIdAndStoreIdAsync(int partnerId, int storeId)
        {
            try
            {
                return await this._dbContext.StorePartners.Include(x => x.PartnerProducts)
                                                         .SingleOrDefaultAsync(s => s.PartnerId == partnerId && s.StoreId == storeId && s.Status != (int)StorePartnerEnum.Status.DISABLE);
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
                                                        .Include(x => x.Store).ThenInclude(x => x.KitchenCenter)
                                                        .Where(x => x.Status != (int)StorePartnerEnum.Status.DISABLE &&
                                                                     (brandId != null
                                                                     ? x.Store.Brand.BrandId == brandId
                                                                     : true)
                                                                   )
                                                         .Where(delegate (StorePartner storePartner)
                                                         {
                                                             if (StringUtil.RemoveSign4VietnameseString(storePartner.Partner.Name).ToLower().Contains(searchValueWithoutUnicode.ToLower()))
                                                             {
                                                                 return true;
                                                             }
                                                             return false;
                                                         }).AsQueryable().Count();
                }
                else if (searchName != null && searchValueWithoutUnicode == null)
                {
                    return await this._dbContext.StorePartners.Include(x => x.Partner)
                                                              .Include(x => x.Store).ThenInclude(x => x.KitchenCenter)
                                                              .Where(x => x.Status != (int)StorePartnerEnum.Status.DISABLE &&
                                                                     x.Partner.Name.ToLower().Contains(searchName.ToLower()) &&

                                                                     (brandId != null
                                                                     ? x.Store.Brand.BrandId == brandId
                                                                     : true)).CountAsync();

                }
                return await this._dbContext.StorePartners.Include(x => x.Partner)
                                                          .Include(x => x.Store).ThenInclude(x => x.KitchenCenter)
                                                          .Where(x => x.Status != (int)StorePartnerEnum.Status.DISABLE &&
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

        public async Task<List<StorePartner>> GetStorePartnersAsync(string? searchValue, string? searchValueWithoutUnicode,
            int currentPage, int itemsPerPage, string? sortByASC, string? sortByDESC, int brandId, bool? isGetAll)
        {
            try
            {
                if (isGetAll != null && isGetAll == true)
                {
                    return this._dbContext.StorePartners.Include(x => x.Partner)
                                                          .Include(x => x.Store).ThenInclude(x => x.KitchenCenter)
                                                          .Where(x => x.Status != (int)StorePartnerEnum.Status.DISABLE &&
                                                                     (brandId != null
                                                                     ? x.Store.Brand.BrandId == brandId
                                                                     : true)).ToList();

                }
                if (searchValue == null && searchValueWithoutUnicode != null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.StorePartners.Include(x => x.Partner)
                                                        .Include(x => x.Store).ThenInclude(x => x.KitchenCenter)
                                                        .Where(x => x.Status != (int)StorePartnerEnum.Status.DISABLE &&
                                                                     (brandId != null
                                                                     ? x.Store.Brand.BrandId == brandId
                                                                     : true))

                                                         .Where(delegate (StorePartner storePartner)
                                                         {
                                                             if (StringUtil.RemoveSign4VietnameseString(storePartner.Partner.Name).ToLower().Contains(searchValueWithoutUnicode.ToLower()))
                                                             {
                                                                 return true;
                                                             }
                                                             return false;
                                                         })
                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("partnername"),
                                                                  then => then.OrderBy(x => x.Partner.Name))
                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("username"),
                                                                  then => then.OrderBy(x => x.UserName))
                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("password"),
                                                                  then => then.OrderBy(x => x.Password))
                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("commission"),
                                                                  then => then.OrderBy(x => x.Commission))
                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                                  then => then.OrderBy(x => x.Status).Reverse())
                                                         .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();
                    else if (sortByDESC is not null)
                        return this._dbContext.StorePartners.Include(x => x.Partner)
                                                        .Include(x => x.Store).ThenInclude(x => x.KitchenCenter)
                                                        .Where(x => x.Status != (int)StorePartnerEnum.Status.DISABLE &&
                                                                     (brandId != null
                                                                     ? x.Store.Brand.BrandId == brandId
                                                                     : true))

                                                         .Where(delegate (StorePartner storePartner)
                                                         {
                                                             if (StringUtil.RemoveSign4VietnameseString(storePartner.Partner.Name).ToLower().Contains(searchValueWithoutUnicode.ToLower()))
                                                             {
                                                                 return true;
                                                             }
                                                             return false;
                                                         })
                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("partnername"),
                                                                  then => then.OrderByDescending(x => x.Partner.Name))
                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("username"),
                                                                  then => then.OrderByDescending(x => x.UserName))
                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("password"),
                                                                  then => then.OrderByDescending(x => x.Password))
                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("commission"),
                                                                  then => then.OrderByDescending(x => x.Commission))
                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                                  then => then.OrderByDescending(x => x.Status).Reverse())
                                                         .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    return this._dbContext.StorePartners.Include(x => x.Partner)
                                                        .Include(x => x.Store).ThenInclude(x => x.KitchenCenter)
                                                        .Where(x => x.Status != (int)StorePartnerEnum.Status.DISABLE &&
                                                                     (brandId != null
                                                                     ? x.Store.Brand.BrandId == brandId
                                                                     : true))
                                                         .Where(delegate (StorePartner storePartner)
                                                         {
                                                             if (StringUtil.RemoveSign4VietnameseString(storePartner.Partner.Name).ToLower().Contains(searchValueWithoutUnicode.ToLower()))
                                                             {
                                                                 return true;
                                                             }
                                                             return false;
                                                         })
                                                         .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.StorePartners.Include(x => x.Partner)
                                                              .Include(x => x.Store).ThenInclude(x => x.KitchenCenter)
                                                              .Where(x => x.Status != (int)StorePartnerEnum.Status.DISABLE &&
                                                                      x.Partner.Name.ToLower().Contains(searchValue.ToLower()) &&
                                                                     (brandId != null
                                                                     ? x.Store.Brand.BrandId == brandId
                                                                     : true))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("partnername"),
                                                                  then => then.OrderBy(x => x.Partner.Name))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("username"),
                                                                       then => then.OrderBy(x => x.UserName))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("password"),
                                                                       then => then.OrderBy(x => x.Password))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("commission"),
                                                                       then => then.OrderBy(x => x.Commission))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                                  then => then.OrderBy(x => x.Status).Reverse())
                                                              .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.StorePartners.Include(x => x.Partner)
                                                              .Include(x => x.Store).ThenInclude(x => x.KitchenCenter)
                                                              .Where(x => x.Status != (int)StorePartnerEnum.Status.DISABLE &&
                                                                      x.Partner.Name.ToLower().Contains(searchValue.ToLower()) &&
                                                                     (brandId != null
                                                                     ? x.Store.Brand.BrandId == brandId
                                                                     : true))

                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("partnername"),
                                                                  then => then.OrderByDescending(x => x.Partner.Name))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("username"),
                                                                       then => then.OrderByDescending(x => x.UserName))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("password"),
                                                                       then => then.OrderByDescending(x => x.Password))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("commission"),
                                                                       then => then.OrderByDescending(x => x.Commission))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                                  then => then.OrderByDescending(x => x.Status).Reverse())
                                                              .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    return this._dbContext.StorePartners.Include(x => x.Partner)
                                                              .Include(x => x.Store).ThenInclude(x => x.KitchenCenter)
                                                              .Where(x => x.Status != (int)StorePartnerEnum.Status.DISABLE &&
                                                                      x.Partner.Name.ToLower().Contains(searchValue.ToLower()) &&
                                                                     (brandId != null
                                                                     ? x.Store.Brand.BrandId == brandId
                                                                     : true))
                                                              .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                if (sortByASC is not null)
                    return this._dbContext.StorePartners.Include(x => x.Partner)
                                                          .Include(x => x.Store).ThenInclude(x => x.KitchenCenter)
                                                          .Where(x => x.Status != (int)StorePartnerEnum.Status.DISABLE &&

                                                                     (brandId != null
                                                                     ? x.Store.Brand.BrandId == brandId
                                                                     : true))
                                                          .If(sortByASC != null && sortByASC.ToLower().Equals("partnername"),
                                                                  then => then.OrderBy(x => x.Partner.Name))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("username"),
                                                                    then => then.OrderBy(x => x.UserName))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("password"),
                                                                    then => then.OrderBy(x => x.Password))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("commission"),
                                                                    then => then.OrderBy(x => x.Commission))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                                  then => then.OrderBy(x => x.Status).Reverse())
                                                          .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                else if (sortByDESC is not null)
                    return this._dbContext.StorePartners.Include(x => x.Partner)
                                                          .Include(x => x.Store).ThenInclude(x => x.KitchenCenter)
                                                          .Where(x => x.Status != (int)StorePartnerEnum.Status.DISABLE &&

                                                                     (brandId != null
                                                                     ? x.Store.Brand.BrandId == brandId
                                                                     : true))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("partnername"),
                                                                  then => then.OrderByDescending(x => x.Partner.Name))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("username"),
                                                                       then => then.OrderByDescending(x => x.UserName))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("password"),
                                                                       then => then.OrderByDescending(x => x.Password))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("commission"),
                                                                       then => then.OrderByDescending(x => x.Commission))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                                  then => then.OrderByDescending(x => x.Status).Reverse())
                                                          .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                return this._dbContext.StorePartners.Include(x => x.Partner)
                                                          .Include(x => x.Store).ThenInclude(x => x.KitchenCenter)
                                                          .Where(x => x.Status != (int)StorePartnerEnum.Status.DISABLE &&
                                                                     (brandId != null
                                                                     ? x.Store.Brand.BrandId == brandId
                                                                     : true))
                                                          .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task InsertRangeAsync(IEnumerable<StorePartner> storePartners)
        {
            try
            {
                await this._dbSet.AddRangeAsync(storePartners);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
