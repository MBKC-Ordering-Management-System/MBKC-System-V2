using MBKC.Repository.DBContext;
using MBKC.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace MBKC.Repository.Repositories
{
    public class CashierRepository
    {
        private MBKCDbContext _dbContext;
        public CashierRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<List<Cashier>> GetCashiersAsync(string? searchValue, string? searchValueWithoutUnicode,
            int currentPage, int itemsPerPage, string? sortByASC, string? sortByDESC)
        {
            try
            {
                if (searchValue is null && searchValueWithoutUnicode is not null)
                {

                }
                else if (searchValue is not null && searchValueWithoutUnicode is null)
                {

                }
                return await this._dbContext.Cashiers.OrderBy(x => (sortByASC != null && sortByASC.ToLower().Equals("fullname") 
                                                             ? x.FullName
                                                             : null
                                                             )).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Cashier> GetCashierAsync(string email)
        {
            try
            {
                return await this._dbContext.Cashiers.Include(x => x.Account).SingleOrDefaultAsync(x => x.Account.Email.Equals(email));
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        
        public async Task<Cashier> GetCashierWithCitizenNumberAsync(string citizenNumber)
        {
            try
            {
                return await this._dbContext.Cashiers.SingleOrDefaultAsync(x => x.CitizenNumber.Equals(citizenNumber));
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CreateCashierAsync(Cashier cashier)
        {
            try
            {
                await this._dbContext.Cashiers.AddAsync(cashier);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        } 
    }
}
