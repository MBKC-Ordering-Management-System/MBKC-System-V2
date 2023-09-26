using MBKC.DAL.DBContext;
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
                                              .SingleOrDefaultAsync(b => b.BrandId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Brands
        public async Task<List<Brand>> GetBrandsAsync(string? keySearchNameUniCode, string? keySearchNameNotUniCode, int? keyStatusFilter, int itemsPerPage, int currentPage)
        {
            try
            {
                if (keySearchNameUniCode == null && keySearchNameNotUniCode != null && keyStatusFilter == null)
                {
                    return this._dbContext.Brands.AsQueryable()
                                                 .Include(brand => brand.BrandAccounts)
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
                    }).Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                else if (keySearchNameUniCode == null && keySearchNameNotUniCode != null && keyStatusFilter != null)
                {
                    return this._dbContext.Brands.AsQueryable()
                                                 .Include(brand => brand.BrandAccounts)
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
                    }).Where(x => x.Status == keyStatusFilter).Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                else if (keySearchNameUniCode != null && keySearchNameNotUniCode == null && keyStatusFilter == null)
                {
                    return await this._dbContext.Brands
                                                       .Include(brand => brand.BrandAccounts)
                                                       .ThenInclude(brandAccount => brandAccount.Account)
                                                       .ThenInclude(account => account.Role)
                                                       .Where(x => x.Name.ToLower().Contains(keySearchNameUniCode.ToLower()))
                                                       .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
                }
                else if (keySearchNameUniCode != null && keySearchNameNotUniCode == null && keyStatusFilter != null)
                {
                    return await this._dbContext.Brands
                                                       .Include(brand => brand.BrandAccounts)
                                                       .ThenInclude(brandAccount => brandAccount.Account)
                                                       .ThenInclude(account => account.Role)
                                                       .Where(x => x.Name.ToLower().Contains(keySearchNameUniCode.ToLower()) && x.Status == keyStatusFilter)
                                                       .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
                }

                else if (keySearchNameUniCode == null && keySearchNameNotUniCode == null && keyStatusFilter != null)
                {
                    return await this._dbContext.Brands
                                                       .Include(brand => brand.BrandAccounts)
                                                       .ThenInclude(brandAccount => brandAccount.Account)
                                                       .ThenInclude(account => account.Role)
                                                       .Where(x => x.Status == keyStatusFilter)
                                                       .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
                }

                return await this._dbContext.Brands
                                                       .Include(brand => brand.BrandAccounts)
                                                       .ThenInclude(brandAccount => brandAccount.Account)
                                                       .ThenInclude(account => account.Role)
                                                       .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
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
                    }).AsQueryable().Count();
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
                    return await this._dbContext.Brands.Where(x => x.Name.ToLower().Contains(keySearchUniCode.ToLower())).CountAsync();
                }
                else if (keySearchUniCode != null && keySearchNotUniCode == null && keyStatusFilter != null)
                {
                    return await this._dbContext.Brands.Where(x => x.Name.ToLower().Contains(keySearchUniCode.ToLower()) && x.Status == keyStatusFilter).CountAsync();
                }
                else if (keySearchUniCode == null && keySearchNotUniCode == null && keyStatusFilter != null)
                {
                    return await this._dbContext.Brands.Where(x => x.Status == keyStatusFilter).CountAsync();
                }
                return await this._dbContext.Brands.CountAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        public async Task<List<Brand>> GetBrandsAsync()
        {
            return await this._dbContext.Brands.ToListAsync();
        }
    }
}
