namespace CareProjct.web.Models
{
    public class CustomerProfileViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public List<BookingHistoryItem> BookingHistory { get; set; } = new();
    }

    public class BookingHistoryItem
    {
        public int BookingId { get; set; }
        public string NurseName { get; set; }
        public string ServiceType { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
    }
}