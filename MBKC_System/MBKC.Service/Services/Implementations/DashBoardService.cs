using AutoMapper;
using MBKC.Repository.Enums;
using MBKC.Repository.Infrastructures;
using MBKC.Repository.Models;
using MBKC.Service.Constants;
using MBKC.Service.DTOs.Brands;
using MBKC.Service.DTOs.Cashiers.Responses;
using MBKC.Service.DTOs.DashBoards;
using MBKC.Service.DTOs.DashBoards.Brand;
using MBKC.Service.DTOs.DashBoards.KitchenCenter;
using MBKC.Service.DTOs.KitchenCenters;
using MBKC.Service.DTOs.PartnerProducts;
using MBKC.Service.DTOs.Stores;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
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

                GetAdminDashBoardResponse getAdminDashBoardResponse = new GetAdminDashBoardResponse()
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
                string email = claims.First(x => x.Type == ClaimTypes.Email).Value;
                var existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(email);
                var columnChartMoneyExchangeInLastSevenDay = new List<GetColumnChartMoneyExchangesResponse>();
                Dictionary<DateTime, decimal> amountOfEachDay;

                // total
                var totalStoreParticipating = await this._unitOfWork.StoreRepository.CountStoreNumberIsActiveFindByKitchenCenterIdAsync(existedKitchenCenter.KitchenCenterId);
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
                        if (amountOfEachDay.ContainsKey(moneyExchange.Transactions.Last().TransactionTime.Date))
                        {
                            amountOfEachDay[moneyExchange.Transactions.Last().TransactionTime.Date] += moneyExchange.Amount;
                        }
                    }

                    foreach (var day in amountOfEachDay)
                    {
                        GetColumnChartMoneyExchangesResponse getColumnChartMoneyExchangesResponse = new GetColumnChartMoneyExchangesResponse()
                        {
                            Date = day.Key,
                            Amount = day.Value,
                        };
                        columnChartMoneyExchangeInLastSevenDay.Add(getColumnChartMoneyExchangesResponse);
                    }

                    if (amountOfEachDay.ContainsKey(DateTime.Now.Date)) TotalMoneyExchangesOfKitchenCenterDaily = amountOfEachDay[DateTime.Now.Date];
                }

                // list
                var stores = await this._unitOfWork.StoreRepository.GetFiveStoreSortByActiveFindByKitchenCenterIdAsync(existedKitchenCenter.KitchenCenterId);
                var cashiers = await this._unitOfWork.CashierRepository.GetFiveCashierSortByActiveFindByKitchenCenterIdAsync(existedKitchenCenter.KitchenCenterId);

                GetKitchenCenterDashBoardResponse getKitchenCenterDashBoardResponse = new GetKitchenCenterDashBoardResponse()
                {
                    TotalStore = totalStoreParticipating,
                    TotalCashier = totalCashierInSystem,
                    TotalBalanceDaily = TotalMoneyExchangesOfKitchenCenterDaily,
                    ColumnChartMoneyExchanges = columnChartMoneyExchangeInLastSevenDay,
                    Stores = this._mapper.Map<List<GetStoreResponse>>(stores),
                    Cashiers = this._mapper.Map<List<GetCashierResponse>>(cashiers),
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
        public async Task<GetBrandDashBoardResponse> GetBrandDashBoardAsync(IEnumerable<Claim> claims, GetSearchDateDashBoardRequest getSearchDateDashBoardRequest)
        {
            try
            {
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                var brandExisted = await _unitOfWork.BrandRepository.GetBrandAsync(registeredEmailClaim.Value);

                int? totalStore = brandExisted.Stores.Where(x => x.Status == (int)StoreEnum.Status.ACTIVE).Count();
                int? totalNormalCategory = brandExisted.Categories.Where(x => x.Type.Equals(CategoryEnum.Type.NORMAL.ToString()) &&
                                                                              x.Status == (int)CategoryEnum.Status.ACTIVE).Count();
                int? totalExtraCategory = brandExisted.Categories.Where(x => x.Type.Equals(CategoryEnum.Type.EXTRA.ToString()) &&
                                                                              x.Status == (int)CategoryEnum.Status.ACTIVE).Count();
                int? totalProduct = brandExisted.Products.Where(x => x.Status == (int)ProductEnum.Status.ACTIVE).Count();

                List<GetStoreResponse> getStoreResponses = _mapper.Map<List<GetStoreResponse>>(brandExisted.Stores);
                DateTime startDateStore = DateTime.Now.Date;
                DateTime endDateStore = DateTime.Now.Date;
                DateTime startDateProduct = DateTime.Now.Date;
                DateTime endDateProduct = DateTime.Now.Date;
                if (getSearchDateDashBoardRequest.StoreSearchDateTo != null && getSearchDateDashBoardRequest.StoreSearchDateFrom != null)
                {
                    startDateStore = DateTime.ParseExact(getSearchDateDashBoardRequest.StoreSearchDateFrom, "dd/MM/yyyy", null);
                    endDateStore = DateTime.ParseExact(getSearchDateDashBoardRequest.StoreSearchDateTo, "dd/MM/yyyy", null);
                }
                if (getSearchDateDashBoardRequest.ProductSearchDateFrom != null && getSearchDateDashBoardRequest.ProductSearchDateTo != null)
                {
                    startDateProduct = DateTime.ParseExact(getSearchDateDashBoardRequest.ProductSearchDateFrom, "dd/MM/yyyy", null);
                    endDateProduct = DateTime.ParseExact(getSearchDateDashBoardRequest.ProductSearchDateTo, "dd/MM/yyyy", null);
                }
                var store = brandExisted.Stores.SingleOrDefault(x => x.StoreId == getSearchDateDashBoardRequest.StoreId && x.Status == (int)StoreEnum.Status.ACTIVE);
                GetStoreRevenueResponse? getStoreRevenueResponse = null;
                if (getSearchDateDashBoardRequest.StoreId != null && brandExisted.Stores.SingleOrDefault(x => x.StoreId == getSearchDateDashBoardRequest.StoreId).Orders.Any() && store != null)
                {
                    var totalRevenueOfStore = brandExisted.Stores.SingleOrDefault(x => x.StoreId == getSearchDateDashBoardRequest.StoreId)
                                                             .Orders.Where(x => x.PartnerOrderStatus.Equals(OrderEnum.Status.COMPLETED.ToString()) && x.SystemStatus.Equals(OrderEnum.SystemStatus.COMPLETED.ToString()))
                                                             .SelectMany(x => x.ShipperPayments)
                                                             .Where(x => x.CreateDate.Date >= startDateStore.Date && x.CreateDate.Date <= endDateStore.Date).ToList().Select(x => x.Amount).Sum();
                    // Get Store revenue
                    getStoreRevenueResponse = new GetStoreRevenueResponse
                    {
                        StoreName = store.Name,
                        Revenue = totalRevenueOfStore
                    };
                }
                List<NumberOfProductsSoldResponse> numberOfProductsSoldResponse = new List<NumberOfProductsSoldResponse>();
                if (brandExisted.Stores.SelectMany(x => x.Orders).Any())
                {
                    var orders = brandExisted.Stores.SelectMany(x => x.Orders)
                                                          .Where(x => x.PartnerOrderStatus.Equals(OrderEnum.Status.COMPLETED.ToString()) && x.SystemStatus.Equals(OrderEnum.SystemStatus.COMPLETED.ToString()))
                                                          .Select(x => x.OrderDetails.Select(x => x.Product.Name));


                    /*foreach (var order in orders)
                    {
                        numberOfProductsSoldResponse.Add(new NumberOfProductsSoldResponse
                        {
                            ProductName = order.OrderDetails.Select(x => x.Product.Name).SingleOrDefault(),
                            Quantity = 
                        });
                    }*/
                }
                return new GetBrandDashBoardResponse
                {
                    TotalNormalCategory = totalNormalCategory,
                    TotalExtraCategory = totalExtraCategory,
                    TotalStore = totalStore,
                    TotalProduct = totalProduct,
                    GetStoreResponse = getStoreResponses,
                    GetStoreRevenueResponses = getStoreRevenueResponse,
                    NumberOfProductsSoldResponses = numberOfProductsSoldResponse
                };

            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
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
