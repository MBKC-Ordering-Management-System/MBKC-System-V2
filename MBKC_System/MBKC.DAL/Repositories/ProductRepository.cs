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
    public class ProductRepository
    {
        private MBKCDbContext _dbContext;
        public ProductRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        #region Get Products By Category Id
        public async Task<List<Product>> GetProductsByCategoryIdAsync(int categoryId, int brandId, string? keySearchNameUniCode, string? keySearchNameNotUniCode, int itemsPerPage, int currentPage)
         {
            try
            {
                if (keySearchNameUniCode == null && keySearchNameNotUniCode != null)
                {
                    return this._dbContext.Products.Where(delegate (Product product)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(product.Name.ToLower()).Contains(keySearchNameNotUniCode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).Where(p => p.Category.CategoryId == categoryId && p.Brand.BrandId == brandId && p.Status ==(int)ProductEnum.Status.ACTIVE).Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                else if (keySearchNameUniCode != null && keySearchNameNotUniCode == null)
                {
                    return await this._dbContext.Products
                        .Where(p => p.Name.ToLower().Contains(keySearchNameUniCode.ToLower()) && p.Category.CategoryId == categoryId && p.Brand.BrandId == brandId && p.Status == (int)ProductEnum.Status.ACTIVE)
                        .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
                }
                return await this._dbContext.Products
                    .Where(p => p.Category.CategoryId == categoryId && p.Brand.BrandId == brandId && p.Status == (int)ProductEnum.Status.ACTIVE)
                    .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Number Products
        public async Task<int> GetNumberProductsAsync(string? keySearchUniCode, string? keySearchNotUniCode, int brandId, int categoryId)
        {
            try
            {
                if (keySearchUniCode == null && keySearchNotUniCode != null)
                {
                    return this._dbContext.Products.Where(delegate (Product product)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(product.Name.ToLower()).Contains(keySearchNotUniCode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).Where(p => p.Brand.BrandId == brandId && p.Category.CategoryId == categoryId && p.Status == (int)ProductEnum.Status.ACTIVE).AsQueryable().Count();
                }
                else if (keySearchUniCode != null && keySearchNotUniCode == null)
                {
                    return await this._dbContext.Products.Where(p => p.Name.ToLower().Contains(keySearchUniCode.ToLower()) && p.Brand.BrandId == brandId && p.Category.CategoryId == categoryId && p.Status == (int)ProductEnum.Status.ACTIVE).CountAsync();
                }
                return await this._dbContext.Products.CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
