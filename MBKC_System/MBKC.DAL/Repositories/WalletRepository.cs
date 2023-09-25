using MBKC.DAL.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.DAOs
{
    public class WalletRepository
    {
        private MBKCDbContext _dbContext;
        public WalletRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
    }
}
