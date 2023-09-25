using MBKC.DAL.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.DAOs
{
    public class ProductRepository
    {
        private MBKCDbContext _dbContext;
        public ProductRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
    }
}
