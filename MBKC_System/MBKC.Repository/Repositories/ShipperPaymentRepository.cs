using MBKC.Repository.DBContext;
using MBKC.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Repositories
{
    public class ShipperPaymentRepository
    {
        private MBKCDbContext _dbContext;
        public ShipperPaymentRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task CreateShipperPaymentAsync(ShipperPayment shipperPayment)
        {
            try
            {
                await this._dbContext.ShipperPayments.AddAsync(shipperPayment);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
