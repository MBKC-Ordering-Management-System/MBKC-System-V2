using MBKC.Repository.DBContext;
using MBKC.Repository.Enums;
using MBKC.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Repositories
{

    public class PartnerRepository
    {
        private MBKCDbContext _dbContext;
        public PartnerRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<Partner> GetPartnerAsync(int partnerId)
        {
            try
            {
                return await this._dbContext.Partners.FirstOrDefaultAsync(p => p.PartnerId == partnerId && p.Status != (int)PartnerEnum.Status.DEACTIVE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
