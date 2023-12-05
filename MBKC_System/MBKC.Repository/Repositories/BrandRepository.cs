using MBKC.Repository.Utils;
using MBKC.Repository.DBContext;
using MBKC.Repository.Enums;
using MBKC.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Repositories
{
    public class BrandRepository
    {
        private MBKCDbContext _dbContext;
        public BrandRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        #region Create Brand
        public async Task CreateBrandAsync(Brand brand)
        {
            try
            {
                await this._dbContext.Brands.AddAsync(brand);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Update Brand
        public void UpdateBrand(Brand brand)
        {
            try
            {
                this._dbContext.Entry<Brand>(brand).State = EntityState.Modified;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Brand By Id
        public async Task<Brand> GetBrandByIdAsync(int id)
        {
            try
            {
                return await _dbContext.Brands.Include(brand => brand.BrandAccounts)
                                              .ThenInclude(brandAccount => brandAccount.Account)
                                              .ThenInclude(account => account.Role)
                                              .Include(brand => brand.Categories)
                                              .ThenInclude(category => category.ExtraCategoryProductCategories)
                                              .Include(brand => brand.Products)
                                              .Include(brand => brand.Stores).ThenInclude(store => store.KitchenCenter)
                                              .Include(x => x.Stores).ThenInclude(x => x.StorePartners).ThenInclude(x => x.PartnerProducts)
                                              .SingleOrDefaultAsync(b => b.BrandId == id && b.Status != (int)BrandEnum.Status.DISABLE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Brands
        public async Task<List<Brand>> GetBrandsAsync(string? searchValue, string? searchValueWithoutUnicode,
            int currentPage, int itemsPerPage, string? sortByASC, string? sortByDESC)
        {
            try
            {
                if (searchValue == null && searchValueWithoutUnicode is not null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Brands.Include(brand => brand.BrandAccounts)
                                                     .ThenInclude(brandAccount => brandAccount.Account)
                                                     .ThenInclude(account => account.Role)
                                                     .Where(delegate (Brand brand)
                                                     {
                                                         if (StringUtil.RemoveSign4VietnameseString(brand.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                         {
                                                             return true;
                                                         }
                                                         else
                                                         {
                                                             return false;
                                                         }
                                                     }).Where(x => x.Status != (int)BrandEnum.Status.DISABLE)
                                                       .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                                    then => then.OrderBy(x => x.Name))
                                                       .If(sortByASC != null && sortByASC.ToLower().Equals("address"),
                                                                then => then.OrderBy(x => x.Address))
                                                       .If(sortByASC != null && sortByASC.ToLower().Equals("brandmanageremail"),
                                                                then => then.OrderBy(x => x.BrandManagerEmail))
                                                       .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                                  then => then.OrderBy(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                    else if (sortByDESC is not null)
                        return this._dbContext.Brands.Include(brand => brand.BrandAccounts)
                                                     .ThenInclude(brandAccount => brandAccount.Account)
                                                     .ThenInclude(account => account.Role)
                                                     .Where(delegate (Brand brand)
                                                     {
                                                         if (StringUtil.RemoveSign4VietnameseString(brand.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                         {
                                                             return true;
                                                         }
                                                         else
                                                         {
                                                             return false;
                                                         }
                                                     }).Where(x => x.Status != (int)BrandEnum.Status.DISABLE)
                                                       .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                                    then => then.OrderByDescending(x => x.Name))
                                                       .If(sortByDESC != null && sortByDESC.ToLower().Equals("address"),
                                                                then => then.OrderByDescending(x => x.Address))
                                                       .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                                then => then.OrderByDescending(x => x.Status))
                                                       .If(sortByDESC != null && sortByDESC.ToLower().Equals("brandmanageremail"),
                                                                then => then.OrderByDescending(x => x.BrandManagerEmail))
                                                       .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                                    then => then.OrderByDescending(x => x.Status).Reverse())
                                                       .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    return this._dbContext.Brands.Include(brand => brand.BrandAccounts)
                                                 .ThenInclude(brandAccount => brandAccount.Account)
                                                 .ThenInclude(account => account.Role)
                                                 .Where(delegate (Brand brand)
                                                     {
                                                         if (StringUtil.RemoveSign4VietnameseString(brand.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                                                         {
                                                             return true;
                                                         }
                                                         else
                                                         {
                                                             return false;
                                                         }
                                                     }).Where(x => x.Status != (int)BrandEnum.Status.DISABLE)
                                                       .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }

                else if (searchValue != null && searchValueWithoutUnicode == null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Brands.Include(brand => brand.BrandAccounts)
                                                       .ThenInclude(brandAccount => brandAccount.Account)
                                                       .ThenInclude(account => account.Role)
                                                       .Where(x => x.Name.ToLower().Contains(searchValue.ToLower()) && x.Status != (int)BrandEnum.Status.DISABLE)
                                                       .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                                    then => then.OrderBy(x => x.Name))
                                                       .If(sortByASC != null && sortByASC.ToLower().Equals("address"),
                                                                then => then.OrderBy(x => x.Address))
                                                       .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                                then => then.OrderBy(x => x.Status))
                                                       .If(sortByASC != null && sortByASC.ToLower().Equals("brandmanageremail"),
                                                                then => then.OrderBy(x => x.BrandManagerEmail))
                                                       .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                                  then => then.OrderBy(x => x.Status).Reverse())
                                                       .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Brands.Include(brand => brand.BrandAccounts)
                                                      .ThenInclude(brandAccount => brandAccount.Account)
                                                      .ThenInclude(account => account.Role)
                                                      .Where(x => x.Name.ToLower().Contains(searchValue.ToLower()) && x.Status != (int)BrandEnum.Status.DISABLE)
                                                      .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                                    then => then.OrderByDescending(x => x.Name))
                                                      .If(sortByDESC != null && sortByDESC.ToLower().Equals("address"),
                                                               then => then.OrderByDescending(x => x.Address))
                                                      .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                               then => then.OrderByDescending(x => x.Status))
                                                      .If(sortByDESC != null && sortByDESC.ToLower().Equals("brandmanageremail"),
                                                               then => then.OrderByDescending(x => x.BrandManagerEmail))
                                                      .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                                    then => then.OrderByDescending(x => x.Status).Reverse())
                                                      .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();


                    return await this._dbContext.Brands.Include(brand => brand.BrandAccounts)
                                                       .ThenInclude(brandAccount => brandAccount.Account)
                                                       .ThenInclude(account => account.Role)
                                                       .Where(x => x.Name.ToLower().Contains(searchValue.ToLower()) && x.Status != (int)BrandEnum.Status.DISABLE)
                                                       .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
                }

                if (sortByASC is not null)
                    return this._dbContext.Brands.Include(brand => brand.BrandAccounts)
                                                 .ThenInclude(brandAccount => brandAccount.Account)
                                                 .ThenInclude(account => account.Role)
                                                 .Where(x => x.Status != (int)BrandEnum.Status.DISABLE)
                                                 .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                              then => then.OrderBy(x => x.Name))
                                                 .If(sortByASC != null && sortByASC.ToLower().Equals("address"),
                                                          then => then.OrderBy(x => x.Address))
                                                 .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                          then => then.OrderBy(x => x.Status))
                                                 .If(sortByASC != null && sortByASC.ToLower().Equals("brandmanageremail"),
                                                          then => then.OrderBy(x => x.BrandManagerEmail))
                                                 .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                            then => then.OrderBy(x => x.Status).Reverse())
                                                 .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                else if (sortByDESC is not null)
                    return this._dbContext.Brands.Include(brand => brand.BrandAccounts)
                                                 .ThenInclude(brandAccount => brandAccount.Account)
                                                 .ThenInclude(account => account.Role)
                                                 .Where(x => x.Status != (int)BrandEnum.Status.DISABLE)
                                                 .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                               then => then.OrderByDescending(x => x.Name))
                                                 .If(sortByDESC != null && sortByDESC.ToLower().Equals("address"),
                                                          then => then.OrderByDescending(x => x.Address))
                                                 .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                          then => then.OrderByDescending(x => x.Status))
                                                 .If(sortByDESC != null && sortByDESC.ToLower().Equals("brandmanageremail"),
                                                          then => then.OrderByDescending(x => x.BrandManagerEmail))
                                                 .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                               then => then.OrderByDescending(x => x.Status).Reverse())
                                                 .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                return await this._dbContext.Brands.Include(brand => brand.BrandAccounts)
                                                   .ThenInclude(brandAccount => brandAccount.Account)
                                                   .ThenInclude(account => account.Role)
                                                   .Where(x => x.Status != (int)BrandEnum.Status.DISABLE)
                                                   .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Number Brands
        public async Task<int> GetNumberBrandsAsync(string? keySearchUniCode, string? keySearchNotUniCode)
        {
            try
            {
                if (keySearchUniCode == null && keySearchNotUniCode != null)
                {
                    return this._dbContext.Brands.Where(delegate (Brand brand)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(brand.Name.ToLower()).Contains(keySearchNotUniCode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).Where(x => x.Status != (int)BrandEnum.Status.DISABLE).AsQueryable().Count();
                }
                else if (keySearchUniCode != null && keySearchNotUniCode == null)
                {
                    return await this._dbContext.Brands.Where(x => x.Name.ToLower().Contains(keySearchUniCode.ToLower()) && x.Status != (int)BrandEnum.Status.DISABLE).CountAsync();
                }
                return await this._dbContext.Brands.Where(x => x.Status != (int)BrandEnum.Status.DISABLE).CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        public async Task<Brand> GetBrandAsync(string managerEmail)
        {
            try
            {
                return await this._dbContext.Brands.Include(x => x.Stores).ThenInclude(x => x.Orders).ThenInclude(x => x.ShipperPayments)
                                                   .Include(x => x.Products)
                                                   .Include(x => x.Categories)
                                                   .FirstOrDefaultAsync(x => x.BrandManagerEmail.Equals(managerEmail) &&
                                                                              x.Status != (int)BrandEnum.Status.DISABLE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region Get brand by email
        public async Task<Brand?> GetBrandByEmailAsync(string managerEmail)
        {
            try
            {
                return await this._dbContext.Brands.Include(x => x.Categories)
                                                   .FirstOrDefaultAsync(b => b.BrandManagerEmail.Equals(managerEmail) && b.Status == (int)BrandEnum.Status.ACTIVE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get brand for dash board
        public async Task<Brand?> GetBrandForDashBoardAsync(string managerEmail)
        {
            try
            {
                return await this._dbContext.Brands.Include(b => b.Stores.Where(s => s.Status == (int)StoreEnum.Status.ACTIVE || s.Status == (int)StoreEnum.Status.INACTIVE)
                                                                         .OrderByDescending(s => s.Status)
                                                                         .Take(5)).ThenInclude(s => s.KitchenCenter)
                                                   .Include(b => b.Products)
                                                   .FirstOrDefaultAsync(b => b.BrandManagerEmail.Equals(managerEmail) && b.Status == (int)BrandEnum.Status.ACTIVE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region count number of brand
        public async Task<int> CountBrandNumberAsync()
        {
            try
            {
                return await _dbContext.Brands.Where(b => b.Status != (int)BrandEnum.Status.DISABLE).CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region get 5 brand with status active
        public async Task<List<Brand>> GetFiveBrandSortByActiveAsync()
        {
            try
            {
                return await _dbContext.Brands.Where(b => b.Status != (int)BrandEnum.Status.DISABLE)
                                                      .OrderByDescending(b => b.Status)
                                                      .Take(5)
                                                      .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
