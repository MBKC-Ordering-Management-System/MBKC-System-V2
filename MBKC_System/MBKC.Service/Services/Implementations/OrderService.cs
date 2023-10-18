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
                string email = claims.First(x => x.Type == ClaimTypes.Email).Value;
                Cashier existedCashier;
                BankingAccount? existedBankingAccount;
                Order? existedOrder = await this._unitOfWork.OrderRepository.GetOrderByOrderPartnerIdAsync(confirmOrderToCompleted.OrderPartnerId);

                if (existedOrder == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistOrderPartnerId);
                }

                existedCashier = await this._unitOfWork.CashierRepository.GetCashierAsync(email);
                if (confirmOrderToCompleted.BankingAccountId != null)
                {
                    if (confirmOrderToCompleted.BankingAccountId <= 0)
                    {
                        throw new BadRequestException(MessageConstant.CommonMessage.InvalidBankingAccountId);
                    }

                    existedBankingAccount = await this._unitOfWork.BankingAccountRepository.GetBankingAccountAsync(confirmOrderToCompleted.BankingAccountId.Value);
                    if (existedBankingAccount == null)
                    {
                        throw new NotFoundException(MessageConstant.CommonMessage.NotExistBankingAccountId);
                    }

                    if (existedCashier.KitchenCenter.BankingAccounts.Any(ba => ba.BankingAccountId == existedBankingAccount.BankingAccountId) == false)
                    {
                        throw new BadRequestException(MessageConstant.BankingAccountMessage.BankingAccountNotBelongToKitchenCenter);
                    }
                }

                if (existedCashier.KitchenCenter.Stores.Any(s => s.StoreId == existedOrder.StoreId) == false)
                {
                    throw new BadRequestException(MessageConstant.OrderMessage.OrderNotBelongToKitchenCenter);
                }

            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
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
