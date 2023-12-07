using MBKC.Service.DTOs.OrderDetails;

namespace MBKC.Service.DTOs.Orders
{
    public class PostOrderRequest
    {
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
        public float Tax { get; set; }
        public string PartnerOrderStatus { get; set; }
        public int PartnerId { get; set; }
        public int StoreId { get; set; }
        public string DisplayId { get; set; }
        public string Address { get; set; }
        public int? Cutlery { get; set; }
        public float StorePartnerCommission { get; set; }
        public List<PostOrderDetailRequest> OrderDetails { get; set; }
    }
}
