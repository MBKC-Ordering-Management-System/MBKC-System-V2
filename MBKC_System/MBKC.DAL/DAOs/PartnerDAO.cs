using MBKC.DAL.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.DAOs
{
    public class PartnerDAO
    {
        private MBKCDbContext _dbContext;
        public PartnerDAO(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
    }
}
