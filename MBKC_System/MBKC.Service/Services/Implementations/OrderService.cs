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

                if (existedOrder.PartnerOrderStatus.Equals(OrderEnum.Status.PREPARING.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsPreparing);
                }
                if (existedOrder.PartnerOrderStatus.Equals(OrderEnum.Status.UPCOMING.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsUpcoming);
                }
                if (existedOrder.PartnerOrderStatus.Equals(OrderEnum.Status.COMPLETED.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsCompleted);
                }
                if (existedOrder.PartnerOrderStatus.Equals(OrderEnum.Status.CANCELLED.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsCancelled);
                }
                #endregion

                #region operation

                #region orders
                existedOrder.PartnerOrderStatus = OrderEnum.Status.COMPLETED.ToString();
                this._unitOfWork.OrderRepository.UpdateOrder(existedOrder);
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
                    case MessageConstant.OrderMessage.OrderShipperPhoneNotMatch:
                        fieldName = "Shipper phone";
                        break;

                    case MessageConstant.OrderMessage.OrderNotBelongToKitchenCenter:
                    case MessageConstant.OrderMessage.OrderIsPreparing:
                    case MessageConstant.OrderMessage.OrderIsReady:
                    case MessageConstant.OrderMessage.OrderIsCompleted:
                    case MessageConstant.OrderMessage.OrderIsCancelled:
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
                string error = ErrorUtil.GetErrorString("Exception", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                throw new Exception(error);
            }
        }
        #endregion

        public async Task<GetOrderResponse> GetOrderAsync(string orderPartnerId)
        {
            try
            {
                Order existedOrder = await this._unitOfWork.OrderRepository.GetOrderByOrderPartnerIdAsync(orderPartnerId);
                if(existedOrder is null)
                {
                    throw new NotFoundException(MessageConstant.OrderMessage.OrderPartnerIdNotExist);
                }
                GetOrderResponse getOrderResponse = this._mapper.Map<GetOrderResponse>(existedOrder);
                if(getOrderResponse.ShipperPayments is not null && getOrderResponse.ShipperPayments.Count > 0)
                {
                    foreach (var shipperpayment in getOrderResponse.ShipperPayments)
                    {
                        Cashier existedCashier = await this._unitOfWork.CashierRepository.GetCashierAsync(shipperpayment.CreatedBy);
                        shipperpayment.CashierCreated = existedCashier.FullName;
                    }
                }
                return getOrderResponse;
            }
            catch(NotFoundException ex)
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

        public async Task CreateOrderAsync(PostOrderRequest postOrderRequest)
        {
            try
            {
                Order existedOrder = await this._unitOfWork.OrderRepository.GetOrderByOrderPartnerIdAsync(postOrderRequest.OrderPartnerId);
                if(existedOrder is not null)
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderPartnerIdAlreadyExist);
                }

                existedOrder = await this._unitOfWork.OrderRepository.GetOrderByDisplayIdAsync(postOrderRequest.DisplayId);
                if (existedOrder is not null)
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.DisplayIdAlreadyExist);
                }

                Store existedStore = await this._unitOfWork.StoreRepository.GetStoreByIdAsync(postOrderRequest.StoreId);
                if(existedStore is null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistStoreId);
                }
                Partner existedPartner = await this._unitOfWork.PartnerRepository.GetPartnerAsync(postOrderRequest.PartnerId);
                if (existedPartner is null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistPartnerId);
                }
                if(existedStore.StorePartners.Any(x => x.PartnerId == existedPartner.PartnerId && x.Status == (int)StorePartnerEnum.Status.ACTIVE) == false)
                {
                    throw new BadRequestException(MessageConstant.StorePartnerMessage.NotLinkedWithParner);
                }
                StorePartner activeStorePartner = existedStore.StorePartners.FirstOrDefault(x => x.PartnerId == existedPartner.PartnerId && x.Status == (int)StorePartnerEnum.Status.ACTIVE);

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
                    OrderDetails = new List<OrderDetail>()
                };

                foreach (var orderDetail in postOrderRequest.OrderDetails)
                {
                    Product existedProduct = await this._unitOfWork.ProductRepository.GetProductAsync(orderDetail.ProductId);
                    if(existedProduct is null)
                    {
                        throw new NotFoundException(MessageConstant.OrderMessage.ProductInOrderNotExistInTheSystem);
                    }
                    
                    if(existedProduct.PartnerProducts.FirstOrDefault(x => x.StoreId == existedStore.StoreId && x.PartnerId == existedPartner.PartnerId && x.CreatedDate == activeStorePartner.CreatedDate && x.ProductId == existedProduct.ProductId) is null)
                    {
                        throw new NotFoundException(MessageConstant.OrderMessage.ProductPartnerNotMappingBefore);
                    }
                    if(existedProduct.PartnerProducts.FirstOrDefault(x => x.StoreId == existedStore.StoreId && x.PartnerId == existedPartner.PartnerId && x.CreatedDate == activeStorePartner.CreatedDate && x.ProductId == existedProduct.ProductId).Status == (int)GrabFoodItemEnum.AvailableStatus.AVAILABLE)
                    {
                        throw new BadRequestException(MessageConstant.PartnerProductMessage.ProductPartnerNotAvailableNow);
                    }
                    if(existedProduct.PartnerProducts.FirstOrDefault(x => x.StoreId == existedStore.StoreId && x.PartnerId == existedPartner.PartnerId && x.CreatedDate == activeStorePartner.CreatedDate && x.ProductId == existedProduct.ProductId).Price != orderDetail.SellingPrice)
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
                    if(orderDetail.ExtraOrderDetails is not null)
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
            catch(BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.OrderMessage.OrderPartnerIdAlreadyExist))
                {
                    fieldName = "Order partner id";
                } else if (ex.Message.Equals(MessageConstant.OrderMessage.DisplayIdAlreadyExist))
                {
                    fieldName = "Display id";
                } else if (ex.Message.Equals(MessageConstant.StorePartnerMessage.NotLinkedWithParner) ||
                    ex.Message.Equals(MessageConstant.PartnerProductMessage.ProductPartnerNotAvailableNow))
                {
                    fieldName = "Partner product";
                } else if(ex.Message.Equals(MessageConstant.PartnerProductMessage.ProductPriceNotMatchWithPartnerProduct)||
                    ex.Message.Equals(MessageConstant.PartnerProductMessage.ExtraProductPriceNotMatchWithPartnerProduct))
                {
                    fieldName = "Price";
                } else if (ex.Message.Equals(MessageConstant.OrderMessage.ProductInOrderNotExistInTheSystem))
                {
                    fieldName = "Product id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch(NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistStoreId))
                {
                    fieldName = "Store id";
                }
                else if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistPartnerId))
                {
                    fieldName = "Partner id";
                } else if(ex.Message.Equals(MessageConstant.OrderMessage.ProductExtraInOrderDetailNotExistInTheSystem) ||
                    ex.Message.Equals(MessageConstant.OrderMessage.ProductInOrderNotExistInTheSystem))
                {
                    fieldName = "Product id";
                }
                else if (ex.Message.Equals(MessageConstant.OrderMessage.ProductPartnerNotMappingBefore)||
                    ex.Message.Equals(MessageConstant.OrderMessage.ProductExtraPartnerNotMappingBefore))
                {
                    fieldName = "Partner product";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch(Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task UpdateOrderAsync(PutOrderIdRequest putOrderIdRequest, PutOrderRequest putOrderRequest)
        {
            try
            {
                Order existedOrder = await this._unitOfWork.OrderRepository.GetOrderByOrderPartnerIdAsync(putOrderIdRequest.Id);
                if(existedOrder is null)
                {
                    throw new NotFoundException(MessageConstant.OrderMessage.OrderPartnerIdNotExist);
                }
                existedOrder.PartnerOrderStatus = putOrderRequest.Status.ToUpper();
                this._unitOfWork.OrderRepository.UpdateOrder(existedOrder);
                await this._unitOfWork.CommitAsync();
            }
            catch(NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Partner order id", ex.Message);
                throw new NotFoundException(error);
            }
            catch(Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
    }
}
