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
                if (postStorePartnerRequest.StoreId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidStoreId);
                }
                foreach (var p in postStorePartnerRequest.partnerAccountRequests)
                {
                    if (p.PartnerId <= 0)
                    {
                        throw new BadRequestException(MessageConstant.CommonMessage.InvalidPartnerId);
                    }
                }

                // Check dupplicated partnerId request.
                var hasDuplicatePartnerId = postStorePartnerRequest.partnerAccountRequests
                      .GroupBy(request => request.PartnerId)
                      .Any(group => group.Count() > 1);

                if (hasDuplicatePartnerId)
                {
                    throw new BadRequestException(MessageConstant.StorePartnerMessage.DupplicatedPartnerId_Create);
                }

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

                // Check number of partner in store partner
                if (store.StorePartners.Where(s => s.Status != (int)StorePartnerEnum.Status.DEACTIVE).Count() >= 3)
                {
                    throw new BadRequestException(MessageConstant.StorePartnerMessage.PartnerExceed3);
                }

                // Check partner existed or not
                foreach (var p in postStorePartnerRequest.partnerAccountRequests)
                {
                    var partner = await this._unitOfWork.PartnerRepository.GetPartnerAsync(p.PartnerId);
                    if (partner == null)
                    {
                        throw new NotFoundException(MessageConstant.CommonMessage.NotExistPartnerId);
                    }
                }

                // Check the store is linked to that partner or not 
                foreach (var p in postStorePartnerRequest.partnerAccountRequests)
                {
                    var storePartner = await this._unitOfWork.StorePartnerRepository.GetStorePartnerByPartnerIdAndStoreIdAsync(p.PartnerId, postStorePartnerRequest.StoreId);
                    if (storePartner != null)
                    {
                        throw new BadRequestException(MessageConstant.StorePartnerMessage.LinkedWithParner);
                    }
                }

                // Check user name with difference store id
                foreach (var p in postStorePartnerRequest.partnerAccountRequests)
                {
                    var checkUserNameInDifferenceStore = await this._unitOfWork.StorePartnerRepository.GetStorePartnersByUserNameAndStoreIdAsync(p.UserName, store.StoreId, p.PartnerId);

                    if (checkUserNameInDifferenceStore.Any())
                    {
                        throw new BadRequestException(MessageConstant.StorePartnerMessage.UsernameExisted);
                    }
                }

                //Insert list store partner to database
                var listStorePartnerInsert = new List<StorePartner>();
                foreach (var p in postStorePartnerRequest.partnerAccountRequests)
                {
                    var storePartnerInsert = new StorePartner()
                    {
                        StoreId = postStorePartnerRequest.StoreId,
                        PartnerId = p.PartnerId,
                        CreatedDate = DateTime.Now,
                        UserName = p.UserName,
                        Password = p.Password,
                        Status = (int)StorePartnerEnum.Status.ACTIVE
                    };
                    listStorePartnerInsert.Add(storePartnerInsert);
                }
                await this._unitOfWork.StorePartnerRepository.InsertRangeAsync(listStorePartnerInsert);
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
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidStoreId))
                {
                    fieldName = "Store id";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidPartnerId))
                {
                    fieldName = "Partner id";
                }
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand))
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

        public async Task DeleteStorePartnerAsync(int storeId, int partnerId, IEnumerable<Claim> claims)
        {
            try
            {

                if (storeId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidStoreId);
                }
                if (partnerId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidPartnerId);
                }

                // Get brandId from claim
                Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
                var brandAccount = await this._unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId.Value));
                var brandId = brandAccount.Brand.BrandId;

                // Check store belong to brand or not
                var store = await this._unitOfWork.StoreRepository.GetStoreAsync(storeId);
                if (store != null)
                {
                    if (store.Brand.BrandId != brandId)
                    {
                        throw new BadRequestException(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand);
                    }
                }
                else
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistStoreId);
                }

                // Check partner existed or not
                var partner = await this._unitOfWork.PartnerRepository.GetPartnerAsync(partnerId);
                if (partner == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistPartnerId);
                }

                // Check the store is linked to that partner or not 
                var storePartner = await this._unitOfWork.StorePartnerRepository.GetStorePartnerByPartnerIdAndStoreIdAsync(partnerId, storeId);
                if (storePartner != null)
                {
                    storePartner.Status = (int)StorePartnerEnum.Status.DEACTIVE;
                }
                else
                {
                    throw new BadRequestException(MessageConstant.StorePartnerMessage.NotLinkedWithParner);
                }

                this._unitOfWork.StorePartnerRepository.UpdateStorePartner(storePartner);
                this._unitOfWork.Commit();
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
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.NotLinkedWithParner))
                {
                    fieldName = "Store id, Partner id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidStoreId))
                {
                    fieldName = "Store id";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidPartnerId))
                {
                    fieldName = "Partner id";
                }
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand))
                {
                    fieldName = "Store id";
                }
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.InactiveStore_Create))
                {
                    fieldName = "Store id";
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

        public async Task<GetStorePartnerResponse> GetStorePartnerAsync(int storeId, int partnerId, IEnumerable<Claim> claims)
        {

            try
            {

                if (storeId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidStoreId);
                }
                if (partnerId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidPartnerId);
                }

                // Get brandId from claim
                Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
                var brandAccount = await this._unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId.Value));
                var brandId = brandAccount.Brand.BrandId;

                // Check store belong to brand or not
                var store = await this._unitOfWork.StoreRepository.GetStoreAsync(storeId);
                if (store != null)
                {
                    if (store.Brand.BrandId != brandId)
                    {
                        throw new BadRequestException(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand);
                    }
                }
                else
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistStoreId);
                }

                // Check partner existed or not
                var partner = await this._unitOfWork.PartnerRepository.GetPartnerAsync(partnerId);
                if (partner == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistPartnerId);
                }

                // Check the store is linked to that partner or not 
                var storePartner = await this._unitOfWork.StorePartnerRepository.GetStorePartnerByPartnerIdAndStoreIdAsync(partnerId, storeId);
                if (storePartner == null)
                {
                    throw new BadRequestException(MessageConstant.StorePartnerMessage.NotLinkedWithParner);
                }
                return this._mapper.Map<GetStorePartnerResponse>(storePartner);
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

                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidStoreId))
                {
                    fieldName = "Store id";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidPartnerId))
                {
                    fieldName = "Partner id";
                }
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand))
                {
                    fieldName = "Store id";
                }
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.NotLinkedWithParner))
                {
                    fieldName = "Partner id";
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

        public async Task<GetStorePartnerInformationResponse> GetPartnerInformationAsync(int storeId, IEnumerable<Claim> claims)
        {
            try
            {
                if (storeId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidStoreId);
                }

                // Get brandId from claim
                Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
                var brandAccount = await this._unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId.Value));
                var brandId = brandAccount.Brand.BrandId;

                // Check store belong to brand or not
                var store = await this._unitOfWork.StoreRepository.GetStoreAsync(storeId);
                if (store != null)
                {
                    if (store.Brand.BrandId != brandId)
                    {
                        throw new BadRequestException(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand);
                    }
                }
                else
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistStoreId);
                }
                var storePartner = store.StorePartners.ToList();
                var partnersInformation = new List<GetPartnerInformationResponse>();
                foreach (var p in storePartner)
                {
                    var partner = new GetPartnerInformationResponse();
                    partner.PartnerId = p.PartnerId;
                    partner.PartnerLogo = p.Partner.Logo;
                    partner.Status = StatusUtil.ChangeStorePartnerStatus(p.Status);
                    partner.UserName = p.UserName;
                    partner.Password = p.Password;
                    partner.PartnerName = p.Partner.Name;
                    partnersInformation.Add(partner);
                }
                return new GetStorePartnerInformationResponse()
                {
                    KitchenCenterName = storePartner.Select(x => x.Store.KitchenCenter.Name).FirstOrDefault(),
                    StoreId = storePartner.Select(x => x.StoreId).FirstOrDefault(),
                    StoreName = storePartner.Select(x => x.Store.Name).FirstOrDefault(),
                    StorePartners = partnersInformation
                };
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistStoreId))
                {
                    fieldName = "Store id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";

                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidStoreId))
                {
                    fieldName = "Store id";
                }
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand))
                {
                    fieldName = "Store id";
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

        public async Task<GetStorePartnersResponse> GetStorePartnersAsync(string? searchValue, int? currentPage, int? itemsPerPage, IEnumerable<Claim> claims)
        {
            try
            {

                // Get brandId from claim
                Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
                var brandAccount = await this._unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId.Value));
                var brandId = brandAccount.Brand.BrandId;
                var getStorePartnersResponse = new List<GetStorePartnerResponse>();
                if (currentPage != null && currentPage <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidCurrentPage);
                }
                else if (currentPage == null)
                {
                    currentPage = 1;
                }

                if (itemsPerPage != null && itemsPerPage <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidItemsPerPage);
                }
                else if (itemsPerPage == null)
                {
                    itemsPerPage = 5;
                }

                int numberItems = 0;
                List<StorePartner> storePartners = null;
                if (searchValue != null && StringUtil.IsUnicode(searchValue) == false)
                {
                    numberItems = await this._unitOfWork.StorePartnerRepository.GetNumberStorePartnersAsync(searchValue, null, brandId);
                    storePartners = await this._unitOfWork.StorePartnerRepository.GetStorePartnersAsync(searchValue, null, brandId, currentPage, itemsPerPage);
                }
                else if (searchValue != null && StringUtil.IsUnicode(searchValue))
                {
                    numberItems = await this._unitOfWork.StorePartnerRepository.GetNumberStorePartnersAsync(null, searchValue, brandId);
                    storePartners = await this._unitOfWork.StorePartnerRepository.GetStorePartnersAsync(searchValue, searchValue, brandId, currentPage, itemsPerPage);
                }
                else if (searchValue == null)
                {
                    numberItems = await this._unitOfWork.StorePartnerRepository.GetNumberStorePartnersAsync(null, null, brandId);
                    storePartners = await this._unitOfWork.StorePartnerRepository.GetStorePartnersAsync(null, null, brandId, currentPage, itemsPerPage);
                }

                int totalPages = (int)((numberItems + itemsPerPage) / itemsPerPage);
                if (numberItems == 0)
                {
                    totalPages = 0;
                }
               _mapper.Map(storePartners, getStorePartnersResponse);
                return new GetStorePartnersResponse()
                {
                    StorePartners = getStorePartnersResponse,
                    NumberItems = numberItems,
                    TotalPages = totalPages
                };
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidCurrentPage))
                {
                    fieldName = "Current Page";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidItemsPerPage))
                {
                    fieldName = "Item Per Page";
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

        public async Task UpdateStorePartnerRequestAsync(int storeId, int partnerId, UpdateStorePartnerRequest updateStorePartnerRequest, IEnumerable<Claim> claims)
        {
            try
            {

                if (storeId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidStoreId);
                }
                if (partnerId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidPartnerId);
                }

                // Get brandId from claim
                Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
                var brandAccount = await this._unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId.Value));
                var brandId = brandAccount.Brand.BrandId;

                // Check store belong to brand or not
                var store = await this._unitOfWork.StoreRepository.GetStoreAsync(storeId);
                if (store != null)
                {
                    if (store.Brand.BrandId != brandId)
                    {
                        throw new BadRequestException(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand);
                    }
                }
                else
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistStoreId);
                }

                // Check partner existed or not
                var partner = await this._unitOfWork.PartnerRepository.GetPartnerAsync(partnerId);
                if (partner == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistPartnerId);
                }

                // Check the store is linked to that partner or not 
                var storePartnerExisted = await this._unitOfWork.StorePartnerRepository.GetStorePartnerByPartnerIdAndStoreIdAsync(partnerId, storeId);
                if (storePartnerExisted == null)
                {
                    throw new BadRequestException(MessageConstant.StorePartnerMessage.NotLinkedWithParner);
                }

                // Check user name with difference store id
                var checkUserNameInDifferenceStore = await this._unitOfWork.StorePartnerRepository.GetStorePartnersByUserNameAndStoreIdAsync(updateStorePartnerRequest.UserName, store.StoreId, partnerId);

                if (checkUserNameInDifferenceStore.Any())
                {
                    throw new BadRequestException(MessageConstant.StorePartnerMessage.UsernameExisted);
                }

                if (updateStorePartnerRequest.Status.Trim().ToLower().Equals(StorePartnerEnum.Status.ACTIVE.ToString().ToLower()))
                {
                    storePartnerExisted.Status = (int)StorePartnerEnum.Status.ACTIVE;
                }
                else if (updateStorePartnerRequest.Status.Trim().ToLower().Equals(StorePartnerEnum.Status.INACTIVE.ToString().ToLower()))
                {
                    storePartnerExisted.Status = (int)StorePartnerEnum.Status.INACTIVE;
                }
                storePartnerExisted.UserName = updateStorePartnerRequest.UserName;
                storePartnerExisted.Password = updateStorePartnerRequest.Password;
                this._unitOfWork.StorePartnerRepository.UpdateStorePartner(storePartnerExisted);
                this._unitOfWork.Commit();
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

                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidStoreId))
                {
                    fieldName = "Store id";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidPartnerId))
                {
                    fieldName = "Partner id";
                }
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand))
                {
                    fieldName = "Store id";
                }
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.NotLinkedWithParner))
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

        public async Task UpdateStatusStorePartnerAsync(int storeId, int partnerId, UpdateStorePartnerStatusRequest updateStorePartnerStatusRequest, IEnumerable<Claim> claims)
        {
            try
            {

                if (storeId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidStoreId);
                }
                if (partnerId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidPartnerId);
                }

                // Get brandId from claim
                Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
                var brandAccount = await this._unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId.Value));
                var brandId = brandAccount.Brand.BrandId;

                // Check store belong to brand or not
                var store = await this._unitOfWork.StoreRepository.GetStoreAsync(storeId);
                if (store != null)
                {
                    if (store.Brand.BrandId != brandId)
                    {
                        throw new BadRequestException(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand);
                    }
                }
                else
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistStoreId);
                }

                // Check partner existed or not
                var partner = await this._unitOfWork.PartnerRepository.GetPartnerAsync(partnerId);
                if (partner == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistPartnerId);
                }

                // Check the store is linked to that partner or not 
                var storePartnerExisted = await this._unitOfWork.StorePartnerRepository.GetStorePartnerByPartnerIdAndStoreIdAsync(partnerId, storeId);
                if (storePartnerExisted == null)
                {
                    throw new BadRequestException(MessageConstant.StorePartnerMessage.NotLinkedWithParner);
                }

                if (updateStorePartnerStatusRequest.Status.Trim().ToLower().Equals(StorePartnerEnum.Status.ACTIVE.ToString().ToLower()))
                {
                    storePartnerExisted.Status = (int)StorePartnerEnum.Status.ACTIVE;
                }
                else if (updateStorePartnerStatusRequest.Status.Trim().ToLower().Equals(StorePartnerEnum.Status.INACTIVE.ToString().ToLower()))
                {
                    storePartnerExisted.Status = (int)StorePartnerEnum.Status.INACTIVE;
                }
                else if (updateStorePartnerStatusRequest.Status.Trim().ToLower().Equals(StorePartnerEnum.Status.DEACTIVE.ToString().ToLower()))
                {
                    throw new BadRequestException(MessageConstant.StorePartnerMessage.DeactiveStorePartner_Update);
                }
                this._unitOfWork.StorePartnerRepository.UpdateStorePartner(storePartnerExisted);
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

                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidStoreId))
                {
                    fieldName = "Store id";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidPartnerId))
                {
                    fieldName = "Partner id";
                }
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand))
                {
                    fieldName = "Store id";
                }
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.NotLinkedWithParner))
                {
                    fieldName = "Partner id";
                }
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.DupplicatedPartnerId_Create))
                {
                    fieldName = "Partner id";
                }
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.DeactiveStorePartner_Update))
                {
                    fieldName = "Status";
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

    }
}
