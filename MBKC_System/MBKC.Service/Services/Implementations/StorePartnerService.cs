using AutoMapper;
using MBKC.Service.Services.Interfaces;
using MBKC.Repository.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBKC.Service.DTOs.StorePartners;
using System.Security.Claims;
using MBKC.Repository.Enums;
using MBKC.Service.Constants;
using MBKC.Service.Exceptions;
using MBKC.Repository.Models;
using MBKC.Service.Utils;

namespace MBKC.Service.Services.Implementations
{
    public class StorePartnerService : IStorePartnerService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public StorePartnerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        public async Task CreateStorePartnerAsync(PostStorePartnerRequest postStorePartnerRequest, IEnumerable<Claim> claims)
        {
            try
            {
                // Get brandId from claim
                Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
                var brandAccount = await this._unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId.Value));
                var brandId = brandAccount.Brand.BrandId;

                // Check store belong to brand or not
                var store = await this._unitOfWork.StoreRepository.GetStoreAsync(postStorePartnerRequest.StoreId);
                if (store != null)
                {
                    if (store.Brand.BrandId != brandId)
                    {
                        throw new BadRequestException(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand);
                    }
                    else if (store.Brand.BrandId == brandId && store.Status == (int)StoreEnum.Status.INACTIVE)
                    {
                        throw new BadRequestException(MessageConstant.StorePartnerMessage.InactiveStore_Create);
                    }
                }
                else
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistStoreId);
                }
                // Check partner existed or not
                var partner = await this._unitOfWork.PartnerRepository.GetPartnerAsync(postStorePartnerRequest.PartnerId);
                if (partner == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistPartnerId);
                }
                // Check number of partner in store partner
                if (store.StorePartners.Count() > 3)
                {
                    throw new BadRequestException(MessageConstant.StorePartnerMessage.PartnerExceed3);
                }


                // Check the store is linked to that partner or not 
                var storePartner = await this._unitOfWork.StorePartnerRepository.GetStorePartnerByPartnerIdAndStoreIdAsync(postStorePartnerRequest.PartnerId, postStorePartnerRequest.StoreId);
                if (storePartner != null)
                {
                    throw new BadRequestException(MessageConstant.StorePartnerMessage.LinkedWithParner);
                }

                // Get list store partners in system by user name
                var listStorePartner = await this._unitOfWork.StorePartnerRepository.GetStorePartnersByUserNameAsync(postStorePartnerRequest.UserName);

                // Get list store partners with specific store id 
                var listStorePartnerWithSpecificStoreId = await this._unitOfWork.StorePartnerRepository.GetStorePartnersByStoreIdAsync(postStorePartnerRequest.StoreId);

                // If user name in specific store id dupplicated with user name of any store partner in the system then throw bad request exception.
                if (listStorePartner.Any() && listStorePartnerWithSpecificStoreId.Any())
                {
                    if (listStorePartner.Any(s => s.UserName.Equals(listStorePartnerWithSpecificStoreId.Select(st => st.UserName))))
                    {
                        throw new BadRequestException(MessageConstant.StorePartnerMessage.UsernameExisted);
                    }
                }

                var storePartnerInsert = new StorePartner()
                {
                    StoreId = postStorePartnerRequest.StoreId,
                    PartnerId = postStorePartnerRequest.PartnerId,
                    UserName = postStorePartnerRequest.UserName,
                    Password = StringUtil.EncryptData(postStorePartnerRequest.Password),
                    Status = (int)StorePartnerEnum.Status.ACTIVE
                };

                await this._unitOfWork.StorePartnerRepository.CreateStorePartnerAsync(storePartnerInsert);
                await this._unitOfWork.CommitAsync();

            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistPartnerId))
                {
                    fieldName = "Partner id";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistStoreId))
                {
                    fieldName = "Store id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand))
                {
                    fieldName = "Store id";
                }
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.InactiveStore_Create))
                {
                    fieldName = "Store id";
                }
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.PartnerExceed3))
                {
                    fieldName = "Store partner id";
                }
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.LinkedWithParner))
                {
                    fieldName = "Partner id";
                }
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.UsernameExisted))
                {
                    fieldName = "User name";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }

        }

        public Task DeleteStorePartnerAsync(int storeId, IEnumerable<Claim> claims)
        {
            throw new NotImplementedException();
        }

        public Task GetStorePartnerAsync(int storeId, IEnumerable<Claim> claims)
        {
            throw new NotImplementedException();
        }

        public Task GetStorePartnersAsync(string? searchValue, int? currentPage, int? itemsPerPage, IEnumerable<Claim> claims)
        {
            throw new NotImplementedException();
        }

        public Task UpdateStorePartnerRequestAsync(UpdateStorePartnerRequest updateStorePartnerRequest, IEnumerable<Claim> claims)
        {
            throw new NotImplementedException();
        }
    }
}
