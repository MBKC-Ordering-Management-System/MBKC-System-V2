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
using MBKC.Repository.Constants;

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

                // Check order belong today or not
                if (existedOrder.OrderHistories.Any(x => x.CreatedDate.Date < DateTime.Now.Date))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.NoChangeOrderStatusNotToday);
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
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsInStore);
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
                existedOrder.PartnerOrderStatus = OrderEnum.Status.COMPLETED.ToString().ToUpper();
                existedOrder.SystemStatus = OrderEnum.SystemStatus.COMPLETED.ToString().ToUpper();
                existedOrder.ConfirmedBy = existedCashier.AccountId;
                this._unitOfWork.OrderRepository.UpdateOrder(existedOrder);

                OrderHistory orderHistory = new OrderHistory()
                {
                    Image = urlImage,
                    CreatedDate = DateTime.Now,
                    SystemStatus = OrderEnum.SystemStatus.COMPLETED.ToString().ToUpper(),
                    PartnerOrderStatus = OrderEnum.Status.COMPLETED.ToString().ToUpper(),
                    Order = existedOrder,
                };
                await this._unitOfWork.OrderHistoryRepository.InsertOrderHistoryAsync(orderHistory);
                #endregion

                #region shipper payment and transaction and wallet (Cash only)
                if (existedOrder.PaymentMethod.ToUpper().Equals(OrderEnum.PaymentMethod.CASH.ToString()))
                {
                    decimal finalPrice = 0;
                    if (existedOrder.Partner.Name.ToLower().Equals(PartnerConstant.GrabFood.ToLower()))
                    {
                        decimal discountedPrice = existedOrder.SubTotalPrice - existedOrder.TotalStoreDiscount;
                        decimal commissionPartnerPrice = discountedPrice * (decimal.Parse(existedOrder.StorePartnerCommission.ToString()) / 100);
                        finalPrice = Math.Round(discountedPrice - commissionPartnerPrice - commissionPartnerPrice * (decimal.Parse(existedOrder.TaxPartnerCommission.ToString()) / 100));
                    }
                    //decimal finalToTalPriceSubstractDeliveryFee = existedOrder.FinalTotalPrice - existedOrder.DeliveryFee;
                    ShipperPayment shipperPayment = new ShipperPayment()
                    {
                        Status = (int)ShipperPaymentEnum.Status.SUCCESS,
                        Content = $"Payment for the order From {existedOrder.Partner.Name}[orderId:{existedOrder.Id}] with {existedOrder.StorePartnerCommission}% commission and {existedOrder.TaxPartnerCommission}% tax of commission {StringUtil.GetContentAmountAndTime(finalPrice)}",
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
                    case MessageConstant.OrderMessage.OrderIsInStore:
                    case MessageConstant.OrderMessage.NoChangeOrderStatusNotToday:
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
        public async Task<GetOrderResponse> CreateOrderAsync(PostOrderRequest postOrderRequest)
        {
            try
            {
                List<Configuration> configurations = await this._unitOfWork.ConfigurationRepository.GetConfigurationsAsync();
                Configuration configuration = configurations.First();
                if(DateTime.Now.TimeOfDay > configuration.ScrawlingOrderEndTime || DateTime.Now.TimeOfDay < configuration.ScrawlingOrderStartTime)
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.CannotCreateOrder);
                }
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
                    SystemStatus = OrderEnum.SystemStatus.IN_STORE.ToString().ToUpper(),
                    PartnerOrderStatus = postOrderRequest.PartnerOrderStatus.ToUpper()
                };

                Order newOrder = new Order()
                {
                    OrderPartnerId = postOrderRequest.OrderPartnerId,
                    ShipperName = postOrderRequest.ShipperName,
                    ShipperPhone = postOrderRequest.ShipperPhone,
                    CustomerName = postOrderRequest.CustomerName,
                    CustomerPhone = postOrderRequest.CustomerPhone,
                    Address = postOrderRequest.Address,
                    Cutlery = postOrderRequest.Cutlery,
                    DeliveryFee = decimal.Parse(postOrderRequest.DeliveryFee.ToString().Replace(".", ",")),
                    DisplayId = postOrderRequest.DisplayId,
                    FinalTotalPrice = decimal.Parse(postOrderRequest.FinalTotalPrice.ToString().Replace(".", ",")),
                    Note = postOrderRequest.Note,
                    PartnerId = postOrderRequest.PartnerId,
                    Partner = existedPartner,
                    StoreId = postOrderRequest.StoreId,
                    PaymentMethod = postOrderRequest.PaymentMethod,
                    PartnerOrderStatus = postOrderRequest.PartnerOrderStatus.ToUpper(),
                    SystemStatus = OrderEnum.SystemStatus.IN_STORE.ToString().ToUpper(),
                    SubTotalPrice = decimal.Parse(postOrderRequest.SubTotalPrice.ToString().Replace(".", ",")),
                    TotalStoreDiscount = decimal.Parse(postOrderRequest.TotalStoreDiscount.ToString().Replace(".", ",")),
                    PromotionPrice = decimal.Parse(postOrderRequest.PromotionPrice.ToString().Replace(".", ",")),
                    TaxPartnerCommission = postOrderRequest.TaxPartnerCommission,
                    Store = existedStore,
                    Tax = postOrderRequest.Tax,
                    OrderHistories = new List<OrderHistory>() { orderHistory },
                    StorePartnerCommission = postOrderRequest.StorePartnerCommission
                };
                List<OrderDetail> newOrderDetails = new List<OrderDetail>();
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
                    if (existedProduct.PartnerProducts.FirstOrDefault(x => x.StoreId == existedStore.StoreId && x.PartnerId == existedPartner.PartnerId && x.CreatedDate == activeStorePartner.CreatedDate && x.ProductId == existedProduct.ProductId).Status != (int)PartnerProductEnum.Status.AVAILABLE)
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
                        DiscountPrice = orderDetail.DiscountPrice,
                        Note = orderDetail.Note,
                        MasterOrderDetailId = null,
                        Product = existedProduct,
                        Quantity = orderDetail.Quantity,
                        
                    };
                    newOrderDetails.Add(newOrderDetail);
                    if (orderDetail.ExtraOrderDetails is not null && orderDetail.ExtraOrderDetails.Count() > 0)
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
                            if (existedProductExtra.PartnerProducts.FirstOrDefault(x => x.StoreId == existedStore.StoreId && x.PartnerId == existedPartner.PartnerId && x.CreatedDate == activeStorePartner.CreatedDate && x.ProductId == existedProductExtra.ProductId).Status != (int)PartnerProductEnum.Status.AVAILABLE)
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
                                DiscountPrice = extraOrderDetail.DiscountPrice,
                                MasterOrderDetail = newOrderDetail,
                                Product = existedProductExtra,
                                Quantity = extraOrderDetail.Quantity
                            };
                            newOrderDetails.Add(newOrderDetailExtra);
                        }
                    }
                }
                newOrder.OrderDetails = newOrderDetails;
                await this._unitOfWork.OrderRepository.InsertOrderAsync(newOrder);
                await this._unitOfWork.CommitAsync();
                return this._mapper.Map<GetOrderResponse>(newOrder);
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
        public async Task<GetOrderResponse> UpdateOrderAsync(PutOrderIdRequest putOrderIdRequest, PutOrderRequest putOrderRequest)
        {
            try
            {
                List<Configuration> configurations = await this._unitOfWork.ConfigurationRepository.GetConfigurationsAsync();
                Configuration configuration = configurations.First();
                if (DateTime.Now.TimeOfDay > configuration.ScrawlingOrderEndTime || DateTime.Now.TimeOfDay < configuration.ScrawlingOrderStartTime)
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.CannotUpdateOrderOutRange);
                }
                Order existedOrder = await this._unitOfWork.OrderRepository.GetOrderByOrderPartnerIdAsync(putOrderIdRequest.Id);
                if (existedOrder is null)
                {
                    throw new NotFoundException(MessageConstant.OrderMessage.OrderPartnerIdNotExist);
                }
                if (existedOrder.PartnerOrderStatus.ToLower().Equals(OrderEnum.Status.PREPARING.ToString().ToLower()) &&
                    putOrderRequest.Status.ToLower().Equals(OrderEnum.Status.UPCOMING.ToString().ToLower()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.CannotUpdateOrder);
                }

                if(existedOrder.PartnerOrderStatus.ToLower().Equals(OrderEnum.Status.PREPARING.ToString().ToLower()) &&
                    putOrderRequest.Status.ToLower().Equals(OrderEnum.Status.PREPARING.ToString().ToLower())){
                    throw new BadRequestException(MessageConstant.OrderMessage.CannotUpdateOrderAlreadyPreparing);
                }
                
                if(existedOrder.PartnerOrderStatus.ToLower().Equals(OrderEnum.Status.UPCOMING.ToString().ToLower()) &&
                    putOrderRequest.Status.ToLower().Equals(OrderEnum.Status.UPCOMING.ToString().ToLower())){
                    throw new BadRequestException(MessageConstant.OrderMessage.CannotUpdateOrderAlreadyUpcoming);
                }

                existedOrder.PartnerOrderStatus = putOrderRequest.Status.ToUpper();
                OrderHistory orderHistory = new OrderHistory()
                {
                    CreatedDate = DateTime.Now,
                    PartnerOrderStatus = putOrderRequest.Status.ToUpper(),
                    SystemStatus = OrderEnum.SystemStatus.IN_STORE.ToString().ToUpper(),
                };
                List<OrderHistory> orderHistories = existedOrder.OrderHistories.ToList();
                orderHistories.Add(orderHistory);
                existedOrder.OrderHistories = orderHistories;
                this._unitOfWork.OrderRepository.UpdateOrder(existedOrder);
                await this._unitOfWork.CommitAsync();
                return this._mapper.Map<GetOrderResponse>(existedOrder);
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Partner order id", ex.Message);
                throw new NotFoundException(error);
            }
             catch(BadRequestException ex)
            {
                string error = ErrorUtil.GetErrorString("Status", ex.Message);
                throw new BadRequestException(error);
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
            {                // Get email, role, account id from claims
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
                List<Order>? orders = null;
                if (getOrdersRequest.SearchValue != null && StringUtil.IsUnicode(getOrdersRequest.SearchValue))
                {
                    numberItems = await this._unitOfWork.OrderRepository.GetNumberOrdersAsync(getOrdersRequest.SearchValue, null, storeAccount == null ? null : storeAccount.StoreId, kitchenCenter == null ? null : kitchenCenter.KitchenCenterId, getOrdersRequest.SystemStatus, getOrdersRequest.PartnerOrderStatus, getOrdersRequest.SearchDateFrom, getOrdersRequest.SearchDateTo, cashier == null ? null : cashier.AccountId, getOrdersRequest.ConfirmedBy);
                    orders = await this._unitOfWork.OrderRepository.GetOrdersAsync(getOrdersRequest.SearchValue, null, getOrdersRequest.CurrentPage, getOrdersRequest.ItemsPerPage,
                                                                                                                  getOrdersRequest.SortBy != null && getOrdersRequest.SortBy.ToLower().EndsWith("asc") ? getOrdersRequest.SortBy.Split("_")[0] : null,
                                                                                                                  getOrdersRequest.SortBy != null && getOrdersRequest.SortBy.ToLower().EndsWith("desc") ? getOrdersRequest.SortBy.Split("_")[0] : null,
                                                                                                                  storeAccount == null ? null : storeAccount.StoreId, kitchenCenter == null ? null : kitchenCenter.KitchenCenterId, getOrdersRequest.SystemStatus,
                                                                                                                  getOrdersRequest.PartnerOrderStatus, getOrdersRequest.SearchDateFrom, getOrdersRequest.SearchDateTo, cashier == null ? null : cashier.AccountId, getOrdersRequest.ConfirmedBy);
                }
                else if (getOrdersRequest.SearchValue != null && StringUtil.IsUnicode(getOrdersRequest.SearchValue) == false)
                {
                    numberItems = await this._unitOfWork.OrderRepository.GetNumberOrdersAsync(null, getOrdersRequest.SearchValue, storeAccount == null ? null : storeAccount.StoreId, kitchenCenter == null ? null : kitchenCenter.KitchenCenterId, getOrdersRequest.SystemStatus, getOrdersRequest.PartnerOrderStatus, getOrdersRequest.SearchDateFrom, getOrdersRequest.SearchDateTo, cashier == null ? null : cashier.AccountId, getOrdersRequest.ConfirmedBy);
                    orders = await this._unitOfWork.OrderRepository.GetOrdersAsync(null, getOrdersRequest.SearchValue, getOrdersRequest.CurrentPage, getOrdersRequest.ItemsPerPage,
                                                                                                                  getOrdersRequest.SortBy != null && getOrdersRequest.SortBy.ToLower().EndsWith("asc") ? getOrdersRequest.SortBy.Split("_")[0] : null,
                                                                                                                  getOrdersRequest.SortBy != null && getOrdersRequest.SortBy.ToLower().EndsWith("desc") ? getOrdersRequest.SortBy.Split("_")[0] : null,
                                                                                                                  storeAccount == null ? null : storeAccount.StoreId, kitchenCenter == null ? null : kitchenCenter.KitchenCenterId, getOrdersRequest.SystemStatus,
                                                                                                                  getOrdersRequest.PartnerOrderStatus, getOrdersRequest.SearchDateFrom, getOrdersRequest.SearchDateTo, cashier == null ? null : cashier.AccountId, getOrdersRequest.ConfirmedBy);
                }
                else if (getOrdersRequest.SearchValue == null)
                {
                    numberItems = await this._unitOfWork.OrderRepository.GetNumberOrdersAsync(null, null, storeAccount == null ? null : storeAccount.StoreId, kitchenCenter == null ? null : kitchenCenter.KitchenCenterId, getOrdersRequest.SystemStatus, getOrdersRequest.PartnerOrderStatus, getOrdersRequest.SearchDateFrom, getOrdersRequest.SearchDateTo, cashier == null ? null : cashier.AccountId, getOrdersRequest.ConfirmedBy);
                    orders = await this._unitOfWork.OrderRepository.GetOrdersAsync(null, null, getOrdersRequest.CurrentPage, getOrdersRequest.ItemsPerPage,
                                                                                                                  getOrdersRequest.SortBy != null && getOrdersRequest.SortBy.ToLower().EndsWith("asc") ? getOrdersRequest.SortBy.Split("_")[0] : null,
                                                                                                                  getOrdersRequest.SortBy != null && getOrdersRequest.SortBy.ToLower().EndsWith("desc") ? getOrdersRequest.SortBy.Split("_")[0] : null,
                                                                                                                  storeAccount == null ? null : storeAccount.StoreId, kitchenCenter == null ? null : kitchenCenter.KitchenCenterId, getOrdersRequest.SystemStatus,
                                                                                                                  getOrdersRequest.PartnerOrderStatus, getOrdersRequest.SearchDateFrom, getOrdersRequest.SearchDateTo, cashier == null ? null : cashier.AccountId, getOrdersRequest.ConfirmedBy);
                }

                int totalPages = 0;
                totalPages = (int)((numberItems + getOrdersRequest.ItemsPerPage) / getOrdersRequest.ItemsPerPage);

                if (numberItems == 0)
                {
                    totalPages = 0;
                }
                List<GetOrderResponse> getOrdersResponse = new List<GetOrderResponse>();
                if (numberItems > 0)
                {
                    // Get totalQuantity of each order
                    foreach (var order in orders)
                    {
                        decimal collectedPrice = 0;
                        if (order.Partner.Name.ToLower().Equals(PartnerConstant.GrabFood.ToLower()))
                        {
                            decimal discountedPrice = order.SubTotalPrice - order.TotalStoreDiscount;
                            decimal commissionPartnerPrice = discountedPrice * (decimal.Parse(order.StorePartnerCommission.ToString()) / 100);
                            collectedPrice = Math.Round(discountedPrice - commissionPartnerPrice - commissionPartnerPrice * (decimal.Parse(order.TaxPartnerCommission.ToString()) / 100));
                        }
                        
                        GetOrderResponse getOrderResponse = this._mapper.Map<GetOrderResponse>(order);
                        getOrderResponse.IsPaid = getOrderResponse.PaymentMethod.ToLower().Equals(OrderEnum.PaymentMethod.CASH.ToString().ToLower()) ? false : true;
                        if (getOrderResponse.IsPaid == true)
                        {
                            collectedPrice = 0;
                        }
                        if (getOrderResponse.SystemStatus.ToLower().Equals(OrderEnum.SystemStatus.COMPLETED.ToString().ToLower()) &&
                            getOrderResponse.PartnerOrderStatus.ToLower().Equals(OrderEnum.Status.COMPLETED.ToString().ToLower()))
                        {
                            getOrderResponse.IsPaid = true;
                        }
                        getOrderResponse.CollectedPrice = collectedPrice;
                        List<int> listQuantity = new List<int>();
                        foreach (var orderDetail in getOrderResponse.OrderDetails)
                        {
                            listQuantity.Add(orderDetail.Quantity);
                        }
                        int totalQuantity = listQuantity.Sum();
                        getOrderResponse.TotalQuantity = totalQuantity;
                        getOrdersResponse.Add(getOrderResponse);
                    }
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

                decimal collectedPrice = 0;
                if (existedOrder.Partner.Name.ToLower().Equals(PartnerConstant.GrabFood.ToLower()))
                {
                    decimal discountedPrice = existedOrder.SubTotalPrice - existedOrder.TotalStoreDiscount;
                    decimal commissionPartnerPrice = discountedPrice * (decimal.Parse(existedOrder.StorePartnerCommission.ToString()) / 100);
                    collectedPrice = Math.Round(discountedPrice - commissionPartnerPrice - commissionPartnerPrice * (decimal.Parse(existedOrder.TaxPartnerCommission.ToString()) / 100));
                }

                GetOrderResponse getOrderResponse = this._mapper.Map<GetOrderResponse>(existedOrder);
                getOrderResponse.IsPaid = getOrderResponse.PaymentMethod.ToLower().Equals(OrderEnum.PaymentMethod.CASH.ToString().ToLower()) ? false : true;
                if (getOrderResponse.IsPaid == true)
                {
                    collectedPrice = 0;
                }

                if (getOrderResponse.SystemStatus.ToLower().Equals(OrderEnum.SystemStatus.COMPLETED.ToString().ToLower()) &&
                    getOrderResponse.PartnerOrderStatus.ToLower().Equals(OrderEnum.Status.COMPLETED.ToString().ToLower()))
                {
                    getOrderResponse.IsPaid = true;
                }
                getOrderResponse.CollectedPrice = collectedPrice;
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
                // Check order belong today or not
                if (existedOrder.OrderHistories.Any(x => x.CreatedDate.Date < DateTime.Now.Date))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.NoChangeOrderStatusNotToday);
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
                existedOrder.PartnerOrderStatus = OrderEnum.Status.READY.ToString().ToUpper();
                this._unitOfWork.OrderRepository.UpdateOrder(existedOrder);

                OrderHistory orderHistory = new OrderHistory()
                {
                    CreatedDate = DateTime.Now,
                    PartnerOrderStatus = OrderEnum.Status.READY.ToString().ToUpper(),
                    SystemStatus = OrderEnum.SystemStatus.IN_STORE.ToString().ToUpper(),
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
                    case MessageConstant.OrderMessage.NoChangeOrderStatusNotToday:
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

                // Check order belong today or not
                if (existedOrder.OrderHistories.Any(x => x.CreatedDate.Date < DateTime.Now.Date))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.NoChangeOrderStatusNotToday);
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
                existedOrder.SystemStatus = OrderEnum.SystemStatus.READY_DELIVERY.ToString().ToUpper();
                this._unitOfWork.OrderRepository.UpdateOrder(existedOrder);

                OrderHistory orderHistory = new OrderHistory()
                {
                    CreatedDate = DateTime.Now,
                    PartnerOrderStatus = OrderEnum.Status.READY.ToString().ToUpper(),
                    SystemStatus = OrderEnum.SystemStatus.READY_DELIVERY.ToString().ToUpper(),
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
                    case MessageConstant.OrderMessage.NoChangeOrderStatusNotToday:
                    case MessageConstant.OrderMessage.OrderIsUpcoming_Change_To_ReadyDelivery:
                    case MessageConstant.OrderMessage.OrderIsCompeleted_Change_To_ReadyDelivery:
                    case MessageConstant.OrderMessage.OrderIsCancelled_Change_To_ReadyDelivery:
                    case MessageConstant.OrderMessage.OrderIsReadyDelivery_Change_To_ReadyDelivery:
                    case MessageConstant.OrderMessage.NoChangeOrderStatusWhenClosedShift:
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
        public async Task CancelOrderAsync(OrderRequest orderRequest, PutCancelOrderRequest cancelOrderRequest, IEnumerable<Claim> claims)
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

                // Check order belong today or not
                if (existedOrder.OrderHistories.Any(x => x.CreatedDate.Date < DateTime.Now.Date))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.NoChangeOrderStatusNotToday);
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
                existedOrder.PartnerOrderStatus = OrderEnum.Status.CANCELLED.ToString().ToUpper();
                existedOrder.SystemStatus = OrderEnum.SystemStatus.CANCELLED.ToString().ToUpper();
                existedOrder.RejectedReason = cancelOrderRequest.RejectedReason;
                this._unitOfWork.OrderRepository.UpdateOrder(existedOrder);

                OrderHistory orderHistory = new OrderHistory()
                {
                    CreatedDate = DateTime.Now,
                    PartnerOrderStatus = OrderEnum.Status.CANCELLED.ToString().ToUpper(),
                    SystemStatus = OrderEnum.SystemStatus.CANCELLED.ToString().ToUpper(),
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
                    case MessageConstant.OrderMessage.NoChangeOrderStatusNotToday:
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

