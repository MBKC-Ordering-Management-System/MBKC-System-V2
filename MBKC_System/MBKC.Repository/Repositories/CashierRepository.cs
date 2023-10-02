using MBKC.Repository.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Repositories
{
    public class CashierRepository
    {
        private MBKCDbContext _dbContext;
        public CashierRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
    }
}
