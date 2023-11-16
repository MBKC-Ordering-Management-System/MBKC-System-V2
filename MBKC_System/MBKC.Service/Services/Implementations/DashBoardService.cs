using AutoMapper;
using MBKC.Repository.Enums;
using MBKC.Repository.Infrastructures;
using MBKC.Repository.Models;
using MBKC.Service.Constants;
using MBKC.Service.DTOs.Brands;
using MBKC.Service.DTOs.Cashiers.Responses;
using MBKC.Service.DTOs.DashBoards;
using MBKC.Service.DTOs.DashBoards.Brand;
using MBKC.Service.DTOs.KitchenCenters;
using MBKC.Service.DTOs.PartnerProducts;
using MBKC.Service.DTOs.Stores;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Services.Implementations
{
    public class DashBoardService : IDashBoardService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public DashBoardService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        #region Dash board for admin
        public async Task<GetAdminDashBoardResponse> GetAdminDashBoardAsync()
        {
            try
            {
                // total
                var totalKitchenCenter = await this._unitOfWork.KitchenCenterRepository.CountKitchenCenterNumberAsync();
                var totalBrand = await this._unitOfWork.BrandRepository.CountBrandNumberAsync();
                var totalStore = await this._unitOfWork.StoreRepository.CountStoreNumberAsync();
                var totalPartner = await this._unitOfWork.PartnerRepository.CountPartnerNumberAsync();

                // list
                var kitchenCenters = await this._unitOfWork.KitchenCenterRepository.GetFiveKitchenterSortByActiveAsync();
                var brands = await this._unitOfWork.BrandRepository.GetFiveBrandSortByActiveAsync();
                var stores = await this._unitOfWork.StoreRepository.GetFiveStoreSortByActiveAsync();

                var getAdminDashBoardResponse = new GetAdminDashBoardResponse()
                {
                    TotalKitchenter = totalKitchenCenter,
                    TotalBrand = totalBrand,
                    TotalStore = totalStore,
                    TotalPartner = totalPartner,
                    KitchenCenters = this._mapper.Map<List<GetKitchenCenterResponse>>(kitchenCenters),
                    Brands = this._mapper.Map<List<GetBrandResponse>>(brands),
                    Stores = this._mapper.Map<List<GetStoreResponse>>(stores),
                };

                return getAdminDashBoardResponse;
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                throw new Exception(error);
            }

        }
        #endregion

        #region Dash board for kitchen center
        public async Task<GetKitchenCenterDashBoardResponse> GetKitchenCenterDashBoardAsync(IEnumerable<Claim> claims)
        {
            try
            {
                var email = claims.First(x => x.Type == ClaimTypes.Email).Value;
                var existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterForDashBoardAsync(email);
                var columnChartMoneyExchangeInLastSevenDay = new List<GetColumnChartResponse>();
                Dictionary<DateTime, decimal> amountOfEachDay;

                // total
                var totalStoreParticipating = await this._unitOfWork.StoreRepository.CountStoreNumberIsActiveFindByKitchenCenterIdAsync(existedKitchenCenter!.KitchenCenterId);
                var totalCashierInSystem = await this._unitOfWork.CashierRepository.CountCashierInSystemFindByKitchenCenterIdAsync(existedKitchenCenter.KitchenCenterId);

                // money exchange
                decimal TotalMoneyExchangesOfKitchenCenterDaily = 0;
                DateUtil.AddDateToDictionary(out amountOfEachDay);

                var moneyExchangesInLastSevenDay = await this._unitOfWork.MoneyExchangeRepository.GetColumnChartMoneyExchangeInLastSevenDayAsync(existedKitchenCenter.KitchenCenterId);
                if (moneyExchangesInLastSevenDay.Any())
                {
                    foreach (var moneyExchange in moneyExchangesInLastSevenDay.ToList())
                    {
                        var kitchenMoneyExchange = await this._unitOfWork.KitchenCenterMoneyExchangeRepository.GetKitchenCenterMoneyExchangeAsync(moneyExchange.ExchangeId);
                        if (kitchenMoneyExchange is null) moneyExchangesInLastSevenDay.Remove(moneyExchange);
                    }

                    foreach (var moneyExchange in moneyExchangesInLastSevenDay)
                    {
                        amountOfEachDay[moneyExchange.Transactions.Last().TransactionTime.Date] += moneyExchange.Amount;
                    }

                    foreach (var day in amountOfEachDay)
                    {
                        var getColumnChartMoneyExchangesResponse = new GetColumnChartResponse()
                        {
                            Date = day.Key,
                            Amount = day.Value,
                        };
                        columnChartMoneyExchangeInLastSevenDay.Add(getColumnChartMoneyExchangesResponse);
                    }

                    if (amountOfEachDay.ContainsKey(DateTime.Now.Date)) TotalMoneyExchangesOfKitchenCenterDaily = amountOfEachDay[DateTime.Now.Date];
                }

                var getKitchenCenterDashBoardResponse = new GetKitchenCenterDashBoardResponse()
                {
                    TotalStore = totalStoreParticipating,
                    TotalCashier = totalCashierInSystem,
                    TotalBalanceDaily = TotalMoneyExchangesOfKitchenCenterDaily,
                    ColumnChartMoneyExchanges = columnChartMoneyExchangeInLastSevenDay,
                    Stores = this._mapper.Map<List<GetStoreResponse>>(existedKitchenCenter.Stores),
                    Cashiers = this._mapper.Map<List<GetCashierResponse>>(existedKitchenCenter.Cashiers),
                };

                return getKitchenCenterDashBoardResponse;
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                throw new Exception(error);
            }

        }
        #endregion

        #region Dash board for brand
        public async Task<GetBrandDashBoardResponse> GetBrandDashBoardAsync(IEnumerable<Claim> claims, GetBrandDashBoardRequest getBrandDashBoardRequest)
        {
            try
            {
                #region validation
                string email = claims.First(x => x.Type == ClaimTypes.Email).Value;
                var existedBrand = await _unitOfWork.BrandRepository.GetBrandForDashBoardAsync(email);
                Dictionary<DateTime, decimal> revenueInLastSevenDay;
                Dictionary<int, int> numberOfProductSold = new Dictionary<int, int>();
                var columnChartRevenueInLastSevenDay = new List<GetColumnChartResponse>();
                var numberOfProductsSoldResponse = new List<GetNumberOfProductsSoldResponse>();

                var existedStore = await this._unitOfWork.StoreRepository.GetStoreExceptDeactiveByIdAsync(getBrandDashBoardRequest.StoreId!.Value);
                if (existedStore is null)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.NotExistStoreId);

                }

                if (existedBrand!.BrandId != existedStore.Brand.BrandId)
                {
                    throw new BadRequestException(MessageConstant.StoreMessage.StoreIdNotBelongToBrand);
                }
                #endregion

                // total
                var totalStore = await this._unitOfWork.StoreRepository.CountStoreNumberByBrandIdAsync(existedBrand!.BrandId);
                var totalNormalCategory = await this._unitOfWork.CategoryRepository.CountTypeCategoryNumberByBrandIdAsync(existedBrand!.BrandId, CategoryEnum.Type.NORMAL);
                var totalExtraCategory = await this._unitOfWork.CategoryRepository.CountTypeCategoryNumberByBrandIdAsync(existedBrand!.BrandId, CategoryEnum.Type.EXTRA);
                var totalProduct = await this._unitOfWork.ProductRepository.CountProductNumberByBrandIdAsync(existedBrand.BrandId);

                #region store revenue
                DateUtil.AddDateToDictionary(out revenueInLastSevenDay);
                var ordersForRevenue = await this._unitOfWork.OrderRepository.GetOrderByStoreIdAsync(getBrandDashBoardRequest.StoreId!.Value);
                if (ordersForRevenue.Any())
                {
                    foreach (var order in ordersForRevenue)
                    {
                        revenueInLastSevenDay[order.ShipperPayments.Last().CreateDate.Date] += order.ShipperPayments.Last().Amount;
                    }
                }

                foreach (var day in revenueInLastSevenDay)
                {
                    GetColumnChartResponse columnChartResponse = new GetColumnChartResponse()
                    {
                        Date = day.Key,
                        Amount = day.Value,
                    };
                    columnChartRevenueInLastSevenDay.Add(columnChartResponse);
                }

                var getStoreRevenueResponse = new GetStoreRevenueResponse()
                {
                    StoreId = existedStore.StoreId,
                    StoreName = existedStore.Name,
                    Revenues = columnChartRevenueInLastSevenDay,
                };
                #endregion

                #region number of product sold
                var ordersForProductSold = new List<Order>();
                if (getBrandDashBoardRequest.ProductSearchDateFrom is null
                 && getBrandDashBoardRequest.ProductSearchDateTo is null)
                {
                    ordersForProductSold = await this._unitOfWork.OrderRepository.GetOrderByDateFromAndDateToAsync(DateTime.Now.AddDays(-6), DateTime.Now);
                }
                else if (getBrandDashBoardRequest.ProductSearchDateFrom is not null
                      && getBrandDashBoardRequest.ProductSearchDateTo is not null)
                {
                    ordersForProductSold = await this._unitOfWork.OrderRepository.GetOrderByDateFromAndDateToAsync(DateUtil.ConvertStringToDateTime(getBrandDashBoardRequest.ProductSearchDateFrom), DateUtil.ConvertStringToDateTime(getBrandDashBoardRequest.ProductSearchDateTo));
                }
                else if (getBrandDashBoardRequest.ProductSearchDateFrom is not null)
                {
                    ordersForProductSold = await this._unitOfWork.OrderRepository.GetOrderByDateFromAndDateToAsync(DateUtil.ConvertStringToDateTime(getBrandDashBoardRequest.ProductSearchDateFrom), null);
                }
                else if (getBrandDashBoardRequest.ProductSearchDateTo is not null)
                {
                    ordersForProductSold = await this._unitOfWork.OrderRepository.GetOrderByDateFromAndDateToAsync(null, DateUtil.ConvertStringToDateTime(getBrandDashBoardRequest.ProductSearchDateTo));
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

                foreach(var product in existedBrand.Products)
                {
                    var getNumberOfProductsSoldResponse = new GetNumberOfProductsSoldResponse()
                    {
                        ProductId = product.ProductId,
                        ProductName = product.Name,
                        Quantity = numberOfProductSold.ContainsKey(product.ProductId) ? numberOfProductSold[product.ProductId] : 0,
                    };
                    numberOfProductsSoldResponse.Add(getNumberOfProductsSoldResponse);
                }
                #endregion

                var getBrandDashBoardResponse = new GetBrandDashBoardResponse()
                {
                    TotalStore = totalStore,
                    TotalNormalCategory = totalNormalCategory,
                    TotalExtraCategory = totalExtraCategory,
                    TotalProduct = totalProduct,
                    StoreRevenues = getStoreRevenueResponse,
                    NumberOfProductSolds = numberOfProductsSoldResponse,
                    Stores = this._mapper.Map<List<GetStoreResponse>>(existedBrand.Stores),
                };

                return getBrandDashBoardResponse;

            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                switch (ex.Message)
                {
                    case MessageConstant.StoreMessage.StoreIdNotBelongToBrand:
                        fieldName = "Store id";
                        break;

                    default:
                        fieldName = "Exception";
                        break;
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region Dash board for store
        public async Task<GetStoreDashBoardResponse> GetStoreDashBoardAsync(IEnumerable<Claim> claims)
        {
            try
            {
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                DateTime currentDate = DateTime.Now.Date;
                var storeExisted = await _unitOfWork.StoreRepository.GetStoreAsync(registeredEmailClaim.Value);

                int totalUpcomingOrder = storeExisted.Orders.Where(order => order.SystemStatus.Equals(OrderEnum.SystemStatus.IN_STORE.ToString()) &&
                                                                      order.PartnerOrderStatus.Equals(OrderEnum.Status.UPCOMING.ToString())).Count();

                int totalPreparingOrder = storeExisted.Orders.Where(order => order.SystemStatus.Equals(OrderEnum.SystemStatus.IN_STORE.ToString()) &&
                                                                      order.PartnerOrderStatus.Equals(OrderEnum.Status.PREPARING.ToString())).Count();

                int totalReadyOrder = storeExisted.Orders.Where(order => order.SystemStatus.Equals(OrderEnum.SystemStatus.IN_STORE.ToString()) &&
                                                                      order.PartnerOrderStatus.Equals(OrderEnum.Status.READY.ToString())).Count();

                int totalCompletedOrder = storeExisted.Orders.Where(order => order.SystemStatus.Equals(OrderEnum.SystemStatus.COMPLETED.ToString()) &&
                                                                      order.PartnerOrderStatus.Equals(OrderEnum.Status.COMPLETED.ToString())).Count();

                decimal totalRevenueDaily = storeExisted.Orders.SelectMany(x => x.ShipperPayments).Where(x => x.CreateDate.Date == currentDate).Select(x => x.Amount).Sum();

                return new GetStoreDashBoardResponse
                {
                    TotalCompletedOrder = totalCompletedOrder,
                    TotalPreparingOrder = totalPreparingOrder,
                    TotalReadyOrder = totalReadyOrder,
                    TotalUpcomingOrder = totalUpcomingOrder,
                    TotalRevenueDaily = totalRevenueDaily
                };
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region Dash board for cashier
        public Task<GetStoreDashBoardResponse> GetCashierDashBoardAsync(IEnumerable<Claim> claims)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
