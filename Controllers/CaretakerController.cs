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

        [HttpPost]
        public async Task<IActionResult> Caretaker(Caretaker product)
        {
            try
            {
                Console.WriteLine($"\n📝 ========== REGISTRATION START ==========");
                Console.WriteLine($"📝 Name: {product.FullName}");
                Console.WriteLine($"📝 Email: {product.Email}");

                var exists = _Context.Caretaker
                    .Any(c => c.Email == product.Email);
                if (exists)
                {
                    Console.WriteLine("❌ Email already exists!");
                    TempData["ErrorMessage"] = "A profile with this email already exists. Please login.";
                    return View(product);
                }

                if (product.ImageFile != null && product.ImageFile.Length > 0)
                {
                    product.ImagePath = await SaveFile(product.ImageFile, "images");
                    Console.WriteLine($"✅ Image saved: {product.ImagePath}");
                }

                if (product.LicenseDocumentFile != null && product.LicenseDocumentFile.Length > 0)
                {
                    product.LicenseDocumentPath = await SaveFile(product.LicenseDocumentFile, "documents");
                    Console.WriteLine($"✅ License saved: {product.LicenseDocumentPath}");
                }

                if (product.AadhaarFile != null && product.AadhaarFile.Length > 0)
                {
                    product.AadhaarPath = await SaveFile(product.AadhaarFile, "documents");
                    Console.WriteLine($"✅ Aadhaar saved: {product.AadhaarPath}");
                }

                if (product.PoliceClearanceFile != null && product.PoliceClearanceFile.Length > 0)
                {
                    product.PoliceClearancePath = await SaveFile(product.PoliceClearanceFile, "documents");
                    Console.WriteLine($"✅ Police clearance saved: {product.PoliceClearancePath}");
                }

                product.Category = "Eldercare";
                product.VerificationStatus = "Pending";
                product.RegistrationDate = DateTime.Now;
                product.Available = false;

                Console.WriteLine($"💾 Setting fields:");
                Console.WriteLine($"   - Category: {product.Category}");
                Console.WriteLine($"   - VerificationStatus: {product.VerificationStatus}");
                Console.WriteLine($"   - RegistrationDate: {product.RegistrationDate}");
                Console.WriteLine($"   - Available: {product.Available}");

                _Context.Caretaker.Add(product);
                await _Context.SaveChangesAsync();

                Console.WriteLine($"✅ Saved to database!");
                Console.WriteLine($"✅ Caretaker ID: {product.ID}");

                var saved = _Context.Caretaker.FirstOrDefault(c => c.Email == product.Email);
                if (saved != null)
                {
                    Console.WriteLine($"✅ VERIFIED IN DATABASE:");
                    Console.WriteLine($"   ID: {saved.ID}");
                    Console.WriteLine($"   Status: {saved.VerificationStatus}");
                    Console.WriteLine($"   Date: {saved.RegistrationDate}");
                    Console.WriteLine($"   Available: {saved.Available}");
                }

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

        public IActionResult RegistrationPending()
        {
            return View();
        }

        public IActionResult CaretakerData(string? city = null)
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

        [UserAuthenication]
        public IActionResult CaretakerProfile(int ID)
        {
            var user = _Context.Caretaker.FirstOrDefault(u => u.ID == ID);
            if (user == null) return NotFound();
            return View(user);
        }

        [UserAuthenication]
        public IActionResult MyDashboard()
        {
            Console.WriteLine("\n📊 ========== NURSE DASHBOARD LOAD ==========");

            var email = HttpContext.Session.GetString("UserEmail") 
                     ?? HttpContext.Session.GetString("Email");
            
            Console.WriteLine($"📊 Nurse Email: {email}");

            var nurse = _Context.Caretaker.FirstOrDefault(c => c.Email == email);

            if (nurse == null)
            {
                Console.WriteLine("❌ Nurse not found!");
                return RedirectToAction("Login", "Home");
            }

            Console.WriteLine($"📊 Nurse ID: {nurse.ID}");

            var allBookings = _Context.OrderConfirm
                .Where(o => o.ProductDetails == nurse.ID.ToString())
                .ToList();

            Console.WriteLine($"📊 Total Bookings: {allBookings.Count}");

            decimal totalEarned = 0;
            foreach (var booking in allBookings.Where(b => b.PaymentStatus == "Paid"))
            {
                totalEarned += booking.TotalAmount;
                Console.WriteLine($"📊 Booking {booking.Id}: ₹{booking.TotalAmount} (Status: {booking.BookingStatus})");
            }

            Console.WriteLine($"📊 Total Earned: ₹{totalEarned}");

            var vm = new CaretakerDashboardViewModel
            {
                Profile = nurse,
                NewRequests = allBookings
                    .Where(b => b.BookingStatus == "Requested").ToList(),
                ActiveBookings = allBookings
                    .Where(b => b.BookingStatus == "Accepted"
                             || b.BookingStatus == "ServiceStarted"
                             || b.BookingStatus == "Confirmed").ToList(),
                CompletedBookings = allBookings
                    .Where(b => b.BookingStatus == "Completed").ToList(),
                TotalEarned = totalEarned,
                TotalPatientsServed = allBookings
                    .Count(b => b.BookingStatus == "Completed")
            };

            Console.WriteLine($"========== DASHBOARD LOADED ==========\n");

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

        [UserAuthenication]
        public IActionResult EditProfile()
        {
            var email = HttpContext.Session.GetString("UserEmail")
                     ?? HttpContext.Session.GetString("Email");
            var nurse = _Context.Caretaker.FirstOrDefault(c => c.Email == email);
            if (nurse == null) return RedirectToAction("Login", "Home");
            return View(nurse);
        }

        [HttpPost]
        [UserAuthenication]
        public async Task<IActionResult> EditProfile(Caretaker model)
        {
            var email = HttpContext.Session.GetString("UserEmail")
                     ?? HttpContext.Session.GetString("Email");
            var nurse = _Context.Caretaker.FirstOrDefault(c => c.Email == email);
            if (nurse == null) return RedirectToAction("Login", "Home");

            // Only update safe fields — no verification fields touched
            nurse.FullName      = model.FullName;
            nurse.ContactNumber = model.ContactNumber;
            nurse.City          = model.City;
            nurse.Address       = model.Address;
            nurse.Price         = model.Price;

            // Bank details
            nurse.AccountHolderName = model.AccountHolderName;
            nurse.BankAccountNumber = model.BankAccountNumber;
            nurse.IFSCCode          = model.IFSCCode;
            nurse.BankName          = model.BankName;

            // Profile photo — only replace if a new one is uploaded
            if (model.ImageFile != null && model.ImageFile.Length > 0)
                nurse.ImagePath = await SaveFile(model.ImageFile, "images");

            _Context.SaveChanges();

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("MyDashboard");
        }
        
        private async Task<string> SaveFile(IFormFile file, string folder)
        {
            // Always use lowercase folder names for consistency
            string folderName = folder.ToLower();  // "images" or "documents"

            string uniqueFileName = Guid.NewGuid().ToString()
                + "_" + Path.GetFileName(file.FileName);

            // Save to wwwroot/images/ or wwwroot/documents/
            string uploadsFolder = Path.Combine(
                _hostEnvironment.WebRootPath, folderName);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(fileStream);

            // Return web-accessible path like /images/filename.jpg
            return "/" + folderName + "/" + uniqueFileName;
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
