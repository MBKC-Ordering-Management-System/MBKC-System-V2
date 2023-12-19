using AutoMapper;
using MBKC.Repository.Enums;
using MBKC.Repository.Infrastructures;
using MBKC.Repository.Models;
using MBKC.Service.Constants;
using MBKC.Service.DTOs.PartnerProducts;
using MBKC.Service.DTOs.Products;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Security.Claims;

namespace MBKC.Service.Services.Implementations
{
    public class ProductService : IProductService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        public async Task<GetProductsResponse> GetProductsAsync(GetProductsRequest getProductsRequest, IEnumerable<Claim> claims)
        {
            try
            {
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                Claim registeredRoleClaim = claims.First(x => x.Type.ToLower().Equals("role"));
                string email = registeredEmailClaim.Value;
                string role = registeredRoleClaim.Value;

                Brand existedBrand = null;
                Store existedStore = null;
                KitchenCenter existedKitchenCenter = null;
                if (role.ToLower().Equals(RoleConstant.Brand_Manager.ToLower()))
                {
                    existedBrand = await this._unitOfWork.BrandRepository.GetBrandAsync(email);
                }
                else if (role.ToLower().Equals(RoleConstant.Store_Manager.ToLower()))
                {
                    existedStore = await this._unitOfWork.StoreRepository.GetStoreAsync(email);
                }
                else if (role.ToLower().Equals(RoleConstant.Kitchen_Center_Manager.ToLower()))
                {
                    existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(email);
                }

                string resultProductType = "";
                if (getProductsRequest.ProductType != null)
                {
                    resultProductType = getProductsRequest.ProductType;
                }

                if (getProductsRequest.ProductType != null && Enum.IsDefined(typeof(ProductEnum.Type), resultProductType.ToUpper()) == false)
                {
                    throw new BadRequestException(MessageConstant.ProductMessage.InvalidProductType);
                }

                if (getProductsRequest.IdCategory != null)
                {
                    Category existedCategory = await this._unitOfWork.CategoryRepository.GetCategoryByIdAsync(getProductsRequest.IdCategory.Value);
                    if (existedCategory == null)
                    {
                        throw new NotFoundException(MessageConstant.CommonMessage.NotExistCategoryId);
                    }

                    if (existedBrand != null && existedBrand.Categories.FirstOrDefault(x => x.CategoryId == getProductsRequest.IdCategory) == null)
                    {
                        throw new BadRequestException(MessageConstant.CommonMessage.CategoryIdNotBelongToBrand);
                    }

                    if (existedStore != null && existedStore.Brand.BrandId != existedCategory.Brand.BrandId)
                    {
                        throw new BadRequestException(MessageConstant.ProductMessage.CategoryIdNotBelongToStore);
                    }

                    if (existedKitchenCenter != null && existedCategory.Brand.Stores.Any(x => x.KitchenCenter.KitchenCenterId == existedKitchenCenter.KitchenCenterId) == false)
                    {
                        throw new BadRequestException(MessageConstant.ProductMessage.CategoryIdNotBelongToKitchenCenter);
                    }
                }

                if (getProductsRequest.IdStore != null)
                {
                    if (existedKitchenCenter != null || existedBrand != null || role.ToLower().Equals(RoleConstant.MBKC_Admin.ToLower()))
                    {
                        existedStore = await this._unitOfWork.StoreRepository.GetStoreAsync(getProductsRequest.IdStore.Value);
                        if (existedStore == null)
                        {
                            throw new NotFoundException(MessageConstant.CommonMessage.NotExistStoreId);
                        }
                        if (existedKitchenCenter != null && existedStore.KitchenCenter.KitchenCenterId != existedKitchenCenter.KitchenCenterId)
                        {
                            throw new BadRequestException(MessageConstant.StoreMessage.KitchenCenterNotHaveStore);
                        }

                        if (existedBrand != null && existedStore.Brand.BrandId != existedBrand.BrandId)
                        {
                            throw new BadRequestException(MessageConstant.StoreMessage.BrandNotHaveStore);
                        }

                        if (getProductsRequest.IdCategory != null && existedStore.Brand.Categories.Any(x => x.CategoryId == getProductsRequest.IdCategory) == false)
                        {
                            throw new BadRequestException(MessageConstant.ProductMessage.CategoryIdNotBelongToStore);
                        }
                    }
                    else if (existedStore != null)
                    {
                        if (existedStore.StoreId != getProductsRequest.IdStore)
                        {
                            throw new BadRequestException(MessageConstant.StoreMessage.StoreIdNotBelongToStore);
                        }
                    }

                }

                int numberItems = 0;
                List<Product> products = null;
                if (getProductsRequest.SearchValue != null && StringUtil.IsUnicode(getProductsRequest.SearchValue) == false)
                {
                    numberItems = await this._unitOfWork.ProductRepository.GetNumberProductsAsync(null, getProductsRequest.SearchValue, getProductsRequest.ProductType,
                                                                                                        getProductsRequest.IdCategory, getProductsRequest.IdStore,
                                                                                                        existedBrand == null ? null : existedBrand.BrandId,
                                                                                                        existedKitchenCenter == null ? null : existedKitchenCenter.KitchenCenterId);
                    products = await this._unitOfWork.ProductRepository.GetProductsAsync(null, getProductsRequest.SearchValue, getProductsRequest.CurrentPage, getProductsRequest.ItemsPerPage,
                                                                                               getProductsRequest.SortBy != null && getProductsRequest.SortBy.ToLower().EndsWith("asc") ? getProductsRequest.SortBy.Split("_")[0] : null,
                                                                                               getProductsRequest.SortBy != null && getProductsRequest.SortBy.ToLower().EndsWith("desc") ? getProductsRequest.SortBy.Split("_")[0] : null,
                                                                                               getProductsRequest.ProductType, getProductsRequest.IdCategory, getProductsRequest.IdStore,
                                                                                               existedBrand == null ? null : existedBrand.BrandId,
                                                                                               existedKitchenCenter == null ? null : existedKitchenCenter.KitchenCenterId,
                                                                                               getProductsRequest.IsGetAll);
                }
                else if (getProductsRequest.SearchValue != null && StringUtil.IsUnicode(getProductsRequest.SearchValue))
                {
                    numberItems = await this._unitOfWork.ProductRepository.GetNumberProductsAsync(getProductsRequest.SearchValue, null, getProductsRequest.ProductType,
                                                                                                  getProductsRequest.IdCategory, getProductsRequest.IdStore,
                                                                                                  existedBrand == null ? null : existedBrand.BrandId,
                                                                                                  existedKitchenCenter == null ? null : existedKitchenCenter.KitchenCenterId);
                    products = await this._unitOfWork.ProductRepository.GetProductsAsync(getProductsRequest.SearchValue, null, getProductsRequest.CurrentPage, getProductsRequest.ItemsPerPage,
                                                                                               getProductsRequest.SortBy != null && getProductsRequest.SortBy.ToLower().EndsWith("asc") ? getProductsRequest.SortBy.Split("_")[0] : null,
                                                                                               getProductsRequest.SortBy != null && getProductsRequest.SortBy.ToLower().EndsWith("desc") ? getProductsRequest.SortBy.Split("_")[0] : null,
                                                                                               getProductsRequest.ProductType, getProductsRequest.IdCategory, getProductsRequest.IdStore,
                                                                                               existedBrand == null ? null : existedBrand.BrandId,
                                                                                               existedKitchenCenter == null ? null : existedKitchenCenter.KitchenCenterId,
                                                                                               getProductsRequest.IsGetAll);
                }
                else if (getProductsRequest.SearchValue == null)
                {
                    numberItems = await this._unitOfWork.ProductRepository.GetNumberProductsAsync(null, null, getProductsRequest.ProductType,
                                                                                                              getProductsRequest.IdCategory, getProductsRequest.IdStore,
                                                                                                              existedBrand == null ? null : existedBrand.BrandId,
                                                                                                              existedKitchenCenter == null ? null : existedKitchenCenter.KitchenCenterId);
                    products = await this._unitOfWork.ProductRepository.GetProductsAsync(null, null, getProductsRequest.CurrentPage, getProductsRequest.ItemsPerPage,
                                                                                               getProductsRequest.SortBy != null && getProductsRequest.SortBy.ToLower().EndsWith("asc") ? getProductsRequest.SortBy.Split("_")[0] : null,
                                                                                               getProductsRequest.SortBy != null && getProductsRequest.SortBy.ToLower().EndsWith("desc") ? getProductsRequest.SortBy.Split("_")[0] : null,
                                                                                               getProductsRequest.ProductType, getProductsRequest.IdCategory, getProductsRequest.IdStore,
                                                                                               existedBrand == null ? null : existedBrand.BrandId,
                                                                                               existedKitchenCenter == null ? null : existedKitchenCenter.KitchenCenterId,
                                                                                               getProductsRequest.IsGetAll);
                }

                int totalPages = 0;
                if (numberItems > 0 && getProductsRequest.IsGetAll == null || numberItems > 0 && getProductsRequest.IsGetAll != null && getProductsRequest.IsGetAll == false)
                {
                    totalPages = (int)((numberItems + getProductsRequest.ItemsPerPage) / getProductsRequest.ItemsPerPage);
                }

                if (numberItems == 0)
                {
                    totalPages = 0;
                }
                List<GetProductResponse> getProductResponse = this._mapper.Map<List<GetProductResponse>>(products);
                return new GetProductsResponse()
                {
                    NumberItems = numberItems,
                    TotalPages = totalPages,
                    Products = getProductResponse
                };

            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistCategoryId))
                {
                    fieldName = "Category id";
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
                if (ex.Message.Equals(MessageConstant.ProductMessage.InvalidProductType))
                {
                    fieldName = "Type";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidCategoryId) ||
                    ex.Message.Equals(MessageConstant.CommonMessage.CategoryIdNotBelongToBrand) ||
                    ex.Message.Equals(MessageConstant.ProductMessage.CategoryIdNotBelongToStore) ||
                    ex.Message.Equals(MessageConstant.ProductMessage.CategoryIdNotBelongToKitchenCenter) ||
                    ex.Message.Equals(MessageConstant.ProductMessage.CategoryIdNotBelongToStore))
                {
                    fieldName = "Category id";
                }
                else if (ex.Message.Equals(MessageConstant.StoreMessage.KitchenCenterNotHaveStore) ||
                    ex.Message.Equals(MessageConstant.StoreMessage.BrandNotHaveStore) ||
                    ex.Message.Equals(MessageConstant.StoreMessage.StoreIdNotBelongToStore))
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

