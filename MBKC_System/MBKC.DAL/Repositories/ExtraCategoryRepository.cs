using MBKC.DAL.DBContext;
using MBKC.DAL.Enums;
using MBKC.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.Repositories
{
    public class ExtraCategoryRepository
    {
        public MBKCDbContext _dbContext;
        public ExtraCategoryRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        #region Get ExtraCategoriesByCategoryId
        public async Task<List<ExtraCategory>> GetExtraCategoriesByCategoryIdAsync(int categoryId)
        {
            try
            {
                return await _dbContext.ExtraCategories
                    .Where(e => e.ProductCategoryId == categoryId && e.Status == (int)ExtraCategoryEnum.Status.ACTIVE)
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
