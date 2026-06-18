using System.ComponentModel.DataAnnotations.Schema;

namespace CareProjct.web.Models
{
    public class Caretaker
    {
        public int ID { get; set; }

        // ── Personal Info ──
        public string FullName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } = "";
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string ContactNumber { get; set; }

        // ── Professional Info ──
        public string Qualification { get; set; }
        public string Experience { get; set; }

        // ONLY Eldercare — no BabySitter
        public string Category { get; set; } = "Eldercare";

        public decimal Price { get; set; } = 0m;

        // ── Profile Image ──
        public string ImagePath { get; set; } = "";
        [NotMapped]
        public IFormFile ImageFile { get; set; }

        // ── Verification Documents (file paths) ──
        public string LicenseNumber { get; set; }
        public string LicenseDocumentPath { get; set; } = "";
        [NotMapped]
        public IFormFile LicenseDocumentFile { get; set; }

        public string AadhaarPath { get; set; } = "";
        [NotMapped]
        public IFormFile AadhaarFile { get; set; }

        public string PoliceClearancePath { get; set; } = "";
        [NotMapped]
        public IFormFile PoliceClearanceFile { get; set; }

        // ── Admin Verification ──
        // Pending / UnderReview / Approved / Rejected
        public string VerificationStatus { get; set; } = "Pending";
        public string RejectionReason { get; set; } = "";
        public DateTime? VerifiedOn { get; set; }
        public DateTime RegistrationDate { get; set; }

        // ── Agreements (legal proof) ──
        public bool AgreedToNurseAgreement { get; set; } = false;
        public bool AgreedToNDA { get; set; } = false;
        public bool AgreedToBackgroundCheck { get; set; } = false;
        public DateTime? AgreementSignedOn { get; set; }

        // ── Availability ──
        // Only true after admin approves
        public bool Available { get; set; } = false;

        // ── Bank Details (NOT card number) ──
        public string AccountHolderName { get; set; }
        public string BankAccountNumber { get; set; }
        public string IFSCCode { get; set; }
        public string BankName { get; set; }

        // ── Booking Duration Rules ──
        public int MaxRenewalsAllowed { get; set; } = 2;
        public int CurrentRenewalCount { get; set; } = 0;
        public decimal TotalEarned { get; set; } = 0m;
    }
}