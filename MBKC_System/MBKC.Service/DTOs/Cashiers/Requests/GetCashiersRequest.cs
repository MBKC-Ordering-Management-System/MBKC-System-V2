namespace MBKC.Service.DTOs.Cashiers.Requests
{
    public class GetCashiersRequest
    {
        public string? SearchValue { get; set; }
        public int? ItemsPerPage { get; set; } = 5;
        public int? CurrentPage { get; set; } = 1;
        public string? SortBy { get; set; }
    }
}
