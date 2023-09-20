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

        public async Task CreateBrand(PostBrandRequest postBrandRequest, FireBaseImage fireBaseImage)
        {
            string urlImage = "";
            try
            {

                var brandManagerRole = await _unitOfWork.RoleDAO.GetRoleById((int)RoleEnum.RoleName.BrandManager);

                // Upload image to firebase
                FileStream fileStream = Utils.FileUtil.ConvertFormFileToStream(postBrandRequest.Logo);
                FileUtil.SetCredentials(fireBaseImage);
                urlImage = await Utils.FileUtil.UploadImage(fileStream, "Brand");

                // Create account
                var account = new Account
                {
                    Email = postBrandRequest.ManagerEmail,
                    Password = RandomNumberUtil.GenerateSixDigitNumber().ToString(),
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
            }
            catch (Exception ex)
            {
                if (urlImage.Length > 0)
                {
                    string fileId = SplitStringUtil.SplitString(urlImage, "Brand");
                    await FileUtil.DeleteImageAsync(fileId, "Brand");
                }
                throw new Exception(ex.Message);
            }


        }
    }
}
