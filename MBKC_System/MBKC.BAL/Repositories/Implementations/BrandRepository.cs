using AutoMapper;
using MBKC.BAL.DTOs.Brands;
using MBKC.BAL.DTOs.FireBase;
using MBKC.BAL.Exceptions;
using MBKC.BAL.Repositories.Interfaces;
using MBKC.BAL.Utils;
using MBKC.DAL.DBContext;
using MBKC.DAL.Enums;
using MBKC.DAL.Infrastructures;
using MBKC.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Repositories.Implementations
{
    public class BrandRepository : IBrandRepository
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public BrandRepository(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }
        #region CreateBrand
        public async Task<GetBrandResponse> CreateBrand(PostBrandRequest postBrandRequest, FireBaseImage fireBaseImage)
        {
            string urlImage = "";
            try
            {
                var checkDupplicatedEmail = await _unitOfWork.AccountDAO.GetAccountByEmail(postBrandRequest.ManagerEmail);
                if (checkDupplicatedEmail != null)
                {
                    throw new ConflictException("Email already exists in the system");
                }
                var brandManagerRole = await _unitOfWork.RoleDAO.GetRoleById((int)RoleEnum.RoleName.BrandManager);

                // Upload image to firebase
                FileStream fileStream = Utils.FileUtil.ConvertFormFileToStream(postBrandRequest.Logo);
                FileUtil.SetCredentials(fireBaseImage);
                urlImage = await Utils.FileUtil.UploadImage(fileStream, "Brand");

                // Create account
                var account = new Account
                {
                    Email = postBrandRequest.ManagerEmail,
                    Password = RandomNumberUtil.GenerateEightDigitNumber().ToString(),
                    Status = Convert.ToBoolean(AccountEnum.Status.ACTIVE),
                    Role = brandManagerRole
                };
                await _unitOfWork.AccountDAO.CreateAccount(account);

                // Create brand
                Brand brand = new Brand
                {
                    Name = postBrandRequest.Name,
                    Address = postBrandRequest.Address,
                    Logo = urlImage,
                    Status = (int)BrandEnum.Status.ACTIVE,
                };
                await _unitOfWork.BrandDAO.CreateBrand(brand);

                // Create brand account
                BrandAccount brandAccount = new BrandAccount
                {
                    Account = account,
                    Brand = brand,
                };
                await _unitOfWork.BrandAccountDAO.CreateBrandAccount(brandAccount);
                await _unitOfWork.CommitAsync();

                //Send password to email of Brand Manager
                await SendEmailUtil.SendEmail(account.Email, SendEmailUtil.Message(account.Email, account.Password, "Brand Manager"), "Brand Manager");
                return _mapper.Map<GetBrandResponse>(brand);
            }
            catch (ConflictException ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new ConflictException(error);
            }
            catch (BadRequestException ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                if (urlImage.Length > 0)
                {
                    string fileId = SplitStringUtil.SplitString(urlImage, "Brand");
                    await FileUtil.DeleteImageAsync(fileId, "Brand");
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region UpdateBrand
        public async Task<GetBrandResponse> UpdateBrand(int brandId, UpdateBrandRequest updateBrandRequest, FireBaseImage fireBaseImage)
        {
            string urlImage = "";
            try
            {
                var brand = await _unitOfWork.BrandDAO.GetBrandById(brandId);
                if (brand == null)
                {
                    throw new NotFoundException("Brand does not exist in the system");
                }

                if (updateBrandRequest.Logo != null)
                {
                    //Delete image from database
                    FileUtil.SetCredentials(fireBaseImage);
                    string fileId = SplitStringUtil.SplitString(brand.Logo, "Brand");
                    await FileUtil.DeleteImageAsync(fileId, "Brand");

                    // Upload image to firebase
                    FileStream fileStream = Utils.FileUtil.ConvertFormFileToStream(updateBrandRequest.Logo);
                    urlImage = await Utils.FileUtil.UploadImage(fileStream, "Brand");
                }

                brand.Address = updateBrandRequest.Address;
                brand.Logo = urlImage;
                brand.Name = updateBrandRequest.Name;

                _unitOfWork.BrandDAO.UpdateBrand(brand);
                _unitOfWork.Commit();
                return _mapper.Map<GetBrandResponse>(brand);
            }
            catch (ConflictException ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new ConflictException(error);
            }
            catch (BadRequestException ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                if (urlImage.Length > 0)
                {
                    string fileId = SplitStringUtil.SplitString(urlImage, "Brand");
                    await FileUtil.DeleteImageAsync(fileId, "Brand");
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region Get Brands
        public async Task<List<GetBrandResponse>> GetBrands()
        {
            try
            {
                var brands = await _unitOfWork.BrandDAO.GetBrands();
                return _mapper.Map<List<GetBrandResponse>>(brands);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }

        }
        #endregion

        #region Get Brand By Id
        public async Task<GetBrandResponse> GetBrandById(int id)
        {
            try
            {
                var brand = await _unitOfWork.BrandDAO.GetBrandById(id);
                if (brand == null)
                {
                    throw new NotFoundException("Brand does not exist in the system");
                }
                return _mapper.Map<GetBrandResponse>(brand);
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
        #endregion
    }
}
