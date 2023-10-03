using MBKC.Repository.DBContext;
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
                return await _dbContext.Partners.SingleOrDefaultAsync(p => p.PartnerId == id);
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

        /* #region Get Partners
         public async Task<List<Partner>> GetPartnersAsync(string? keySearchNameUniCode, string? keySearchNameNotUniCode, string type, int itemsPerPage, int currentPage)
         {
             try
             {
                 if (keySearchNameUniCode == null && keySearchNameNotUniCode != null)
                 {
                     return this._dbContext.Categories.Where(delegate (Category category)
                     {
                         if (StringUtil.RemoveSign4VietnameseString(category.Name.ToLower()).Contains(keySearchNameNotUniCode.ToLower()))
                         {
                             return true;
                         }
                         else
                         {
                             return false;
                         }
                     }).Where(c => c.Type.Equals(type.ToUpper()) && !(c.Status == (int)CategoryEnum.Status.DEACTIVE))
                                                  .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();
                 }
                 else if (keySearchNameUniCode != null && keySearchNameNotUniCode == null)
                 {
                     return await this._dbContext.Categories
                         .Where(c => c.Name.ToLower().Contains(keySearchNameUniCode.ToLower()) && c.Type.Equals(type.ToUpper()) && !(c.Status == (int)CategoryEnum.Status.DEACTIVE))
                         .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
                 }
                 return await this._dbContext.Categories.Where(c => c.Type.Equals(type.ToUpper()) && !(c.Status == (int)CategoryEnum.Status.DEACTIVE))
                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
             }
             catch (Exception ex)
             {
                 throw new Exception(ex.Message);
             }
         }
         #endregion*/

        /*#region Get Number Partners
        public async Task<int> GetNumberPartnersAsync(string? keySearchUniCode, string? keySearchNotUniCode, int? keyStatusFilter)
        {
            try
            {
                if (keySearchUniCode == null && keySearchNotUniCode != null && keyStatusFilter == null)
                {
                    return this._dbContext.Brands.Where(delegate (Brand brand)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(brand.Name.ToLower()).Contains(keySearchNotUniCode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).Where(x => x.Status != (int)BrandEnum.Status.DEACTIVE).AsQueryable().Count();
                }
                else if (keySearchUniCode == null && keySearchNotUniCode != null && keyStatusFilter != null)
                {
                    return this._dbContext.Brands.Where(delegate (Brand brand)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(brand.Name.ToLower()).Contains(keySearchNotUniCode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).Where(b => b.Status == keyStatusFilter).AsQueryable().Count();
                }
                else if (keySearchUniCode != null && keySearchNotUniCode == null && keyStatusFilter == null)
                {
                    return await this._dbContext.Brands.Where(x => x.Name.ToLower().Contains(keySearchUniCode.ToLower()) && x.Status != (int)BrandEnum.Status.DEACTIVE).CountAsync();
                }
                else if (keySearchUniCode != null && keySearchNotUniCode == null && keyStatusFilter != null)
                {
                    return await this._dbContext.Brands.Where(x => x.Name.ToLower().Contains(keySearchUniCode.ToLower()) && x.Status == keyStatusFilter).CountAsync();
                }
                else if (keySearchUniCode == null && keySearchNotUniCode == null && keyStatusFilter != null)
                {
                    return await this._dbContext.Brands.Where(x => x.Status == keyStatusFilter).CountAsync();
                }
                return await this._dbContext.Brands.Where(x => x.Status != (int)BrandEnum.Status.DEACTIVE).CountAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion*/
    }
}
