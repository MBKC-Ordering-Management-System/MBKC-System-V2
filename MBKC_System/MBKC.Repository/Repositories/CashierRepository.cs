using MBKC.Repository.DBContext;
using MBKC.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Repositories
{
    public class CashierRepository
    {
        private MBKCDbContext _dbContext;
        public CashierRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        /*public async Task<List<Cashier>> GetCashiersAsync(string? searchValue, string? searchValueWithoutUnicode,
            int currentPage, int itemsPerPage, List<string>? sortByAsync, List<string>? sortByDesc)
        {
            try
            {
                if(searchValue is null && searchValueWithoutUnicode is not null)
                {

                } else if(searchValue is not null && searchValueWithoutUnicode is null)
                {

                }
                return await this._dbContext.Cashiers.OrderBy(x => x, Comparer<Cashier>.Create((a, b) =>
                {
                    if(sortByAsync is not null)
                    {
                        foreach (var propertyName in sortByAsync)
                        {
                            switch (propertyName.ToLower())
                            {
                                case "fullname":
                                    {
                                        return a.FullName.CompareTo(b.FullName);
                                    }
                                case "gender":
                                    {
                                        return a.Gender.CompareTo(b.Gender);
                                    }
                                case "dateofbirth":
                                    {
                                        return a.DateOfBirth.CompareTo()
                                    }
                            }
                        }
                    }
                    return a.AccountId.CompareTo(b.AccountId);
                })).ToListAsync();
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }*/
    }
}
