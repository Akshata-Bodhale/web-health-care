namespace CareProjct.web.Models
{
    public class PaymentInfo
    {
        public int ID { get; set; }
        public string CardholderName { get; set; }
        public string CardNumber { get; set; }
        public DateOnly ExpirationDate { get; set; }
        public int CVV { get; set; }
        public decimal Amount { get; set; }
        public string ReceiptEmail { get; set; }

        public string BillingZip { get; set; }



    }
}
