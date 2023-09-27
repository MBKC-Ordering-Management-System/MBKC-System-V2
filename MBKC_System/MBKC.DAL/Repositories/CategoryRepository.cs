using MBKC.DAL.DBContext;
using MBKC.DAL.Models;
using Microsoft.EntityFrameworkCore;
using MBKC.DAL.Enums;
using MBKC.DAL.Models;
using MBKC.DAL.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.Repositories
{
    public class CategoryRepository
    {
        private MBKCDbContext _dbContext;
        public CategoryRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        #region Get Category By Code
        public async Task<Category> GetCategoryByCodeAsync(string code)
        {
            try
            {
                return await _dbContext.Categories.SingleOrDefaultAsync(c => c.Code.Equals(code));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Category By Code
        public async Task<Category> GetCategoryByDisplayOrderAsync(int displayOrder)
        {
            try
            {
                return await _dbContext.Categories.SingleOrDefaultAsync(c => c.DisplayOrder == displayOrder);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Category By Name
        public async Task<Category> GetCategoryByNameAsync(string name)
        {
            try
            {
                return await _dbContext.Categories.SingleOrDefaultAsync(c => c.Name.Equals(name) && !(c.Status == (int)CategoryEnum.Status.DEACTIVE));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Category By Id
        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            try
            {
                return await _dbContext.Categories
                    .Include(c => c.ExtraCategoryProductCategories)
                    .Include(c => c.Products)
                    .SingleOrDefaultAsync(c => c.CategoryId.Equals(id) && !(c.Status == (int)CategoryEnum.Status.DEACTIVE));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Categories
        public async Task<List<Category>> GetCategoriesAsync(string? keySearchNameUniCode, string? keySearchNameNotUniCode, string type, int itemsPerPage, int currentPage)
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
                                                 }).Where(c => c.Type.Equals(type.ToUpper()) && !(c.Status == (int)CategoryEnum.Status.DEACTIVE)).Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
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
        #endregion

        #region Create Category 
        public async Task CreateCategoryAsyncAsync(Category category)
        {
            try
            {
                await this._dbContext.Categories.AddAsync(category);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Update Category
        public void UpdateCategory(Category category)
        {
            try
            {
                this._dbContext.Entry<Category>(category).State = EntityState.Modified;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Number Categories
        public async Task<int> GetNumberCategoriesAsync(string? keySearchUniCode, string? keySearchNotUniCode, string type)
        {
            try
            {
                if (keySearchUniCode == null && keySearchNotUniCode != null)
                {
                    return this._dbContext.Categories.Where(delegate (Category category)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(category.Name.ToLower()).Contains(keySearchNotUniCode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).Where(c => !(c.Status == (int)CategoryEnum.Status.DEACTIVE) && c.Type.Equals(type.ToUpper())).AsQueryable().Count();
                }
                else if (keySearchUniCode != null && keySearchNotUniCode == null)
                {
                    return await this._dbContext.Categories.Where(c => c.Name.ToLower().Contains(keySearchUniCode.ToLower()) && !(c.Status == (int)CategoryEnum.Status.DEACTIVE) && c.Type.Equals(type.ToUpper())).CountAsync();
                }
                return await this._dbContext.Categories.Where(c => !(c.Status == (int)CategoryEnum.Status.DEACTIVE) && c.Type.Equals(type.ToUpper())).CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Search and Paging extra category
        public List<Category> SearchAndPagingExtraCategory(List<Category> categories, string? keySearchNameUniCode, string? keySearchNameNotUniCode, int itemsPerPage, int currentPage)
        {
            try
            {
                if (keySearchNameUniCode == null && keySearchNameNotUniCode != null)
                {
                    return categories.Where(delegate (Category category)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(category.Name.ToLower()).Contains(keySearchNameNotUniCode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).Where(c => !(c.Status == (int)CategoryEnum.Status.DEACTIVE)).Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                else if (keySearchNameUniCode != null && keySearchNameNotUniCode == null)
                {
                    return categories
                        .Where(c => c.Name.ToLower().Contains(keySearchNameUniCode.ToLower()) && !(c.Status == (int)CategoryEnum.Status.DEACTIVE))
                        .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                return categories
                    .Where(c => !(c.Status == (int)CategoryEnum.Status.DEACTIVE))
                    .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Number ExtraCategories
        public int GetNumberExtraCategories(List<Category> categories, string? keySearchUniCode, string? keySearchNotUniCode)
        {
            try
            {
                if (keySearchUniCode == null && keySearchNotUniCode != null)
                {
                    return categories.Where(delegate (Category category)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(category.Name.ToLower()).Contains(keySearchNotUniCode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).Where(c => !(c.Status == (int)CategoryEnum.Status.DEACTIVE)).AsQueryable().Count();
                }
                else if (keySearchUniCode != null && keySearchNotUniCode == null)
                {
                    return categories.Where(c => c.Name.ToLower().Contains(keySearchUniCode.ToLower()) && !(c.Status == (int)CategoryEnum.Status.DEACTIVE)).Count();
                }
                return categories.Where(c => !(c.Status == (int)CategoryEnum.Status.DEACTIVE)).Count();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        public bool CheckListExtraCategoryId(List<int> listIdExtraCategory)
        {
            bool idsExistInDatabase = this._dbContext.Categories
                .Any(extraCategory => listIdExtraCategory.Contains(extraCategory.CategoryId));
            return idsExistInDatabase;
        }
    }
}
