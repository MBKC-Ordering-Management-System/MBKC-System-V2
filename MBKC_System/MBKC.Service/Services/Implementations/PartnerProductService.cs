using AutoMapper;
using MBKC.Repository.Constants;
using MBKC.Repository.Enums;
using MBKC.Repository.GrabFood.Models;
using MBKC.Repository.Infrastructures;
using MBKC.Repository.Models;
using MBKC.Service.Constants;
using MBKC.Service.DTOs.PartnerProducts;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using System.Security.Claims;

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
        public async Task CreatePartnerProductAsync(PostPartnerProductRequest postPartnerProductRequest, IEnumerable<Claim> claims)
        {
            try
            {
                // Get brandId from claim
                Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
                var brandAccount = await this._unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId.Value));
                var brandId = brandAccount.Brand.BrandId;
                var products = brandAccount.Brand.Products.Where(p => p.Status == (int)ProductEnum.Status.ACTIVE).ToList();
                if (products == null)
                {
                    throw new BadRequestException(MessageConstant.StorePartnerMessage.BrandHasNoActiveProduct);
                }

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
                    if (product.Status == (int)ProductEnum.Status.DISABLE && product.Brand.BrandId == brandId)
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
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistProductId);
                }

                // Check Status valid or not
                if (!postPartnerProductRequest.Status.ToUpper().Equals(PartnerProductEnum.Status.AVAILABLE.ToString())
                    && !postPartnerProductRequest.Status.ToUpper().Equals(PartnerProductEnum.Status.OUT_OF_STOCK_TODAY.ToString())
                    && !postPartnerProductRequest.Status.ToUpper().Equals(PartnerProductEnum.Status.OUT_OF_STOCK_INDENTIFINITELY.ToString()))
                {
                    throw new BadRequestException(MessageConstant.PartnerProductMessage.StatusInValid);
                }

                // Check partner product existed in system or not
                var partnerProduct = await this._unitOfWork.PartnerProductRepository.GetPartnerProductAsync(postPartnerProductRequest.ProductId, postPartnerProductRequest.PartnerId, postPartnerProductRequest.StoreId, storePartner.CreatedDate);
                if (partnerProduct != null && partnerProduct.Status != (int)PartnerProductEnum.Status.DISABLE)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.AlreadyExistPartnerProduct);
                }

                if (product.Type.ToLower().Equals(ProductEnum.Type.PARENT.ToString().ToLower()) && postPartnerProductRequest.Price != 0)
                {
                    throw new BadRequestException(MessageConstant.PartnerProductMessage.ProductParentNotUpdatePrice);
                }

                int status = 0;
                switch (postPartnerProductRequest.Status.ToUpper())
                {
                    case nameof(PartnerProductEnum.Status.AVAILABLE):
                        status = (int)PartnerProductEnum.Status.AVAILABLE;
                        break;
                    case nameof(PartnerProductEnum.Status.OUT_OF_STOCK_TODAY):
                        status = (int)PartnerProductEnum.Status.OUT_OF_STOCK_TODAY;
                        break;
                    case nameof(PartnerProductEnum.Status.OUT_OF_STOCK_INDENTIFINITELY):
                        status = (int)PartnerProductEnum.Status.OUT_OF_STOCK_INDENTIFINITELY;
                        break;
                    default:
                        status = (int)PartnerProductEnum.Status.DISABLE;
                        break;
                }

                if (partner.Name.ToLower().Equals(PartnerConstant.GrabFood.ToLower()))
                {
                    // Check product code existed or not
                    GrabFoodAccount grabFoodAccount = new GrabFoodAccount()
                    {
                        Username = storePartner.UserName,
                        Password = storePartner.Password
                    };

                    string? productCodeParentProduct = null;
                    if (product.Type.ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()))
                    {
                        Product parentProduct = product.ParentProduct;
                        if (parentProduct.PartnerProducts is not null && parentProduct.PartnerProducts.Count() > 0)
                        {
                            productCodeParentProduct = parentProduct.PartnerProducts.FirstOrDefault(x => x.ProductId == parentProduct.ProductId && x.StoreId == postPartnerProductRequest.StoreId && x.PartnerId == postPartnerProductRequest.PartnerId && x.CreatedDate.Equals(storePartner.CreatedDate)).ProductCode;
                        }
                        else
                        {
                            throw new BadRequestException(MessageConstant.StorePartnerMessage.ParentProductMappingNotYet);
                        }

                    }

                    GrabFoodAuthenticationResponse grabFoodAuthenticationResponse = await this._unitOfWork.GrabFoodRepository.LoginGrabFoodAsync(grabFoodAccount);
                    GrabFoodMenu grabFoodMenu = await this._unitOfWork.GrabFoodRepository.GetGrabFoodMenuAsync(grabFoodAuthenticationResponse);
                    GrabFoodUtil.CheckProductCodeFromGrabFood(grabFoodMenu, postPartnerProductRequest.ProductCode, product.Type, postPartnerProductRequest.Price, status, false, productCodeParentProduct, product.Name);
                }

                var partnerProductInsert = new PartnerProduct()
                {
                    ProductId = postPartnerProductRequest.ProductId,
                    PartnerId = postPartnerProductRequest.PartnerId,
                    StoreId = postPartnerProductRequest.StoreId,
                    ProductCode = postPartnerProductRequest.ProductCode,
                    CreatedDate = storePartner.CreatedDate,
                    Price = postPartnerProductRequest.Price,
                    MappedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    Status = status
                };


                await this._unitOfWork.PartnerProductRepository.CreatePartnerProductAsync(partnerProductInsert);
                await this._unitOfWork.CommitAsync();
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

                else if (ex.Message.Equals(MessageConstant.CommonMessage.AlreadyExistPartnerProduct) ||
                    ex.Message.Equals(MessageConstant.StorePartnerMessage.ParentProductMappingNotYet))
                {
                    fieldName = "Mapping product";
                }
                else if (ex.Message.Equals(MessageConstant.PartnerProductMessage.ProductCodeExisted) ||
                    ex.Message.Equals(MessageConstant.PartnerProductMessage.ProductCodeNotExistInGrabFoodSystem) ||
                    ex.Message.Equals(MessageConstant.PartnerProductMessage.GrabFoodProductWithProductCodeNotMatchWithProductSystem))
                {
                    fieldName = "Product code";
                }

                else if (ex.Message.Equals(MessageConstant.PartnerProductMessage.StatusInValid) ||
                    ex.Message.Equals(MessageConstant.PartnerProductMessage.StatusNotMatchWithProductInGrabFoodSystem))
                {
                    fieldName = "Status";
                }
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.BrandHasNoActiveProduct))
                {
                    fieldName = "Product";
                }
                else if (ex.Message.Equals(MessageConstant.PartnerProductMessage.PriceNotMatchWithProductInGrabFoodSystem) || 
                    ex.Message.Equals(MessageConstant.PartnerProductMessage.ProductParentNotUpdatePrice))
                {
                    fieldName = "Price";
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
                else if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistProductId))
                {
                    fieldName = "Product id";
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
        public async Task<GetPartnerProductResponse> GetPartnerProductAsync(int productId, int partnerId, int storeId, IEnumerable<Claim> claims)
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
                    if (product.Status == (int)ProductEnum.Status.DISABLE && product.Brand.BrandId == brandId)
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
                    else if (product.Status == (int)ProductEnum.Status.DISABLE && product.Brand.BrandId != brandId)
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
                if (ex.Message.Equals(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand))
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
        public async Task<GetPartnerProductsResponse> GetPartnerProductsAsync(GetPartnerProductsRequest getPartnerProductsRequest, IEnumerable<Claim> claims)
        {
            try
            {
                // Get brandId from claim
                Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
                var brandAccount = await this._unitOfWork.BrandAccountRepository.GetBrandAccountByAccountIdAsync(int.Parse(accountId.Value));
                var brandId = brandAccount.Brand.BrandId;

                int numberItems = 0;
                List<PartnerProduct> partnerProducts = null;
                if (getPartnerProductsRequest.SearchValue != null && StringUtil.IsUnicode(getPartnerProductsRequest.SearchValue) == false)
                {
                    numberItems = await this._unitOfWork.PartnerProductRepository.GetNumberPartnerProductsAsync(null, getPartnerProductsRequest.SearchValue, brandId);
                    partnerProducts = await this._unitOfWork.PartnerProductRepository.GetPartnerProductsAsync(null, getPartnerProductsRequest.SearchValue, getPartnerProductsRequest.CurrentPage, getPartnerProductsRequest.ItemsPerPage,
                                                                                                              getPartnerProductsRequest.SortBy != null && getPartnerProductsRequest.SortBy.ToLower().EndsWith("asc") ? getPartnerProductsRequest.SortBy.Split("_")[0] : null,
                                                                                                              getPartnerProductsRequest.SortBy != null && getPartnerProductsRequest.SortBy.ToLower().EndsWith("desc") ? getPartnerProductsRequest.SortBy.Split("_")[0] : null, brandId);
                }
                else if (getPartnerProductsRequest.SearchValue != null && StringUtil.IsUnicode(getPartnerProductsRequest.SearchValue))
                {
                    numberItems = await this._unitOfWork.PartnerProductRepository.GetNumberPartnerProductsAsync(getPartnerProductsRequest.SearchValue, null, brandId);
                    partnerProducts = await this._unitOfWork.PartnerProductRepository.GetPartnerProductsAsync(getPartnerProductsRequest.SearchValue, null, getPartnerProductsRequest.CurrentPage, getPartnerProductsRequest.ItemsPerPage,
                                                                                                              getPartnerProductsRequest.SortBy != null && getPartnerProductsRequest.SortBy.ToLower().EndsWith("asc") ? getPartnerProductsRequest.SortBy.Split("_")[0] : null,
                                                                                                              getPartnerProductsRequest.SortBy != null && getPartnerProductsRequest.SortBy.ToLower().EndsWith("desc") ? getPartnerProductsRequest.SortBy.Split("_")[0] : null, brandId);
                }
                else if (getPartnerProductsRequest.SearchValue == null)
                {
                    numberItems = await this._unitOfWork.PartnerProductRepository.GetNumberPartnerProductsAsync(null, null, brandId);
                    partnerProducts = await this._unitOfWork.PartnerProductRepository.GetPartnerProductsAsync(null, null, getPartnerProductsRequest.CurrentPage, getPartnerProductsRequest.ItemsPerPage,
                                                                                                             getPartnerProductsRequest.SortBy != null && getPartnerProductsRequest.SortBy.ToLower().EndsWith("asc") ? getPartnerProductsRequest.SortBy.Split("_")[0] : null,
                                                                                                             getPartnerProductsRequest.SortBy != null && getPartnerProductsRequest.SortBy.ToLower().EndsWith("desc") ? getPartnerProductsRequest.SortBy.Split("_")[0] : null, brandId);
                }

                int totalPages = (int)((numberItems + getPartnerProductsRequest.ItemsPerPage) / getPartnerProductsRequest.ItemsPerPage);
                if (numberItems == 0)
                {
                    totalPages = 0;
                }
                List<GetPartnerProductResponse> getPartnerProductResponse = this._mapper.Map<List<GetPartnerProductResponse>>(partnerProducts);

                return new GetPartnerProductsResponse()
                {
                    NumberItems = numberItems,
                    TotalPages = totalPages,
                    PartnerProducts = getPartnerProductResponse
                };
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region Update Partner Product
        public async Task UpdatePartnerProductAsync(int productId, int partnerId, int storeId, UpdatePartnerProductRequest updatePartnerProductRequest, IEnumerable<Claim> claims)
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
                    if (product.Status == (int)ProductEnum.Status.DISABLE && product.Brand.BrandId == brandId)
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

                // Check Status valid or not
                if (!updatePartnerProductRequest.Status.ToUpper().Equals(PartnerProductEnum.Status.AVAILABLE.ToString())
                    && !updatePartnerProductRequest.Status.ToUpper().Equals(PartnerProductEnum.Status.OUT_OF_STOCK_TODAY.ToString())
                    && !updatePartnerProductRequest.Status.ToUpper().Equals(PartnerProductEnum.Status.OUT_OF_STOCK_INDENTIFINITELY.ToString()))
                {
                    throw new BadRequestException(MessageConstant.PartnerProductMessage.StatusInValid);
                }

                int status = 0;
                switch (updatePartnerProductRequest.Status.ToUpper())
                {
                    case nameof(PartnerProductEnum.Status.AVAILABLE):
                        status = (int)PartnerProductEnum.Status.AVAILABLE;
                        break;
                    case nameof(PartnerProductEnum.Status.OUT_OF_STOCK_TODAY):
                        status = (int)PartnerProductEnum.Status.OUT_OF_STOCK_TODAY;
                        break;
                    case nameof(PartnerProductEnum.Status.OUT_OF_STOCK_INDENTIFINITELY):
                        status = (int)PartnerProductEnum.Status.OUT_OF_STOCK_INDENTIFINITELY;
                        break;
                    default:
                        status = (int)PartnerProductEnum.Status.DISABLE;
                        break;
                }

                if (product.Type.ToLower().Equals(ProductEnum.Type.PARENT.ToString().ToLower()) && updatePartnerProductRequest.Price != 0)
                {
                    throw new BadRequestException(MessageConstant.PartnerProductMessage.ProductParentNotUpdatePrice);
                }

                // Check partner product existed in system or not
                var partnerProductExisted = await this._unitOfWork.PartnerProductRepository.GetPartnerProductAsync(productId, partnerId, storeId, storePartner.CreatedDate);
                if (partnerProductExisted == null)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.NotExistPartnerProduct);
                }


                if (partner.Name.ToLower().Equals(PartnerConstant.GrabFood.ToLower()))
                {
                    GrabFoodAccount grabFoodAccount = new GrabFoodAccount()
                    {
                        Username = storePartner.UserName,
                        Password = storePartner.Password
                    };
                    string? productCodeParentProduct = null;
                    if (product.Type.ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()))
                    {
                        Product parentProduct = product.ParentProduct;
                        productCodeParentProduct = parentProduct.PartnerProducts.FirstOrDefault(x => x.ProductId == parentProduct.ProductId && x.StoreId == storeId && x.PartnerId == partnerId && x.CreatedDate.Equals(storePartner.CreatedDate)).ProductCode;
                    }
                    GrabFoodAuthenticationResponse grabFoodAuthenticationResponse = await this._unitOfWork.GrabFoodRepository.LoginGrabFoodAsync(grabFoodAccount);
                    GrabFoodMenu grabFoodMenu = await this._unitOfWork.GrabFoodRepository.GetGrabFoodMenuAsync(grabFoodAuthenticationResponse);
                    GrabFoodUtil.CheckProductCodeFromGrabFood(grabFoodMenu, updatePartnerProductRequest.ProductCode, product.Type, updatePartnerProductRequest.Price, status, true, productCodeParentProduct, product.Name);
                }

                

                // assign update request to partner product existed
                partnerProductExisted.ProductCode = updatePartnerProductRequest.ProductCode;
                partnerProductExisted.UpdatedDate = DateTime.Now;
                partnerProductExisted.Price = updatePartnerProductRequest.Price;
                partnerProductExisted.Status = status;
                List<PartnerProduct> partnerProductList = new List<PartnerProduct>();
                if (partnerProductExisted.Product.Type.ToLower().Equals(ProductEnum.Type.PARENT.ToString().ToLower()))
                {
                    partnerProductList.Add(partnerProductExisted);
                    if (partnerProductExisted.Product.ChildrenProducts is not null)
                    {
                        foreach (var childrenProduct in partnerProductExisted.Product.ChildrenProducts)
                        {
                            var childPartnerProductExisted = await this._unitOfWork.PartnerProductRepository.GetPartnerProductAsync(childrenProduct.ProductId, partnerId, storeId, storePartner.CreatedDate);
                            childPartnerProductExisted.Status = status;
                            partnerProductList.Add(childPartnerProductExisted);
                        }
                    }
                    this._unitOfWork.PartnerProductRepository.UpdatePartnerProductRange(partnerProductList);
                }
                else
                {
                    this._unitOfWork.PartnerProductRepository.UpdatePartnerProduct(partnerProductExisted);
                }
                await this._unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand))
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
                else if (ex.Message.Equals(MessageConstant.PartnerProductMessage.StatusInValid) ||
                    ex.Message.Equals(MessageConstant.PartnerProductMessage.StatusNotMatchWithProductInGrabFoodSystem))
                {
                    fieldName = "Status";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistPartnerProduct))
                {
                    fieldName = "Mapping product";
                }
                else if (ex.Message.Equals(MessageConstant.PartnerProductMessage.ProductCodeExisted) ||
                    ex.Message.Equals(MessageConstant.PartnerProductMessage.ProductCodeNotExistInGrabFoodSystem))
                {
                    fieldName = "Product code";
                }
                else if (ex.Message.Equals(MessageConstant.PartnerProductMessage.PriceNotMatchWithProductInGrabFoodSystem) ||
                    ex.Message.Equals(MessageConstant.PartnerProductMessage.ProductParentNotUpdatePrice))
                {
                    fieldName = "Price";
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
                    if (product.Brand.BrandId != brandId)
                    {
                        throw new BadRequestException(MessageConstant.ProductMessage.ProductNotBelongToBrand);
                    }

                    if (product.Status == (int)ProductEnum.Status.DISABLE)
                    {
                        throw new BadRequestException(MessageConstant.PartnerProductMessage.DeactiveProduct_Create_Update);
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
                partnerProduct.Status = (int)PartnerProductEnum.Status.DISABLE;
                this._unitOfWork.PartnerProductRepository.UpdatePartnerProduct(partnerProduct);
                this._unitOfWork.Commit();
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
                    fieldName = "Store id";
                }

                else if (ex.Message.Equals(MessageConstant.PartnerProductMessage.DeactiveProduct_Create_Update) ||
                    ex.Message.Equals(MessageConstant.PartnerProductMessage.InactiveProduct_Create_Update) ||
                    ex.Message.Equals(MessageConstant.BrandMessage.ProductNotBelongToBrand) ||
                    ex.Message.Equals(MessageConstant.CommonMessage.NotExistProductId) ||
                    ex.Message.Equals(MessageConstant.BrandMessage.ProductNotBelongToBrand))
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

        #region Update Partner Product Status
        public async Task UpdatePartnerProductStatusAsync(int productId, int partnerId, int storeId, UpdatePartnerProductStatusRequest updatePartnerProductStatusRequest, IEnumerable<Claim> claims)
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
                    if (product.Status == (int)ProductEnum.Status.DISABLE && product.Brand.BrandId == brandId)
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

                if (!updatePartnerProductStatusRequest.Status.ToUpper().Equals(PartnerProductEnum.Status.AVAILABLE.ToString())
                    && !updatePartnerProductStatusRequest.Status.ToUpper().Equals(PartnerProductEnum.Status.OUT_OF_STOCK_TODAY.ToString())
                    && !updatePartnerProductStatusRequest.Status.ToUpper().Equals(PartnerProductEnum.Status.OUT_OF_STOCK_INDENTIFINITELY.ToString()))
                {
                    throw new BadRequestException(MessageConstant.PartnerProductMessage.StatusInValid);
                }


                // Check partner product existed in system or not
                var partnerProductExisted = await this._unitOfWork.PartnerProductRepository.GetPartnerProductAsync(productId, partnerId, storeId, storePartner.CreatedDate);
                if (partnerProductExisted == null)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.NotExistPartnerProduct);
                }

                int status = 0;
                if (updatePartnerProductStatusRequest.Status.Trim().ToUpper().Equals(PartnerProductEnum.Status.AVAILABLE.ToString()))
                {
                    status = (int)PartnerProductEnum.Status.AVAILABLE;
                }
                else if (updatePartnerProductStatusRequest.Status.Trim().ToUpper().Equals(PartnerProductEnum.Status.OUT_OF_STOCK_TODAY.ToString()))
                {
                    status = (int)PartnerProductEnum.Status.OUT_OF_STOCK_TODAY;
                }
                else if (updatePartnerProductStatusRequest.Status.Trim().ToUpper().Equals(PartnerProductEnum.Status.OUT_OF_STOCK_INDENTIFINITELY.ToString()))
                {
                    status = (int)PartnerProductEnum.Status.OUT_OF_STOCK_INDENTIFINITELY;
                }

                GrabFoodAccount grabFoodAccount = new GrabFoodAccount()
                {
                    Username = storePartner.UserName,
                    Password = storePartner.Password
                };
                string? productCodeParentProduct = null;
                if (partnerProductExisted.Product.Type.ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()))
                {
                    Product parentProduct = partnerProductExisted.Product.ParentProduct;
                    productCodeParentProduct = parentProduct.PartnerProducts.FirstOrDefault(x => x.ProductId == parentProduct.ProductId && x.StoreId == storeId && x.PartnerId == partnerId && x.CreatedDate.Equals(storePartner.CreatedDate)).ProductCode;
                }

                if (partner.Name.ToLower().Equals(PartnerConstant.GrabFood.ToLower()))
                {
                    GrabFoodAuthenticationResponse grabFoodAuthenticationResponse = await this._unitOfWork.GrabFoodRepository.LoginGrabFoodAsync(grabFoodAccount);
                    GrabFoodMenu grabFoodMenu = await this._unitOfWork.GrabFoodRepository.GetGrabFoodMenuAsync(grabFoodAuthenticationResponse);
                    GrabFoodUtil.CheckProductCodeFromGrabFood(grabFoodMenu, partnerProductExisted.ProductCode, partnerProductExisted.Product.Type, partnerProductExisted.Price, status, true, productCodeParentProduct, product.Name);
                }

                // assign status to partner product existed
                partnerProductExisted.Status = status;
                List<PartnerProduct> partnerProductList = new List<PartnerProduct>();
                if (partnerProductExisted.Product.Type.ToLower().Equals(ProductEnum.Type.PARENT.ToString().ToLower()))
                {
                    partnerProductList.Add(partnerProductExisted);
                    if (partnerProductExisted.Product.ChildrenProducts is not null)
                    {
                        foreach (var childrenProduct in partnerProductExisted.Product.ChildrenProducts)
                        {
                            var childPartnerProductExisted = await this._unitOfWork.PartnerProductRepository.GetPartnerProductAsync(childrenProduct.ProductId, partnerId, storeId, storePartner.CreatedDate);
                            childPartnerProductExisted.Status = status;
                            partnerProductList.Add(childPartnerProductExisted);
                        }
                    }
                    this._unitOfWork.PartnerProductRepository.UpdatePartnerProductRange(partnerProductList);
                }
                else
                {
                    this._unitOfWork.PartnerProductRepository.UpdatePartnerProduct(partnerProductExisted);
                }
                await this._unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.StorePartnerMessage.StoreNotBelongToBrand))
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
                else if (ex.Message.Equals(MessageConstant.PartnerProductMessage.ProductCodeNotExistInGrabFoodSystem))
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
