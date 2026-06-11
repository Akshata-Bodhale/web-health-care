namespace CareProjct.web.Models
{
    public class CheckoutModel
    {
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; }
        public string PaymentMethod { get; set; }
    }
}
