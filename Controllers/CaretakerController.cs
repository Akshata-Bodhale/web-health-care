using CareProjct.web.Data;
using CareProjct.web.Models;
using CareProjct.web.Filter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareProjct.web.Controllers
{
    public class CaretakerController : Controller
    {
        private readonly ILogger<CaretakerController> _logger;
        private readonly Applicationdbcontext _Context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CaretakerController(ILogger<CaretakerController> logger,
            Applicationdbcontext ct, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _Context = ct;
            _hostEnvironment = hostEnvironment;
        }

        // ── Registration Form (GET) ──
        public IActionResult Caretaker()
        {
            return View();
        }

        // ── Registration Form (POST) ──
        [HttpPost]
        public async Task<IActionResult> Caretaker(Caretaker product)
        {
            try
            {
                // Save profile image
                if (product.ImageFile != null && product.ImageFile.Length > 0)
                    product.ImagePath = await SaveFile(product.ImageFile, "Images");

                // Save nursing license document
                if (product.LicenseDocumentFile != null && product.LicenseDocumentFile.Length > 0)
                    product.LicenseDocumentPath = await SaveFile(product.LicenseDocumentFile, "Documents");

                // Save Aadhaar scan
                if (product.AadhaarFile != null && product.AadhaarFile.Length > 0)
                    product.AadhaarPath = await SaveFile(product.AadhaarFile, "Documents");

                // Save police clearance
                if (product.PoliceClearanceFile != null && product.PoliceClearanceFile.Length > 0)
                    product.PoliceClearancePath = await SaveFile(product.PoliceClearanceFile, "Documents");

                // Force category to Eldercare only
                product.Category = "Eldercare";

                // Status starts as Pending — admin must approve
                product.VerificationStatus = "Pending";
                product.Available = false;

                _Context.Caretaker.Add(product);
                await _Context.SaveChangesAsync();

                TempData["Success"] = "Registration submitted! Admin will review and notify you within 48 hours.";
                return RedirectToAction("RegistrationPending");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
            }
            return View(product);
        }

        // ── Registration Pending Page ──
        public IActionResult RegistrationPending()
        {
            return View();
        }

        // ── Browse Nurses (Customer View) ──
        // Only shows APPROVED nurses
        public IActionResult CaretakerData(string city = null)
        {
            var query = _Context.Caretaker
                .Where(p => p.Available == true
                         && p.VerificationStatus == "Approved"
                         && p.Category == "Eldercare");

            if (!string.IsNullOrEmpty(city))
                query = query.Where(p => p.City == city);

            var data = query.ToList();

            ViewBag.SelectedCity = city;
            ViewBag.Cities = _Context.Caretaker
                .Where(p => p.Available == true && p.VerificationStatus == "Approved")
                .Select(p => p.City)
                .Distinct()
                .ToList();

            return View(data);
        }

        // ── Single Nurse Profile (Customer View) ──
        [UserAuthenication]
        public IActionResult CaretakerProfile(int ID)
        {
            var user = _Context.Caretaker.FirstOrDefault(u => u.ID == ID);
            if (user == null) return NotFound();
            return View(user);
        }

        // ── Nurse Dashboard (Nurse's own view) ──
        [UserAuthenication]
        public IActionResult MyDashboard()
        {
            var email = HttpContext.Session.GetString("Email");
            var nurse = _Context.Caretaker.FirstOrDefault(c => c.Email == email);

            if (nurse == null) return RedirectToAction("Login", "Home");

            var allBookings = _Context.OrderConfirm
                .Where(o => o.UserId == nurse.ID.ToString())
                .ToList();

            var vm = new CaretakerDashboardViewModel
            {
                Profile = nurse,
                NewRequests = allBookings
                    .Where(b => b.BookingStatus == "Requested").ToList(),
                ActiveBookings = allBookings
                    .Where(b => b.BookingStatus == "Accepted"
                             || b.BookingStatus == "ServiceStarted").ToList(),
                CompletedBookings = allBookings
                    .Where(b => b.BookingStatus == "Completed").ToList(),
                TotalEarned = allBookings
                    .Where(b => b.PaymentStatus == "Paid")
                    .Sum(b => b.TotalAmount),
                TotalPatientsServed = allBookings
                    .Count(b => b.BookingStatus == "Completed")
            };

            return View(vm);
        }

        // ── Nurse Accepts Booking ──
        [HttpPost]
        [UserAuthenication]
        public IActionResult AcceptBooking(int bookingId)
        {
            var booking = _Context.OrderConfirm.FirstOrDefault(b => b.Id == bookingId);
            if (booking != null)
            {
                booking.BookingStatus = "Accepted";
                _Context.SaveChanges();
                TempData["Success"] = "Booking accepted successfully!";
            }
            return RedirectToAction("MyDashboard");
        }

        // ── Nurse Rejects Booking ──
        [HttpPost]
        [UserAuthenication]
        public IActionResult RejectBooking(int bookingId, string reason)
        {
            var booking = _Context.OrderConfirm.FirstOrDefault(b => b.Id == bookingId);
            if (booking != null)
            {
                booking.BookingStatus = "Rejected";
                booking.NurseRejectionReason = reason;
                _Context.SaveChanges();
                TempData["Info"] = "Booking rejected.";
            }
            return RedirectToAction("MyDashboard");
        }

        // ── Mark Service Started ──
        [HttpPost]
        [UserAuthenication]
        public IActionResult StartService(int bookingId)
        {
            var booking = _Context.OrderConfirm.FirstOrDefault(b => b.Id == bookingId);
            if (booking != null)
            {
                booking.BookingStatus = "ServiceStarted";
                _Context.SaveChanges();
            }
            return RedirectToAction("MyDashboard");
        }

        // ── Mark Service Completed ──
        [HttpPost]
        [UserAuthenication]
        public IActionResult CompleteService(int bookingId)
        {
            var booking = _Context.OrderConfirm.FirstOrDefault(b => b.Id == bookingId);
            if (booking != null)
            {
                booking.BookingStatus = "Completed";
                _Context.SaveChanges();
                TempData["Success"] = "Service marked as completed!";
            }
            return RedirectToAction("MyDashboard");
        }

        // ── Helper: Save uploaded file ──
        private async Task<string> SaveFile(IFormFile file, string folder)
        {
            string uniqueFileName = Guid.NewGuid().ToString()
                + "_" + Path.GetFileName(file.FileName);
            string uploadsFolder = Path.Combine(
                _hostEnvironment.WebRootPath, folder);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(fileStream);

            return "/" + folder.ToLower() + "/" + uniqueFileName;
        }
    }
}