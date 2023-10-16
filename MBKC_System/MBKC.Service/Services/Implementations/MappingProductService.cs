using AutoMapper;
using MBKC.Repository.Enums;
using MBKC.Repository.Infrastructures;
using MBKC.Repository.Models;
using MBKC.Service.Constants;
using MBKC.Service.DTOs.MappingProducts;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using System.Security.Claims;

namespace MBKC.Service.Services.Implementations
{
    public class MappingProductService : IMappingProductService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public MappingProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        #region Create Mapping Product
        public async Task CreateMappingProduct(PostMappingProductRequest postMappingProductRequest, IEnumerable<Claim> claims)
        {
            try
            {
                if (postMappingProductRequest.StoreId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidStoreId);
                }
                if (postMappingProductRequest.PartnerId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidPartnerId);
                }
                if (postMappingProductRequest.ProductId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidProductId);
                }
                // Get brandId from claim
                Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
                var brandAccount = await this._unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId.Value));
                var brandId = brandAccount.Brand.BrandId;

                // Check store belong to brand or not
                var store = await this._unitOfWork.StoreRepository.GetStoreAsync(postMappingProductRequest.StoreId);
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
                var partner = await this._unitOfWork.PartnerRepository.GetPartnerByIdAsync(postMappingProductRequest.PartnerId);
                if (partner == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistPartnerId);
                }



                // Check the store is linked to that partner or not 
                var storePartner = await this._unitOfWork.StorePartnerRepository.GetStorePartnerByPartnerIdAndStoreIdAsync(postMappingProductRequest.PartnerId, postMappingProductRequest.StoreId);
                if (storePartner == null)
                {
                    throw new BadRequestException(MessageConstant.StorePartnerMessage.NotLinkedWithParner);
                }


                // check product belong to brand or not
                var product = await this._unitOfWork.ProductRepository.GetProductAsync(postMappingProductRequest.ProductId);
                if (product != null)
                {
                    if (product.Status == (int)ProductEnum.Status.DEACTIVE && product.Brand.BrandId == brandId)
                    {
                        throw new BadRequestException(MessageConstant.MappingProductMessage.DeactiveProduct_Create_Update);
                    }
                    else if (product.Status == (int)ProductEnum.Status.INACTIVE && product.Brand.BrandId == brandId)
                    {
                        throw new BadRequestException(MessageConstant.MappingProductMessage.InactiveProduct_Create_Update);
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
                var checkProductCode = await this._unitOfWork.MappingProductRepository.GetMappingProductByProductCodeAsync(postMappingProductRequest.ProductCode);
                if (checkProductCode != null)
                {
                    throw new BadRequestException(MessageConstant.MappingProductMessage.ProductCodeExisted);
                }

                // Check mapping product existed in system or not
                var mappingProduct = await this._unitOfWork.MappingProductRepository.GetMappingProductAsync(postMappingProductRequest.ProductId, postMappingProductRequest.PartnerId, postMappingProductRequest.StoreId, storePartner.CreatedDate);
                if (mappingProduct != null)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.AlreadyExistMappingProduct);
                }

                var mappingProductInsert = new MappingProduct()
                {
                    ProductId = postMappingProductRequest.ProductId,
                    PartnerId = postMappingProductRequest.PartnerId,
                    StoreId = postMappingProductRequest.StoreId,
                    ProductCode = postMappingProductRequest.ProductCode,
                    CreatedDate = storePartner.CreatedDate
                };
                await this._unitOfWork.MappingProductRepository.CreateMappingProductAsync(mappingProductInsert);
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

                else if (ex.Message.Equals(MessageConstant.MappingProductMessage.DeactiveProduct_Create_Update))
                {
                    fieldName = "Product id";
                }

                else if (ex.Message.Equals(MessageConstant.MappingProductMessage.InactiveProduct_Create_Update))
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

                else if (ex.Message.Equals(MessageConstant.CommonMessage.AlreadyExistMappingProduct))
                {
                    fieldName = "Mapping product";
                }
                else if (ex.Message.Equals(MessageConstant.MappingProductMessage.ProductCodeExisted))
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

        #region Get Specific Mapping Product
        public async Task<GetMappingProductResponse> GetMappingProduct(int productId, int partnerId, int storeId, IEnumerable<Claim> claims)
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
                        throw new BadRequestException(MessageConstant.MappingProductMessage.DeactiveProduct_Create_Update);
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

                // Check mapping product existed in system or not
                var mappingProduct = await this._unitOfWork.MappingProductRepository.GetMappingProductAsync(productId, partnerId, storeId, storePartner.CreatedDate);
                if (mappingProduct == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistMappingProduct);
                }

                return this._mapper.Map<GetMappingProductResponse>(mappingProduct);
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

                else if (ex.Message.Equals(MessageConstant.MappingProductMessage.DeactiveProduct_Create_Update))
                {
                    fieldName = "Product id";
                }

                else if (ex.Message.Equals(MessageConstant.MappingProductMessage.InactiveProduct_Create_Update))
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
                else if (ex.Message.Equals(MessageConstant.MappingProductMessage.ProductCodeExisted))
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
                else if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistMappingProduct))
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

        #region Get Mapping Products
        public async Task<GetMappingProductsResponse> GetMappingProducts(string? searchName, int? currentPage, int? itemsPerPage, IEnumerable<Claim> claims)
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
                List<MappingProduct> mappingProducts = null;
                if (searchName != null && StringUtil.IsUnicode(searchName) == false)
                {
                    numberItems = await this._unitOfWork.MappingProductRepository.GetNumberMappingProductsAsync(searchName, null, brandId);
                    mappingProducts = await this._unitOfWork.MappingProductRepository.GetMappingProductsAsync(searchName, null, currentPage, itemsPerPage, brandId);

                }
                else if (searchName != null && StringUtil.IsUnicode(searchName))
                {
                    numberItems = await this._unitOfWork.MappingProductRepository.GetNumberMappingProductsAsync(null, searchName, brandId);
                    mappingProducts = await this._unitOfWork.MappingProductRepository.GetMappingProductsAsync(null, searchName, currentPage, itemsPerPage, brandId);
                }
                else if (searchName == null)
                {
                    numberItems = await this._unitOfWork.MappingProductRepository.GetNumberMappingProductsAsync(null, null, brandId);
                    mappingProducts = await this._unitOfWork.MappingProductRepository.GetMappingProductsAsync(null, null, currentPage, itemsPerPage, brandId);
                }

                int totalPages = (int)((numberItems + itemsPerPage) / itemsPerPage);
                if (numberItems == 0)
                {
                    totalPages = 0;
                }
                List<GetMappingProductResponse> getMappingProductResponse = this._mapper.Map<List<GetMappingProductResponse>>(mappingProducts);
                return new GetMappingProductsResponse()
                {
                    NumberItems = numberItems,
                    TotalPages = totalPages,
                    MappingProducts = getMappingProductResponse
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

        #region Update Mapping Product
        public async Task UpdateMappingProduct(int productId, int partnerId, int storeId, UpdateMappingProductRequest updateMappingProductRequest, IEnumerable<Claim> claims)
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
                        throw new BadRequestException(MessageConstant.MappingProductMessage.InactiveStore_Update);
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
                        throw new BadRequestException(MessageConstant.MappingProductMessage.DeactiveProduct_Create_Update);
                    }
                    else if (product.Status == (int)ProductEnum.Status.INACTIVE && product.Brand.BrandId == brandId)
                    {
                        throw new BadRequestException(MessageConstant.MappingProductMessage.InactiveProduct_Create_Update);
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
                var checkProductCode = await this._unitOfWork.MappingProductRepository.GetMappingProductByProductCodeAsync(updateMappingProductRequest.ProductCode);
                if (checkProductCode != null)
                {
                    throw new BadRequestException(MessageConstant.MappingProductMessage.ProductCodeExisted);
                }

                // Check mapping product existed in system or not
                var mappingProductExisted = await this._unitOfWork.MappingProductRepository.GetMappingProductAsync(productId, partnerId, storeId, storePartner.CreatedDate);
                if (mappingProductExisted == null)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.NotExistMappingProduct);
                }
                mappingProductExisted.ProductCode = updateMappingProductRequest.ProductCode;

                this._unitOfWork.MappingProductRepository.UpdateMappingProduct(mappingProductExisted);
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

                else if (ex.Message.Equals(MessageConstant.MappingProductMessage.InactiveStore_Update))
                {
                    fieldName = "Store id";
                }

                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.NotLinkedWithParner))
                {
                    fieldName = "Store id";
                }

                else if (ex.Message.Equals(MessageConstant.MappingProductMessage.DeactiveProduct_Create_Update))
                {
                    fieldName = "Product id";
                }

                else if (ex.Message.Equals(MessageConstant.MappingProductMessage.InactiveProduct_Create_Update))
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

                else if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistMappingProduct))
                {
                    fieldName = "Mapping product";
                }
                else if (ex.Message.Equals(MessageConstant.MappingProductMessage.ProductCodeExisted))
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
    }
}
