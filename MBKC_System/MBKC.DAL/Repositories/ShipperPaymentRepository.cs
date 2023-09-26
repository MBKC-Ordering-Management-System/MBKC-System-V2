using MBKC.DAL.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.Repositories
{
    public class ShipperPaymentRepository
    {
        private MBKCDbContext _dbContext;
        public ShipperPaymentRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
    }
}
