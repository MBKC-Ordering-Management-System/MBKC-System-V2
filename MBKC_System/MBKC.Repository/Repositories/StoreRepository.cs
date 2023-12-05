using MBKC.Repository.DBContext;
using MBKC.Repository.Enums;
using MBKC.Repository.Models;
using MBKC.Repository.Utils;
using Microsoft.EntityFrameworkCore;

namespace MBKC.Repository.Repositories
{

    public class StoreRepository
    {
        private MBKCDbContext _dbContext;
        public StoreRepository(MBKCDbContext dbContext)


        {
            this._dbContext = dbContext;

        }

        public async Task<int> GetNumberStoresAsync(string? searchValue, string? searchValueWithoutUnicode, int? brandId, int? kitchenCenterId, int? statusParam)
        {
            try
            {
                if (searchValue == null && searchValueWithoutUnicode != null && brandId == null && kitchenCenterId == null)
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
                    }).Where(x => x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true)).AsQueryable().Count();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null && brandId == null && kitchenCenterId == null)
                {
                    return await this._dbContext.Stores.Where(x => x.Name.ToLower().Contains(searchValue.ToLower()) && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true)).CountAsync();
                }
                else if (searchValue == null && searchValueWithoutUnicode != null && brandId != null && kitchenCenterId == null)
                {
                    return this._dbContext.Stores.Include(x => x.Brand).Where(delegate (Store store)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).Where(x => x.Brand.BrandId == brandId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true)).AsQueryable().Count();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null && brandId != null && kitchenCenterId == null)
                {
                    return await this._dbContext.Stores.Include(x => x.Brand).Where(x => x.Name.ToLower().Contains(searchValue.ToLower()) &&
                                                                                         x.Brand.BrandId == brandId && x.Status != (int)BrandEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true)).CountAsync();
                }
                else if (searchValue == null && searchValueWithoutUnicode == null && brandId != null && kitchenCenterId == null)
                {
                    return await this._dbContext.Stores.Include(x => x.Brand).Where(x => x.Brand.BrandId == brandId && x.Status != (int)BrandEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true)).CountAsync();
                }
                else if (searchValue == null && searchValueWithoutUnicode != null && brandId == null && kitchenCenterId != null)
                {
                    return this._dbContext.Stores.Include(x => x.KitchenCenter).Where(delegate (Store store)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status != (int)KitchenCenterEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true)).AsQueryable().Count();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null && brandId == null && kitchenCenterId != null)
                {
                    return await this._dbContext.Stores.Include(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId)
                                                       .Where(x => x.Name.ToLower().Contains(searchValue.ToLower())
                                                                && x.KitchenCenter.KitchenCenterId == kitchenCenterId
                                                                && x.Status != (int)KitchenCenterEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true)).CountAsync();
                }
                else if (searchValue == null && searchValueWithoutUnicode == null && brandId == null && kitchenCenterId != null)
                {
                    return await this._dbContext.Stores.Include(x => x.KitchenCenter)
                                                       .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status != (int)KitchenCenterEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                       .CountAsync();
                }
                else if (searchValue == null && searchValueWithoutUnicode == null && brandId != null && kitchenCenterId != null)
                {
                    return await this._dbContext.Stores.Include(x => x.Brand).Include(x => x.KitchenCenter)
                                                 .Where(x => x.Brand.BrandId == brandId
                                                          && x.KitchenCenter.KitchenCenterId == kitchenCenterId
                                                          && x.Status != (int)KitchenCenterEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                 .CountAsync();
                }
                else if (searchValue == null && searchValueWithoutUnicode != null && brandId != null && kitchenCenterId != null)
                {
                    return this._dbContext.Stores.Include(x => x.Brand).Include(x => x.KitchenCenter)
                        .Where(x => x.Brand.BrandId == brandId && x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status != (int)KitchenCenterEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                        .Where(delegate (Store store)
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
                else if (searchValue != null && searchValueWithoutUnicode == null && brandId != null && kitchenCenterId != null)
                {
                    return await this._dbContext.Stores.Include(x => x.Brand).Where(x => x.Name.ToLower().Contains(searchValue.ToLower()) &&
                                                                                         x.Brand.BrandId == brandId &&
                                                                                         x.KitchenCenter.KitchenCenterId == kitchenCenterId &&
                                                                                         x.Status != (int)KitchenCenterEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                                             .CountAsync();
                }
                return await this._dbContext.Stores.Where(x => x.Status != (int)KitchenCenterEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true)).CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Store>> GetStoresAsync(string? searchValue, string? searchValueWithoutUnicode,
            int currentPage, int itemsPerPage, string? sortByASC, string? sortByDESC, int? brandId, int? kitchenCenterId, int? statusParam, bool? isGetAll)
        {
            try
            {
                if (isGetAll != null && isGetAll == true)
                {
                    return await this._dbContext.Stores.Include(x => x.Wallet)
                                                   .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                   .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                   .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                   .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                   .Where(x => x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true) &&
                                                   (brandId != null ? x.Brand.BrandId == brandId : true) &&
                                                   (kitchenCenterId != null ? x.KitchenCenter.KitchenCenterId == kitchenCenterId : true))
                                                   .ToListAsync();
                }

                if (searchValue == null && searchValueWithoutUnicode != null && brandId == null && kitchenCenterId == null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                     .Where(delegate (Store store)
                                                     {
                                                         if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                         {
                                                             return true;
                                                         }
                                                         else
                                                         {
                                                             return false;
                                                         }
                                                     })
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                                then => then.OrderBy(x => x.Name))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("storemanageremail"),
                                                                then => then.OrderBy(x => x.StoreManagerEmail))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                                then => then.OrderBy(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                     .Where(delegate (Store store)
                                                     {
                                                         if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                         {
                                                             return true;
                                                         }
                                                         else
                                                         {
                                                             return false;
                                                         }
                                                     })
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                                then => then.OrderByDescending(x => x.Name))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("storemanageremail"),
                                                                then => then.OrderByDescending(x => x.StoreManagerEmail))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                                then => then.OrderByDescending(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    return this._dbContext.Stores.Include(x => x.Wallet)
                        .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                 .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                 .Where(delegate (Store store)
                                                    {
                                                        if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                        {
                                                            return true;
                                                        }
                                                        else
                                                        {
                                                            return false;
                                                        }
                                                    })
                                                    .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null && brandId == null && kitchenCenterId == null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Name.ToLower().Contains(searchValue) && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                              then => then.OrderBy(x => x.Name))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("storemanageremail"),
                                                              then => then.OrderBy(x => x.StoreManagerEmail))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                              then => then.OrderBy(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Name.ToLower().Contains(searchValue) && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                              then => then.OrderByDescending(x => x.Name))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("storemanageremail"),
                                                              then => then.OrderByDescending(x => x.StoreManagerEmail))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                              then => then.OrderByDescending(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    return this._dbContext.Stores.Include(x => x.Wallet)
                        .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.Name.ToLower().Contains(searchValue) && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                 .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                else if (searchValue == null && searchValueWithoutUnicode != null && brandId != null && kitchenCenterId == null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Brand.BrandId == brandId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                     .Where(delegate (Store store)
                                                      {
                                                          if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                          {
                                                              return true;
                                                          }
                                                          else
                                                          {
                                                              return false;
                                                          }
                                                      })
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                           then => then.OrderBy(x => x.Name))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("storemanageremail"),
                                                           then => then.OrderBy(x => x.StoreManagerEmail))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                           then => then.OrderBy(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Brand.BrandId == brandId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                     .Where(delegate (Store store)
                                                     {
                                                         if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                         {
                                                             return true;
                                                         }
                                                         else
                                                         {
                                                             return false;
                                                         }
                                                     })
                                                 .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                       then => then.OrderByDescending(x => x.Name))
                                                 .If(sortByDESC != null && sortByDESC.ToLower().Equals("storemanageremail"),
                                                       then => then.OrderByDescending(x => x.StoreManagerEmail))
                                                 .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                       then => then.OrderByDescending(x => x.Status).Reverse())
                                                 .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    return this._dbContext.Stores.Include(x => x.Wallet)
                        .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.Brand.BrandId == brandId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                 .Where(delegate (Store store)
                                                 {
                                                     if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                     {
                                                         return true;
                                                     }
                                                     else
                                                     {
                                                         return false;
                                                     }
                                                 }).Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null && brandId != null && kitchenCenterId == null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Name.ToLower().Contains(searchValue) && x.Brand.BrandId == brandId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                           then => then.OrderBy(x => x.Name))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("storemanageremail"),
                                                           then => then.OrderBy(x => x.StoreManagerEmail))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                           then => then.OrderBy(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Name.ToLower().Contains(searchValue) && x.Brand.BrandId == brandId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                           then => then.OrderByDescending(x => x.Name))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("storemanageremail"),
                                                           then => then.OrderByDescending(x => x.StoreManagerEmail))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                           then => then.OrderByDescending(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    return this._dbContext.Stores.Include(x => x.Wallet)
                        .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.Name.ToLower().Contains(searchValue) && x.Brand.BrandId == brandId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                 .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                else if (searchValue == null && searchValueWithoutUnicode == null && brandId != null && kitchenCenterId == null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Brand.BrandId == brandId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                               then => then.OrderBy(x => x.Name))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("storemanageremail"),
                                                               then => then.OrderBy(x => x.StoreManagerEmail))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                               then => then.OrderBy(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Brand.BrandId == brandId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                               then => then.OrderByDescending(x => x.Name))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("storemanageremail"),
                                                               then => then.OrderByDescending(x => x.StoreManagerEmail))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                               then => then.OrderByDescending(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    return this._dbContext.Stores.Include(x => x.Wallet)
                        .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.Brand.BrandId == brandId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                 .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                else if (searchValue == null && searchValueWithoutUnicode != null && brandId == null && kitchenCenterId != null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                     .Where(delegate (Store store)
                                                     {
                                                         if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                         {
                                                             return true;
                                                         }
                                                         else
                                                         {
                                                             return false;
                                                         }
                                                     })
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                               then => then.OrderBy(x => x.Name))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("storemanageremail"),
                                                               then => then.OrderBy(x => x.StoreManagerEmail))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                               then => then.OrderBy(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                     .Where(delegate (Store store)
                                                     {
                                                         if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                         {
                                                             return true;
                                                         }
                                                         else
                                                         {
                                                             return false;
                                                         }
                                                     })
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                               then => then.OrderByDescending(x => x.Name))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("storemanageremail"),
                                                               then => then.OrderByDescending(x => x.StoreManagerEmail))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                               then => then.OrderByDescending(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    return this._dbContext.Stores.Include(x => x.Wallet)
                        .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                 .Where(delegate (Store store)
                                                 {
                                                     if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                     {
                                                         return true;
                                                     }
                                                     else
                                                     {
                                                         return false;
                                                     }
                                                 }).Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null && brandId == null && kitchenCenterId != null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Name.ToLower().Contains(searchValue) && x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                          then => then.OrderBy(x => x.Name))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("storemanageremail"),
                                                          then => then.OrderBy(x => x.StoreManagerEmail))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                          then => then.OrderBy(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Name.ToLower().Contains(searchValue) && x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                           then => then.OrderByDescending(x => x.Name))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("storemanageremail"),
                                                           then => then.OrderByDescending(x => x.StoreManagerEmail))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                           then => then.OrderByDescending(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    return this._dbContext.Stores.Include(x => x.Wallet)
                        .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.Name.ToLower().Contains(searchValue) && x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                 .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                else if (searchValue == null && searchValueWithoutUnicode == null && brandId == null && kitchenCenterId != null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                             then => then.OrderBy(x => x.Name))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("storemanageremail"),
                                                             then => then.OrderBy(x => x.StoreManagerEmail))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                             then => then.OrderBy(x => x.Status).Reverse())
                                                     .Take(itemsPerPage).Skip(itemsPerPage * (currentPage - 1)).ToList();
                    else if (sortByDESC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                          then => then.OrderByDescending(x => x.Name))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("storemanageremail"),
                                                          then => then.OrderByDescending(x => x.StoreManagerEmail))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                          then => then.OrderByDescending(x => x.Status).Reverse())
                                                     .Take(itemsPerPage).Skip(itemsPerPage * (currentPage - 1)).ToList();

                    return this._dbContext.Stores.Include(x => x.Wallet)
                        .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                 .Take(itemsPerPage).Skip(itemsPerPage * (currentPage - 1)).ToList();
                }
                else if (searchValue == null && searchValueWithoutUnicode == null && brandId != null && kitchenCenterId != null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Brand.BrandId == brandId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                             then => then.OrderBy(x => x.Name))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("storemanageremail"),
                                                             then => then.OrderBy(x => x.StoreManagerEmail))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                             then => then.OrderBy(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Brand.BrandId == brandId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                          then => then.OrderByDescending(x => x.Name))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("storemanageremail"),
                                                          then => then.OrderByDescending(x => x.StoreManagerEmail))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                          then => then.OrderByDescending(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    return this._dbContext.Stores.Include(x => x.Wallet)
                        .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Brand.BrandId == brandId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                 .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                else if (searchValue == null && searchValueWithoutUnicode != null && brandId != null && kitchenCenterId != null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Brand.BrandId == brandId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                     .Where(delegate (Store store)
                                                     {
                                                         if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                         {
                                                             return true;
                                                         }
                                                         else
                                                         {
                                                             return false;
                                                         }
                                                     })
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                             then => then.OrderBy(x => x.Name))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("storemanageremail"),
                                                             then => then.OrderBy(x => x.StoreManagerEmail))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                             then => then.OrderBy(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Brand.BrandId == brandId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                     .Where(delegate (Store store)
                                                     {
                                                         if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                         {
                                                             return true;
                                                         }
                                                         else
                                                         {
                                                             return false;
                                                         }
                                                     })
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                            then => then.OrderByDescending(x => x.Name))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("storemanageremail"),
                                                            then => then.OrderByDescending(x => x.StoreManagerEmail))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                            then => then.OrderByDescending(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    return this._dbContext.Stores.Include(x => x.Wallet)
                        .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Brand.BrandId == brandId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                 .Where(delegate (Store store)
                                                 {
                                                     if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                     {
                                                         return true;
                                                     }
                                                     else
                                                     {
                                                         return false;
                                                     }
                                                 }).Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null && brandId != null && kitchenCenterId != null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Name.ToLower().Contains(searchValue) && x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Brand.BrandId == brandId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                           then => then.OrderBy(x => x.Name))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("storemanageremail"),
                                                           then => then.OrderBy(x => x.StoreManagerEmail))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                           then => then.OrderBy(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Name.ToLower().Contains(searchValue) && x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Brand.BrandId == brandId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                           then => then.OrderByDescending(x => x.Name))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("storemanageremail"),
                                                           then => then.OrderByDescending(x => x.StoreManagerEmail))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                           then => then.OrderByDescending(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    return this._dbContext.Stores.Include(x => x.Wallet)
                        .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.Name.ToLower().Contains(searchValue) && x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Brand.BrandId == brandId && x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                 .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                return await this._dbContext.Stores.Include(x => x.Wallet)
                    .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                   .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                   .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                   .Where(x => x.Status != (int)StoreEnum.Status.DISABLE && (statusParam != null ? x.Status == statusParam : true))
                                                   .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Store> GetStoreAsync(int id)
        {
            try
            {
                return await this._dbContext.Stores.Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                   .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                   .Include(x => x.StorePartners).ThenInclude(x => x.PartnerProducts)
                                                   .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                   .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                   .Include(x => x.Wallet)
                                                   .FirstOrDefaultAsync(x => x.StoreId == id && x.Status != (int)StoreEnum.Status.DISABLE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Store> GetStoreByIdAsync(int id)
        {
            try
            {
                return await this._dbContext.Stores.Include(x => x.Wallet)
                                                   .Include(x => x.StorePartners)
                                                   .Include(x => x.Brand)
                                                   .FirstOrDefaultAsync(x => x.StoreId == id && x.Status == (int)StoreEnum.Status.ACTIVE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Store?> GetStoreExceptDeactiveByIdAsync(int id)
        {
            try
            {
                return await this._dbContext.Stores.Include(s => s.Brand)
                                                   .FirstOrDefaultAsync(s => s.StoreId == id && s.Status != (int)StoreEnum.Status.DISABLE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CreateStoreAsync(Store store)
        {
            try
            {
                await this._dbContext.AddAsync(store);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateStore(Store store)
        {
            try
            {
                this._dbContext.Stores.Update(store);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Store> GetStoreAsync(string managerEmail)
        {
            try
            {
                return await this._dbContext.Stores.Include(x => x.Brand)
                                                   .ThenInclude(x => x.Categories)
                                                   .Include(x => x.KitchenCenter)
                                                   .ThenInclude(x => x.Manager)
                                                   .Include(x => x.KitchenCenter)
                                                   .ThenInclude(x => x.Cashiers)
                                                   .Include(x => x.StoreMoneyExchanges)
                                                   .ThenInclude(x => x.MoneyExchange)
                                                   .ThenInclude(x => x.Transactions)
                                                   .Include(x => x.Orders).ThenInclude(x => x.ShipperPayments).ThenInclude(x => x.BankingAccount)
                                                   .Include(x => x.Orders).ThenInclude(x => x.OrderHistories)
                                                   .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.UserDevices)
                                                   .Include(x => x.Brand).ThenInclude(x => x.Products)
                                                   .Where(x => x.StoreAccounts.Any(b => b.Account.Email.Equals(x.StoreManagerEmail)))
                                                   .SingleOrDefaultAsync(x => x.StoreManagerEmail.Equals(managerEmail));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Store> GetStoreIncludeCashierAsync(string managerEmail)
        {
            try
            {
                return await this._dbContext.Stores.Include(x => x.KitchenCenter)
                                                   .ThenInclude(x => x.Cashiers)
                                                   .SingleOrDefaultAsync(x => x.StoreManagerEmail.Equals(managerEmail));

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Store>> GetStoresAsync()
        {
            try
            {
                return await this._dbContext.Stores
                                      .Include(x => x.StorePartners.Where(x => x.Status == (int)StorePartnerEnum.Status.ACTIVE))
                                      .ThenInclude(x => x.Partner)
                                      .Include(x => x.StorePartners.Where(x => x.Status == (int)StorePartnerEnum.Status.ACTIVE))
                                      .ThenInclude(x => x.PartnerProducts).ThenInclude(x => x.Product).ThenInclude(x => x.ChildrenProducts)
                                      .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.UserDevices)
                                      .Include(x => x.KitchenCenter)
                                      .Where(x => x.Status == (int)StoreEnum.Status.ACTIVE && x.StoreAccounts.Any(b => b.Account.Email.Equals(x.StoreManagerEmail))).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region count number of store
        public async Task<int> CountStoreNumberAsync()
        {
            try
            {
                return await _dbContext.Stores.Where(s => s.Status == (int)StoreEnum.Status.ACTIVE || s.Status == (int)StoreEnum.Status.INACTIVE).CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region count number of store by id brand
        public async Task<int> CountStoreNumberByBrandIdAsync(int brandId)
        {
            try
            {
                return await _dbContext.Stores.Where(s => (s.Status == (int)StoreEnum.Status.ACTIVE || s.Status == (int)StoreEnum.Status.INACTIVE) && s.Brand.BrandId == brandId).CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region count number of store is active
        public async Task<int> CountStoreNumberIsActiveFindByKitchenCenterIdAsync(int kitchenCenterId)
        {
            try
            {
                return await _dbContext.Stores.Where(s => s.Status == (int)StoreEnum.Status.ACTIVE && s.KitchenCenter.KitchenCenterId == kitchenCenterId).CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region get 5 store with status active
        public async Task<List<Store>> GetFiveStoreSortByActiveAsync()
        {
            try
            {
                return await _dbContext.Stores.Where(s => s.Status == (int)StoreEnum.Status.ACTIVE || s.Status == (int)StoreEnum.Status.INACTIVE)
                                                      .OrderByDescending(s => s.Status)
                                                      .Take(5)
                                                      .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        public async Task<Store> GetStoreMoneyExchangeAsync(string managerEmail)
        {
            try
            {
                return await this._dbContext.Stores
                                                   .Include(x => x.KitchenCenter)
                                                   .Include(x => x.StoreMoneyExchanges)
                                                   .ThenInclude(x => x.MoneyExchange)
                                                   .ThenInclude(x => x.Transactions)
                                                   
                                                   .SingleOrDefaultAsync(x => x.StoreManagerEmail.Equals(managerEmail));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetNumberStoresActiveAndInactiveAsync(string? searchValue, string? searchValueWithoutUnicode, int? brandId, int? kitchenCenterId, int? statusParam)
        {
            try
            {
                if (searchValue == null && searchValueWithoutUnicode != null && brandId == null && kitchenCenterId == null)
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
                    }).Where(x => x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true)).AsQueryable().Count();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null && brandId == null && kitchenCenterId == null)
                {
                    return await this._dbContext.Stores.Where(x => x.Name.ToLower().Contains(searchValue.ToLower()) && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true)).CountAsync();
                }
                else if (searchValue == null && searchValueWithoutUnicode != null && brandId != null && kitchenCenterId == null)
                {
                    return this._dbContext.Stores.Include(x => x.Brand).Where(delegate (Store store)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).Where(x => x.Brand.BrandId == brandId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true)).AsQueryable().Count();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null && brandId != null && kitchenCenterId == null)
                {
                    return await this._dbContext.Stores.Include(x => x.Brand).Where(x => x.Name.ToLower().Contains(searchValue.ToLower()) &&
                                                                                         x.Brand.BrandId == brandId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true)).CountAsync();
                }
                else if (searchValue == null && searchValueWithoutUnicode == null && brandId != null && kitchenCenterId == null)
                {
                    return await this._dbContext.Stores.Include(x => x.Brand).Where(x => x.Brand.BrandId == brandId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true)).CountAsync();
                }
                else if (searchValue == null && searchValueWithoutUnicode != null && brandId == null && kitchenCenterId != null)
                {
                    return this._dbContext.Stores.Include(x => x.KitchenCenter).Where(delegate (Store store)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true)).AsQueryable().Count();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null && brandId == null && kitchenCenterId != null)
                {
                    return await this._dbContext.Stores.Include(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId)
                                                       .Where(x => x.Name.ToLower().Contains(searchValue.ToLower())
                                                                && x.KitchenCenter.KitchenCenterId == kitchenCenterId
                                                                && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true)).CountAsync();
                }
                else if (searchValue == null && searchValueWithoutUnicode == null && brandId == null && kitchenCenterId != null)
                {
                    return await this._dbContext.Stores.Include(x => x.KitchenCenter)
                                                       .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                       .CountAsync();
                }
                else if (searchValue == null && searchValueWithoutUnicode == null && brandId != null && kitchenCenterId != null)
                {
                    return await this._dbContext.Stores.Include(x => x.Brand).Include(x => x.KitchenCenter)
                                                 .Where(x => x.Brand.BrandId == brandId
                                                          && x.KitchenCenter.KitchenCenterId == kitchenCenterId
                                                          && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                 .CountAsync();
                }
                else if (searchValue == null && searchValueWithoutUnicode != null && brandId != null && kitchenCenterId != null)
                {
                    return this._dbContext.Stores.Include(x => x.Brand).Include(x => x.KitchenCenter)
                        .Where(x => x.Brand.BrandId == brandId && x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                        .Where(delegate (Store store)
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
                else if (searchValue != null && searchValueWithoutUnicode == null && brandId != null && kitchenCenterId != null)
                {
                    return await this._dbContext.Stores.Include(x => x.Brand).Where(x => x.Name.ToLower().Contains(searchValue.ToLower()) &&
                                                                                         x.Brand.BrandId == brandId &&
                                                                                         x.KitchenCenter.KitchenCenterId == kitchenCenterId &&
                                                                                         x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                                             .CountAsync();
                }
                return await this._dbContext.Stores.Where(x => x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true)).CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Store>> GetStoresActiveAndInactiveAsync(string? searchValue, string? searchValueWithoutUnicode,
            int currentPage, int itemsPerPage, string? sortByASC, string? sortByDESC, int? brandId, int? kitchenCenterId, int? statusParam, bool? isGetAll)
        {
            try
            {
                if (isGetAll != null && isGetAll == true && brandId != null)
                {
                    return await this._dbContext.Stores.Include(x => x.Wallet)
                                                   .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                   .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                   .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                   .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                   .Where(x => x.Brand.BrandId == brandId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true) &&
                                                   /*(brandId != null ? x.Brand.BrandId == brandId : true) &&*/
                                                   (kitchenCenterId != null ? x.KitchenCenter.KitchenCenterId == kitchenCenterId : true))
                                                   .ToListAsync();
                }

                if (searchValue == null && searchValueWithoutUnicode != null && brandId == null && kitchenCenterId == null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                     .Where(delegate (Store store)
                                                     {
                                                         if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                         {
                                                             return true;
                                                         }
                                                         else
                                                         {
                                                             return false;
                                                         }
                                                     })
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                                then => then.OrderBy(x => x.Name))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("storemanageremail"),
                                                                then => then.OrderBy(x => x.StoreManagerEmail))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                                then => then.OrderBy(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                     .Where(delegate (Store store)
                                                     {
                                                         if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                         {
                                                             return true;
                                                         }
                                                         else
                                                         {
                                                             return false;
                                                         }
                                                     })
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                                then => then.OrderByDescending(x => x.Name))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("storemanageremail"),
                                                                then => then.OrderByDescending(x => x.StoreManagerEmail))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                                then => then.OrderByDescending(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    return this._dbContext.Stores.Include(x => x.Wallet)
                        .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                 .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                 .Where(delegate (Store store)
                                                 {
                                                     if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                     {
                                                         return true;
                                                     }
                                                     else
                                                     {
                                                         return false;
                                                     }
                                                 })
                                                    .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null && brandId == null && kitchenCenterId == null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Name.ToLower().Contains(searchValue) && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                              then => then.OrderBy(x => x.Name))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("storemanageremail"),
                                                              then => then.OrderBy(x => x.StoreManagerEmail))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                              then => then.OrderBy(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Name.ToLower().Contains(searchValue) && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                              then => then.OrderByDescending(x => x.Name))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("storemanageremail"),
                                                              then => then.OrderByDescending(x => x.StoreManagerEmail))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                              then => then.OrderByDescending(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    return this._dbContext.Stores.Include(x => x.Wallet)
                        .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.Name.ToLower().Contains(searchValue) && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                 .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                else if (searchValue == null && searchValueWithoutUnicode != null && brandId != null && kitchenCenterId == null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Brand.BrandId == brandId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                     .Where(delegate (Store store)
                                                     {
                                                         if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                         {
                                                             return true;
                                                         }
                                                         else
                                                         {
                                                             return false;
                                                         }
                                                     })
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                           then => then.OrderBy(x => x.Name))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("storemanageremail"),
                                                           then => then.OrderBy(x => x.StoreManagerEmail))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                           then => then.OrderBy(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                     .Where(delegate (Store store)
                                                     {
                                                         if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                         {
                                                             return true;
                                                         }
                                                         else
                                                         {
                                                             return false;
                                                         }
                                                     })
                                                 .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                       then => then.OrderByDescending(x => x.Name))
                                                 .If(sortByDESC != null && sortByDESC.ToLower().Equals("storemanageremail"),
                                                       then => then.OrderByDescending(x => x.StoreManagerEmail))
                                                 .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                       then => then.OrderByDescending(x => x.Status).Reverse())
                                                 .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    return this._dbContext.Stores.Include(x => x.Wallet)
                        .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.Brand.BrandId == brandId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                 .Where(delegate (Store store)
                                                 {
                                                     if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                     {
                                                         return true;
                                                     }
                                                     else
                                                     {
                                                         return false;
                                                     }
                                                 }).Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null && brandId != null && kitchenCenterId == null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Name.ToLower().Contains(searchValue) && x.Brand.BrandId == brandId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                           then => then.OrderBy(x => x.Name))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("storemanageremail"),
                                                           then => then.OrderBy(x => x.StoreManagerEmail))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                           then => then.OrderBy(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Name.ToLower().Contains(searchValue) && x.Brand.BrandId == brandId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                           then => then.OrderByDescending(x => x.Name))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("storemanageremail"),
                                                           then => then.OrderByDescending(x => x.StoreManagerEmail))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                           then => then.OrderByDescending(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    return this._dbContext.Stores.Include(x => x.Wallet)
                        .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.Name.ToLower().Contains(searchValue) && x.Brand.BrandId == brandId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                 .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                else if (searchValue == null && searchValueWithoutUnicode == null && brandId != null && kitchenCenterId == null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Brand.BrandId == brandId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                               then => then.OrderBy(x => x.Name))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("storemanageremail"),
                                                               then => then.OrderBy(x => x.StoreManagerEmail))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                               then => then.OrderBy(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Brand.BrandId == brandId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                               then => then.OrderByDescending(x => x.Name))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("storemanageremail"),
                                                               then => then.OrderByDescending(x => x.StoreManagerEmail))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                               then => then.OrderByDescending(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    return this._dbContext.Stores.Include(x => x.Wallet)
                        .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.Brand.BrandId == brandId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                 .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                else if (searchValue == null && searchValueWithoutUnicode != null && brandId == null && kitchenCenterId != null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                     .Where(delegate (Store store)
                                                     {
                                                         if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                         {
                                                             return true;
                                                         }
                                                         else
                                                         {
                                                             return false;
                                                         }
                                                     })
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                               then => then.OrderBy(x => x.Name))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("storemanageremail"),
                                                               then => then.OrderBy(x => x.StoreManagerEmail))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                               then => then.OrderBy(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                     .Where(delegate (Store store)
                                                     {
                                                         if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                         {
                                                             return true;
                                                         }
                                                         else
                                                         {
                                                             return false;
                                                         }
                                                     })
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                               then => then.OrderByDescending(x => x.Name))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("storemanageremail"),
                                                               then => then.OrderByDescending(x => x.StoreManagerEmail))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                               then => then.OrderByDescending(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    return this._dbContext.Stores.Include(x => x.Wallet)
                        .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                 .Where(delegate (Store store)
                                                 {
                                                     if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                     {
                                                         return true;
                                                     }
                                                     else
                                                     {
                                                         return false;
                                                     }
                                                 }).Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null && brandId == null && kitchenCenterId != null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Name.ToLower().Contains(searchValue) && x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                          then => then.OrderBy(x => x.Name))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("storemanageremail"),
                                                          then => then.OrderBy(x => x.StoreManagerEmail))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                          then => then.OrderBy(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Name.ToLower().Contains(searchValue) && x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                           then => then.OrderByDescending(x => x.Name))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("storemanageremail"),
                                                           then => then.OrderByDescending(x => x.StoreManagerEmail))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                           then => then.OrderByDescending(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    return this._dbContext.Stores.Include(x => x.Wallet)
                        .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.Name.ToLower().Contains(searchValue) && x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                 .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                else if (searchValue == null && searchValueWithoutUnicode == null && brandId == null && kitchenCenterId != null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                             then => then.OrderBy(x => x.Name))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("storemanageremail"),
                                                             then => then.OrderBy(x => x.StoreManagerEmail))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                             then => then.OrderBy(x => x.Status).Reverse())
                                                     .Take(itemsPerPage).Skip(itemsPerPage * (currentPage - 1)).ToList();
                    else if (sortByDESC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                          then => then.OrderByDescending(x => x.Name))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("storemanageremail"),
                                                          then => then.OrderByDescending(x => x.StoreManagerEmail))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                          then => then.OrderByDescending(x => x.Status).Reverse())
                                                     .Take(itemsPerPage).Skip(itemsPerPage * (currentPage - 1)).ToList();

                    return this._dbContext.Stores.Include(x => x.Wallet)
                        .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                 .Take(itemsPerPage).Skip(itemsPerPage * (currentPage - 1)).ToList();
                }
                else if (searchValue == null && searchValueWithoutUnicode == null && brandId != null && kitchenCenterId != null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Brand.BrandId == brandId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                             then => then.OrderBy(x => x.Name))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("storemanageremail"),
                                                             then => then.OrderBy(x => x.StoreManagerEmail))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                             then => then.OrderBy(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Brand.BrandId == brandId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                          then => then.OrderByDescending(x => x.Name))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("storemanageremail"),
                                                          then => then.OrderByDescending(x => x.StoreManagerEmail))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                          then => then.OrderByDescending(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    return this._dbContext.Stores.Include(x => x.Wallet)
                        .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Brand.BrandId == brandId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                 .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                else if (searchValue == null && searchValueWithoutUnicode != null && brandId != null && kitchenCenterId != null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Brand.BrandId == brandId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                     .Where(delegate (Store store)
                                                     {
                                                         if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                         {
                                                             return true;
                                                         }
                                                         else
                                                         {
                                                             return false;
                                                         }
                                                     })
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                             then => then.OrderBy(x => x.Name))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("storemanageremail"),
                                                             then => then.OrderBy(x => x.StoreManagerEmail))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                             then => then.OrderBy(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Brand.BrandId == brandId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                     .Where(delegate (Store store)
                                                     {
                                                         if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                         {
                                                             return true;
                                                         }
                                                         else
                                                         {
                                                             return false;
                                                         }
                                                     })
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                            then => then.OrderByDescending(x => x.Name))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("storemanageremail"),
                                                            then => then.OrderByDescending(x => x.StoreManagerEmail))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                            then => then.OrderByDescending(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    return this._dbContext.Stores.Include(x => x.Wallet)
                        .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Brand.BrandId == brandId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                 .Where(delegate (Store store)
                                                 {
                                                     if (StringUtil.RemoveSign4VietnameseString(store.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                     {
                                                         return true;
                                                     }
                                                     else
                                                     {
                                                         return false;
                                                     }
                                                 }).Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null && brandId != null && kitchenCenterId != null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Name.ToLower().Contains(searchValue) && x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Brand.BrandId == brandId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                           then => then.OrderBy(x => x.Name))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("storemanageremail"),
                                                           then => then.OrderBy(x => x.StoreManagerEmail))
                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                           then => then.OrderBy(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Stores.Include(x => x.Wallet)
                            .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                     .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                     .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                     .Where(x => x.Name.ToLower().Contains(searchValue) && x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Brand.BrandId == brandId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                           then => then.OrderByDescending(x => x.Name))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("storemanageremail"),
                                                           then => then.OrderByDescending(x => x.StoreManagerEmail))
                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                           then => then.OrderByDescending(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    return this._dbContext.Stores.Include(x => x.Wallet)
                        .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.StorePartners).ThenInclude(x => x.Partner)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.Name.ToLower().Contains(searchValue) && x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Brand.BrandId == brandId && x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                 .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                return await this._dbContext.Stores.Include(x => x.Wallet)
                    .Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                   .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                   .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                   .Where(x => x.Status == (int)StoreEnum.Status.ACTIVE || x.Status == (int)StoreEnum.Status.INACTIVE && (statusParam != null ? x.Status == statusParam : true))
                                                   .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}


