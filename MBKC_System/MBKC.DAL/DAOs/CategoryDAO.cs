using MBKC.DAL.DBContext;
using MBKC.DAL.Enums;
using MBKC.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.DAOs
{
    public class CategoryDAO
    {
        private MBKCDbContext _dbContext;
        public CategoryDAO(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        #region Get Category By Code
        public async Task<Category> GetCategoryByCodeAsync(string code)
        {
            try
            {
                return await _dbContext.Categories.SingleOrDefaultAsync(c => c.Code.Equals(code));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Category By Name
        public async Task<Category> GetCategoryByNameAsync(string name)
        {
            try
            {
                return await _dbContext.Categories.SingleOrDefaultAsync(c => c.Name.Equals(name));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Category By Id
        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            try
            {
                return await _dbContext.Categories
                    .Include(c => c.ExtraCategoryProductCategories)
                    .Include(c => c.Products)
                    .SingleOrDefaultAsync(c => c.CategoryId.Equals(id));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Categories
        public async Task<List<Category>> GetCategoriesAsync()
        {
            try
            {
                return await _dbContext.Categories.Where(c => !(c.Status == (int)CategoryEnum.Status.DEACTIVE)).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Create Category 
        public async Task CreateCategoryAsyncAsync(Category category)
        {
            try
            {
                await this._dbContext.Categories.AddAsync(category);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Update Category
        public void UpdateCategory(Category category)
        {
            try
            {
                this._dbContext.Entry<Category>(category).State = EntityState.Modified;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
