using MBKC.Repository.DBContext;
using MBKC.Repository.Enums;
using MBKC.Repository.Models;
using MBKC.Repository.Utils;
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

        #region Create Partner
        public async Task CreatePartnerAsync(Partner partner)
        {
            try
            {
                await this._dbContext.Partners.AddAsync(partner);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Update Partner
        public void UpdatePartner(Partner partner)
        {
            try
            {
                this._dbContext.Entry<Partner>(partner).State = EntityState.Modified;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Partner By Id
        public async Task<Partner> GetPartnerByIdAsync(int id)
        {
            try
            {
                return await _dbContext.Partners
                    .Include(p => p.StorePartners)
                    .SingleOrDefaultAsync(p => p.PartnerId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Partner By Name
        public async Task<Partner> GetPartnerByNameAsync(string name)
        {
            try
            {
                return await _dbContext.Partners.SingleOrDefaultAsync(p => p.Name.Equals(name));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Partner By WebUrl
        public async Task<Partner> GetPartnerByWebUrlAsync(string webUrl)
        {
            try
            {
                return await _dbContext.Partners.SingleOrDefaultAsync(p => p.WebUrl.Equals(webUrl));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Partners
        public async Task<List<Partner>> GetPartnersAsync(string? searchValue, string? searchValueWithoutUnicode,
            int currentPage, int itemsPerPage, string? sortByASC, string? sortByDESC, bool? isGetAll)
        {
            try
            {
                if (isGetAll != null && isGetAll == true)
                {
                    return await this._dbContext.Partners.Where(c => !(c.Status == (int)PartnerEnum.Status.DISABLE)).ToListAsync();
                }
                if (searchValue == null && searchValueWithoutUnicode != null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Partners.Where(delegate (Partner partner)
                        {
                            if (StringUtil.RemoveSign4VietnameseString(partner.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }).Where(c => !(c.Status == (int)PartnerEnum.Status.DISABLE))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                                        then => then.OrderBy(x => x.Name))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                                      then => then.OrderBy(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Partners.Where(delegate (Partner partner)
                        {
                            if (StringUtil.RemoveSign4VietnameseString(partner.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }).Where(c => !(c.Status == (int)PartnerEnum.Status.DISABLE))
                                                           .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                                        then => then.OrderByDescending(x => x.Name))
                                                           .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                                      then => then.OrderByDescending(x => x.Status).Reverse())
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    return this._dbContext.Partners.Where(delegate (Partner partner)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(partner.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).Where(c => !(c.Status == (int)PartnerEnum.Status.DISABLE)).Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();



                }
                else if (searchValue != null && searchValueWithoutUnicode == null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Partners
                        .Where(c => c.Name.ToLower().Contains(searchValue.ToLower()) && !(c.Status == (int)PartnerEnum.Status.DISABLE))
                        .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                 then => then.OrderBy(x => x.Name))
                        .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                 then => then.OrderBy(x => x.Status).Reverse())
                        .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Partners
                        .Where(c => c.Name.ToLower().Contains(searchValue.ToLower()) && !(c.Status == (int)PartnerEnum.Status.DISABLE))
                        .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                 then => then.OrderByDescending(x => x.Name))
                        .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                 then => then.OrderByDescending(x => x.Status).Reverse())
                        .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    return this._dbContext.Partners
                        .Where(c => c.Name.ToLower().Contains(searchValue.ToLower()) && !(c.Status == (int)PartnerEnum.Status.DISABLE))
                        .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }

                if (sortByASC is not null)
                    return this._dbContext.Partners.Where(c => !(c.Status == (int)PartnerEnum.Status.DISABLE))
                        .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                 then => then.OrderBy(x => x.Name))
                        .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                 then => then.OrderBy(x => x.Status).Reverse())
                        .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                else if (sortByDESC is not null)
                    return this._dbContext.Partners.Where(c => !(c.Status == (int)PartnerEnum.Status.DISABLE))
                       .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                 then => then.OrderByDescending(x => x.Name))
                       .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                 then => then.OrderByDescending(x => x.Status).Reverse())
                       .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                return await this._dbContext.Partners.Where(c => !(c.Status == (int)PartnerEnum.Status.DISABLE))
                                               .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Number Parners
        public async Task<int> GetNumberPartnersAsync(string? keySearchUniCode, string? keySearchNotUniCode)
        {
            try
            {
                if (keySearchUniCode == null && keySearchNotUniCode != null)
                {
                    return this._dbContext.Partners.Where(delegate (Partner partner)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(partner.Name.ToLower()).Contains(keySearchNotUniCode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).Where(c => !(c.Status == (int)PartnerEnum.Status.DISABLE)).AsQueryable().Count();
                }
                else if (keySearchUniCode != null && keySearchNotUniCode == null)
                {
                    return await this._dbContext.Partners.Where(c => c.Name.ToLower().Contains(keySearchUniCode.ToLower()) && !(c.Status == (int)PartnerEnum.Status.DISABLE)).CountAsync();
                }
                return await this._dbContext.Partners.Where(c => !(c.Status == (int)PartnerEnum.Status.DISABLE)).CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        public async Task<Partner> GetPartnerAsync(int partnerId)
        {
            try
            {
                return await this._dbContext.Partners.FirstOrDefaultAsync(p => p.PartnerId == partnerId && p.Status != (int)PartnerEnum.Status.DISABLE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region count number of partner
        public async Task<int> CountPartnerNumberAsync()
        {
            try
            {
                return await _dbContext.Partners.Where(p => p.Status != (int)PartnerEnum.Status.DISABLE).CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
