namespace CareProjct.web.Models
{
    public class CartItem
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public string ImagePath { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Gender { get; set; }
        public int Quantity { get; set; }
    }
}
