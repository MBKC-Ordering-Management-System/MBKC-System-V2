using MBKC.DAL.DBContext;
using MBKC.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.DAOs
{
    public class CategoryRepository
    {
        private MBKCDbContext _dbContext;
        public CategoryRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
    }
}
