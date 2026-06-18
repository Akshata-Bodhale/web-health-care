namespace CareProjct.web.Models
{
    public class OrderConfirm
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string? UserId { get; set; }

        // ── OLD FIELDS ──
        public string? OrderStatus { get; set; } = "Confirmed";
        public string? ProductDetails { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        public string? Method { get; set; }
        public decimal Cost { get; set; }
        public string? CardHolderName { get; set; }
        public string? CardLastFourDigits { get; set; }

        // ── PATIENT DETAILS ──
        public string? PatientName { get; set; }
        public int PatientAge { get; set; }
        public string? PatientCondition { get; set; }
        public string? PatientNotes { get; set; }

        // ── SERVICE DETAILS ──
        public string? ServiceAddress { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfDays { get; set; }
        public string? ShiftType { get; set; }

        // ── BOOKING STATUS ──
        public string? BookingStatus { get; set; } = "Requested";
        public string? NurseRejectionReason { get; set; }

        // ── PAYMENT ──
        public decimal TotalAmount { get; set; }
        public decimal DiscountPercent { get; set; } 
        public decimal OriginalAmount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; } = "Pending";
        public DateTime? PaymentDate { get; set; }
        

        // ── RENEWAL TRACKING ──
        public bool IsRenewal { get; set; } = false;
        public int? PreviousBookingId { get; set; }
        public int RenewalCount { get; set; } = 0;
        public bool RenewalReminderSent { get; set; } = false;

        // ── AGREEMENT ──
        public bool AcceptedBookingTerms { get; set; } = false;
        public DateTime? TermsAcceptedOn { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        // ── CUSTOMER INFO ──
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }


        // ── ADMIN PAYMENT TRACKING ──
        public string? CustomerPaymentStatus { get; set; } = "Unpaid"; // Unpaid / PaidToAdmin
        public DateTime? CustomerPaidToAdminOn { get; set; }
        public string? NursePaymentStatus { get; set; } = "Unpaid";    // Unpaid / PaidToNurse
        public DateTime? NursePaidOn { get; set; }
        public decimal PlatformFee { get; set; } = 50m;
        public decimal NursePayableAmount { get; set; } = 0m;
    }
}