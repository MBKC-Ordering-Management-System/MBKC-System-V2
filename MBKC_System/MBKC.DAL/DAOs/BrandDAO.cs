using MBKC.DAL.DBContext;
using MBKC.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.DAOs
{
    public class BrandDAO
    {
        private MBKCDbContext _dbContext;
        public BrandDAO(MBKCDbContext dbContext)
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
        public async Task<List<Brand>> GetBrandsAsync()
        {
            try
            {
                return await _dbContext.Brands.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
