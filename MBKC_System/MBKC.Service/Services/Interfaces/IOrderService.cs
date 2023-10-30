using MBKC.Service.DTOs.Orders;
using MBKC.Service.DTOs.Partners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Services.Interfaces
{
    public interface IOrderService
    {
        public Task ConfirmOrderToCompletedAsync(ConfirmOrderToCompletedRequest confirmOrderToCompleted, IEnumerable<Claim> claims);
        public Task<GetOrderResponse> GetOrderAsync(string orderPartnerId);
    }
}
