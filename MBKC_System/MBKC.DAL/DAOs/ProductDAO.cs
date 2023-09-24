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
    public class ProductDAO
    {
        private MBKCDbContext _dbContext;
        public ProductDAO(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        #region Get Products By Category Id
        public async Task<List<Product>> GetProductsByCategoryIdAsync(int categoryId)
        {
            try
            {
                return await _dbContext.Products.Where(p => p.Category.CategoryId == categoryId).ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
