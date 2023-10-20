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
using MBKC.Repository.SMTPModels;
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

                if (!existedOrder.ShipperPhone.Equals(confirmOrderToCompleted.ShipperPhone))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderShipperPhoneNotMatch);
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

                if (existedOrder.Status.Equals(OrderEnum.Status.PREPARING.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsPreparing);
                }
                if (existedOrder.Status.Equals(OrderEnum.Status.READY.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsReady);
                }
                if (existedOrder.Status.Equals(OrderEnum.Status.COMPLETED.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsCompleted);
                }
                if (existedOrder.Status.Equals(OrderEnum.Status.CANCELLED.ToString()))
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderIsCancelled);
                }
                #endregion

                #region operation

                #region orders
                existedOrder.Status = OrderEnum.Status.COMPLETED.ToString();
                this._unitOfWork.OrderRepository.UpdateOrder(existedOrder);
                #endregion

                #region shipper payment and transaction and wallet
                if (existedOrder.PaymentMethod.ToUpper().Equals(OrderEnum.PaymentMethod.CASH.ToString()))
                {
                    decimal finalPrice = existedOrder.FinalTotalPrice - (existedOrder.FinalTotalPrice * existedOrder.Commission/100);
                    ShipperPayment shipperPayment = new ShipperPayment()
                    {
                        Status = (int)ShipperPaymentEnum.Status.SUCCESS,
                        Content = $"Payment for the order[orderId:{existedOrder.Id}] with {existedOrder.Commission}% commission {StringUtil.GetContentAmountAndTime(finalPrice, DateTime.Now)}",
                        OrderId = existedOrder.Id,
                        Amount = finalPrice,
                        CreateDate = DateTime.Now,
                        PaymentMethod = confirmOrderToCompleted.BankingAccountId == null
                        ? ShipperPaymentEnum.PaymentMethod.CASH.ToString()
                        : ShipperPaymentEnum.PaymentMethod.CASHLESS.ToString(),
                        KCBankingAccountId = confirmOrderToCompleted.BankingAccountId,
                        CreateBy = existedCashier.AccountId,
                    };
                    await this._unitOfWork.ShipperPaymentRepository.CreateShipperPaymentAsync(shipperPayment);

                    Transaction transaction = new Transaction()
                    {
                        TransactionTime = DateTime.Now,
                        Status = (int)TransactionEnum.Status.SUCCESS,
                        ShipperPayment = shipperPayment,
                        Wallet = existedCashier.Wallet,
                    };
                    await this._unitOfWork.TransactionRepository.CreateTransactionAsync(transaction);

                    existedCashier.Wallet.Balance += finalPrice;
                    this._unitOfWork.WalletRepository.UpdateWallet(existedCashier.Wallet);

                }
                #endregion

                #endregion

                await this._unitOfWork.CommitAsync();
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
    }
}
