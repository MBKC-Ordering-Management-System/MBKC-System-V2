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
    public class ExtraCategoryRepository
    {
        private MBKCDbContext _dbContext;
        protected readonly DbSet<ExtraCategory> _dbSet;
        public ExtraCategoryRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
            this._dbSet = dbContext.Set<ExtraCategory>();
        }

        #region Get ExtraCategoriesByCategoryId
        public async Task<List<ExtraCategory>> GetExtraCategoriesByCategoryIdAsync(int categoryId)
        {
            try
            {
                return await _dbContext.ExtraCategories
                    .Where(e => e.ProductCategoryId == categoryId && e.Status != (int)ExtraCategoryEnum.Status.DISABLE)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        public async Task InsertRangeAsync(IEnumerable<ExtraCategory> extraCategories)
        {
            try
            {
                await this._dbSet.AddRangeAsync(extraCategories);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void DeleteRange(IEnumerable<ExtraCategory> extraCategories)
        {
            try
            {
                this._dbSet.RemoveRange(extraCategories);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
