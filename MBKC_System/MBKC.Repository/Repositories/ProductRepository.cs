using MBKC.Repository.DBContext;
using MBKC.Repository.Models;
using MBKC.Repository.Utils;
using Microsoft.EntityFrameworkCore;

namespace MBKC.Repository.Repositories
{
    public class ProductRepository
    {
        private MBKCDbContext _dbContext;
        public ProductRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        #region Get Products By Category Id
        public async Task<List<Product>> GetProductsByCategoryIdAsync(int categoryId, string? keySearchNameUniCode, string? keySearchNameNotUniCode, int itemsPerPage, int currentPage)
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
                    }).Where(p => p.Category.CategoryId == categoryId).Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                else if (keySearchNameUniCode != null && keySearchNameNotUniCode == null)
                {
                    return await this._dbContext.Products
                        .Where(p => p.Name.ToLower().Contains(keySearchNameUniCode.ToLower()) && p.Category.CategoryId == categoryId)
                        .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
                }
                return await this._dbContext.Products
                    .Where(p => p.Category.CategoryId == categoryId)
                    .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Number Products
        public async Task<int> GetNumberProductsAsync(string? keySearchUniCode, string? keySearchNotUniCode)
        {
            try
            {
                if (keySearchUniCode == null && keySearchNotUniCode != null)
                {
                    return this._dbContext.Categories.Where(delegate (Category category)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(category.Name.ToLower()).Contains(keySearchNotUniCode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).AsQueryable().Count();
                }
                else if (keySearchUniCode != null && keySearchNotUniCode == null)
                {
                    return await this._dbContext.Categories.Where(x => x.Name.ToLower().Contains(keySearchUniCode.ToLower())).CountAsync();
                }
                return await this._dbContext.Categories.CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
