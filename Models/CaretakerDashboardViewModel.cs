namespace CareProjct.web.Models
{
    public class CaretakerDashboardViewModel
    {
        public Caretaker Profile { get; set; }
        public List<OrderConfirm> NewRequests { get; set; }
        public List<OrderConfirm> ActiveBookings { get; set; }
        public List<OrderConfirm> CompletedBookings { get; set; }
        public decimal TotalEarned { get; set; }
        public int TotalPatientsServed { get; set; }
    }
}