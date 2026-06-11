using System.ComponentModel.DataAnnotations.Schema;

namespace CareProjct.web.Models
{
    public class Caretaker
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Qualification { get; set; }
        public string Experience { get; set; }
        public string ContactNumber { get; set; }
        
        public string ImagePath { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        
        public DateTime DateOfBirth { get; set; }

        public string CardholderName { get; set; }
        public string CardNumber { get; set; }
        public string ExpirationDate { get; set; }

        public string CVV { get; set; }

        public bool Available { get; set; }

        public string Category { get; set; }
    }
}


