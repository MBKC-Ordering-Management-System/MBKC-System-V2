using AutoMapper;
using MBKC.Service.Services.Interfaces;
using MBKC.Repository.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBKC.Service.DTOs.Orders;
using System.Security.Claims;
using MBKC.Service.Exceptions;
using MBKC.Service.Constants;
using MBKC.Repository.Models;
using MBKC.Service.Utils;
using MBKC.Repository.Enums;
using MBKC.Service.DTOs.Orders.MBKC.Service.DTOs.Orders;
using MBKC.Service.DTOs.MoneyExchanges;

namespace MBKC.Service.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        #region change order status to completed
        public async Task ConfirmOrderToCompletedAsync(ConfirmOrderToCompletedRequest confirmOrderToCompleted, IEnumerable<Claim> claims)
        {
            string folderName = "Orders";
            string imageId = "";
            bool uploaded = false;
            try
            {
                #region validation
                string email = claims.First(x => x.Type == ClaimTypes.Email).Value;
                Cashier existedCashier;
                BankingAccount? existedBankingAccount = null;
                Order? existedOrder = await this._unitOfWork.OrderRepository.GetOrderByOrderPartnerIdAsync(confirmOrderToCompleted.OrderPartnerId);

                if (existedOrder == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistOrderPartnerId);
                }
                existedCashier = await this._unitOfWork.CashierRepository.GetCashierAsync(email);


                if (!existedCashier.KitchenCenter.Stores.Any(s => s.StoreId == existedOrder.StoreId))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderNotBelongToKitchenCenter);
                }

                if (existedCashier.CashierMoneyExchanges.Any())
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.NoChangeOrderStatusWhenClosedShift);
                }

                if (confirmOrderToCompleted.BankingAccountId != null)
                {
                    if (existedOrder.PaymentMethod.ToUpper().Equals(OrderEnum.PaymentMethod.CASHLESS.ToString()))
                    {
                        throw new BadRequestException(MessageConstant.OrderMessage.OrderAlreadyPaid);
                    }

                    existedBankingAccount = await this._unitOfWork.BankingAccountRepository.GetBankingAccountAsync(confirmOrderToCompleted.BankingAccountId.Value);
                    if (existedBankingAccount == null)
                    {
                        throw new NotFoundException(MessageConstant.CommonMessage.NotExistBankingAccountId);
                    }

                    if (existedBankingAccount.Status == (int)BankingAccountEnum.Status.INACTIVE)
                    {
                        throw new BadRequestException(MessageConstant.BankingAccountMessage.BankingAccountIsInactive);
                    }

                    if (!existedCashier.KitchenCenter.BankingAccounts.Any(ba => ba.BankingAccountId == existedBankingAccount.BankingAccountId))
                    {
                        throw new BadRequestException(MessageConstant.BankingAccountMessage.BankingAccountNotBelongToKitchenCenter);
                    }
                }

                if (existedOrder.PartnerOrderStatus.ToUpper().Equals(OrderEnum.Status.PREPARING.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsPreparing);
                }
                if (existedOrder.PartnerOrderStatus.ToUpper().Equals(OrderEnum.Status.UPCOMING.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsUpcoming);
                }
                if (existedOrder.PartnerOrderStatus.ToUpper().Equals(OrderEnum.Status.COMPLETED.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsCompleted);
                }
                if (existedOrder.PartnerOrderStatus.ToUpper().Equals(OrderEnum.Status.CANCELLED.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsCancelled);
                }

                if (existedOrder.SystemStatus.ToUpper().Equals(OrderEnum.SystemStatus.IN_STORE.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsPreparing);
                }
                if (existedOrder.SystemStatus.ToUpper().Equals(OrderEnum.SystemStatus.COMPLETED.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsCompleted);
                }
                if (existedOrder.SystemStatus.ToUpper().Equals(OrderEnum.SystemStatus.CANCELLED.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsCancelled);
                }
                #endregion

                #region operation

                #region upload file
                FileStream fileStream = FileUtil.ConvertFormFileToStream(confirmOrderToCompleted.Image);
                imageId = Guid.NewGuid().ToString();
                string urlImage = await this._unitOfWork.FirebaseStorageRepository.UploadImageAsync(fileStream, folderName, imageId);
                if (urlImage != null && urlImage.Length > 0)
                {
                    uploaded = true;
                    urlImage += $"&imageId={imageId}";
                }
                #endregion

                #region orders
                existedOrder.PartnerOrderStatus = OrderEnum.Status.COMPLETED.ToString();
                existedOrder.SystemStatus = OrderEnum.SystemStatus.COMPLETED.ToString();
                this._unitOfWork.OrderRepository.UpdateOrder(existedOrder);

                OrderHistory orderHistory = new OrderHistory()
                {
                    Image = urlImage,
                    CreatedDate = DateTime.Now,
                    SystemStatus = OrderEnum.SystemStatus.COMPLETED.ToString(),
                    PartnerOrderStatus = OrderEnum.Status.COMPLETED.ToString(),
                    Order = existedOrder,
                };
                await this._unitOfWork.OrderHistoryRepository.InsertOrderHistoryAsync(orderHistory);
                #endregion

                #region shipper payment and transaction and wallet (Cash only)
                if (existedOrder.PaymentMethod.ToUpper().Equals(OrderEnum.PaymentMethod.CASH.ToString()))
                {
                    decimal finalToTalPriceSubstractDeliveryFee = existedOrder.FinalTotalPrice - existedOrder.DeliveryFee;
                    decimal finalPrice = finalToTalPriceSubstractDeliveryFee - (finalToTalPriceSubstractDeliveryFee * (decimal)existedOrder.Commission / 100);
                    ShipperPayment shipperPayment = new ShipperPayment()
                    {
                        Status = (int)ShipperPaymentEnum.Status.SUCCESS,
                        Content = $"Payment for the order[orderId:{existedOrder.Id}] with {existedOrder.Commission}% commission {StringUtil.GetContentAmountAndTime(finalPrice)}",
                        OrderId = existedOrder.Id,
                        Amount = finalPrice,
                        CreateDate = DateTime.Now,
                        PaymentMethod = confirmOrderToCompleted.BankingAccountId == null
                        ? ShipperPaymentEnum.PaymentMethod.CASH.ToString()
                        : ShipperPaymentEnum.PaymentMethod.CASHLESS.ToString(),
                        KCBankingAccountId = confirmOrderToCompleted.BankingAccountId,
                        CreateBy = existedCashier.AccountId,
                        Transactions = new List<Transaction>()
                        {
                            new Transaction()
                            {
                                TransactionTime = DateTime.Now,
                                Status = (int)TransactionEnum.Status.SUCCESS,
                                Wallet = existedCashier.Wallet,
                            }
                        }
                    };
                    await this._unitOfWork.ShipperPaymentRepository.CreateShipperPaymentAsync(shipperPayment);

                    existedCashier.Wallet.Balance += finalPrice;
                    this._unitOfWork.WalletRepository.UpdateWallet(existedCashier.Wallet);

                }
                #endregion

                await this._unitOfWork.CommitAsync();
                #endregion

            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                switch (ex.Message)
                {
                    case MessageConstant.CommonMessage.NotExistOrderPartnerId:
                        fieldName = "Order partner id";
                        break;

                    case MessageConstant.CommonMessage.NotExistBankingAccountId:
                        fieldName = "Banking account id";
                        break;

                    default:
                        fieldName = "Exception";
                        break;
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                switch (ex.Message)
                {
                    case MessageConstant.OrderMessage.OrderNotBelongToKitchenCenter:
                    case MessageConstant.OrderMessage.OrderIsPreparing:
                    case MessageConstant.OrderMessage.OrderIsReady:
                    case MessageConstant.OrderMessage.OrderIsCompleted:
                    case MessageConstant.OrderMessage.OrderIsCancelled:
                    case MessageConstant.OrderMessage.NoChangeOrderStatusWhenClosedShift:
                        fieldName = "Order";
                        break;

                    case MessageConstant.CommonMessage.InvalidBankingAccountId:
                    case MessageConstant.OrderMessage.OrderAlreadyPaid:
                        fieldName = "Banking account id";
                        break;

                    case MessageConstant.BankingAccountMessage.BankingAccountIsInactive:
                    case MessageConstant.BankingAccountMessage.BankingAccountNotBelongToKitchenCenter:
                        fieldName = "Banking account";
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
                if (uploaded)
                {
                    await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(imageId, folderName);
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region Get order by order partner id
        public async Task<GetOrderResponse> GetOrderAsync(string orderPartnerId)
        {
            try
            {
                Order existedOrder = await this._unitOfWork.OrderRepository.GetOrderByOrderPartnerIdAsync(orderPartnerId);
                if (existedOrder is null)
                {
                    throw new NotFoundException(MessageConstant.OrderMessage.OrderPartnerIdNotExist);
                }
                GetOrderResponse getOrderResponse = this._mapper.Map<GetOrderResponse>(existedOrder);
                if (getOrderResponse.ShipperPayments is not null && getOrderResponse.ShipperPayments.Count > 0)
                {
                    foreach (var shipperpayment in getOrderResponse.ShipperPayments)
                    {
                        Cashier existedCashier = await this._unitOfWork.CashierRepository.GetCashierAsync(shipperpayment.CreatedBy);
                        shipperpayment.CashierCreated = existedCashier.FullName;
                    }
                }
                return getOrderResponse;
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Order partner id", ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region Create order
        public async Task CreateOrderAsync(PostOrderRequest postOrderRequest)
        {
            try
            {
                Order existedOrder = await this._unitOfWork.OrderRepository.GetOrderByOrderPartnerIdAsync(postOrderRequest.OrderPartnerId);
                if (existedOrder is not null)
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderPartnerIdAlreadyExist);
                }

                existedOrder = await this._unitOfWork.OrderRepository.GetOrderByDisplayIdAsync(postOrderRequest.DisplayId);
                if (existedOrder is not null)
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.DisplayIdAlreadyExist);
                }

                Store existedStore = await this._unitOfWork.StoreRepository.GetStoreByIdAsync(postOrderRequest.StoreId);
                if (existedStore is null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistStoreId);
                }
                Partner existedPartner = await this._unitOfWork.PartnerRepository.GetPartnerAsync(postOrderRequest.PartnerId);
                if (existedPartner is null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistPartnerId);
                }
                if (existedStore.StorePartners.Any(x => x.PartnerId == existedPartner.PartnerId && x.Status == (int)StorePartnerEnum.Status.ACTIVE) == false)
                {
                    throw new BadRequestException(MessageConstant.StorePartnerMessage.NotLinkedWithParner);
                }
                StorePartner activeStorePartner = existedStore.StorePartners.FirstOrDefault(x => x.PartnerId == existedPartner.PartnerId && x.Status == (int)StorePartnerEnum.Status.ACTIVE);

                OrderHistory orderHistory = new OrderHistory()
                {
                    CreatedDate = DateTime.Now,
                    SystemStatus = OrderEnum.SystemStatus.IN_STORE.ToString().Split("_")[0] + " " + OrderEnum.SystemStatus.IN_STORE.ToString().Split("_")[1],
                    PartnerOrderStatus = postOrderRequest.Status.ToUpper()
                };

                Order newOrder = new Order()
                {
                    OrderPartnerId = postOrderRequest.OrderPartnerId,
                    ShipperName = postOrderRequest.ShipperName,
                    ShipperPhone = postOrderRequest.ShipperPhone,
                    CustomerName = postOrderRequest.CustomerName,
                    CustomerPhone = postOrderRequest.CustomerPhone,
                    Address = postOrderRequest.Address,
                    Commission = postOrderRequest.Commission,
                    Cutlery = postOrderRequest.Cutlery,
                    DeliveryFee = postOrderRequest.DeliveryFee,
                    DisplayId = postOrderRequest.DisplayId,
                    FinalTotalPrice = postOrderRequest.FinalTotalPrice,
                    Note = postOrderRequest.Note,
                    PartnerId = postOrderRequest.PartnerId,
                    Partner = existedPartner,
                    StoreId = postOrderRequest.StoreId,
                    PaymentMethod = postOrderRequest.PaymentMethod,
                    PartnerOrderStatus = postOrderRequest.Status.ToUpper(),
                    SystemStatus = OrderEnum.SystemStatus.IN_STORE.ToString().Split("_")[0] + " " + OrderEnum.SystemStatus.IN_STORE.ToString().Split("_")[1],
                    SubTotalPrice = postOrderRequest.SubTotalPrice,
                    TotalDiscount = postOrderRequest.TotalDiscount,
                    Store = existedStore,
                    Tax = postOrderRequest.Tax,
                    OrderDetails = new List<OrderDetail>(),
                    OrderHistories = new List<OrderHistory>() { orderHistory }
                };

                foreach (var orderDetail in postOrderRequest.OrderDetails)
                {
                    Product existedProduct = await this._unitOfWork.ProductRepository.GetProductAsync(orderDetail.ProductId);
                    if (existedProduct is null)
                    {
                        throw new NotFoundException(MessageConstant.OrderMessage.ProductInOrderNotExistInTheSystem);
                    }

                    if (existedProduct.PartnerProducts.FirstOrDefault(x => x.StoreId == existedStore.StoreId && x.PartnerId == existedPartner.PartnerId && x.CreatedDate == activeStorePartner.CreatedDate && x.ProductId == existedProduct.ProductId) is null)
                    {
                        throw new NotFoundException(MessageConstant.OrderMessage.ProductPartnerNotMappingBefore);
                    }
                    if (existedProduct.PartnerProducts.FirstOrDefault(x => x.StoreId == existedStore.StoreId && x.PartnerId == existedPartner.PartnerId && x.CreatedDate == activeStorePartner.CreatedDate && x.ProductId == existedProduct.ProductId).Status == (int)GrabFoodItemEnum.AvailableStatus.AVAILABLE)
                    {
                        throw new BadRequestException(MessageConstant.PartnerProductMessage.ProductPartnerNotAvailableNow);
                    }
                    if (existedProduct.PartnerProducts.FirstOrDefault(x => x.StoreId == existedStore.StoreId && x.PartnerId == existedPartner.PartnerId && x.CreatedDate == activeStorePartner.CreatedDate && x.ProductId == existedProduct.ProductId).Price != orderDetail.SellingPrice)
                    {
                        throw new BadRequestException(MessageConstant.PartnerProductMessage.ProductPriceNotMatchWithPartnerProduct);
                    }
                    OrderDetail newOrderDetail = new OrderDetail()
                    {
                        SellingPrice = orderDetail.SellingPrice,
                        Note = orderDetail.Note,
                        MasterOrderDetailId = null,
                        Product = existedProduct,
                        Quantity = orderDetail.Quantity
                    };
                    newOrder.OrderDetails.ToList().Add(newOrderDetail);
                    if (orderDetail.ExtraOrderDetails is not null)
                    {
                        foreach (var extraOrderDetail in orderDetail.ExtraOrderDetails)
                        {
                            Product existedProductExtra = await this._unitOfWork.ProductRepository.GetProductAsync(extraOrderDetail.ProductId);
                            if (existedProductExtra is null)
                            {
                                throw new NotFoundException(MessageConstant.OrderMessage.ProductExtraInOrderDetailNotExistInTheSystem);
                            }
                            if (existedProductExtra.PartnerProducts.FirstOrDefault(x => x.StoreId == existedStore.StoreId && x.PartnerId == existedPartner.PartnerId && x.CreatedDate == activeStorePartner.CreatedDate && x.ProductId == existedProductExtra.ProductId) is null)
                            {
                                throw new NotFoundException(MessageConstant.OrderMessage.ProductExtraPartnerNotMappingBefore);
                            }
                            if (existedProductExtra.PartnerProducts.FirstOrDefault(x => x.StoreId == existedStore.StoreId && x.PartnerId == existedPartner.PartnerId && x.CreatedDate == activeStorePartner.CreatedDate && x.ProductId == existedProductExtra.ProductId).Status == (int)GrabFoodItemEnum.AvailableStatus.AVAILABLE)
                            {
                                throw new BadRequestException(MessageConstant.PartnerProductMessage.ProductPartnerNotAvailableNow);
                            }
                            if (existedProductExtra.PartnerProducts.FirstOrDefault(x => x.StoreId == existedStore.StoreId && x.PartnerId == existedPartner.PartnerId && x.CreatedDate == activeStorePartner.CreatedDate && x.ProductId == existedProductExtra.ProductId).Price != extraOrderDetail.SellingPrice)
                            {
                                throw new BadRequestException(MessageConstant.PartnerProductMessage.ExtraProductPriceNotMatchWithPartnerProduct);
                            }
                            OrderDetail newOrderDetailExtra = new OrderDetail()
                            {
                                Note = extraOrderDetail.Note,
                                SellingPrice = extraOrderDetail.SellingPrice,
                                MasterOrderDetail = newOrderDetail,
                                Product = existedProductExtra,
                                Quantity = extraOrderDetail.Quantity
                            };
                            newOrder.OrderDetails.ToList().Add(newOrderDetailExtra);
                        }
                    }
                }
                await this._unitOfWork.OrderRepository.InsertOrderAsync(newOrder);
                await this._unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.OrderMessage.OrderPartnerIdAlreadyExist))
                {
                    fieldName = "Order partner id";
                }
                else if (ex.Message.Equals(MessageConstant.OrderMessage.DisplayIdAlreadyExist))
                {
                    fieldName = "Display id";
                }
                else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.NotLinkedWithParner) ||
                  ex.Message.Equals(MessageConstant.PartnerProductMessage.ProductPartnerNotAvailableNow))
                {
                    fieldName = "Partner product";
                }
                else if (ex.Message.Equals(MessageConstant.PartnerProductMessage.ProductPriceNotMatchWithPartnerProduct) ||
                  ex.Message.Equals(MessageConstant.PartnerProductMessage.ExtraProductPriceNotMatchWithPartnerProduct))
                {
                    fieldName = "Price";
                }
                else if (ex.Message.Equals(MessageConstant.OrderMessage.ProductInOrderNotExistInTheSystem))
                {
                    fieldName = "Product id";
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
                else if (ex.Message.Equals(MessageConstant.OrderMessage.ProductExtraInOrderDetailNotExistInTheSystem) ||
                  ex.Message.Equals(MessageConstant.OrderMessage.ProductInOrderNotExistInTheSystem))
                {
                    fieldName = "Product id";
                }
                else if (ex.Message.Equals(MessageConstant.OrderMessage.ProductPartnerNotMappingBefore) ||
                    ex.Message.Equals(MessageConstant.OrderMessage.ProductExtraPartnerNotMappingBefore))
                {
                    fieldName = "Partner product";
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

        #region Update order
        public async Task UpdateOrderAsync(PutOrderIdRequest putOrderIdRequest, PutOrderRequest putOrderRequest)
        {
            try
            {
                Order existedOrder = await this._unitOfWork.OrderRepository.GetOrderByOrderPartnerIdAsync(putOrderIdRequest.Id);
                if (existedOrder is null)
                {
                    throw new NotFoundException(MessageConstant.OrderMessage.OrderPartnerIdNotExist);
                }
                existedOrder.PartnerOrderStatus = putOrderRequest.Status.ToUpper();
                OrderHistory orderHistory = new OrderHistory()
                {
                    CreatedDate = DateTime.Now,
                    PartnerOrderStatus = putOrderRequest.Status.ToUpper(),
                    SystemStatus = OrderEnum.SystemStatus.IN_STORE.ToString().Split("_")[0] + " " + OrderEnum.SystemStatus.IN_STORE.ToString().Split("_")[1],
                };
                existedOrder.OrderHistories.ToList().Add(orderHistory);
                this._unitOfWork.OrderRepository.UpdateOrder(existedOrder);
                await this._unitOfWork.CommitAsync();
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Partner order id", ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region Get Orders
        public async Task<GetOrdersResponse> GetOrdersAsync(GetOrdersRequest getOrdersRequest, IEnumerable<Claim> claims)
        {
            try
            {
                // Get email, role, account id from claims
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                Claim registeredRoleClaim = claims.First(x => x.Type.ToLower().Equals("role"));
                Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));

                var email = registeredEmailClaim.Value;
                var role = registeredRoleClaim.Value;
                KitchenCenter? kitchenCenter = null;
                StoreAccount? storeAccount = null;
                Cashier? cashier = null;

                // Check role when user login 
                if (registeredRoleClaim.Value.Equals(RoleConstant.Kitchen_Center_Manager))
                {
                    kitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(email);
                }
                else if (registeredRoleClaim.Value.Equals(RoleConstant.Cashier))
                {
                    cashier = await this._unitOfWork.CashierRepository.GetCashierAsync(int.Parse(accountId.Value));
                    kitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(cashier.KitchenCenter.KitchenCenterId);
                }
                else if (registeredRoleClaim.Value.Equals(RoleConstant.Store_Manager))
                {
                    storeAccount = await this._unitOfWork.StoreAccountRepository.GetStoreAccountAsync(int.Parse(accountId.Value));
                }
                int numberItems = 0;
                List<Order> orders = null;
                if (getOrdersRequest.SearchValue != null && StringUtil.IsUnicode(getOrdersRequest.SearchValue))
                {
                    numberItems = await this._unitOfWork.OrderRepository.GetNumberOrdersAsync(getOrdersRequest.SearchValue, null, storeAccount == null ? null : storeAccount.StoreId, kitchenCenter == null ? null : kitchenCenter.KitchenCenterId);
                    orders = await this._unitOfWork.OrderRepository.GetOrdersAsync(getOrdersRequest.SearchValue, null, getOrdersRequest.CurrentPage, getOrdersRequest.ItemsPerPage,
                                                                                                                  getOrdersRequest.SortBy != null && getOrdersRequest.SortBy.ToLower().EndsWith("asc") ? getOrdersRequest.SortBy.Split("_")[0] : null,
                                                                                                                  getOrdersRequest.SortBy != null && getOrdersRequest.SortBy.ToLower().EndsWith("desc") ? getOrdersRequest.SortBy.Split("_")[0] : null,
                                                                                                                  storeAccount == null ? null : storeAccount.StoreId, kitchenCenter == null ? null : kitchenCenter.KitchenCenterId, getOrdersRequest.SystemStatus,
                                                                                                                  getOrdersRequest.PartnerOrderStatus);
                }
                else if (getOrdersRequest.SearchValue != null && StringUtil.IsUnicode(getOrdersRequest.SearchValue) == false)
                {
                    numberItems = await this._unitOfWork.OrderRepository.GetNumberOrdersAsync(null, getOrdersRequest.SearchValue, storeAccount == null ? null : storeAccount.StoreId, kitchenCenter == null ? null : kitchenCenter.KitchenCenterId);
                    orders = await this._unitOfWork.OrderRepository.GetOrdersAsync(null, getOrdersRequest.SearchValue, getOrdersRequest.CurrentPage, getOrdersRequest.ItemsPerPage,
                                                                                                                  getOrdersRequest.SortBy != null && getOrdersRequest.SortBy.ToLower().EndsWith("asc") ? getOrdersRequest.SortBy.Split("_")[0] : null,
                                                                                                                  getOrdersRequest.SortBy != null && getOrdersRequest.SortBy.ToLower().EndsWith("desc") ? getOrdersRequest.SortBy.Split("_")[0] : null,
                                                                                                                  storeAccount == null ? null : storeAccount.StoreId, kitchenCenter == null ? null : kitchenCenter.KitchenCenterId, getOrdersRequest.SystemStatus,
                                                                                                                  getOrdersRequest.PartnerOrderStatus);
                }
                else if (getOrdersRequest.SearchValue == null)
                {
                    numberItems = await this._unitOfWork.OrderRepository.GetNumberOrdersAsync(null, null, storeAccount == null ? null : storeAccount.StoreId, kitchenCenter == null ? null : kitchenCenter.KitchenCenterId);
                    orders = await this._unitOfWork.OrderRepository.GetOrdersAsync(null, null, getOrdersRequest.CurrentPage, getOrdersRequest.ItemsPerPage,
                                                                                                                  getOrdersRequest.SortBy != null && getOrdersRequest.SortBy.ToLower().EndsWith("asc") ? getOrdersRequest.SortBy.Split("_")[0] : null,
                                                                                                                  getOrdersRequest.SortBy != null && getOrdersRequest.SortBy.ToLower().EndsWith("desc") ? getOrdersRequest.SortBy.Split("_")[0] : null,
                                                                                                                  storeAccount == null ? null : storeAccount.StoreId, kitchenCenter == null ? null : kitchenCenter.KitchenCenterId, getOrdersRequest.SystemStatus,
                                                                                                                  getOrdersRequest.PartnerOrderStatus);
                }

                // Search by date from - date to
                if (getOrdersRequest.SearchDateFrom != null && getOrdersRequest.SearchDateTo != null)
                {
                    DateTime startDate = DateTime.ParseExact(getOrdersRequest.SearchDateFrom, "dd/MM/yyyy", null);
                    DateTime endDate = DateTime.ParseExact(getOrdersRequest.SearchDateTo, "dd/MM/yyyy", null);
                    if (orders is not null && orders.Any())
                    {
                        orders = orders.Where(x => x.OrderHistories.Any(o => o.CreatedDate >= startDate && o.CreatedDate <= endDate)).ToList();
                    }
                }
                int totalPages = 0;
                totalPages = (int)((numberItems + getOrdersRequest.ItemsPerPage) / getOrdersRequest.ItemsPerPage);

                if (numberItems == 0)
                {
                    totalPages = 0;
                }
                List<GetOrderResponse> getOrdersResponse = this._mapper.Map<List<GetOrderResponse>>(orders);

                // Get totalQuantity of each order
                foreach (GetOrderResponse order in getOrdersResponse)
                {
                    List<int> listQuantity = new List<int>();
                    foreach (var orderDetail in order.OrderDetails)
                    {
                        listQuantity.Add(orderDetail.Quantity);
                    }
                    int totalQuantity = listQuantity.Sum();
                    order.TotalQuantity = totalQuantity;
                }
                GetOrdersResponse getKitchenCenters = new GetOrdersResponse()
                {
                    NumberItems = numberItems,
                    TotalPages = totalPages,
                    Orders = getOrdersResponse
                };
                return getKitchenCenters;
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        #endregion

        #region Get order by order id
        public async Task<GetOrderResponse> GetOrderAsync(OrderRequest getOrderRequest, IEnumerable<Claim> claims)
        {
            try
            {
                Order existedOrder = await this._unitOfWork.OrderRepository.GetOrderAsync(getOrderRequest.Id);
                // Get email, role, account id from claims
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                Claim registeredRoleClaim = claims.First(x => x.Type.ToLower().Equals("role"));
                Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));

                var email = registeredEmailClaim.Value;
                var role = registeredRoleClaim.Value;
                KitchenCenter? kitchenCenter = null;
                StoreAccount? storeAccount = null;
                Cashier? cashier = null;

                // Check role when user login 
                if (registeredRoleClaim.Value.Equals(RoleConstant.Kitchen_Center_Manager))
                {
                    kitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(email);
                }
                else if (registeredRoleClaim.Value.Equals(RoleConstant.Cashier))
                {
                    cashier = await this._unitOfWork.CashierRepository.GetCashierAsync(int.Parse(accountId.Value));
                    kitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(cashier.KitchenCenter.KitchenCenterId);
                }
                else if (registeredRoleClaim.Value.Equals(RoleConstant.Store_Manager))
                {
                    storeAccount = await this._unitOfWork.StoreAccountRepository.GetStoreAccountAsync(int.Parse(accountId.Value));
                }

                // Check order id exist or not
                if (existedOrder == null)
                {
                    throw new NotFoundException(MessageConstant.OrderMessage.OrderIdNotExist);
                }

                // Check order belong to store or not.
                if (storeAccount != null)
                {
                    if (existedOrder.StoreId != storeAccount.StoreId)
                    {
                        throw new BadRequestException(MessageConstant.OrderMessage.OrderIdNotBelongToStore);
                    }
                }
                else if (kitchenCenter != null) // Check order belong to kitchen center or not.
                {
                    if (existedOrder.Store.KitchenCenter.KitchenCenterId != kitchenCenter.KitchenCenterId)
                    {
                        throw new BadRequestException(MessageConstant.OrderMessage.OrderIdNotBelongToKitchenCenter);
                    }
                }
                else if (cashier != null) // Check order belong to kitchen center or not.
                {
                    if (existedOrder.Store.KitchenCenter.KitchenCenterId != kitchenCenter.KitchenCenterId)
                    {
                        throw new BadRequestException(MessageConstant.OrderMessage.OrderIdNotBelongToKitchenCenter);
                    }
                }
                GetOrderResponse getOrderResponse = this._mapper.Map<GetOrderResponse>(existedOrder);
                return getOrderResponse;

            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.OrderMessage.OrderIdNotExist))
                {
                    fieldName = "Order id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }

            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.OrderMessage.OrderIdNotBelongToKitchenCenter)
                    || ex.Message.Equals(MessageConstant.OrderMessage.OrderIdNotBelongToKitchenCenter))
                {
                    fieldName = "Order id";
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

        #region Change status order to ready
        public async Task ChangeOrderStatusToReadyAsync(OrderRequest orderRequest, IEnumerable<Claim> claims)
        {
            try
            {
                // get account id from claims
                Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
                var storeAccount = await this._unitOfWork.StoreAccountRepository.GetStoreAccountAsync(int.Parse(accountId.Value));
                Order existedOrder = await this._unitOfWork.OrderRepository.GetOrderAsync(orderRequest.Id);

                // Check order id exist or not
                if (existedOrder == null)
                {
                    throw new NotFoundException(MessageConstant.OrderMessage.OrderIdNotExist);
                }

                // Check order belong to store or not.
                if (storeAccount != null)
                {
                    if (existedOrder.StoreId != storeAccount.StoreId)
                    {
                        throw new BadRequestException(MessageConstant.OrderMessage.OrderIdNotBelongToStore);
                    }
                }

                // Check partner order status  - partner order status must be PREPARING
                if (existedOrder.PartnerOrderStatus.ToUpper().Equals(OrderEnum.Status.UPCOMING.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsUpcoming_Change_To_Ready);
                }
                if (existedOrder.PartnerOrderStatus.ToUpper().Equals(OrderEnum.Status.COMPLETED.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsCompleted_Change_To_Ready);
                }
                if (existedOrder.PartnerOrderStatus.ToUpper().Equals(OrderEnum.Status.CANCELLED.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsCancelled_Change_To_Ready);
                }
                if (existedOrder.PartnerOrderStatus.ToUpper().Equals(OrderEnum.Status.READY.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsReady_Change_To_Ready);
                }

                // Check system status - system satatus must be IN_STORE
                if (existedOrder.SystemStatus.ToUpper().Equals(OrderEnum.SystemStatus.COMPLETED.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsCompleted_Change_To_Ready);
                }
                if (existedOrder.SystemStatus.ToUpper().Equals(OrderEnum.SystemStatus.CANCELLED.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsCancelled_Change_To_Ready);
                }
                if (existedOrder.SystemStatus.ToUpper().Equals(OrderEnum.SystemStatus.READY_DELIVERY.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsReadyDelivery_Change_To_Ready);
                }

                #region orders
                // assign READY status to partner order status.
                existedOrder.PartnerOrderStatus = OrderEnum.Status.READY.ToString();
                this._unitOfWork.OrderRepository.UpdateOrder(existedOrder);

                OrderHistory orderHistory = new OrderHistory()
                {
                    CreatedDate = DateTime.Now,
                    PartnerOrderStatus = OrderEnum.Status.READY.ToString(),
                    SystemStatus = OrderEnum.SystemStatus.IN_STORE.ToString(),
                    Order = existedOrder,
                };
                await this._unitOfWork.OrderHistoryRepository.InsertOrderHistoryAsync(orderHistory);
                await this._unitOfWork.CommitAsync();
                #endregion
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.OrderMessage.OrderIdNotExist))
                {
                    fieldName = "Order id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                switch (ex.Message)
                {
                    case MessageConstant.OrderMessage.OrderIsReady_Change_To_Ready:
                    case MessageConstant.OrderMessage.OrderIsUpcoming_Change_To_Ready:
                    case MessageConstant.OrderMessage.OrderIsCompleted_Change_To_Ready:
                    case MessageConstant.OrderMessage.OrderIsCancelled_Change_To_Ready:
                    case MessageConstant.OrderMessage.OrderIsReadyDelivery_Change_To_Ready:
                        fieldName = "Order";
                        break;
                    case MessageConstant.OrderMessage.OrderIdNotBelongToStore:
                        fieldName = "Order id";
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
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        #region Change order status to ready delivery
        public async Task ChangeOrderStatusToReadyDeliveryAsync(OrderRequest orderRequest, IEnumerable<Claim> claims)
        {
            try
            {
                // get account id from claims
                Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
                var cashier = await this._unitOfWork.CashierRepository.GetCashierWithMoneyExchangeTypeIsSendAsync(int.Parse(accountId.Value));
                if (cashier == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistCashierId);
                }

                // Check cashier shift ended or not
                if (cashier.CashierMoneyExchanges.Any())
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.NoChangeOrderStatusWhenClosedShift);
                }

                // Check kitchenCenter exist or not
                var kitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(cashier.KitchenCenter.KitchenCenterId);
                if (kitchenCenter == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistKitchenCenterId);
                }

                // Check order id exist or not
                Order existedOrder = await this._unitOfWork.OrderRepository.GetOrderAsync(orderRequest.Id);
                if (existedOrder == null)
                {
                    throw new NotFoundException(MessageConstant.OrderMessage.OrderIdNotExist);
                }

                // Check order belong to kitchen center or not
                if (kitchenCenter != null)
                {
                    if (existedOrder.Store.KitchenCenter.KitchenCenterId != kitchenCenter.KitchenCenterId)
                    {
                        throw new BadRequestException(MessageConstant.OrderMessage.OrderIdNotBelongToKitchenCenter);
                    }
                }

                // Check partner order status - partner order status must be READY
                if (existedOrder.PartnerOrderStatus.ToUpper().Equals(OrderEnum.Status.UPCOMING.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsUpcoming_Change_To_ReadyDelivery);
                }
                if (existedOrder.PartnerOrderStatus.ToUpper().Equals(OrderEnum.Status.COMPLETED.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsCompeleted_Change_To_ReadyDelivery);
                }
                if (existedOrder.PartnerOrderStatus.ToUpper().Equals(OrderEnum.Status.CANCELLED.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsCancelled_Change_To_ReadyDelivery);
                }
                if (existedOrder.PartnerOrderStatus.ToUpper().Equals(OrderEnum.Status.PREPARING.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsPreparing_Change_To_ReadyDelivery);
                }

                // Check system status - system status must be IN_STORE
                if (existedOrder.SystemStatus.ToUpper().Equals(OrderEnum.SystemStatus.COMPLETED.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsCompeleted_Change_To_ReadyDelivery);
                }
                if (existedOrder.SystemStatus.ToUpper().Equals(OrderEnum.SystemStatus.CANCELLED.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsCancelled_Change_To_ReadyDelivery);
                }
                if (existedOrder.SystemStatus.ToUpper().Equals(OrderEnum.SystemStatus.READY_DELIVERY.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsReadyDelivery_Change_To_ReadyDelivery);
                }

                #region orders
                existedOrder.SystemStatus = OrderEnum.SystemStatus.READY_DELIVERY.ToString();
                this._unitOfWork.OrderRepository.UpdateOrder(existedOrder);

                OrderHistory orderHistory = new OrderHistory()
                {
                    CreatedDate = DateTime.Now,
                    PartnerOrderStatus = OrderEnum.Status.READY.ToString(),
                    SystemStatus = OrderEnum.SystemStatus.READY_DELIVERY.ToString(),
                    Order = existedOrder,
                };
                await this._unitOfWork.OrderHistoryRepository.InsertOrderHistoryAsync(orderHistory);
                await this._unitOfWork.CommitAsync();
                #endregion
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                switch (ex.Message)
                {
                    case MessageConstant.OrderMessage.OrderIsPreparing_Change_To_ReadyDelivery:
                    case MessageConstant.OrderMessage.OrderIsUpcoming_Change_To_ReadyDelivery:
                    case MessageConstant.OrderMessage.OrderIsCompeleted_Change_To_ReadyDelivery:
                    case MessageConstant.OrderMessage.OrderIsCancelled_Change_To_ReadyDelivery:
                    case MessageConstant.OrderMessage.OrderIsReadyDelivery_Change_To_ReadyDelivery:
                        fieldName = "Order";
                        break;
                    case MessageConstant.OrderMessage.OrderIdNotBelongToKitchenCenter:
                        fieldName = "Order id";
                        break;

                    default:
                        fieldName = "Exception";
                        break;
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                switch (ex.Message)
                {
                    case MessageConstant.CommonMessage.NotExistCashierId:
                        fieldName = "Cashier id";
                        break;
                    case MessageConstant.CommonMessage.NotExistKitchenCenterId:
                        fieldName = "Kitchen center id";
                        break;
                    case MessageConstant.OrderMessage.OrderIdNotExist:
                        fieldName = "Order id";
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
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }


        #endregion 

        #region Cancel Order
        public async Task CancelOrderAsync(OrderRequest orderRequest, IEnumerable<Claim> claims)
        {
            try
            {
                // get account id from claims
                Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
                var storeAccount = await this._unitOfWork.StoreAccountRepository.GetStoreAccountAsync(int.Parse(accountId.Value));
                Order existedOrder = await this._unitOfWork.OrderRepository.GetOrderAsync(orderRequest.Id);

                // Check order id exist or not
                if (existedOrder == null)
                {
                    throw new NotFoundException(MessageConstant.OrderMessage.OrderIdNotExist);
                }

                // Check order belong to store or not.
                if (storeAccount != null)
                {
                    if (existedOrder.StoreId != storeAccount.StoreId)
                    {
                        throw new BadRequestException(MessageConstant.OrderMessage.OrderIdNotBelongToStore);
                    }
                }

                // Check partner order status  - partner order status must be UPCOMING or PREPARING
                if (existedOrder.PartnerOrderStatus.ToUpper().Equals(OrderEnum.Status.COMPLETED.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsCompleted_Cancel);
                }
                if (existedOrder.PartnerOrderStatus.ToUpper().Equals(OrderEnum.Status.CANCELLED.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsCancelled_Cancel);
                }
                if (existedOrder.PartnerOrderStatus.ToUpper().Equals(OrderEnum.Status.READY.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsReady_Cancel);
                }

                // Check system status - system status must be IN_STORE
                if (existedOrder.SystemStatus.ToUpper().Equals(OrderEnum.SystemStatus.COMPLETED.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsCompleted_Cancel);
                }
                if (existedOrder.SystemStatus.ToUpper().Equals(OrderEnum.SystemStatus.CANCELLED.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsCancelled_Cancel);
                }
                if (existedOrder.SystemStatus.ToUpper().Equals(OrderEnum.SystemStatus.READY_DELIVERY.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsReadyDelivery_Cancel);
                }

                #region orders
                // assign CANCELLED status to partner order status and system status
                existedOrder.PartnerOrderStatus = OrderEnum.Status.CANCELLED.ToString();
                existedOrder.SystemStatus = OrderEnum.SystemStatus.CANCELLED.ToString();
                this._unitOfWork.OrderRepository.UpdateOrder(existedOrder);

                OrderHistory orderHistory = new OrderHistory()
                {
                    CreatedDate = DateTime.Now,
                    PartnerOrderStatus = OrderEnum.Status.CANCELLED.ToString(),
                    SystemStatus = OrderEnum.SystemStatus.CANCELLED.ToString(),
                    Order = existedOrder,
                };
                await this._unitOfWork.OrderHistoryRepository.InsertOrderHistoryAsync(orderHistory);
                await this._unitOfWork.CommitAsync();
                #endregion
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.OrderMessage.OrderIdNotExist))
                {
                    fieldName = "Order id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                switch (ex.Message)
                {
                    case MessageConstant.OrderMessage.OrderIsReady_Cancel:
                    case MessageConstant.OrderMessage.OrderIsCompleted_Cancel:
                    case MessageConstant.OrderMessage.OrderIsCancelled_Cancel:
                    case MessageConstant.OrderMessage.OrderIsReadyDelivery_Cancel:
                        fieldName = "Order";
                        break;
                    case MessageConstant.OrderMessage.OrderIdNotBelongToStore:
                        fieldName = "Order id";
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
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
        #endregion
    }
}

