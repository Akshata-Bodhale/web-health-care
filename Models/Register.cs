namespace CareProjct.web.Models
{
    public class Register
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        // "Customer" / "Caretaker" / "Admin"
        public string Type { get; set; }

        // Agreement acceptance for customers
        public bool AgreedToTerms { get; set; } = false;
        public bool AgreedToPrivacy { get; set; } = false;
        public DateTime? AgreementAcceptedOn { get; set; }
    }
}