        public async Task<GetProductResponse> GetProductAsync(int idProduct, IEnumerable<Claim> claims)
        {
            try
            {
                Product existedProduct = await this._unitOfWork.ProductRepository.GetProductAsync(idProduct);
                if (existedProduct == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistProductId);
                }
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                Claim registeredRoleClaim = claims.First(x => x.Type.ToLower().Equals("role"));
                string email = registeredEmailClaim.Value;
                string role = registeredRoleClaim.Value;

                Brand existedBrand = null;
                Store existedStore = null;
                KitchenCenter existedKitchenCenter = null;
                if (role.ToLower().Equals(RoleConstant.Brand_Manager.ToLower()))
                {
                    existedBrand = await this._unitOfWork.BrandRepository.GetBrandAsync(email);
                }
                else if (role.ToLower().Equals(RoleConstant.Store_Manager.ToLower()))
                {
                    existedStore = await this._unitOfWork.StoreRepository.GetStoreAsync(email);
                }
                else if (role.ToLower().Equals(RoleConstant.Kitchen_Center_Manager.ToLower()))
                {
                    existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(email);
                }

                if (existedBrand != null && existedBrand.Products.SingleOrDefault(x => x.ProductId == idProduct) == null)
                {
                    throw new BadRequestException(MessageConstant.ProductMessage.ProductNotBelongToBrand);
                }

                if (existedStore != null && existedStore.Brand.Products.SingleOrDefault(x => x.ProductId == idProduct) == null)
                {
                    throw new BadRequestException(MessageConstant.ProductMessage.ProductNotBelongToStore);
                }

                if (existedKitchenCenter != null && existedKitchenCenter.Stores.Any(x => x.Brand.Products.SingleOrDefault(x => x.ProductId == idProduct) != null) == false)
                {
                    throw new BadRequestException(MessageConstant.ProductMessage.ProductNotSpendToStore);
                }

                GetProductResponse getProductResponse = this._mapper.Map<GetProductResponse>(existedProduct);

                List<GetPartnerProductsForProductDetailResponse> partnerProducts = new List<GetPartnerProductsForProductDetailResponse>();
                if (existedProduct.PartnerProducts.Any())
                {
                    foreach (var p in existedProduct.PartnerProducts)
                    {
                        var partnerProduct = new GetPartnerProductsForProductDetailResponse()
                        {
                            PartnerName = p.StorePartner.Partner.Name,
                            ProductCode = p.ProductCode,
                            StoreName = p.StorePartner.Store.Name
                        };
                        partnerProducts.Add(partnerProduct);
                    }
                    getProductResponse.PartnerProducts = partnerProducts;
                }
                Category existedCategory = await this._unitOfWork.CategoryRepository.GetCategoryByIdAsync(existedProduct.Category.CategoryId);
                List<GetProductResponse> extraProducts = new List<GetProductResponse>();

                if (existedCategory != null && existedCategory.ExtraCategoryProductCategories.Count() > 0)
                {
                    foreach (var extraCategory in existedCategory.ExtraCategoryProductCategories)
                    {
                        if (extraCategory.ExtraCategoryNavigation is not null && extraCategory.ExtraCategoryNavigation.Products.Count() > 0)
                        {
                            foreach (var product in extraCategory.ExtraCategoryNavigation.Products)
                            {
                                GetProductResponse productResponse = this._mapper.Map<GetProductResponse>(product);
                                extraProducts.Add(productResponse);
                            }
                        }
                    }
                }
                getProductResponse.ExtraProducts = extraProducts;

                return getProductResponse;
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.ProductMessage.ProductNotBelongToBrand) ||
                    ex.Message.Equals(MessageConstant.ProductMessage.ProductNotBelongToStore) ||
                    ex.Message.Equals(MessageConstant.ProductMessage.ProductNotSpendToStore))
                {
                    fieldName = "Product id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Product id", ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task CreateProductAsync(CreateProductRequest createProductRequest, IEnumerable<Claim> claims)
        {
            string folderName = "Products";
            string logoId = "";
            bool isUploaded = false;

            try
            {
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                string email = registeredEmailClaim.Value;
                Brand existedBrand = await this._unitOfWork.BrandRepository.GetBrandAsync(email);

                Product existedProductCode = await this._unitOfWork.ProductRepository.CheckProductCodeInBrandAsync(createProductRequest.Code, existedBrand.BrandId);
                if (existedProductCode != null)
                {
                    throw new BadRequestException(MessageConstant.ProductMessage.ProductCodeExistedInBrand);
                }
                Product existedProductName = await this._unitOfWork.ProductRepository.CheckProductNameInBrandAsync(createProductRequest.Name, existedBrand.BrandId);
                if (existedProductName != null)
                {
                    throw new BadRequestException(MessageConstant.ProductMessage.ProductNameExistedInBrand);
                }
                Product existedParentProduct = null;
                if (createProductRequest.ParentProductId != null)
                {
                    existedParentProduct = await this._unitOfWork.ProductRepository.GetProductAsync(createProductRequest.ParentProductId.Value);
                    if (existedParentProduct == null)
                    {
                        throw new NotFoundException(MessageConstant.ProductMessage.ParentProductIdNotExist);
                    }

                    if (existedBrand.Products.FirstOrDefault(x => x.ProductId == createProductRequest.ParentProductId) == null)
                    {
                        throw new BadRequestException(MessageConstant.ProductMessage.ParentProductIdNotBelongToBrand);
                    }
                }

                Category existedCategory = null;
                if (createProductRequest.CategoryId != null)
                {
                    existedCategory = await this._unitOfWork.CategoryRepository.GetCategoryByIdAsync(createProductRequest.CategoryId.Value);
                    if (existedCategory == null)
                    {
                        throw new NotFoundException(MessageConstant.CommonMessage.NotExistCategoryId);
                    }

                    if (existedBrand.Categories.FirstOrDefault(x => x.CategoryId == createProductRequest.CategoryId) == null)
                    {
                        throw new BadRequestException(MessageConstant.CommonMessage.CategoryIdNotBelongToBrand);
                    }

                    if (createProductRequest.Type.Trim().ToLower().Equals(ProductEnum.Type.SINGLE.ToString().ToLower()) ||
                        createProductRequest.Type.Trim().ToLower().Equals(ProductEnum.Type.PARENT.ToString().ToLower()))
                    {
                        if (existedCategory.Type.Trim().ToLower().Equals(CategoryEnum.Type.NORMAL.ToString().ToLower()) == false)
                        {
                            throw new BadRequestException(MessageConstant.ProductMessage.CategoryNotSuitableForSingleOrParentProductType);
                        }
                    }

                    if (createProductRequest.Type.Trim().ToLower().Equals(ProductEnum.Type.EXTRA.ToString().ToLower()))
                    {
                        if (existedCategory.Type.Trim().ToLower().Equals(CategoryEnum.Type.EXTRA.ToString().ToLower()) == false)
                        {
                            throw new BadRequestException(MessageConstant.ProductMessage.CategoryNotSuitableForEXTRAProductType);
                        }
                    }

                }
                else if (createProductRequest.CategoryId == null && createProductRequest.Type.ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()))
                {
                    existedCategory = existedParentProduct.Category;
                    if (createProductRequest.Name.Trim().ToLower().Equals($"{existedParentProduct.Name.ToLower()} - size {createProductRequest.Size.ToLower()}") == false)
                    {
                        throw new BadRequestException(MessageConstant.ProductMessage.ProductNameNotFollowingFormat);
                    }
                }

                FileStream imageFileStream = FileUtil.ConvertFormFileToStream(createProductRequest.Image);
                Guid guid = Guid.NewGuid();
                logoId = guid.ToString();
                string imageUrl = await this._unitOfWork.FirebaseStorageRepository.UploadImageAsync(imageFileStream, folderName, logoId);
                if (imageUrl != null && imageUrl.Length > 0)
                {
                    isUploaded = true;
                }
                imageUrl += $"&imageId={logoId}";

                Product newProduct = new Product()
                {
                    Code = createProductRequest.Code,
                    Name = createProductRequest.Name,
                    Description = createProductRequest.Description,
                    HistoricalPrice = createProductRequest.HistoricalPrice == null ? 0 : createProductRequest.HistoricalPrice.Value,
                    SellingPrice = createProductRequest.SellingPrice == null ? 0 : createProductRequest.SellingPrice.Value,
                    DiscountPrice = createProductRequest.DiscountPrice == null ? 0 : createProductRequest.DiscountPrice.Value,
                    Size = createProductRequest.Size,
                    DisplayOrder = createProductRequest.DisplayOrder,
                    Image = imageUrl,
                    Status = (int)ProductEnum.Status.ACTIVE,
                    Type = createProductRequest.Type.ToUpper(),
                    ParentProductId = createProductRequest.ParentProductId == 0 ? null : createProductRequest.ParentProductId,
                    Brand = existedBrand,
                    Category = existedCategory
                };


                await this._unitOfWork.ProductRepository.CreateProductAsync(newProduct);
                await this._unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {

                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.ProductMessage.ParentProductIdNotBelongToBrand))
                {
                    fieldName = "Parent product id";
                }
                else if (ex.Message.Equals(MessageConstant.ProductMessage.ProductCodeExistedInBrand))
                {
                    fieldName = "Code";
                }
                else if (ex.Message.Equals(MessageConstant.ProductMessage.ProductNameExistedInBrand))
                {
                    fieldName = "Product name";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.CategoryIdNotBelongToBrand) ||
                    ex.Message.Equals(MessageConstant.ProductMessage.CategoryNotSuitableForSingleOrParentProductType) ||
                    ex.Message.Equals(MessageConstant.ProductMessage.CategoryNotSuitableForEXTRAProductType))
                {
                    fieldName = "Category id";
                }
                else if (ex.Message.Equals(MessageConstant.ProductMessage.ProductNameNotFollowingFormat))
                {
                    fieldName = "Name";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.ProductMessage.ParentProductIdNotExist))
                {
                    fieldName = "Parent product id";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistCategoryId))
                {
                    fieldName = "Category id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                if (isUploaded == false && logoId.Length > 0)
                {
                    await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(logoId, folderName);
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task UpdateProductStatusAsync(int idProduct, UpdateProductStatusRequest updateProductStatusRequest, IEnumerable<Claim> claims)
        {
            try
            {
                Product existedProduct = await this._unitOfWork.ProductRepository.GetProductAsync(idProduct);
                if (existedProduct == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistProductId);
                }
                else if (existedProduct is not null && existedProduct.Status == (int)ProductEnum.Status.DISABLE)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistProductId);
                }
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                string email = registeredEmailClaim.Value;
                Brand existedBrand = await this._unitOfWork.BrandRepository.GetBrandAsync(email);
                if (existedBrand.Products.SingleOrDefault(x => x.ProductId == idProduct) == null)
                {
                    throw new BadRequestException(MessageConstant.ProductMessage.ProductNotBelongToBrand);
                }

                if (updateProductStatusRequest.Status.Trim().ToLower().Equals(ProductEnum.Status.ACTIVE.ToString().ToLower()))
                {
                    existedProduct.Status = (int)ProductEnum.Status.ACTIVE;
                }
                else if (updateProductStatusRequest.Status.Trim().ToLower().Equals(ProductEnum.Status.INACTIVE.ToString().ToLower()))
                {
                    existedProduct.Status = (int)ProductEnum.Status.INACTIVE;
                }
                this._unitOfWork.ProductRepository.UpdateProduct(existedProduct);
                await this._unitOfWork.CommitAsync();
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Product id", ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string error = ErrorUtil.GetErrorString("Product id", ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task DeleteProductAsync(int idProduct, IEnumerable<Claim> claims)
        {
            try
            {
                Product existedProduct = await this._unitOfWork.ProductRepository.GetProductAsync(idProduct);
                if (existedProduct == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistProductId);
                }
                else if (existedProduct is not null && existedProduct.Status == (int)ProductEnum.Status.DISABLE)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistProductId);
                }
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                string email = registeredEmailClaim.Value;
                Brand existedBrand = await this._unitOfWork.BrandRepository.GetBrandAsync(email);
                if (existedBrand.Products.SingleOrDefault(x => x.ProductId == idProduct) == null)
                {
                    throw new BadRequestException(MessageConstant.ProductMessage.ProductNotBelongToBrand);
                }
                if (existedProduct.Type.ToLower().Equals(ProductEnum.Type.PARENT.ToString().ToLower()))
                {
                    if(existedProduct.ChildrenProducts is not null && existedProduct.ChildrenProducts.Count() > 0)
                    {
                        foreach (var childrenProduct in existedProduct.ChildrenProducts)
                        {
                            childrenProduct.Status = (int)ProductEnum.Status.DISABLE;
                        }
                    }
                }

                if(existedProduct.PartnerProducts is not null && existedProduct.PartnerProducts.Count() > 0)
                {
                    foreach (var partnerProduct in existedProduct.PartnerProducts)
                    {
                        partnerProduct.Status = (int)PartnerProductEnum.Status.DISABLE;
                    }
                }
                existedProduct.Status = (int)ProductEnum.Status.DISABLE;
                this._unitOfWork.ProductRepository.UpdateProduct(existedProduct);
                await this._unitOfWork.CommitAsync();
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Product id", ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string error = ErrorUtil.GetErrorString("Product id", ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task UpdateProductAsync(int idProduct, UpdateProductRequest updateProductRequest, IEnumerable<Claim> claims)
        {
            string folderName = "Products";
            string logoId = "";
            bool isUploaded = false;
            bool isDeleted = false;
            try
            {
                Product existedProduct = await this._unitOfWork.ProductRepository.GetProductAsync(idProduct);
                if (existedProduct == null )
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistProductId);
                } else if (existedProduct is not null && existedProduct.Status == (int)ProductEnum.Status.DISABLE)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistProductId);
                }

                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                string email = registeredEmailClaim.Value;
                Brand existedBrand = await this._unitOfWork.BrandRepository.GetBrandAsync(email);
                if (existedBrand.Products.SingleOrDefault(x => x.ProductId == idProduct) == null)
                {
                    throw new BadRequestException(MessageConstant.ProductMessage.ProductNotBelongToBrand);
                }

                if (existedProduct.Type.ToUpper().Equals(ProductEnum.Type.CHILD.ToString().ToUpper()) && updateProductRequest.Name != null)
                {
                    throw new BadRequestException(MessageConstant.ProductMessage.ProductNameTypeChildNotAllowUpdate);
                }
                bool isUpdatedProductName = false;
                // Check product name existed in brand or not
                if (updateProductRequest.Name != null && updateProductRequest.Name.ToLower().Equals(existedProduct.Name.ToLower()) == false)
                {
                    Product existedProductName = await this._unitOfWork.ProductRepository.CheckProductNameInBrandAsync(updateProductRequest.Name, existedBrand.BrandId);
                    if (existedProductName != null && existedProductName.ProductId != idProduct)
                    {
                        throw new BadRequestException(MessageConstant.ProductMessage.ProductNameExistedInBrand);
                    }
                    // assign product name to existed product.
                    existedProduct.Name = updateProductRequest.Name;
                    isUpdatedProductName = true;
                }

                if (existedProduct.PartnerProducts is not null && existedProduct.PartnerProducts.Where(x => x.Status != (int) PartnerProductEnum.Status.DISABLE).Count() > 0 && isUpdatedProductName == true)
                {
                    throw new BadRequestException(MessageConstant.ProductMessage.CanNotUpdateProductNameWhenHavePartnerProduct);
                }

                Product existedParentProduct = null;
                if (updateProductRequest.ParentProductId != null)
                {
                    existedParentProduct = await this._unitOfWork.ProductRepository.GetProductAsync(updateProductRequest.ParentProductId.Value);
                    if (existedParentProduct == null)
                    {
                        throw new NotFoundException(MessageConstant.ProductMessage.ParentProductIdNotExist);
                    }

                    if (existedBrand.Products.FirstOrDefault(x => x.ProductId == updateProductRequest.ParentProductId) == null)
                    {
                        throw new BadRequestException(MessageConstant.ProductMessage.ParentProductIdNotBelongToBrand);
                    }

                    if (existedParentProduct.Type.ToUpper().Equals(ProductEnum.Type.PARENT.ToString().ToUpper()) == false)
                    {
                        throw new BadRequestException(MessageConstant.ProductMessage.ProductIdNotParentType);
                    }
                    existedProduct.ParentProduct = existedParentProduct;
                }

                Category existedCategory = null;
                if (updateProductRequest.CategoryId != null)
                {
                    existedCategory = await this._unitOfWork.CategoryRepository.GetCategoryByIdAsync(updateProductRequest.CategoryId.Value);
                    if (existedCategory == null)
                    {
                        throw new NotFoundException(MessageConstant.CommonMessage.NotExistCategoryId);
                    }

                    if (existedBrand.Categories.FirstOrDefault(x => x.CategoryId == updateProductRequest.CategoryId) == null)
                    {
                        throw new BadRequestException(MessageConstant.CommonMessage.CategoryIdNotBelongToBrand);
                    }

                    if (existedProduct.Type.Trim().ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()) ||
                        existedProduct.Type.Trim().ToLower().Equals(ProductEnum.Type.PARENT.ToString().ToLower()))
                    {
                        if (existedCategory.Type.Trim().ToLower().Equals(CategoryEnum.Type.NORMAL.ToString().ToLower()) == false)
                        {
                            throw new BadRequestException(MessageConstant.ProductMessage.CategoryNotSuitableForSingleOrParentProductType);
                        }
                    }

                    if (existedProduct.Type.Trim().ToLower().Equals(ProductEnum.Type.EXTRA.ToString().ToLower()))
                    {
                        if (existedCategory.Type.Trim().ToLower().Equals(CategoryEnum.Type.EXTRA.ToString().ToLower()) == false)
                        {
                            throw new BadRequestException(MessageConstant.ProductMessage.CategoryNotSuitableForEXTRAProductType);
                        }
                    }
                    existedProduct.Category = existedCategory;
                }
                else if (updateProductRequest.CategoryId == null && existedProduct.Type.ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()))
                {
                    existedProduct.Category = existedParentProduct.Category;
                }

                if (existedProduct.Type.Trim().ToLower().Equals(ProductEnum.Type.PARENT.ToString().ToLower()))
                {
                    foreach (var childProduct in existedProduct.ChildrenProducts)
                    {
                        childProduct.Category = existedProduct.Category;
                        childProduct.Name = existedProduct.Name + $" - Size {childProduct.Size.ToLower()}";
                    }
                }

                string oldLogo = existedProduct.Image;
                if (updateProductRequest.Image != null)
                {
                    Guid guid = Guid.NewGuid();
                    logoId = guid.ToString();
                    FileStream logoFileStream = FileUtil.ConvertFormFileToStream(updateProductRequest.Image);
                    string logoLink = await this._unitOfWork.FirebaseStorageRepository.UploadImageAsync(logoFileStream, folderName, logoId);
                    if (logoLink != null && logoLink.Length > 0)
                    {
                        isUploaded = true;
                    }
                    logoLink += $"&imageId={logoId}";
                    existedProduct.Image = logoLink;
                    await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(FileUtil.GetImageIdFromUrlImage(oldLogo, "imageId"), folderName);
                    isDeleted = true;
                }

                if (updateProductRequest.Status.Trim().ToLower().Equals(ProductEnum.Status.ACTIVE.ToString().ToLower()))
                {
                    existedProduct.Status = (int)ProductEnum.Status.ACTIVE;
                }
                else if (updateProductRequest.Status.Trim().ToLower().Equals(ProductEnum.Status.INACTIVE.ToString().ToLower()))
                {
                    existedProduct.Status = (int)ProductEnum.Status.INACTIVE;
                }

                existedProduct.Description = updateProductRequest.Description;
                existedProduct.SellingPrice = updateProductRequest.SellingPrice == null ? 0 : updateProductRequest.SellingPrice.Value;
                existedProduct.HistoricalPrice = updateProductRequest.HistoricalPrice == null ? 0 : updateProductRequest.HistoricalPrice.Value;
                existedProduct.DiscountPrice = updateProductRequest.DiscountPrice == null ? 0 : updateProductRequest.DiscountPrice.Value;
                existedProduct.DisplayOrder = updateProductRequest.DisplayOrder;

                this._unitOfWork.ProductRepository.UpdateProduct(existedProduct);
                await this._unitOfWork.CommitAsync();
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistCategoryId))
                {
                    fieldName = "Category id";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistProductId))
                {
                    fieldName = "Product id";
                }
                else if (ex.Message.Equals(MessageConstant.ProductMessage.ParentProductIdNotExist))
                {
                    fieldName = "Parent product id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {

                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.ProductMessage.ProductNotBelongToBrand) ||
                    ex.Message.Equals(MessageConstant.ProductMessage.ProductIdNotParentType))
                {
                    fieldName = "Product id";
                }
                else if (ex.Message.Equals(MessageConstant.ProductMessage.ProductNameTypeChildNotAllowUpdate) ||
                    ex.Message.Equals(MessageConstant.ProductMessage.ProductNameExistedInBrand) ||
                    ex.Message.Equals(MessageConstant.ProductMessage.CanNotUpdateProductNameWhenHavePartnerProduct))
                {
                    fieldName = "Product name";
                }
                else if (ex.Message.Equals(MessageConstant.ProductMessage.ParentProductIdNotBelongToBrand))
                {
                    fieldName = "Parent product id";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.CategoryIdNotBelongToBrand) ||
                  ex.Message.Equals(MessageConstant.ProductMessage.CategoryNotSuitableForSingleOrParentProductType) ||
                  ex.Message.Equals(MessageConstant.ProductMessage.CategoryNotSuitableForEXTRAProductType))
                {
                    fieldName = "Category id";
                }
                string error = ErrorUtil.GetErrorString("Product id", ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                if (isUploaded && isDeleted == false && logoId.Length > 0)
                {
                    await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(logoId, folderName);
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task<GetProductsResponse> GetProductsWithNumberOfProductSoldAsync(GetProductWithNumberSoldRequest getProductsRequest, IEnumerable<Claim> claims)
        {
            try
            {
                #region validation
                string email = claims.First(x => x.Type == ClaimTypes.Email).Value;
                var existedBrand = await _unitOfWork.BrandRepository.GetBrandByEmailAsync(email);
                Dictionary<int, int> numberOfProductSold = new Dictionary<int, int>();

                Category? existedCategory = null;
                if (getProductsRequest.IdCategory is not null)
                {
                    existedCategory = await this._unitOfWork.CategoryRepository.GetOnlyCategoryByIdAsync(getProductsRequest.IdCategory.Value);
                    if (existedCategory is null)
                    {
                        throw new NotFoundException(MessageConstant.CommonMessage.NotExistCategoryId);
                    }

                    if (existedBrand!.Categories.Any(c => c.CategoryId == existedCategory.CategoryId) == false)
                    {
                        throw new BadRequestException(MessageConstant.CommonMessage.CategoryIdNotBelongToBrand);
                    }
                }
                #endregion

                int numberItems = 0;
                List<Product>? products = null;
                if (getProductsRequest.SearchValue != null && StringUtil.IsUnicode(getProductsRequest.SearchValue) == false)
                {
                    numberItems = await this._unitOfWork.ProductRepository.GetNumberProductWithNumberProductsSoldAsync(null, getProductsRequest.SearchValue,
                                                                                                                             getProductsRequest.ProductType,
                                                                                                                             getProductsRequest.IdCategory,
                                                                                                                             existedBrand!.BrandId);

                    products = await this._unitOfWork.ProductRepository.GetProductsWithNumberProductsSoldAsync(null, getProductsRequest.SearchValue, getProductsRequest.CurrentPage, getProductsRequest.ItemsPerPage,
                                                                                               getProductsRequest.SortBy != null && getProductsRequest.SortBy.ToLower().EndsWith("asc") ? getProductsRequest.SortBy.Split("_")[0] : null,
                                                                                               getProductsRequest.SortBy != null && getProductsRequest.SortBy.ToLower().EndsWith("desc") ? getProductsRequest.SortBy.Split("_")[0] : null,
                                                                                               getProductsRequest.ProductType, getProductsRequest.IdCategory,
                                                                                               existedBrand.BrandId,
                                                                                               getProductsRequest.IsGetAll);
                }
                else if (getProductsRequest.SearchValue != null && StringUtil.IsUnicode(getProductsRequest.SearchValue))
                {
                    numberItems = await this._unitOfWork.ProductRepository.GetNumberProductWithNumberProductsSoldAsync(getProductsRequest.SearchValue, null,
                                                                                                                       getProductsRequest.ProductType,
                                                                                                                       getProductsRequest.IdCategory,
                                                                                                                       existedBrand!.BrandId);

                    products = await this._unitOfWork.ProductRepository.GetProductsWithNumberProductsSoldAsync(getProductsRequest.SearchValue, null, getProductsRequest.CurrentPage, getProductsRequest.ItemsPerPage,
                                                                                               getProductsRequest.SortBy != null && getProductsRequest.SortBy.ToLower().EndsWith("asc") ? getProductsRequest.SortBy.Split("_")[0] : null,
                                                                                               getProductsRequest.SortBy != null && getProductsRequest.SortBy.ToLower().EndsWith("desc") ? getProductsRequest.SortBy.Split("_")[0] : null,
                                                                                               getProductsRequest.ProductType, getProductsRequest.IdCategory,
                                                                                               existedBrand.BrandId,
                                                                                               getProductsRequest.IsGetAll);
                }
                else if (getProductsRequest.SearchValue == null)
                {
                    numberItems = await this._unitOfWork.ProductRepository.GetNumberProductWithNumberProductsSoldAsync(null, null, getProductsRequest.ProductType,
                                                                                                                                   getProductsRequest.IdCategory,
                                                                                                                                   existedBrand!.BrandId);

                    products = await this._unitOfWork.ProductRepository.GetProductsWithNumberProductsSoldAsync(null, null, getProductsRequest.CurrentPage, getProductsRequest.ItemsPerPage,
                                                                                               getProductsRequest.SortBy != null && getProductsRequest.SortBy.ToLower().EndsWith("asc") ? getProductsRequest.SortBy.Split("_")[0] : null,
                                                                                               getProductsRequest.SortBy != null && getProductsRequest.SortBy.ToLower().EndsWith("desc") ? getProductsRequest.SortBy.Split("_")[0] : null,
                                                                                               getProductsRequest.ProductType, getProductsRequest.IdCategory,
                                                                                               existedBrand.BrandId,
                                                                                               getProductsRequest.IsGetAll);
                }

                int totalPages = 0;
                if (numberItems > 0 && getProductsRequest.IsGetAll == null || numberItems > 0 && getProductsRequest.IsGetAll != null && getProductsRequest.IsGetAll == false)
                {
                    totalPages = (int)((numberItems + getProductsRequest.ItemsPerPage) / getProductsRequest.ItemsPerPage);
                }

                if (numberItems == 0)
                {
                    totalPages = 0;
                }

                List<GetProductResponse> getProductResponse = this._mapper.Map<List<GetProductResponse>>(products);

                #region number of product sold
                var ordersForProductSold = new List<Order>();
                if (getProductsRequest.ProductSearchDateFrom is null
                 && getProductsRequest.ProductSearchDateTo is null)
                {
                    ordersForProductSold = await this._unitOfWork.OrderRepository.GetOrderByDateFromAndDateToAsync(DateTime.Now.AddDays(-6), DateTime.Now, existedBrand!.BrandId);
                }
                else if (getProductsRequest.ProductSearchDateFrom is not null
                      && getProductsRequest.ProductSearchDateTo is not null)
                {
                    ordersForProductSold = await this._unitOfWork.OrderRepository.GetOrderByDateFromAndDateToAsync(DateUtil.ConvertStringToDateTime(getProductsRequest.ProductSearchDateFrom), DateUtil.ConvertStringToDateTime(getProductsRequest.ProductSearchDateTo), existedBrand!.BrandId);
                }
                else if (getProductsRequest.ProductSearchDateFrom is not null)
                {
                    ordersForProductSold = await this._unitOfWork.OrderRepository.GetOrderByDateFromAndDateToAsync(DateUtil.ConvertStringToDateTime(getProductsRequest.ProductSearchDateFrom), null, existedBrand!.BrandId);
                }
                else if (getProductsRequest.ProductSearchDateTo is not null)
                {
                    ordersForProductSold = await this._unitOfWork.OrderRepository.GetOrderByDateFromAndDateToAsync(null, DateUtil.ConvertStringToDateTime(getProductsRequest.ProductSearchDateTo), existedBrand!.BrandId);
                }

                foreach (var order in ordersForProductSold)
                {
                    foreach (var orderDetail in order.OrderDetails)
                    {
                        if (numberOfProductSold.ContainsKey(orderDetail.Product.ProductId))
                        {
                            numberOfProductSold[orderDetail.Product.ProductId] += orderDetail.Quantity;
                        }
                        else
                        {
                            numberOfProductSold.Add(orderDetail.Product.ProductId, orderDetail.Quantity);
                        }
                    }
                }

                foreach (var product in getProductResponse)
                {
                    product.NumberOfProductsSold = numberOfProductSold.ContainsKey(product.ProductId) ? numberOfProductSold[product.ProductId] : 0;
                }
                #endregion

                return new GetProductsResponse()
                {
                    NumberItems = numberItems,
                    TotalPages = totalPages,
                    Products = getProductResponse
                };
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistCategoryId))
                {
                    fieldName = "Category id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.CategoryIdNotBelongToBrand))
                {
                    fieldName = "Category";
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
