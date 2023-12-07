using MBKC.Repository.Models;
using MBKC.Service.DTOs.OrderDetails;
using MBKC.Service.DTOs.OrdersHistories;
using MBKC.Service.DTOs.Partners;
using MBKC.Service.DTOs.ShipperPayments;
using MBKC.Service.DTOs.Stores;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Orders
{
    public class GetOrderResponse
    {
        public int Id { get; set; }
        public string OrderPartnerId { get; set; }
        public string ShipperName { get; set; }
        public string ShipperPhone { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string Note { get; set; }
        public string PaymentMethod { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal SubTotalPrice { get; set; }
        public decimal TotalStoreDiscount { get; set; }
        public decimal FinalTotalPrice { get; set; }
        public decimal PromotionPrice { get; set; }
        public float TaxPartnerCommission { get; set; }
        public decimal? CollectedPrice { get; set; }
        public bool? IsPaid { get; set; }
        public float Tax { get; set; }
        public string SystemStatus { get; set; }
        public string DisplayId { get; set; }
        public string Address { get; set; }
        public int? Cutlery { get; set; }
        public string PartnerOrderStatus { get; set; }
        public int TotalQuantity { get; set; }
        public int? ConfirmedBy { get; set; }
        public string? RejectedReason { get; set; }
        public float StorePartnerCommission { get; set; }
        public GetStoreResponse Store { get; set; }
        public GetPartnerResponse Partner { get; set; }
        public List<GetShipperPaymentOrderResponse> ShipperPayments { get; set; }
        public List<GetOrderDetailResponse> OrderDetails { get; set; }
        public List<OrderHistoryResponse> OrderHistories { get; set; }
    }
}
