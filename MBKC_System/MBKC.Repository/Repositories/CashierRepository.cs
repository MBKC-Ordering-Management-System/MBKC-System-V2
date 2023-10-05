﻿using MBKC.Repository.DBContext;
using MBKC.Repository.Enums;
using MBKC.Repository.Models;
using MBKC.Repository.Utils;
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
            int currentPage, int itemsPerPage, string? sortByASC, string? sortByDESC, int kitchenCenterId)
        {
            try
            {
                if (searchValue is null && searchValueWithoutUnicode is not null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Cashiers.Include(x => x.Account).Include(x => x.KitchenCenter)
                                                             .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Account.Status != (int)AccountEnum.Status.DEACTIVE)
                                                             .Where(delegate (Cashier cashier)
                                                             {
                                                                 if (StringUtil.RemoveSign4VietnameseString(cashier.FullName).ToLower().Contains(searchValueWithoutUnicode.ToLower())) return true;
                                                                 return false;
                                                             })
                                                             .If(sortByASC != null && sortByASC.ToLower().Equals("fullname"),
                                                                  then => then.OrderBy(x => x.FullName))
                                                             .If(sortByASC != null && sortByASC.ToLower().Equals("gender"),
                                                                  then => then.OrderBy(x => x.Gender))
                                                             .If(sortByASC != null && sortByASC.ToLower().Equals("dateofbirth"),
                                                                  then => then.OrderBy(x => x.DateOfBirth))
                                                             .If(sortByASC != null && sortByASC.ToLower().Equals("citizennumber"),
                                                                  then => then.OrderBy(x => x.CitizenNumber))
                                                             .If(sortByASC != null && sortByASC.ToLower().Equals("email"),
                                                                  then => then.OrderBy(x => x.Account.Email))
                                                             .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                                  then => then.OrderBy(x => x.Account.Status))
                                                             .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage)
                                                             .ToList();
                    else if (sortByDESC is not null)
                        return this._dbContext.Cashiers.Include(x => x.Account).Include(x => x.KitchenCenter)
                                                             .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Account.Status != (int)AccountEnum.Status.DEACTIVE)
                                                             .Where(delegate (Cashier cashier)
                                                             {
                                                                 if (StringUtil.RemoveSign4VietnameseString(cashier.FullName).ToLower().Contains(searchValueWithoutUnicode.ToLower())) return true;
                                                                 return false;
                                                             })
                                                             .If(sortByDESC != null && sortByDESC.ToLower().Equals("fullname"),
                                                                  then => then.OrderByDescending(x => x.FullName))
                                                             .If(sortByDESC != null && sortByDESC.ToLower().Equals("gender"),
                                                                  then => then.OrderByDescending(x => x.Gender))
                                                             .If(sortByDESC != null && sortByDESC.ToLower().Equals("dateofbirth"),
                                                                  then => then.OrderByDescending(x => x.DateOfBirth))
                                                             .If(sortByDESC != null && sortByDESC.ToLower().Equals("citizennumber"),
                                                                  then => then.OrderByDescending(x => x.CitizenNumber))
                                                             .If(sortByDESC != null && sortByDESC.ToLower().Equals("email"),
                                                                  then => then.OrderByDescending(x => x.Account.Email))
                                                             .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                                  then => then.OrderByDescending(x => x.Account.Status))
                                                             .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage)
                                                             .ToList();

                    return this._dbContext.Cashiers.Include(x => x.Account).Include(x => x.KitchenCenter)
                                                         .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Account.Status != (int)AccountEnum.Status.DEACTIVE)
                                                         .Where(delegate (Cashier cashier)
                                                         {
                                                             if (StringUtil.RemoveSign4VietnameseString(cashier.FullName).ToLower().Contains(searchValueWithoutUnicode.ToLower())) return true;
                                                             return false;
                                                         })
                                                         .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                else if (searchValue is not null && searchValueWithoutUnicode is null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Cashiers.Include(x => x.Account).Include(x => x.KitchenCenter)
                                                             .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Account.Status != (int)AccountEnum.Status.DEACTIVE && x.FullName.ToLower().Contains(searchValue.ToLower()))
                                                             .If(sortByASC != null && sortByASC.ToLower().Equals("fullname"),
                                                                  then => then.OrderBy(x => x.FullName))
                                                             .If(sortByASC != null && sortByASC.ToLower().Equals("gender"),
                                                                  then => then.OrderBy(x => x.Gender))
                                                             .If(sortByASC != null && sortByASC.ToLower().Equals("dateofbirth"),
                                                                  then => then.OrderBy(x => x.DateOfBirth))
                                                             .If(sortByASC != null && sortByASC.ToLower().Equals("citizennumber"),
                                                                  then => then.OrderBy(x => x.CitizenNumber))
                                                             .If(sortByASC != null && sortByASC.ToLower().Equals("email"),
                                                                  then => then.OrderBy(x => x.Account.Email))
                                                             .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                                  then => then.OrderBy(x => x.Account.Status))
                                                             .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage)
                                                             .ToList();
                    else if (sortByDESC is not null)
                        return this._dbContext.Cashiers.Include(x => x.Account).Include(x => x.KitchenCenter)
                                                             .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Account.Status != (int)AccountEnum.Status.DEACTIVE && x.FullName.ToLower().Contains(searchValue.ToLower()))
                                                             .If(sortByDESC != null && sortByDESC.ToLower().Equals("fullname"),
                                                                  then => then.OrderByDescending(x => x.FullName))
                                                             .If(sortByDESC != null && sortByDESC.ToLower().Equals("gender"),
                                                                  then => then.OrderByDescending(x => x.Gender))
                                                             .If(sortByDESC != null && sortByDESC.ToLower().Equals("dateofbirth"),
                                                                  then => then.OrderByDescending(x => x.DateOfBirth))
                                                             .If(sortByDESC != null && sortByDESC.ToLower().Equals("citizennumber"),
                                                                  then => then.OrderByDescending(x => x.CitizenNumber))
                                                             .If(sortByDESC != null && sortByDESC.ToLower().Equals("email"),
                                                                  then => then.OrderByDescending(x => x.Account.Email))
                                                             .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                                  then => then.OrderByDescending(x => x.Account.Status))
                                                             .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage)
                                                             .ToList();

                    return await this._dbContext.Cashiers.Include(x => x.Account).Include(x => x.KitchenCenter)
                                                         .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Account.Status != (int)AccountEnum.Status.DEACTIVE && x.FullName.ToLower().Contains(searchValue.ToLower()))
                                                         .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
                }

                if (sortByASC is not null)
                    return this._dbContext.Cashiers.Include(x => x.Account).Include(x => x.KitchenCenter)
                                                         .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Account.Status != (int)AccountEnum.Status.DEACTIVE)
                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("fullname"),
                                                              then => then.OrderBy(x => x.FullName))
                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("gender"),
                                                              then => then.OrderBy(x => x.Gender))
                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("dateofbirth"),
                                                              then => then.OrderBy(x => x.DateOfBirth))
                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("citizennumber"),
                                                              then => then.OrderBy(x => x.CitizenNumber))
                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("email"),
                                                              then => then.OrderBy(x => x.Account.Email))
                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                              then => then.OrderBy(x => x.Account.Status))
                                                         .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage)
                                                         .ToList();
                else if(sortByDESC is not null)
                    return this._dbContext.Cashiers.Include(x => x.Account).Include(x => x.KitchenCenter)
                                                         .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Account.Status != (int)AccountEnum.Status.DEACTIVE)
                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("fullname"),
                                                              then => then.OrderByDescending(x => x.FullName))
                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("gender"),
                                                              then => then.OrderByDescending(x => x.Gender))
                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("dateofbirth"),
                                                              then => then.OrderByDescending(x => x.DateOfBirth))
                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("citizennumber"),
                                                              then => then.OrderByDescending(x => x.CitizenNumber))
                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("email"),
                                                              then => then.OrderByDescending(x => x.Account.Email))
                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                              then => then.OrderByDescending(x => x.Account.Status))
                                                         .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage)
                                                         .ToList();

                return await this._dbContext.Cashiers.Include(x => x.Account).Include(x => x.KitchenCenter)
                                                     .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Account.Status != (int)AccountEnum.Status.DEACTIVE)
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<int> GetNumberCashiersAsync(string? searchValue, string? searchValueWithoutUnicode, int kitchenCenterId)
        {
            try
            {
                if(searchValue is not null && searchValueWithoutUnicode is null)
                {
                    return await this._dbContext.Cashiers.Include(x => x.Account).Include(x => x.KitchenCenter)
                                                             .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Account.Status != (int)AccountEnum.Status.DEACTIVE && x.FullName.ToLower().Contains(searchValue.ToLower()))
                                                             .CountAsync();
                } else if(searchValue is null && searchValueWithoutUnicode is not null)
                {
                    return this._dbContext.Cashiers.Include(x => x.Account).Include(x => x.KitchenCenter)
                                                             .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Account.Status != (int)AccountEnum.Status.DEACTIVE)
                                                             .Where(delegate (Cashier cashier)
                                                             {
                                                                 if (StringUtil.RemoveSign4VietnameseString(cashier.FullName).ToLower().Contains(searchValueWithoutUnicode.ToLower())) return true;
                                                                 return false;
                                                             }).Count();
                }
                return await this._dbContext.Cashiers.Include(x => x.Account).Include(x => x.KitchenCenter)
                                                     .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Account.Status != (int)AccountEnum.Status.DEACTIVE)
                                                     .CountAsync();
            } catch(Exception ex)
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

        public async Task<Cashier> GetCashierAsync(int idCashier)
        {
            try
            {
                return await this._dbContext.Cashiers.Include(x => x.Account).Include(x => x.KitchenCenter)
                                                     .SingleOrDefaultAsync(x => x.AccountId == idCashier && x.Account.Status != (int)AccountEnum.Status.DEACTIVE);
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateCashierAsync(Cashier cashier)
        {
            try
            {
                this._dbContext.Cashiers.Update(cashier);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
