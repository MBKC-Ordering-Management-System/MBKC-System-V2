using MBKC.DAL.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.Infrastructures
{
    public class DbFactory : Disposable, IDbFactory
    {
        private MBKCDbContext _dbContext;
        public DbFactory()
        {

        }

        public MBKCDbContext InitDbContext()
        {
            if (_dbContext == null)
            {
                _dbContext = new MBKCDbContext();
            }
            return _dbContext;
        }

        protected override void DisposeCore()
        {
            if (this._dbContext != null)
            {
                this._dbContext.Dispose();
            }
        }

    }
}
