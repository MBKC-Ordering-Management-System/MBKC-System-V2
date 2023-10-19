using AutoMapper;
using MBKC.Service.Services.Interfaces;
using MBKC.Repository.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBKC.Repository.Enums;
using MBKC.Repository.Models;
using MBKC.Service.Constants;
using MBKC.Service.DTOs.Orders;
using MBKC.Service.Exceptions;
using MBKC.Service.Utils;
using System.Security.Claims;

namespace MBKC.Service.Services.Implementations
{

    public class MoneyExchangeService : IMoneyExchangeService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public MoneyExchangeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        #region money exchange to kitchen center
        public async Task MoneyExchangeToKitchenCenterAsync(IEnumerable<Claim> claims)
        {
            try
            {
                #region validation
                string email = claims.First(x => x.Type == ClaimTypes.Email).Value;
                Cashier existedCashier = await this._unitOfWork.CashierRepository.GetCashierAsync(email);
                #endregion

                #region operation
                MoneyExchange moneyExchangeCashier = new MoneyExchange()
                {
                    Amount = existedCashier.Wallet.Balance,
                    ExchangeType = MoneyExchangeEnum.ExchangeType.SEND.ToString(),
                    Content = "",
                    Status = (int)MoneyExchangeEnum.Status.SUCCESS,
                    SenderId = existedCashier.AccountId,
                    ReceiveId = existedCashier.KitchenCenter.Manager.AccountId,
                };

                await this._unitOfWork.MoneyExchangeRepository.CreateMoneyExchangeAsync(moneyExchangeCashier);

                #endregion
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

