using AutoMapper;
using MBKC.Repository.Constants;
using MBKC.Repository.Enums;
using MBKC.Repository.GrabFood.Models;
using MBKC.Repository.Infrastructures;
using MBKC.Repository.Models;
using MBKC.Service.Constants;
using MBKC.Service.DTOs.StorePartners;
using MBKC.Service.Exceptions;
using MBKC.Service.DTOs.GrabFoods;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using OfficeOpenXml;
using System.Data;
using System.Net.Mail;
using System.Security.Claims;

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
                // Check dupplicated partnerId request.
                var hasDuplicatePartnerId = postStorePartnerRequest.PartnerAccounts
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
                    else if (store.Brand.BrandId == brandId && store.Status == (int)StoreEnum.Status.INACTIVE || store.Brand.BrandId == brandId && store.Status == (int)StoreEnum.Status.DISABLE)
                    {
                        throw new BadRequestException(MessageConstant.StorePartnerMessage.InactiveStore_Create);
                    }
                }
                else
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistStoreId);
                }

                // Check partner existed or not
                Dictionary<string, int> namePartners = new Dictionary<string, int>();
                var listStorePartnerInsert = new List<StorePartner>();
                foreach (var p in postStorePartnerRequest.PartnerAccounts)
                {
                    var partner = await this._unitOfWork.PartnerRepository.GetPartnerAsync(p.PartnerId);
                    if (partner == null)
                    {
                        throw new NotFoundException(MessageConstant.CommonMessage.NotExistPartnerId);
                    }
                    
                    //check partner account is valid
                    if (partner.Name.ToLower().Equals(PartnerConstant.GrabFood.ToLower()))
                    {
                        GrabFoodAccount grabFoodAccount = new GrabFoodAccount()
                        {
                            Username = p.UserName,
                            Password = p.Password
                        };
                        GrabFoodAuthenticationResponse grabFoodAuthenticationResponse = await this._unitOfWork.GrabFoodRepository.LoginGrabFoodAsync(grabFoodAccount);
                        /*if(grabFoodAuthenticationResponse != null && grabFoodAuthenticationResponse.Data.User_Profile.Role.ToLower().Equals(RoleConstant.Store_Manager.ToLower()) == false)
                        {
                            throw new BadRequestException(MessageConstant.StorePartnerMessage.GrabFoodAccountMustBeStoreManager);
                        }*/
                    }
                    if (namePartners.Where(x => x.Key.ToLower().Equals(partner.Name.ToLower())).Count() == 0)
                    {
                        namePartners.Add(partner.Name, partner.PartnerId);
                    }

                    var storePartner = await this._unitOfWork.StorePartnerRepository.GetStorePartnerByPartnerIdAndStoreIdAsync(p.PartnerId, postStorePartnerRequest.StoreId);
                    if (storePartner != null)
                    {
                        throw new BadRequestException(MessageConstant.StorePartnerMessage.LinkedWithParner);
                    }

                    var checkUserNameInDifferenceStore = await this._unitOfWork.StorePartnerRepository.GetStorePartnersByUserNameAndStoreIdAsync(p.UserName, store.StoreId, p.PartnerId);

                    if (checkUserNameInDifferenceStore.Any(x => x.Status != (int)StorePartnerEnum.Status.DISABLE))
                    {
                        throw new BadRequestException(MessageConstant.StorePartnerMessage.UsernameExisted);
                    }

                    var storePartnerInsert = new StorePartner()
                    {
                        StoreId = postStorePartnerRequest.StoreId,
                        PartnerId = p.PartnerId,
                        CreatedDate = DateTime.Now,
                        UserName = p.UserName,
                        Password = p.Password,
                        Status = (int)StorePartnerEnum.Status.ACTIVE,
                        Commission = p.Commission
                    };
                    listStorePartnerInsert.Add(storePartnerInsert);
                }

                List<PartnerProduct> newPartnerProducts = null;
                List<PartnerProduct> oldPartnerProducts = new List<PartnerProduct>();
                if (postStorePartnerRequest.IsMappingProducts)
                {
                    foreach (var namePartner in namePartners)
                    {
                        if (namePartner.Key.ToLower().Equals(PartnerConstant.GrabFood.ToLower()))
                        {
                            foreach (var storePartner in listStorePartnerInsert)
                            {
                                GrabFoodAccount grabFoodAccount = new GrabFoodAccount()
                                {
                                    Username = storePartner.UserName,
                                    Password = storePartner.Password
                                };
                                GrabFoodAuthenticationResponse grabFoodAuthenticationResponse = await this._unitOfWork.GrabFoodRepository.LoginGrabFoodAsync(grabFoodAccount);
                                GrabFoodMenu grabFoodMenu = await this._unitOfWork.GrabFoodRepository.GetGrabFoodMenuAsync(grabFoodAuthenticationResponse);
                                List<Category> storeCategoires = await this._unitOfWork.CategoryRepository.GetCategories(storePartner.StoreId);
                                PartnerProductsFromGrabFood partnerProductsFromGrabFood = GrabFoodUtil.GetPartnerProductsFromGrabFood(grabFoodMenu, storeCategoires, storePartner.StoreId, storePartner.PartnerId, storePartner.CreatedDate);
                                if (partnerProductsFromGrabFood.NewPartnerProducts is not null && partnerProductsFromGrabFood.NewPartnerProducts.Count() > 0)
                                {
                                    newPartnerProducts = partnerProductsFromGrabFood.NewPartnerProducts;
                                    await this._unitOfWork.PartnerProductRepository.CreateRangePartnerProductsAsync(newPartnerProducts);
                                }

                                if (partnerProductsFromGrabFood.OldPartnerProducts is not null && partnerProductsFromGrabFood.OldPartnerProducts.Count() > 0)
                                {
                                    oldPartnerProducts = partnerProductsFromGrabFood.OldPartnerProducts;
                                    this._unitOfWork.PartnerProductRepository.UpdateRangePartnerProductsAsync(oldPartnerProducts);
                                }
                                if (partnerProductsFromGrabFood.NotMappingFromGrabFood is not null)
                                {
                                    // send email notification
                                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                                    DataTable noneMappingGrabFoodItemData = ExcelUtil.GetNoneMappingGrabFoobItemData(partnerProductsFromGrabFood.NotMappingFromGrabFood.NotMappingGrabFoodItems);
                                    DataTable noneMappingGrabFoodModifierGroupData = ExcelUtil.GetNoneMappingGrabFoodModifierGroupData(partnerProductsFromGrabFood.NotMappingFromGrabFood.NotMappingGrabFoodModifierGroups);
                                    MemoryStream outputStream = new MemoryStream();
                                    using (ExcelPackage package = new ExcelPackage(outputStream))
                                    {
                                        ExcelWorksheet grabFoodItemsWorksheet = package.Workbook.Worksheets.Add("GrabFood Items");
                                        grabFoodItemsWorksheet.Cells.LoadFromDataTable(noneMappingGrabFoodItemData, true);

                                        ExcelWorksheet grabFoodModifierGroupWorksheet = package.Workbook.Worksheets.Add("GrabFood Modifier Groups");
                                        grabFoodModifierGroupWorksheet.Cells.LoadFromDataTable(noneMappingGrabFoodModifierGroupData, true);

                                        package.Save();
                                    }

                                    outputStream.Position = 0;
                                    Attachment attachment = new Attachment(outputStream, "Non-mapping_Product_From_GrabFood.xlsx", "application/vnd.ms-excel");

                                    string message = EmailMessageConstant.StorePartner.Message;
                                    string messageBody = this._unitOfWork.EmailRepository.GetMessageToNotifyNonMappingProduct(brandAccount.Account.Email, "GrabFood", message);
                                    await this._unitOfWork.EmailRepository.SendEmailToNotifyNonMappingProduct(brandAccount.Account.Email, messageBody, "GrabFood", attachment);
                                }
                            }
                        }
                    }
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
                if (ex.Message.Equals(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand))
                {
                    fieldName = "Store id";
                }
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.InactiveStore_Create))
                {
                    fieldName = "Store id";
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
                string error = "";
                if (ex.Message.Contains("for GrabFood Partner."))
                {
                    error = ErrorUtil.GetErrorString("GrabFood Account", ex.Message);
                    throw new BadRequestException(error);
                }
                error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }

        }

        public async Task DeleteStorePartnerAsync(int storeId, int partnerId, IEnumerable<Claim> claims)
        {
            try
            {
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
                    storePartner.Status = (int)StorePartnerEnum.Status.DISABLE;
                    if (storePartner.PartnerProducts.Any())
                    {
                        foreach (var partnerProduct in storePartner.PartnerProducts)
                        {
                            partnerProduct.Status = (int)PartnerProductEnum.Status.DISABLE;
                        }
                    }
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
                if (ex.Message.Equals(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand))
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

                if (ex.Message.Equals(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand))
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

        public async Task<GetStorePartnerInformationResponse> GetPartnerInformationAsync(int storeId, GetPartnerInformationRequest getPartnerInformationRequest, IEnumerable<Claim> claims)
        {
            try
            {
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
                    partner.Commission = p.Commission;
                    partnersInformation.Add(partner);
                }
                if (getPartnerInformationRequest.keySortName != null && getPartnerInformationRequest.keySortName.ToUpper().Equals(StorePartnerEnum.KeySort.ASC.ToString()))
                {
                    partnersInformation = partnersInformation.OrderBy(x => x.PartnerName).ToList();
                }
                else if (getPartnerInformationRequest.keySortName != null && getPartnerInformationRequest.keySortName.ToUpper().Equals(StorePartnerEnum.KeySort.DESC.ToString()))
                {
                    partnersInformation = partnersInformation.OrderByDescending(x => x.PartnerName).ToList();
                }

                if (getPartnerInformationRequest.keySortStatus != null && getPartnerInformationRequest.keySortStatus.ToUpper().Equals(StorePartnerEnum.KeySort.ASC.ToString()))
                {
                    partnersInformation = partnersInformation.OrderBy(x => x.Status).ToList();
                }
                else if (getPartnerInformationRequest.keySortStatus != null && getPartnerInformationRequest.keySortStatus.ToUpper().Equals(StorePartnerEnum.KeySort.DESC.ToString()))
                {
                    partnersInformation = partnersInformation.OrderByDescending(x => x.Status).ToList();
                }

                if (getPartnerInformationRequest.keySortCommission != null && getPartnerInformationRequest.keySortCommission.ToUpper().Equals(StorePartnerEnum.KeySort.ASC.ToString()))
                {
                    partnersInformation = partnersInformation.OrderBy(x => x.Commission).ToList();
                }
                else if (getPartnerInformationRequest.keySortCommission != null && getPartnerInformationRequest.keySortCommission.ToUpper().Equals(StorePartnerEnum.KeySort.DESC.ToString()))
                {
                    partnersInformation = partnersInformation.OrderByDescending(x => x.Commission).ToList();
                }
                if(storePartner.Count == 0)
                {
                    return new GetStorePartnerInformationResponse()
                    {
                        KitchenCenterName = store.KitchenCenter.Name,
                        StoreId = store.StoreId,
                        StoreName = store.Name,
                        StorePartners = partnersInformation
                    };
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

                if (ex.Message.Equals(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand))
                {
                    fieldName = "Store id";
                }
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand))
                {
                    fieldName = "Store id";
                }
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.KeySortNotExist))
                {
                    fieldName = "Key sort";
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

        public async Task<GetStorePartnersResponse> GetStorePartnersAsync(GetStorePartnersRequest getStorePartnersRequest, IEnumerable<Claim> claims)
        {
            try
            {
                // Get brandId from claim
                Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
                var brandAccount = await this._unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId.Value));
                var brandId = brandAccount.Brand.BrandId;
                var getStorePartnersResponse = new List<GetStorePartnerResponse>();

                int numberItems = 0;
                List<StorePartner> storePartners = null;
                if (getStorePartnersRequest.SearchValue != null && StringUtil.IsUnicode(getStorePartnersRequest.SearchValue) == false)
                {
                    numberItems = await this._unitOfWork.StorePartnerRepository.GetNumberStorePartnersAsync(null, getStorePartnersRequest.SearchValue, brandId);
                    storePartners = await this._unitOfWork.StorePartnerRepository.GetStorePartnersAsync(null, getStorePartnersRequest.SearchValue, getStorePartnersRequest.CurrentPage, getStorePartnersRequest.ItemsPerPage,
                                                                                                        getStorePartnersRequest.SortBy != null && getStorePartnersRequest.SortBy.ToLower().EndsWith("asc") ? getStorePartnersRequest.SortBy.Split("_")[0] : null,
                                                                                                        getStorePartnersRequest.SortBy != null && getStorePartnersRequest.SortBy.ToLower().EndsWith("desc") ? getStorePartnersRequest.SortBy.Split("_")[0] : null, brandId, getStorePartnersRequest.IsGetAll);
                }
                else if (getStorePartnersRequest.SearchValue != null && StringUtil.IsUnicode(getStorePartnersRequest.SearchValue))
                {
                    numberItems = await this._unitOfWork.StorePartnerRepository.GetNumberStorePartnersAsync(getStorePartnersRequest.SearchValue, null, brandId);
                    storePartners = await this._unitOfWork.StorePartnerRepository.GetStorePartnersAsync(getStorePartnersRequest.SearchValue, null, getStorePartnersRequest.CurrentPage, getStorePartnersRequest.ItemsPerPage,
                                                                                                        getStorePartnersRequest.SortBy != null && getStorePartnersRequest.SortBy.ToLower().EndsWith("asc") ? getStorePartnersRequest.SortBy.Split("_")[0] : null,
                                                                                                        getStorePartnersRequest.SortBy != null && getStorePartnersRequest.SortBy.ToLower().EndsWith("desc") ? getStorePartnersRequest.SortBy.Split("_")[0] : null, brandId, getStorePartnersRequest.IsGetAll);
                }
                else if (getStorePartnersRequest.SearchValue == null)
                {
                    numberItems = await this._unitOfWork.StorePartnerRepository.GetNumberStorePartnersAsync(null, null, brandId);
                    storePartners = await this._unitOfWork.StorePartnerRepository.GetStorePartnersAsync(null, null, getStorePartnersRequest.CurrentPage, getStorePartnersRequest.ItemsPerPage,
                                                                                                        getStorePartnersRequest.SortBy != null && getStorePartnersRequest.SortBy.ToLower().EndsWith("asc") ? getStorePartnersRequest.SortBy.Split("_")[0] : null,
                                                                                                        getStorePartnersRequest.SortBy != null && getStorePartnersRequest.SortBy.ToLower().EndsWith("desc") ? getStorePartnersRequest.SortBy.Split("_")[0] : null, brandId, getStorePartnersRequest.IsGetAll);
                }

                int totalPages = 0;
                if (numberItems > 0 && getStorePartnersRequest.IsGetAll == null || numberItems > 0 && getStorePartnersRequest.IsGetAll != null && getStorePartnersRequest.IsGetAll == false)
                {
                    totalPages = (int)((numberItems + getStorePartnersRequest.ItemsPerPage) / getStorePartnersRequest.ItemsPerPage);
                }

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

                if (checkUserNameInDifferenceStore.Any(x => x.Status != (int)StorePartnerEnum.Status.DISABLE))
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
                storePartnerExisted.Commission = updateStorePartnerRequest.Commission;
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

                if (ex.Message.Equals(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand))
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
                else if (updateStorePartnerStatusRequest.Status.Trim().ToLower().Equals(StorePartnerEnum.Status.DISABLE.ToString().ToLower()))
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

                if (ex.Message.Equals(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand))
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
