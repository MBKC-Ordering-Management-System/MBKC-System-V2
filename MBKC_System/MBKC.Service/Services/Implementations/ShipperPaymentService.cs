using AutoMapper;
using MBKC.Repository.Infrastructures;
using MBKC.Repository.Models;
using MBKC.Service.Constants;
using MBKC.Service.DTOs.ShipperPayments;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Services.Implementations
{
    public class ShipperPaymentService : IShipperPaymentService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public ShipperPaymentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        public async Task<GetShipperPaymentsResponse> GetShipperPayments(GetShipperPaymentsRequest getShipperPaymentsRequest, IEnumerable<Claim> claims)
        {
            try
            {
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                Claim registeredRoleClaim = claims.First(x => x.Type.ToLower().Equals("role"));
                string email = registeredEmailClaim.Value;
                string role = registeredRoleClaim.Value;
                Cashier? existedCashier = null;
                Store? existedStore = null;
                KitchenCenter? existedKitchenCenter = null;
                List<ShipperPayment>? existedShipperPayments = new List<ShipperPayment>();

                // Check role when user login
                if (role.ToLower().Equals(RoleConstant.Cashier.ToLower()))
                {
                    existedCashier = await this._unitOfWork.CashierRepository.GetCashierShipperPaymentAsync(email);
                    existedShipperPayments = await this._unitOfWork.ShipperPaymentRepository.GetShiperPaymentsByCashierIdAsync(existedCashier.AccountId);
                }
                else if (role.ToLower().Equals(RoleConstant.Store_Manager.ToLower()))
                {
                    existedStore = await this._unitOfWork.StoreRepository.GetStoreIncludeCashierAsync(email);
                    foreach (var cashier in existedStore.KitchenCenter.Cashiers)
                    {
                        var shipperPayments = await this._unitOfWork.ShipperPaymentRepository.GetShiperPaymentsByCashierIdAsync(cashier.AccountId);
                        existedShipperPayments.AddRange(shipperPayments);
                    }
                }
                else if (role.ToLower().Equals(RoleConstant.Kitchen_Center_Manager.ToLower()))
                {
                    existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterIncludeCashierAsync(email);
                    foreach (var cashier in existedKitchenCenter.Cashiers)
                    {
                        var shipperPayments = await this._unitOfWork.ShipperPaymentRepository.GetShiperPaymentsByCashierIdAsync(cashier.AccountId);
                        existedShipperPayments.AddRange(shipperPayments);
                    }
                }

                // Change status string to int
                int? status = null;
                if (getShipperPaymentsRequest.Status != null)
                {
                    status = StatusUtil.ChangeShipperPaymentStatus(getShipperPaymentsRequest.Status);
                }

                int numberItems = 0;
                List<ShipperPayment>? listShipperPayments = null;
                numberItems = this._unitOfWork.ShipperPaymentRepository.GetNumberShipperPaymentsAsync(existedShipperPayments,
                                                                             getShipperPaymentsRequest.PaymentMethod, status,
                                                                             getShipperPaymentsRequest.SearchDateFrom,
                                                                             getShipperPaymentsRequest.SearchDateTo);

                listShipperPayments = this._unitOfWork.ShipperPaymentRepository.GetShipperPaymentsAsync(existedShipperPayments,
                                                                                 getShipperPaymentsRequest.CurrentPage, getShipperPaymentsRequest.ItemsPerPage,
                                                                                 getShipperPaymentsRequest.SortBy != null && getShipperPaymentsRequest.SortBy.ToLower().EndsWith("asc") ? getShipperPaymentsRequest.SortBy.Split("_")[0] : null,
                                                                                 getShipperPaymentsRequest.SortBy != null && getShipperPaymentsRequest.SortBy.ToLower().EndsWith("desc") ? getShipperPaymentsRequest.SortBy.Split("_")[0] : null,
                                                                                 getShipperPaymentsRequest.PaymentMethod, status, getShipperPaymentsRequest.SearchDateFrom, getShipperPaymentsRequest.SearchDateTo);

                var getShipperPaymentResponse = this._mapper.Map<List<GetShipperPaymentResponse>>(listShipperPayments);


                if (existedStore != null)
                {
                    foreach (var item in getShipperPaymentResponse)
                    {
                        item.CashierCreated = existedStore.KitchenCenter.Cashiers
                                                          .Where(x => x.KitchenCenter.KitchenCenterId == existedStore.KitchenCenter.KitchenCenterId)
                                                          .Select(x => x.FullName)
                                                          .FirstOrDefault();
                    }
                }
                if (existedCashier != null)
                {
                    foreach (var item in getShipperPaymentResponse)
                    {
                        item.CashierCreated = existedCashier.FullName;
                    }
                }

                if (existedKitchenCenter != null)
                {
                    foreach (var item in getShipperPaymentResponse)
                    {
                        item.CashierCreated = existedKitchenCenter.Cashiers
                                                                  .Where(x => x.KitchenCenter.KitchenCenterId == existedKitchenCenter.KitchenCenterId)
                                                                  .Select(x => x.FullName)
                                                                  .FirstOrDefault();
                    }
                }
                int totalPages = 0;
                totalPages = (int)((numberItems + getShipperPaymentsRequest.ItemsPerPage) / getShipperPaymentsRequest.ItemsPerPage);

                if (numberItems == 0)
                {
                    totalPages = 0;
                }
                return new GetShipperPaymentsResponse
                {
                    TotalPages = totalPages,
                    NumberItems = numberItems,
                    ShipperPayments = getShipperPaymentResponse
                };
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                throw new Exception(error);
            }
        }
    }
}
