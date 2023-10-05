using AutoMapper;
using MBKC.Service.Services.Interfaces;
using MBKC.Repository.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBKC.Service.DTOs.Cashiers;
using MBKC.API.Validators.Cashiers;
using System.Security.Claims;
using MBKC.Service.Utils;
using MBKC.Repository.Models;

namespace MBKC.Service.Services.Implementations
{
    public class CashierService : ICashierService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public CashierService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        /*public async Task<GetCashiersResponse> GetCashiersAsync(GetCashiersRequest getCashiersRequest, IEnumerable<Claim> claims)
        {
            try
            {
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                string email = registeredEmailClaim.Value;

                KitchenCenter existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(email);

                int numberItems = 0;
                List<Cashier> cashiers = null;
                if(getCashiersRequest.SearchValue is not null && StringUtil.IsUnicode(getCashiersRequest.SearchValue))
                {

                } else if(getCashiersRequest.SearchValue is not null && StringUtil.IsUnicode(getCashiersRequest.SearchValue))
                {

                } else if(getCashiersRequest.SearchValue is null)
                {

                }
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }*/
    }
}
