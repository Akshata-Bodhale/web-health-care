namespace CareProjct.web.Models
{
    public class OrderConfirm
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string UserId { get; set; }

        // Order summary
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string OrderStatus { get; set; } = "Confirmed";

        // Product details (stored as JSON or serialized string)
        public string ProductDetails { get; set; }

        // Shipping information
        public string FullName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
       // public string ShippingState { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string Method { get; set; }
        //public string Status { get; set; }
        public decimal Cost { get; set; }

        // Payment information
        public string PaymentMethod { get; set; }
        public string CardHolderName { get; set; }
        public string CardLastFourDigits { get; set; }
        public string PaymentStatus { get; set; } = "Pending";
        public DateTime? PaymentDate { get; set; }
    }
}
