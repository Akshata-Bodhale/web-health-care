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
            var email = HttpContext.Session.GetString("UserEmail") 
                     ?? HttpContext.Session.GetString("Email");

            if (!string.IsNullOrEmpty(email))
            {
                var existing = _Context.Caretaker
                    .FirstOrDefault(c => c.Email == email);

                if (existing != null)
                {
                    if (existing.VerificationStatus == "Approved")
                        return RedirectToAction("MyDashboard");
                    else
                        return RedirectToAction("RegistrationPending");
                }

                var model = new Caretaker
                {
                    Email = email,
                    FullName = HttpContext.Session.GetString("UserName") ?? ""
                };
                return View(model);
            }

            return View(new Caretaker());
        }

        // ── Registration Form (POST) ──
        [HttpPost]
        public async Task<IActionResult> Caretaker(Caretaker product)
        {
            try
            {
                Console.WriteLine($"\n📝 ========== REGISTRATION START ==========");
                Console.WriteLine($"📝 Name: {product.FullName}");
                Console.WriteLine($"📝 Email: {product.Email}");

                // ── Prevent duplicate profile ──
                var exists = _Context.Caretaker
                    .Any(c => c.Email == product.Email);
                if (exists)
                {
                    Console.WriteLine("❌ Email already exists!");
                    TempData["ErrorMessage"] = "A profile with this email already exists. Please login.";
                    return View(product);
                }

                // ── Save profile image ──
                if (product.ImageFile != null && product.ImageFile.Length > 0)
                {
                    product.ImagePath = await SaveFile(product.ImageFile, "Images");
                    Console.WriteLine($"✅ Image saved: {product.ImagePath}");
                }

                // ── Save nursing license document ──
                if (product.LicenseDocumentFile != null && product.LicenseDocumentFile.Length > 0)
                {
                    product.LicenseDocumentPath = await SaveFile(product.LicenseDocumentFile, "Documents");
                    Console.WriteLine($"✅ License saved: {product.LicenseDocumentPath}");
                }

                // ── Save Aadhaar scan ──
                if (product.AadhaarFile != null && product.AadhaarFile.Length > 0)
                {
                    product.AadhaarPath = await SaveFile(product.AadhaarFile, "Documents");
                    Console.WriteLine($"✅ Aadhaar saved: {product.AadhaarPath}");
                }

                // ── Save police clearance ──
                if (product.PoliceClearanceFile != null && product.PoliceClearanceFile.Length > 0)
                {
                    product.PoliceClearancePath = await SaveFile(product.PoliceClearanceFile, "Documents");
                    Console.WriteLine($"✅ Police clearance saved: {product.PoliceClearancePath}");
                }

                // ✅ CRITICAL: Set these fields BEFORE saving
                product.Category = "Eldercare";
                product.VerificationStatus = "Pending";       // ← For admin to see
                product.RegistrationDate = DateTime.Now;      // ← When registered
                product.Available = false;                     // ← Not live yet

                Console.WriteLine($"💾 Setting fields:");
                Console.WriteLine($"   - Category: {product.Category}");
                Console.WriteLine($"   - VerificationStatus: {product.VerificationStatus}");
                Console.WriteLine($"   - RegistrationDate: {product.RegistrationDate}");
                Console.WriteLine($"   - Available: {product.Available}");

                // ── Save to database ──
                _Context.Caretaker.Add(product);
                await _Context.SaveChangesAsync();

                Console.WriteLine($"✅ Saved to database!");
                Console.WriteLine($"✅ Caretaker ID: {product.ID}");

                // ── Verify it was saved ──
                var saved = _Context.Caretaker.FirstOrDefault(c => c.Email == product.Email);
                if (saved != null)
                {
                    Console.WriteLine($"✅ VERIFIED IN DATABASE:");
                    Console.WriteLine($"   ID: {saved.ID}");
                    Console.WriteLine($"   Status: {saved.VerificationStatus}");
                    Console.WriteLine($"   Date: {saved.RegistrationDate}");
                    Console.WriteLine($"   Available: {saved.Available}");
                }

                // ── Set session ──
                HttpContext.Session.SetString("UserEmail", product.Email);
                HttpContext.Session.SetString("UserName", product.FullName);
                HttpContext.Session.SetString("UserType", "Caretaker");
                HttpContext.Session.SetString("userId", product.ID.ToString());

                TempData["SuccessMessage"] = "✅ Registration submitted! Admin will review within 48 hours.";
                
                Console.WriteLine($"🎉 SUCCESS - Redirecting to RegistrationPending");
                Console.WriteLine($"========== REGISTRATION COMPLETE ==========\n");
                
                return RedirectToAction("RegistrationPending");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ ========== REGISTRATION ERROR ==========");
                Console.WriteLine($"❌ Exception: {ex.Message}");
                Console.WriteLine($"❌ Inner Exception: {ex.InnerException?.Message}");
                Console.WriteLine($"❌ Stack Trace: {ex.StackTrace}");

                foreach (var state in ModelState.Values)
                {
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"⚠️ Validation Error: {error.ErrorMessage}");
                    }
                }
                Console.WriteLine($"========== ERROR COMPLETE ==========\n");

                ModelState.AddModelError("", "Error: " + ex.InnerException?.Message ?? ex.Message);
                return View(product);
            }
        }

        // ── Registration Pending Page ──
        public IActionResult RegistrationPending()
        {
            return View();
        }

        // ── Browse Nurses (Customer View) ──
        public IActionResult CaretakerData(string city = null, string gender = null, decimal? maxPrice = null)
        {
            var query = _Context.Caretaker
                .Where(p => p.VerificationStatus == "Approved");

            if (!string.IsNullOrEmpty(city))
                query = query.Where(p => p.City == city);

            if (!string.IsNullOrEmpty(gender))
                query = query.Where(p => p.Gender == gender);

            if (maxPrice.HasValue && maxPrice > 0)
                query = query.Where(p => p.Price <= maxPrice.Value);

            var data = query.ToList();

            ViewBag.SelectedCity   = city;
            ViewBag.SelectedGender = gender;
            ViewBag.SelectedPrice  = maxPrice;
            ViewBag.Cities = _Context.Caretaker
                .Where(p => p.VerificationStatus == "Approved")
                .Select(p => p.City)
                .Distinct()
                .ToList();

            return View(data);
        }

        // ── Single Nurse Profile ──
        [UserAuthenication]
        public IActionResult CaretakerProfile(int ID)
        {
            var user = _Context.Caretaker.FirstOrDefault(u => u.ID == ID);
            if (user == null) return NotFound();
            return View(user);
        }

        // ── Nurse Dashboard ──
        [UserAuthenication]
        public IActionResult MyDashboard()
        {
            var email = HttpContext.Session.GetString("UserEmail") 
                     ?? HttpContext.Session.GetString("Email");
            var nurse = _Context.Caretaker.FirstOrDefault(c => c.Email == email);

            if (nurse == null) return RedirectToAction("Login", "Home");

            var allBookings = _Context.OrderConfirm
                .Where(o => o.ProductDetails == nurse.ID.ToString())
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

            return "/" + folder + "/" + uniqueFileName;
        }

        [UserAuthenication]
        public IActionResult MyProfile()
        {
            var email = HttpContext.Session.GetString("UserEmail") 
                     ?? HttpContext.Session.GetString("Email");
            var nurse = _Context.Caretaker.FirstOrDefault(c => c.Email == email);
            if (nurse == null) return RedirectToAction("Login", "Home");
            return View("CaretakerProfile", nurse);
        }
    }
}