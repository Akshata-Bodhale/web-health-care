namespace CareProjct.web.Models
{
    public class OrderConfirm
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string UserId { get; set; }

        // ── OLD FIELDS (kept to avoid breaking existing views) ──
        public string OrderStatus { get; set; } = "Confirmed";
        public string ProductDetails { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string Method { get; set; }
        public decimal Cost { get; set; }
        public string CardHolderName { get; set; }
        public string CardLastFourDigits { get; set; }

        // ── PATIENT DETAILS (new) ──
        public string PatientName { get; set; }
        public int PatientAge { get; set; }
        // Newborn Care / Post-Surgery Recovery /
        // Elderly Bedridden / Dementia Care /
        // General Elderly Assistance
        public string PatientCondition { get; set; }
        public string PatientNotes { get; set; }

        // ── SERVICE DETAILS (new) ──
        public string ServiceAddress { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfDays { get; set; }
        // Day (8am-8pm) / Night (8pm-8am) / Full 24 Hours
        public string ShiftType { get; set; }

        // ── BOOKING STATUS (new) ──
        // Requested / Accepted / Rejected /
        // ServiceStarted / Completed / Disputed / Cancelled
        public string BookingStatus { get; set; } = "Requested";
        public string NurseRejectionReason { get; set; }

        // ── PAYMENT ──
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; } = "Pending";
        public DateTime? PaymentDate { get; set; }

        // ── RENEWAL TRACKING (new) ──
        public bool IsRenewal { get; set; } = false;
        public int? PreviousBookingId { get; set; }
        public int RenewalCount { get; set; } = 0;
        public bool RenewalReminderSent { get; set; } = false;

        // ── AGREEMENT ACCEPTANCE (new) ──
        public bool AcceptedBookingTerms { get; set; } = false;
        public DateTime? TermsAcceptedOn { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        // ── CUSTOMER INFO ──
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactPhone { get; set; }
    }
}