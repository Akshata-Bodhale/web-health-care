using System.ComponentModel.DataAnnotations.Schema;

namespace CareProjct.web.Models
{
    public class OrderItems
    {
        public int Id { get; set; }  // Primary Key

        [ForeignKey("Orders1")] // Explicitly defining foreign key
        public int OrderId { get; set; }  // Foreign Key

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        // Navigation property
        public Orders1 Orders1 { get; set; }

    }
}
