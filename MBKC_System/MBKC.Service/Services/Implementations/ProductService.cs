using AutoMapper;
using MBKC.Service.Services.Interfaces;
using MBKC.Repository.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBKC.Service.DTOs.Products;
using MBKC.Service.Utils;
using System.Security.Claims;
using MBKC.Repository.Models;
using MBKC.Service.Exceptions;
using MBKC.Service.Constants;
using MBKC.Repository.Enums;
using Microsoft.Extensions.Logging.Abstractions;
using MBKC.Service.DTOs.Stores;
using Microsoft.AspNetCore.Http;
using Spire.Xls;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using ExcelPicture = Spire.Xls.ExcelPicture;
using MBKC.Service.Errors;
using System.Security.Policy;
using System.ComponentModel;
using FluentValidation;
using FluentValidation.Results;
using MBKC.Service.DTOs.MoneyExchanges;

namespace MBKC.Service.Services.Implementations
{
    public class ProductService : IProductService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private IValidator<CreateProductExcelRequest> _createProductExcelValidator;
        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateProductExcelRequest> createProductExcelValidator)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._createProductExcelValidator = createProductExcelValidator;
        }

        public async Task<GetProductsResponse> GetProductsAsync(string? searchName, int? currentPage, int? itemsPerPage,
            string? productType, bool? isGetAll, int? idCategory, int? idStore, IEnumerable<Claim> claims)
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

                if (isGetAll == true)
                {
                    currentPage = null;
                    itemsPerPage = null;
                }

                string resultProductType = "";
                if (productType != null)
                {
                    resultProductType = productType;
                }

                if (productType != null && Enum.IsDefined(typeof(ProductEnum.Type), resultProductType.ToUpper()) == false)
                {
                    throw new BadRequestException(MessageConstant.ProductMessage.InvalidProductType);
                }

                if (idCategory != null)
                {
                    if (idCategory.Value <= 0)
                    {
                        throw new BadRequestException(MessageConstant.CommonMessage.InvalidCategoryId);
                    }
                    Category existedCategory = await this._unitOfWork.CategoryRepository.GetCategoryByIdAsync(idCategory.Value);
                    if (existedCategory == null)
                    {
                        throw new NotFoundException(MessageConstant.CommonMessage.NotExistCategoryId);
                    }

                    if (existedBrand != null && existedBrand.Categories.FirstOrDefault(x => x.CategoryId == idCategory) == null)
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

                if (idStore != null)
                {
                    if (idStore.Value <= 0)
                    {
                        throw new BadRequestException(MessageConstant.CommonMessage.InvalidStoreId);
                    }
                    if (existedKitchenCenter != null || existedBrand != null || role.ToLower().Equals(RoleConstant.MBKC_Admin.ToLower()))
                    {
                        existedStore = await this._unitOfWork.StoreRepository.GetStoreAsync(idStore.Value);
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

                        if (idCategory != null && existedStore.Brand.Categories.Any(x => x.CategoryId == idCategory) == false)
                        {
                            throw new BadRequestException(MessageConstant.ProductMessage.CategoryIdNotBelongToStore);
                        }
                    }
                    else if (existedStore != null)
                    {
                        if (existedStore.StoreId != idStore)
                        {
                            throw new BadRequestException(MessageConstant.StoreMessage.StoreIdNotBelongToStore);
                        }
                    }

                }

                int numberItems = 0;
                List<Product> products = null;
                if (searchName != null && StringUtil.IsUnicode(searchName) == false)
                {
                    numberItems = await this._unitOfWork.ProductRepository.GetNumberProductsAsync(null, searchName, productType, idCategory, idStore,
                                                                                             existedBrand == null ? null : existedBrand.BrandId,
                                                                                             existedKitchenCenter == null ? null : existedKitchenCenter.KitchenCenterId);
                    products = await this._unitOfWork.ProductRepository.GetProductsAsync(null, searchName, productType, idCategory, idStore,
                                                                                         existedBrand == null ? null : existedBrand.BrandId,
                                                                                         existedKitchenCenter == null ? null : existedKitchenCenter.KitchenCenterId,
                                                                                         currentPage, itemsPerPage);
                }
                else if (searchName != null && StringUtil.IsUnicode(searchName))
                {
                    numberItems = await this._unitOfWork.ProductRepository.GetNumberProductsAsync(searchName, null, productType, idCategory, idStore,
                                                                                             existedBrand == null ? null : existedBrand.BrandId,
                                                                                             existedKitchenCenter == null ? null : existedKitchenCenter.KitchenCenterId);
                    products = await this._unitOfWork.ProductRepository.GetProductsAsync(searchName, null, productType, idCategory, idStore,
                                                                                         existedBrand == null ? null : existedBrand.BrandId,
                                                                                         existedKitchenCenter == null ? null : existedKitchenCenter.KitchenCenterId,
                                                                                         currentPage, itemsPerPage);
                }
                else if (searchName == null)
                {
                    numberItems = await this._unitOfWork.ProductRepository.GetNumberProductsAsync(null, null, productType, idCategory, idStore,
                                                                                             existedBrand == null ? null : existedBrand.BrandId,
                                                                                             existedKitchenCenter == null ? null : existedKitchenCenter.KitchenCenterId);
                    products = await this._unitOfWork.ProductRepository.GetProductsAsync(null, null, productType, idCategory, idStore,
                                                                                         existedBrand == null ? null : existedBrand.BrandId,
                                                                                         existedKitchenCenter == null ? null : existedKitchenCenter.KitchenCenterId,
                                                                                         currentPage, itemsPerPage);
                }

                int totalPages = 0;
                if (numberItems > 0 && isGetAll == null || numberItems > 0 && isGetAll != null && isGetAll == false)
                {
                    totalPages = (int)((numberItems + itemsPerPage.Value) / itemsPerPage.Value);
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
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidCurrentPage))
                {
                    fieldName = "Current page";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidItemsPerPage))
                {
                    fieldName = "Items per page";
                }
                else if (ex.Message.Equals(MessageConstant.ProductMessage.InvalidProductType))
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
                if (idProduct <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidProductId);
                }
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
                return getProductResponse;
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidProductId) ||
                    ex.Message.Equals(MessageConstant.ProductMessage.ProductNotBelongToBrand) ||
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

                Product existedProduct = await this._unitOfWork.ProductRepository.GetProductAsync(createProductRequest.Code);
                if (existedProduct != null && existedProduct.Status != (int)ProductEnum.Status.DEACTIVE)
                {
                    throw new BadRequestException(MessageConstant.ProductMessage.ProductCodeExisted);
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

                    if (createProductRequest.Type.Trim().ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()) ||
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
                else if (ex.Message.Equals(MessageConstant.ProductMessage.ProductCodeExisted))
                {
                    fieldName = "Code";
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
                if (idProduct <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidProductId);
                }
                Product existedProduct = await this._unitOfWork.ProductRepository.GetProductAsync(idProduct);
                if (existedProduct == null)
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
                if (idProduct <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidProductId);
                }
                Product existedProduct = await this._unitOfWork.ProductRepository.GetProductAsync(idProduct);
                if (existedProduct == null)
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
                existedProduct.Status = (int)ProductEnum.Status.DEACTIVE;
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
                if (idProduct <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidProductId);
                }
                Product existedProduct = await this._unitOfWork.ProductRepository.GetProductAsync(idProduct);
                if (existedProduct == null)
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

                if (updateProductRequest.Name != null)
                {
                    existedProduct.Name = updateProductRequest.Name;
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
                        childProduct.Name = existedProduct.Name + $"Size {childProduct.Size.ToLower()}";
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
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidProductId) ||
                    ex.Message.Equals(MessageConstant.ProductMessage.ProductNotBelongToBrand) ||
                    ex.Message.Equals(MessageConstant.ProductMessage.ProductIdNotParentType))
                {
                    fieldName = "Product id";
                }
                else if (ex.Message.Equals(MessageConstant.ProductMessage.ProductNameTypeChildNotAllowUpdate))
                {
                    fieldName = "Name";
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

        #region upload file excel
        public async Task UploadExelFile(IFormFile file, IEnumerable<Claim> claims)
        {
            string folderName = "Products";
            List<string> logos = new List<string>();
            bool isUploaded = false;
            List<ErrorDetail> errorDetails = new List<ErrorDetail>();

            try
            {
                #region validation
                // reading excel file
                List<CreateProductExcelRequest> excelData = FileUtil.GetDataFromExcelFile(file);

                if (!excelData.Any())
                {
                    throw new BadRequestException(MessageConstant.ProductMessage.ExcelFileHasNoData);
                }

                if (excelData.GroupBy(ex => ex.Code).Any(group => group.Count() > 1))
                {
                    throw new BadRequestException(MessageConstant.ProductMessage.DuplicateProductCode);
                }

                List<Product> productsToAdd = new List<Product>();
                Dictionary<string, FileStream> imagesToAdd = new Dictionary<string, FileStream>();
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                string email = registeredEmailClaim.Value;
                Brand existedBrand = await this._unitOfWork.BrandRepository.GetBrandAsync(email);

                foreach (var product in excelData)
                {
                    List<string> errorsOnProduct = new List<string>();
                    ValidationResult validationResult = await this._createProductExcelValidator.ValidateAsync(product);
                    if (validationResult.IsValid == false)
                    {
                        errorsOnProduct = ErrorUtil.GetErrorsOnObject(validationResult);
                    }

                    if (product.Code is not null)
                    {
                        Product existedProduct = await this._unitOfWork.ProductRepository.GetProductAsync(product.Code!);
                        if (existedProduct is not null && existedProduct.Status != (int)ProductEnum.Status.DEACTIVE)
                        {
                            errorsOnProduct.Add(MessageConstant.ProductMessage.ProductCodeExisted);
                        }
                    }

                    Product? existedParentProduct = null;
                    if (product.ParentProductId is not null)
                    {
                        existedParentProduct = await this._unitOfWork.ProductRepository.GetProductAsync(product.ParentProductId.Value);
                        if (existedParentProduct is null)
                        {
                            errorsOnProduct.Add(MessageConstant.ProductMessage.ParentProductIdNotExist);
                        }

                        if (existedParentProduct is not null && existedBrand.Products.FirstOrDefault(x => x.ProductId == product.ParentProductId) is null)
                        {
                            errorsOnProduct.Add(MessageConstant.ProductMessage.ParentProductIdNotBelongToBrand);
                        }
                    }

                    Category? existedCategory = null;
                    if (product.Type is not null
                    && (product.Type.Trim().ToUpper().Equals(ProductEnum.Type.PARENT.ToString())
                     || product.Type.Trim().ToUpper().Equals(ProductEnum.Type.SINGLE.ToString())
                     || product.Type.Trim().ToUpper().Equals(ProductEnum.Type.EXTRA.ToString()))
                    && product.CategoryId is not null)
                    {
                        existedCategory = await this._unitOfWork.CategoryRepository.GetCategoryByIdAsync(product.CategoryId.Value);
                        if (existedCategory == null)
                        {
                            errorsOnProduct.Add(MessageConstant.CommonMessage.NotExistCategoryId);
                        }

                        if (existedCategory is not null && existedBrand.Categories.FirstOrDefault(x => x.CategoryId == product.CategoryId) is null)
                        {
                            errorsOnProduct.Add(MessageConstant.CommonMessage.CategoryIdNotBelongToBrand);
                        }

                        if ((product.Type.Trim().ToUpper().Equals(ProductEnum.Type.SINGLE.ToString())
                          || product.Type.Trim().ToUpper().Equals(ProductEnum.Type.PARENT.ToString()))
                        && existedCategory is not null)
                        {
                            if (existedCategory.Type.Trim().ToUpper().Equals(CategoryEnum.Type.NORMAL.ToString()) == false)
                            {
                                errorsOnProduct.Add(MessageConstant.ProductMessage.CategoryNotSuitableForSingleOrParentProductType);
                            }
                        }

                        if (product.Type.Trim().ToUpper().Equals(ProductEnum.Type.EXTRA.ToString())
                         && existedCategory is not null)
                        {
                            if (existedCategory.Type.Trim().ToUpper().Equals(CategoryEnum.Type.EXTRA.ToString()) == false)
                            {
                                errorsOnProduct.Add(MessageConstant.ProductMessage.CategoryNotSuitableForEXTRAProductType);
                            }
                        }

                    }
                    else if (product.Type is not null && product.Type.ToUpper().Equals(ProductEnum.Type.CHILD.ToString())
                          && product.CategoryId is null
                          && existedParentProduct is not null)
                    {
                        existedCategory = existedParentProduct.Category;
                        if (product.Name is not null && product.Size is not null
                         && product.Name.Trim().ToLower().Equals($"{existedParentProduct.Name.ToLower()} - size {product.Size.ToLower()}") == false)
                        {
                            errorsOnProduct.Add(MessageConstant.ProductMessage.ProductNameNotFollowingFormat);
                        }
                    }


                    if (errorsOnProduct.Any())
                    {
                        ErrorDetail errorDetail = new ErrorDetail()
                        {
                            FieldNameError = $"Product at row [{product.Row}] in excel file",
                            DescriptionError = errorsOnProduct,
                        };

                        errorDetails.Add(errorDetail);
                    }
                    else
                    {
                        Product newProduct = new Product()
                        {
                            Code = product.Code!,
                            Name = product.Name!,
                            Description = product.Description!,
                            HistoricalPrice = product.HistoricalPrice!.Value,
                            SellingPrice = product.SellingPrice!.Value,
                            DiscountPrice = product.DiscountPrice!.Value,
                            Size = product.Size,
                            DisplayOrder = product.DisplayOrder!.Value,
                            Status = (int)ProductEnum.Status.ACTIVE,
                            Type = product.Type!.ToUpper(),
                            ParentProductId = product.ParentProductId,
                            Brand = existedBrand,
                            Category = existedCategory!
                        };
                        productsToAdd.Add(newProduct);
                        imagesToAdd.Add(product.Code!, product.Image!);
                    }
                }

                if (errorDetails.Any())
                {
                    throw new BadRequestException(MessageConstant.ProductMessage.InvalidOnField);
                }

                foreach(var product in productsToAdd)
                {
                    Guid guid = Guid.NewGuid();
                    string logoId = guid.ToString();
                    string imageUrl = await this._unitOfWork.FirebaseStorageRepository.UploadImageAsync(imagesToAdd[product.Code], folderName, logoId);
                    if (imageUrl != null && imageUrl.Length > 0 && !isUploaded)
                    {   
                        isUploaded = true;
                    }
                    imageUrl += $"&imageId={logoId}";
                    product.Image = imageUrl;
                    logos.Add(logoId);
                }
                
                #endregion

                await this._unitOfWork.ProductRepository.CreateRangProductAsync(productsToAdd);
                await this._unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                switch (ex.Message)
                {
                    case MessageConstant.ProductMessage.ExcelImageIsNotValid:
                    case MessageConstant.ProductMessage.ExcelFileHasNoData:
                        fieldName = "Excel file";
                        break;

                    case MessageConstant.ProductMessage.DuplicateProductCode:
                        fieldName = "Product code";
                        break;

                    case MessageConstant.ProductMessage.InvalidOnField:
                        string errors = ErrorUtil.GetErrorString(errorDetails);
                        throw new BadRequestException(errors);

                    default:
                        fieldName = "Exception";
                        break;
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                if (isUploaded)
                {
                    foreach (var logoId in logos) await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(logoId, folderName);
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                throw new Exception(error);
            }
        }
        #endregion
    }
}
