﻿using MBKC.DAL.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.Repositories
{
    public class StoreAccountRepository
    {
        private MBKCDbContext _dbContext;
        public StoreAccountRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
    }
}