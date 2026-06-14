namespace CareProjct.web.Models
{
    public class NurseAgreementViewModel
    {
        // Agreement 1 — Caretaker Service Agreement
        public bool AgreedToNurseAgreement { get; set; }

        // Agreement 2 — Non Disclosure Agreement
        public bool AgreedToNDA { get; set; }

        // Agreement 3 — Background Check Consent
        public bool AgreedToBackgroundCheck { get; set; }

        // Carry caretaker ID forward after agreements signed
        public int CaretakerId { get; set; }
    }
}