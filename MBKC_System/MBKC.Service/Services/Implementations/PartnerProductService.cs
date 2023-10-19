using AutoMapper;
using MBKC.Service.Services.Interfaces;
using MBKC.Repository.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBKC.Service.DTOs.PartnerProducts;
using System.Security.Claims;
using MBKC.Service.Exceptions;
using MBKC.Service.Constants;
using MBKC.Repository.Enums;
using MBKC.Repository.Models;
using MBKC.Service.Utils;

namespace MBKC.Service.Services.Implementations
{
    public class PartnerProductService : IPartnerProductService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public PartnerProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        #region Create Partner Product
        public async Task CreatePartnerProduct(PostPartnerProductRequest postPartnerProductRequest, IEnumerable<Claim> claims)
        {
            try
            {
                if (postPartnerProductRequest.StoreId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidStoreId);
                }
                if (postPartnerProductRequest.PartnerId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidPartnerId);
                }
                if (postPartnerProductRequest.ProductId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidProductId);
                }
                // Get brandId from claim
                Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
                var brandAccount = await this._unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId.Value));
                var brandId = brandAccount.Brand.BrandId;

                // Check store belong to brand or not
                var store = await this._unitOfWork.StoreRepository.GetStoreAsync(postPartnerProductRequest.StoreId);
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
                var partner = await this._unitOfWork.PartnerRepository.GetPartnerByIdAsync(postPartnerProductRequest.PartnerId);
                if (partner == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistPartnerId);
                }



                // Check the store is linked to that partner or not 
                var storePartner = await this._unitOfWork.StorePartnerRepository.GetStorePartnerByPartnerIdAndStoreIdAsync(postPartnerProductRequest.PartnerId, postPartnerProductRequest.StoreId);
                if (storePartner == null)
                {
                    throw new BadRequestException(MessageConstant.StorePartnerMessage.NotLinkedWithParner);
                }


                // check product belong to brand or not
                var product = await this._unitOfWork.ProductRepository.GetProductAsync(postPartnerProductRequest.ProductId);
                if (product != null)
                {
                    if (product.Status == (int)ProductEnum.Status.DEACTIVE && product.Brand.BrandId == brandId)
                    {
                        throw new BadRequestException(MessageConstant.PartnerProductMessage.DeactiveProduct_Create_Update);
                    }
                    else if (product.Status == (int)ProductEnum.Status.INACTIVE && product.Brand.BrandId == brandId)
                    {
                        throw new BadRequestException(MessageConstant.PartnerProductMessage.InactiveProduct_Create_Update);
                    }
                    else if (product.Status == (int)ProductEnum.Status.ACTIVE && product.Brand.BrandId != brandId)
                    {
                        throw new BadRequestException(MessageConstant.BrandMessage.ProductNotBelongToBrand);
                    }
                }
                else
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.NotExistProductId);
                }

                // Check product code existed or not
                var checkProductCode = await this._unitOfWork.PartnerProductRepository.GetPartnerProductByProductCodeAsync(postPartnerProductRequest.ProductCode);
                if (checkProductCode != null)
                {
                    throw new BadRequestException(MessageConstant.PartnerProductMessage.ProductCodeExisted);
                }

                // Check partner product existed in system or not
                var PartnerProduct = await this._unitOfWork.PartnerProductRepository.GetPartnerProductAsync(postPartnerProductRequest.ProductId, postPartnerProductRequest.PartnerId, postPartnerProductRequest.StoreId, storePartner.CreatedDate);
                if (PartnerProduct != null)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.AlreadyExistPartnerProduct);
                }

                var PartnerProductInsert = new PartnerProduct()
                {
                    ProductId = postPartnerProductRequest.ProductId,
                    PartnerId = postPartnerProductRequest.PartnerId,
                    StoreId = postPartnerProductRequest.StoreId,
                    ProductCode = postPartnerProductRequest.ProductCode,
                    CreatedDate = storePartner.CreatedDate,
                    Status = (int)PartnerProductEnum.Status.ACTIVE
                };
                await this._unitOfWork.PartnerProductRepository.CreatePartnerProductAsync(PartnerProductInsert);
                await this._unitOfWork.CommitAsync();
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
                else if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidProductId))
                {
                    fieldName = "Product id";
                }
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand))
                {
                    fieldName = "Store id";
                }

                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.InactiveStore_Create))
                {
                    fieldName = "Store id";
                }

                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.NotLinkedWithParner))
                {
                    fieldName = "Store id";
                }

                else if (ex.Message.Equals(MessageConstant.PartnerProductMessage.DeactiveProduct_Create_Update))
                {
                    fieldName = "Product id";
                }

                else if (ex.Message.Equals(MessageConstant.PartnerProductMessage.InactiveProduct_Create_Update))
                {
                    fieldName = "Product id";
                }

                else if (ex.Message.Equals(MessageConstant.BrandMessage.ProductNotBelongToBrand))
                {
                    fieldName = "Product id";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistProductId))
                {
                    fieldName = "Product id";
                }

                else if (ex.Message.Equals(MessageConstant.CommonMessage.AlreadyExistPartnerProduct))
                {
                    fieldName = "Mapping product";
                }
                else if (ex.Message.Equals(MessageConstant.PartnerProductMessage.ProductCodeExisted))
                {
                    fieldName = "Product code";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistStoreId))
                {
                    fieldName = "Store id";
                }

                else if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistPartnerId))
                {
                    fieldName = "Partner id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region Get Specific Partner Product
        public async Task<GetPartnerProductResponse> GetPartnerProduct(int productId, int partnerId, int storeId, IEnumerable<Claim> claims)
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
                if (productId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidProductId);
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
                var partner = await this._unitOfWork.PartnerRepository.GetPartnerByIdAsync(partnerId);
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


                // check product belong to brand or not
                var product = await this._unitOfWork.ProductRepository.GetProductAsync(productId);
                if (product != null)
                {
                    if (product.Status == (int)ProductEnum.Status.DEACTIVE && product.Brand.BrandId == brandId)
                    {
                        throw new BadRequestException(MessageConstant.PartnerProductMessage.DeactiveProduct_Create_Update);
                    }
                    else if (product.Status == (int)ProductEnum.Status.ACTIVE && product.Brand.BrandId != brandId)
                    {
                        throw new BadRequestException(MessageConstant.BrandMessage.ProductNotBelongToBrand);
                    }
                    else if (product.Status == (int)ProductEnum.Status.INACTIVE && product.Brand.BrandId != brandId)
                    {
                        throw new BadRequestException(MessageConstant.BrandMessage.ProductNotBelongToBrand);
                    }
                    else if (product.Status == (int)ProductEnum.Status.DEACTIVE && product.Brand.BrandId != brandId)
                    {
                        throw new BadRequestException(MessageConstant.BrandMessage.ProductNotBelongToBrand);
                    }
                }
                else
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.NotExistProductId);
                }

                // Check partner product existed in system or not
                var PartnerProduct = await this._unitOfWork.PartnerProductRepository.GetPartnerProductAsync(productId, partnerId, storeId, storePartner.CreatedDate);
                if (PartnerProduct == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistPartnerProduct);
                }

                return this._mapper.Map<GetPartnerProductResponse>(PartnerProduct);
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
                else if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidProductId))
                {
                    fieldName = "Product id";
                }
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand))
                {
                    fieldName = "Store id";
                }

                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.InactiveStore_Create))
                {
                    fieldName = "Store id";
                }

                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.NotLinkedWithParner))
                {
                    fieldName = "Store id";
                }

                else if (ex.Message.Equals(MessageConstant.PartnerProductMessage.DeactiveProduct_Create_Update))
                {
                    fieldName = "Product id";
                }

                else if (ex.Message.Equals(MessageConstant.PartnerProductMessage.InactiveProduct_Create_Update))
                {
                    fieldName = "Product id";
                }

                else if (ex.Message.Equals(MessageConstant.BrandMessage.ProductNotBelongToBrand))
                {
                    fieldName = "Product id";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistProductId))
                {
                    fieldName = "Product id";
                }
                else if (ex.Message.Equals(MessageConstant.PartnerProductMessage.ProductCodeExisted))
                {
                    fieldName = "Product code";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand))
                {
                    fieldName = "Store id";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistPartnerId))
                {
                    fieldName = "Partner id";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistPartnerProduct))
                {
                    fieldName = "Partner id, Product id, Store id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }

        }
        #endregion

        #region Get Partner Products
        public async Task<GetPartnerProductsResponse> GetPartnerProducts(string? searchName, int? currentPage, int? itemsPerPage, IEnumerable<Claim> claims)
        {
            try
            {
                // Get brandId from claim
                Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
                var brandAccount = await this._unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId.Value));
                var brandId = brandAccount.Brand.BrandId;

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
                List<PartnerProduct> PartnerProducts = null;
                if (searchName != null && StringUtil.IsUnicode(searchName) == false)
                {
                    numberItems = await this._unitOfWork.PartnerProductRepository.GetNumberPartnerProductsAsync(searchName, null, brandId);
                    PartnerProducts = await this._unitOfWork.PartnerProductRepository.GetPartnerProductsAsync(searchName, null, currentPage, itemsPerPage, brandId);

                }
                else if (searchName != null && StringUtil.IsUnicode(searchName))
                {
                    numberItems = await this._unitOfWork.PartnerProductRepository.GetNumberPartnerProductsAsync(null, searchName, brandId);
                    PartnerProducts = await this._unitOfWork.PartnerProductRepository.GetPartnerProductsAsync(null, searchName, currentPage, itemsPerPage, brandId);
                }
                else if (searchName == null)
                {
                    numberItems = await this._unitOfWork.PartnerProductRepository.GetNumberPartnerProductsAsync(null, null, brandId);
                    PartnerProducts = await this._unitOfWork.PartnerProductRepository.GetPartnerProductsAsync(null, null, currentPage, itemsPerPage, brandId);
                }

                int totalPages = (int)((numberItems + itemsPerPage) / itemsPerPage);
                if (numberItems == 0)
                {
                    totalPages = 0;
                }
                List<GetPartnerProductResponse> getPartnerProductResponse = this._mapper.Map<List<GetPartnerProductResponse>>(PartnerProducts);
                return new GetPartnerProductsResponse()
                {
                    NumberItems = numberItems,
                    TotalPages = totalPages,
                    PartnerProducts = getPartnerProductResponse
                };
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidItemsPerPage))
                {
                    fieldName = "Items per page";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidCurrentPage))
                {
                    fieldName = "Current page";
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
        #endregion

        #region Update Partner Product
        public async Task UpdatePartnerProduct(int productId, int partnerId, int storeId, UpdatePartnerProductRequest updatePartnerProductRequest, IEnumerable<Claim> claims)
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
                if (productId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidProductId);
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
                    else if (store.Brand.BrandId == brandId && store.Status == (int)StoreEnum.Status.INACTIVE)
                    {
                        throw new BadRequestException(MessageConstant.PartnerProductMessage.InactiveStore_Update);
                    }
                }
                else
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistStoreId);
                }

                // Check partner existed or not
                var partner = await this._unitOfWork.PartnerRepository.GetPartnerByIdAsync(partnerId);
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


                // check product belong to brand or not
                var product = await this._unitOfWork.ProductRepository.GetProductAsync(productId);
                if (product != null)
                {
                    if (product.Status == (int)ProductEnum.Status.DEACTIVE && product.Brand.BrandId == brandId)
                    {
                        throw new BadRequestException(MessageConstant.PartnerProductMessage.DeactiveProduct_Create_Update);
                    }
                    else if (product.Status == (int)ProductEnum.Status.INACTIVE && product.Brand.BrandId == brandId)
                    {
                        throw new BadRequestException(MessageConstant.PartnerProductMessage.InactiveProduct_Create_Update);
                    }
                    else if (product.Status == (int)ProductEnum.Status.ACTIVE && product.Brand.BrandId != brandId)
                    {
                        throw new BadRequestException(MessageConstant.BrandMessage.ProductNotBelongToBrand);
                    }
                }
                else
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.NotExistProductId);
                }

                // Check product code existed or not
                var checkProductCode = await this._unitOfWork.PartnerProductRepository.GetPartnerProductByProductCodeAsync(updatePartnerProductRequest.ProductCode);
                if (checkProductCode != null)
                {
                    throw new BadRequestException(MessageConstant.PartnerProductMessage.ProductCodeExisted);
                }

                // Check partner product status valid or not
                if (!updatePartnerProductRequest.Status.ToUpper().Equals(PartnerProductEnum.Status.ACTIVE.ToString()) &&
                    !updatePartnerProductRequest.Status.ToUpper().Equals(PartnerProductEnum.Status.INACTIVE.ToString()))
                {
                    throw new BadRequestException(MessageConstant.PartnerProductMessage.StatusInValid);
                }

                // Check partner product existed in system or not
                var partnerProductExisted = await this._unitOfWork.PartnerProductRepository.GetPartnerProductAsync(productId, partnerId, storeId, storePartner.CreatedDate);
                if (partnerProductExisted == null)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.NotExistPartnerProduct);
                }

                // assign update request to partner product existed
                if (updatePartnerProductRequest.Status.Trim().ToLower().Equals(PartnerProductEnum.Status.ACTIVE.ToString().ToLower()))
                {
                    partnerProductExisted.Status = (int)StorePartnerEnum.Status.ACTIVE;
                }
                else if (updatePartnerProductRequest.Status.Trim().ToLower().Equals(PartnerProductEnum.Status.INACTIVE.ToString().ToLower()))
                {
                    partnerProductExisted.Status = (int)StorePartnerEnum.Status.INACTIVE;
                }
                partnerProductExisted.ProductCode = updatePartnerProductRequest.ProductCode;

                this._unitOfWork.PartnerProductRepository.UpdatePartnerProduct(partnerProductExisted);
                await this._unitOfWork.CommitAsync();
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
                else if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidProductId))
                {
                    fieldName = "Product id";
                }
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand))
                {
                    fieldName = "Store id";
                }

                else if (ex.Message.Equals(MessageConstant.PartnerProductMessage.InactiveStore_Update))
                {
                    fieldName = "Store id";
                }

                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.NotLinkedWithParner))
                {
                    fieldName = "Store id";
                }

                else if (ex.Message.Equals(MessageConstant.PartnerProductMessage.DeactiveProduct_Create_Update))
                {
                    fieldName = "Product id";
                }

                else if (ex.Message.Equals(MessageConstant.PartnerProductMessage.InactiveProduct_Create_Update))
                {
                    fieldName = "Product id";
                }

                else if (ex.Message.Equals(MessageConstant.BrandMessage.ProductNotBelongToBrand))
                {
                    fieldName = "Product id";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistProductId))
                {
                    fieldName = "Product id";
                }
                else if (ex.Message.Equals(MessageConstant.PartnerProductMessage.StatusInValid))
                {
                    fieldName = "Status";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistPartnerProduct))
                {
                    fieldName = "Mapping product";
                }
                else if (ex.Message.Equals(MessageConstant.PartnerProductMessage.ProductCodeExisted))
                {
                    fieldName = "Product code";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand))
                {
                    fieldName = "Store id";
                }

                else if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistPartnerId))
                {
                    fieldName = "Partner id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region Delete Partner Product
        public async Task DeletePartnerProductByIdAsync(int productId, int partnerId, int storeId, IEnumerable<Claim> claims)
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
                if (productId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidProductId);
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
                var partner = await this._unitOfWork.PartnerRepository.GetPartnerByIdAsync(partnerId);
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


                // check product belong to brand or not
                var product = await this._unitOfWork.ProductRepository.GetProductAsync(productId);
                if (product != null)
                {
                    if (product.Status == (int)ProductEnum.Status.DEACTIVE && product.Brand.BrandId == brandId)
                    {
                        throw new BadRequestException(MessageConstant.PartnerProductMessage.DeactiveProduct_Create_Update);
                    }
                    else if (product.Status == (int)ProductEnum.Status.INACTIVE && product.Brand.BrandId == brandId)
                    {
                        throw new BadRequestException(MessageConstant.PartnerProductMessage.InactiveProduct_Create_Update);
                    }
                    else if (product.Status == (int)ProductEnum.Status.ACTIVE && product.Brand.BrandId != brandId)
                    {
                        throw new BadRequestException(MessageConstant.BrandMessage.ProductNotBelongToBrand);
                    }
                    else if (product.Status == (int)ProductEnum.Status.INACTIVE && product.Brand.BrandId != brandId)
                    {
                        throw new BadRequestException(MessageConstant.BrandMessage.ProductNotBelongToBrand);
                    }
                    else if (product.Status == (int)ProductEnum.Status.DEACTIVE && product.Brand.BrandId != brandId)
                    {
                        throw new BadRequestException(MessageConstant.BrandMessage.ProductNotBelongToBrand);
                    }
                }
                else
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.NotExistProductId);
                }

                // Check partner product existed in system or not
                var partnerProduct = await this._unitOfWork.PartnerProductRepository.GetPartnerProductAsync(productId, partnerId, storeId, storePartner.CreatedDate);
                if (partnerProduct == null)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.NotExistPartnerProduct);
                }
                // Change status of partner product to deactive.
                partnerProduct.Status = (int)PartnerProductEnum.Status.DEACTIVE;
                this._unitOfWork.PartnerProductRepository.UpdatePartnerProduct(partnerProduct);
                this._unitOfWork.Commit();
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
                else if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidProductId))
                {
                    fieldName = "Product id";
                }
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand))
                {
                    fieldName = "Store id";
                }
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.NotLinkedWithParner))
                {
                    fieldName = "Store id";
                }

                else if (ex.Message.Equals(MessageConstant.PartnerProductMessage.DeactiveProduct_Create_Update))
                {
                    fieldName = "Product id";
                }

                else if (ex.Message.Equals(MessageConstant.PartnerProductMessage.InactiveProduct_Create_Update))
                {
                    fieldName = "Product id";
                }

                else if (ex.Message.Equals(MessageConstant.BrandMessage.ProductNotBelongToBrand))
                {
                    fieldName = "Product id";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistProductId))
                {
                    fieldName = "Product id";
                }

                else if (ex.Message.Equals(MessageConstant.CommonMessage.AlreadyExistPartnerProduct))
                {
                    fieldName = "Partner product";
                }
                else if (ex.Message.Equals(MessageConstant.PartnerProductMessage.ProductCodeExisted))
                {
                    fieldName = "Product code";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistStoreId))
                {
                    fieldName = "Store id";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistPartnerId))
                {
                    fieldName = "Partner id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
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
