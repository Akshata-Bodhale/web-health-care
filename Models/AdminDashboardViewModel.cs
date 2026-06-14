namespace CareProjct.web.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalNurses { get; set; }
        public int PendingVerifications { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalBookings { get; set; }
        public int ActiveBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<RecentBookingItem> RecentBookings { get; set; } = new();
        public List<PendingNurseItem> PendingNurses { get; set; } = new();
    }

    public class RecentBookingItem
    {
        public int BookingId { get; set; }
        public string CustomerName { get; set; }
        public string NurseName { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
    }

    public class PendingNurseItem
    {
        public int NurseId { get; set; }
        public string Name { get; set; }
        public string Specialization { get; set; }
        public DateTime AppliedDate { get; set; }
    }
}