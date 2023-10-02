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
    public class BrandAccountRepository
    {
        private MBKCDbContext _dbContext;
        public BrandAccountRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task CreateBrandAccount(BrandAccount brandAccount)
        {
            try
            {
                await this._dbContext.BrandAccounts.AddAsync(brandAccount);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region Get Brand Account By Id
        public async Task<BrandAccount> GetBrandAccountByBrandIdAsync(int id)
        {
            try
            {
                return await _dbContext.BrandAccounts
                    .Include(b => b.Account)
                    .Where(b => b.Account.Status == (int)AccountEnum.Status.ACTIVE && b.Account.Role.RoleId == (int)RoleEnum.Role.BRAND_MANAGER)
                    .SingleOrDefaultAsync(b => b.BrandId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Brand Account By Id
        public async Task<BrandAccount> GetBrandAccountByAccountIdAsync(int id)
        {
            try
            {
                return await _dbContext.BrandAccounts
                    .Include(b => b.Account)
                    .Include(b => b.Brand)
                    .Where(b => b.Account.Status != (int)AccountEnum.Status.DEACTIVE && b.Account.Role.RoleId == (int)RoleEnum.Role.BRAND_MANAGER)
                    .SingleOrDefaultAsync(b => b.AccountId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Update BrandAccount
        public void UpdateBrandAccount(BrandAccount brandAccount)
        {
            try
            {
                this._dbContext.Entry<BrandAccount>(brandAccount).State = EntityState.Modified;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

    }
}
