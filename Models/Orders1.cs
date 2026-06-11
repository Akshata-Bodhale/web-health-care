namespace CareProjct.web.Models
{
    public class Orders1
    {
        public int Id { get; set; }  // Primary Key
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }

        // Navigation property for related order items
        public List<OrderItems> OrderItems { get; set; }
    }
}
