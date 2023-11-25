using MBKC.Repository.Models;
using MBKC.Service.DTOs.Orders;
using MBKC.Service.DTOs.Orders.MBKC.Service.DTOs.Orders;
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
        public Task<GetOrderResponse> CreateOrderAsync(PostOrderRequest postOrderRequest);
        public Task<GetOrderResponse> UpdateOrderAsync(PutOrderIdRequest putOrderIdRequest, PutOrderRequest putOrderRequest);
        public Task<GetOrdersResponse> GetOrdersAsync(GetOrdersRequest getOrdersRequest, IEnumerable<Claim> claims);
        public Task<GetOrderResponse> GetOrderAsync(OrderRequest getOrderRequest, IEnumerable<Claim> claims);
        public Task ChangeOrderStatusToReadyAsync(OrderRequest orderRequest, IEnumerable<Claim> claims);
        public Task ChangeOrderStatusToReadyDeliveryAsync(OrderRequest orderRequest, IEnumerable<Claim> claims);
        public Task CancelOrderAsync(OrderRequest orderRequest, PutCancelOrderRequest cancelOrderRequest, IEnumerable<Claim> claims);
    }
}
