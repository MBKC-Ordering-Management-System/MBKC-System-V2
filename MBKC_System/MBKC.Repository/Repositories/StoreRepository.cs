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

        public async Task<int> GetNumberStoresAsync(string? searchValue, string? searchValueWithoutUnicode, int? brandId, int? kitchenCenterId)
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
                    }).Where(x => x.Status != (int)StoreEnum.Status.DEACTIVE).AsQueryable().Count();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null && brandId == null && kitchenCenterId == null)
                {
                    return await this._dbContext.Stores.Where(x => x.Name.ToLower().Contains(searchValue.ToLower()) && x.Status != (int)StoreEnum.Status.DEACTIVE).CountAsync();
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
                    }).Where(x => x.Brand.BrandId == brandId && x.Status != (int)StoreEnum.Status.DEACTIVE).AsQueryable().Count();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null && brandId != null && kitchenCenterId == null)
                {
                    return await this._dbContext.Stores.Include(x => x.Brand).Where(x => x.Name.ToLower().Contains(searchValue.ToLower()) &&
                                                                                         x.Brand.BrandId == brandId && x.Status != (int)BrandEnum.Status.DEACTIVE).CountAsync();
                }
                else if (searchValue == null && searchValueWithoutUnicode == null && brandId != null && kitchenCenterId == null)
                {
                    return await this._dbContext.Stores.Include(x => x.Brand).Where(x => x.Brand.BrandId == brandId && x.Status != (int)BrandEnum.Status.DEACTIVE).CountAsync();
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
                    }).Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status != (int)KitchenCenterEnum.Status.DEACTIVE).AsQueryable().Count();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null && brandId == null && kitchenCenterId != null)
                {
                    return await this._dbContext.Stores.Include(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId)
                                                       .Where(x => x.Name.ToLower().Contains(searchValue.ToLower())
                                                                && x.KitchenCenter.KitchenCenterId == kitchenCenterId
                                                                && x.Status != (int)KitchenCenterEnum.Status.DEACTIVE).CountAsync();
                }
                else if (searchValue == null && searchValueWithoutUnicode == null && brandId == null && kitchenCenterId != null)
                {
                    return await this._dbContext.Stores.Include(x => x.KitchenCenter)
                                                       .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status != (int)KitchenCenterEnum.Status.DEACTIVE)
                                                       .CountAsync();
                }
                else if (searchValue == null && searchValueWithoutUnicode == null && brandId != null && kitchenCenterId != null)
                {
                    return await this._dbContext.Stores.Include(x => x.Brand).Include(x => x.KitchenCenter)
                                                 .Where(x => x.Brand.BrandId == brandId
                                                          && x.KitchenCenter.KitchenCenterId == kitchenCenterId
                                                          && x.Status != (int)KitchenCenterEnum.Status.DEACTIVE)
                                                 .CountAsync();
                }
                else if (searchValue == null && searchValueWithoutUnicode != null && brandId != null && kitchenCenterId != null)
                {
                    return this._dbContext.Stores.Include(x => x.Brand).Include(x => x.KitchenCenter)
                        .Where(x => x.Brand.BrandId == brandId && x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status != (int)KitchenCenterEnum.Status.DEACTIVE)
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
                                                                                         x.Status != (int)KitchenCenterEnum.Status.DEACTIVE)
                                                                             .CountAsync();
                }
                return await this._dbContext.Stores.Where(x => x.Status != (int)KitchenCenterEnum.Status.DEACTIVE).CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Store>> GetStoresAsync(string? searchValue, string? searchValueWithoutUnicode, int itemsPerPage, int currentPage, int? brandId, int? kitchenCenterId)
        {
            try
            {
                if (searchValue == null && searchValueWithoutUnicode != null && brandId == null && kitchenCenterId == null)
                {
                    return this._dbContext.Stores.Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.Status != (int)StoreEnum.Status.DEACTIVE)
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
                else if (searchValue != null && searchValueWithoutUnicode == null && brandId == null && kitchenCenterId == null)
                {
                    return await this._dbContext.Stores.Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                       .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                       .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                       .Where(x => x.Name.ToLower().Contains(searchValue) && x.Status != (int)StoreEnum.Status.DEACTIVE)
                                                       .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
                }
                else if (searchValue == null && searchValueWithoutUnicode != null && brandId != null && kitchenCenterId == null)
                {
                    return this._dbContext.Stores.Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.Brand.BrandId == brandId && x.Status != (int)StoreEnum.Status.DEACTIVE)
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
                    return await this._dbContext.Stores.Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                       .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                       .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                       .Where(x => x.Name.ToLower().Contains(searchValue) && x.Brand.BrandId == brandId && x.Status != (int)StoreEnum.Status.DEACTIVE)
                                                       .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
                }
                else if (searchValue == null && searchValueWithoutUnicode == null && brandId != null && kitchenCenterId == null)
                {
                    return await this._dbContext.Stores.Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                       .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                       .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                       .Where(x => x.Brand.BrandId == brandId && x.Status != (int)StoreEnum.Status.DEACTIVE)
                                                       .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
                }
                else if (searchValue == null && searchValueWithoutUnicode != null && brandId == null && kitchenCenterId != null)
                {
                    return this._dbContext.Stores.Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status != (int)StoreEnum.Status.DEACTIVE)
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
                    return await this._dbContext.Stores.Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                      .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                      .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                      .Where(x => x.Name.ToLower().Contains(searchValue) && x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status != (int)StoreEnum.Status.DEACTIVE)
                                                      .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
                }
                else if (searchValue == null && searchValueWithoutUnicode == null && brandId == null && kitchenCenterId != null)
                {
                    return await this._dbContext.Stores.Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                       .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                       .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                       .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Status != (int)StoreEnum.Status.DEACTIVE)
                                                       .Take(itemsPerPage).Skip(itemsPerPage * (currentPage - 1)).ToListAsync();
                }
                else if (searchValue == null && searchValueWithoutUnicode == null && brandId != null && kitchenCenterId != null)
                {
                    return await this._dbContext.Stores.Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                   .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                   .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                   .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Brand.BrandId == brandId && x.Status != (int)StoreEnum.Status.DEACTIVE)
                                                   .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
                }
                else if (searchValue == null && searchValueWithoutUnicode != null && brandId != null && kitchenCenterId != null)
                {
                    return this._dbContext.Stores.Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                 .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                 .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Brand.BrandId == brandId && x.Status != (int)StoreEnum.Status.DEACTIVE)
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
                    return await this._dbContext.Stores.Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                       .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                       .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                       .Where(x => x.Name.ToLower().Contains(searchValue) && x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Brand.BrandId == brandId && x.Status != (int)StoreEnum.Status.DEACTIVE)
                                                       .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
                }
                return await this._dbContext.Stores.Include(x => x.KitchenCenter).ThenInclude(x => x.Manager)
                                                   .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                   .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                   .Where(x => x.Status != (int)StoreEnum.Status.DEACTIVE)
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
                                                   .Include(x => x.Brand).ThenInclude(x => x.BrandAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                   .Include(x => x.StoreAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                                                   .FirstOrDefaultAsync(x => x.StoreId == id && x.Status != (int)StoreEnum.Status.DEACTIVE);
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
                return await this._dbContext.Stores.Include(x => x.Brand).ThenInclude(x => x.Categories).Include(x => x.KitchenCenter).ThenInclude(x => x.Manager).SingleOrDefaultAsync(x => x.StoreManagerEmail.Equals(managerEmail));
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}


