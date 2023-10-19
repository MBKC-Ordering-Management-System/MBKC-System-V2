﻿using MBKC.Repository.DBContext;
using MBKC.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Repositories
{
   public class WalletRepository
    {
        private MBKCDbContext _dbContext;
        public WalletRepository(MBKCDbContext dbContext)

        {
            this._dbContext = dbContext;
        }

        #region update wallet
        public void UpdateWallet(Wallet wallet)
        {
            try
            {
                this._dbContext.Entry<Wallet>(wallet).State = EntityState.Modified;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

    }
}


