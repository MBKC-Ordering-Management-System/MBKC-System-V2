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
                                              .Include(brand => brand.Stores)
                                              .SingleOrDefaultAsync(b => b.BrandId == id && b.Status != (int)BrandEnum.Status.DEACTIVE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Brands
        public async Task<List<Brand>> GetBrandsAsync(string? keySearchNameUniCode, string? keySearchNameNotUniCode, int? keyStatusFilter, int? itemsPerPage, int? currentPage)
        {
            try
            {
                if (itemsPerPage != null && currentPage != null)
                {
                    if (keySearchNameUniCode == null && keySearchNameNotUniCode != null && keyStatusFilter == null)
                    {
                        return this._dbContext.Brands.Include(brand => brand.BrandAccounts)
                                                     .ThenInclude(brandAccount => brandAccount.Account)
                                                     .ThenInclude(account => account.Role)
                                                     .Where(delegate (Brand brand)
                                                     {
                                                         if (StringUtil.RemoveSign4VietnameseString(brand.Name.ToLower()).Contains(keySearchNameNotUniCode.ToLower()))
                                                         {
                                                             return true;
                                                         }
                                                         else
                                                         {
                                                             return false;
                                                         }
                                                     }).Where(x => x.Status != (int)BrandEnum.Status.DEACTIVE)
                                                     .Skip(itemsPerPage.Value * (currentPage.Value - 1)).Take(itemsPerPage.Value).AsQueryable().ToList();
                    }
                    else if (keySearchNameUniCode == null && keySearchNameNotUniCode != null && keyStatusFilter != null)
                    {
                        return this._dbContext.Brands.Include(brand => brand.BrandAccounts)
                                                     .ThenInclude(brandAccount => brandAccount.Account)
                                                     .ThenInclude(account => account.Role)
                                                     .Where(delegate (Brand brand)
                                                     {
                                                         if (StringUtil.RemoveSign4VietnameseString(brand.Name.ToLower()).Contains(keySearchNameNotUniCode.ToLower()))
                                                         {
                                                             return true;
                                                         }
                                                         else
                                                         {
                                                             return false;
                                                         }
                                                     }).Where(x => x.Status == keyStatusFilter && x.Status != (int)BrandEnum.Status.DEACTIVE)
                                                       .Skip(itemsPerPage.Value * (currentPage.Value - 1)).Take(itemsPerPage.Value).AsQueryable().ToList();
                    }
                    else if (keySearchNameUniCode != null && keySearchNameNotUniCode == null && keyStatusFilter == null)
                    {
                        return await this._dbContext.Brands.Include(brand => brand.BrandAccounts)
                                                           .ThenInclude(brandAccount => brandAccount.Account)
                                                           .ThenInclude(account => account.Role)
                                                           .Where(x => x.Name.ToLower().Contains(keySearchNameUniCode.ToLower()) && x.Status != (int)BrandEnum.Status.DEACTIVE)
                                                           .Skip(itemsPerPage.Value * (currentPage.Value - 1)).Take(itemsPerPage.Value).ToListAsync();
                    }
                    else if (keySearchNameUniCode != null && keySearchNameNotUniCode == null && keyStatusFilter != null)
                    {
                        return await this._dbContext.Brands.Include(brand => brand.BrandAccounts)
                                                           .ThenInclude(brandAccount => brandAccount.Account)
                                                           .ThenInclude(account => account.Role)
                                                           .Where(x => x.Name.ToLower().Contains(keySearchNameUniCode.ToLower()) && x.Status == keyStatusFilter)
                                                           .Skip(itemsPerPage.Value * (currentPage.Value - 1)).Take(itemsPerPage.Value).ToListAsync();
                    }

                    else if (keySearchNameUniCode == null && keySearchNameNotUniCode == null && keyStatusFilter != null)
                    {
                        return await this._dbContext.Brands.Include(brand => brand.BrandAccounts)
                                                           .ThenInclude(brandAccount => brandAccount.Account)
                                                           .ThenInclude(account => account.Role)
                                                           .Where(x => x.Status == keyStatusFilter)
                                                           .Skip(itemsPerPage.Value * (currentPage.Value - 1)).Take(itemsPerPage.Value).ToListAsync();
                    }

                    return await this._dbContext.Brands.Include(brand => brand.BrandAccounts).ThenInclude(brandAccount => brandAccount.Account).ThenInclude(account => account.Role)
                                                       .Where(x => x.Status != (int)BrandEnum.Status.DEACTIVE)
                                                       .Skip(itemsPerPage.Value * (currentPage.Value - 1)).Take(itemsPerPage.Value).ToListAsync();
                }
                if (keySearchNameUniCode == null && keySearchNameNotUniCode != null && keyStatusFilter == null)
                {
                    return this._dbContext.Brands.Include(brand => brand.BrandAccounts)
                                                 .ThenInclude(brandAccount => brandAccount.Account)
                                                 .ThenInclude(account => account.Role)
                                                 .Where(delegate (Brand brand)
                                                 {
                                                     if (StringUtil.RemoveSign4VietnameseString(brand.Name.ToLower()).Contains(keySearchNameNotUniCode.ToLower()))
                                                     {
                                                         return true;
                                                     }
                                                     else
                                                     {
                                                         return false;
                                                     }
                                                 }).Where(x => x.Status != (int)BrandEnum.Status.DEACTIVE).AsQueryable().ToList();
                }
                else if (keySearchNameUniCode == null && keySearchNameNotUniCode != null && keyStatusFilter != null)
                {
                    return this._dbContext.Brands.Include(brand => brand.BrandAccounts)
                                                 .ThenInclude(brandAccount => brandAccount.Account)
                                                 .ThenInclude(account => account.Role)
                                                 .Where(delegate (Brand brand)
                                                 {
                                                     if (StringUtil.RemoveSign4VietnameseString(brand.Name.ToLower()).Contains(keySearchNameNotUniCode.ToLower()))
                                                     {
                                                         return true;
                                                     }
                                                     else
                                                     {
                                                         return false;
                                                     }
                                                 }).Where(x => x.Status == keyStatusFilter).AsQueryable().ToList();
                }
                else if (keySearchNameUniCode != null && keySearchNameNotUniCode == null && keyStatusFilter == null)
                {
                    return await this._dbContext.Brands.Include(brand => brand.BrandAccounts)
                                                       .ThenInclude(brandAccount => brandAccount.Account)
                                                       .ThenInclude(account => account.Role)
                                                       .Where(x => x.Name.ToLower().Contains(keySearchNameUniCode.ToLower()) && x.Status != (int)BrandEnum.Status.DEACTIVE)
                                                       .ToListAsync();
                }
                else if (keySearchNameUniCode != null && keySearchNameNotUniCode == null && keyStatusFilter != null)
                {
                    return await this._dbContext.Brands.Include(brand => brand.BrandAccounts)
                                                       .ThenInclude(brandAccount => brandAccount.Account)
                                                       .ThenInclude(account => account.Role)
                                                       .Where(x => x.Name.ToLower().Contains(keySearchNameUniCode.ToLower()) && x.Status == keyStatusFilter)
                                                       .ToListAsync();
                }

                else if (keySearchNameUniCode == null && keySearchNameNotUniCode == null && keyStatusFilter != null)
                {
                    return await this._dbContext.Brands.Include(brand => brand.BrandAccounts)
                                                       .ThenInclude(brandAccount => brandAccount.Account)
                                                       .ThenInclude(account => account.Role)
                                                       .Where(x => x.Status == keyStatusFilter)
                                                       .ToListAsync();
                }

                return await this._dbContext.Brands.Include(brand => brand.BrandAccounts).ThenInclude(brandAccount => brandAccount.Account).ThenInclude(account => account.Role)
                                                   .Where(x => x.Status != (int)BrandEnum.Status.DEACTIVE)
                                                   .ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Number Brands
        public async Task<int> GetNumberBrandsAsync(string? keySearchUniCode, string? keySearchNotUniCode, int? keyStatusFilter)
        {
            try
            {
                if (keySearchUniCode == null && keySearchNotUniCode != null && keyStatusFilter == null)
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
                    }).Where(x => x.Status != (int)BrandEnum.Status.DEACTIVE).AsQueryable().Count();
                }
                else if (keySearchUniCode == null && keySearchNotUniCode != null && keyStatusFilter != null)
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
                    }).Where(b => b.Status == keyStatusFilter).AsQueryable().Count();
                }
                else if (keySearchUniCode != null && keySearchNotUniCode == null && keyStatusFilter == null)
                {
                    return await this._dbContext.Brands.Where(x => x.Name.ToLower().Contains(keySearchUniCode.ToLower()) && x.Status != (int)BrandEnum.Status.DEACTIVE).CountAsync();
                }
                else if (keySearchUniCode != null && keySearchNotUniCode == null && keyStatusFilter != null)
                {
                    return await this._dbContext.Brands.Where(x => x.Name.ToLower().Contains(keySearchUniCode.ToLower()) && x.Status == keyStatusFilter).CountAsync();
                }
                else if (keySearchUniCode == null && keySearchNotUniCode == null && keyStatusFilter != null)
                {
                    return await this._dbContext.Brands.Where(x => x.Status == keyStatusFilter).CountAsync();
                }
                return await this._dbContext.Brands.Where(x => x.Status != (int)BrandEnum.Status.DEACTIVE).CountAsync();

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
                return await this._dbContext.Brands.Include(x => x.Stores)
                                                    .FirstOrDefaultAsync(x => x.BrandManagerEmail.Equals(managerEmail) && 
                                                                              x.Status != (int)BrandEnum.Status.DEACTIVE);
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
