using MBKC.Repository.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Repositories
{
    public class BrandPartnerRepository
    {
        private MBKCDbContext _dbContext;
        public BrandPartnerRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
    }
}
