﻿using MBKC.Repository.DBContext;
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

        public async Task<BrandAccount> GetBrandAccountByAccountIdAsync(int accountId)
        {
            try
            {
                return await this._dbContext.BrandAccounts
                      .Include(brandAccocunt => brandAccocunt.Brand)
                      .ThenInclude(brand => brand.Products)
                      .Include(brandAccocunt => brandAccocunt.Brand)
                      .ThenInclude(brand => brand.Categories.Where(c => c.Status != (int)CategoryEnum.Status.DEACTIVE))
                      .ThenInclude(category => category.ExtraCategoryProductCategories)
                      .SingleOrDefaultAsync(b => b.AccountId == accountId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}