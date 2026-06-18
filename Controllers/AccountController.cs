using CareProjct.web.Data;
using CareProjct.web.Models;
using CareProjct.web.Filter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CareProjct.web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly Applicationdbcontext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly OrderReceiptService _receiptService;

        public AccountController(
            ILogger<AccountController> logger,
            Applicationdbcontext ct,
            IWebHostEnvironment hostEnvironment,
            OrderReceiptService order)
        {
            _logger          = logger;
            _context         = ct;
            _hostEnvironment = hostEnvironment;
            _receiptService  = order;
        }

        // ────────────────────────────────────
        //  INDEX
        // ────────────────────────────────────
        public IActionResult Index()
        {
            return View();
        }

       


        

        // ────────────────────────────────────
        //  BOOK NURSE (from CaretakerProfile)
        // ────────────────────────────────────
        [HttpPost]
        public IActionResult BookNurse(
            OrderConfirm model,
            int CaretakerId,
            decimal PricePerDay)
        {
            var userId = HttpContext.Session.GetString("userId");
            if (userId == null)
                return RedirectToAction("Login", "Home");

            // Pull actual customer name from Register table
            if (int.TryParse(userId, out int userIdInt))
            {
                var customer = _context.Register.FirstOrDefault(u => u.ID == userIdInt);
                if (customer != null)
                    model.FullName = customer.FirstName + " " + customer.LastName;
            }
            model.BookingStatus   = "Requested";
            model.PaymentStatus   = "Pending";
            model.OrderStatus     = "Confirmed";
            model.TermsAcceptedOn = DateTime.Now;
            var nurse = _context.Caretaker.FirstOrDefault(c => c.ID == CaretakerId);
            model.ProductDetails  = CaretakerId.ToString();
            
            decimal grossAmount = PricePerDay * model.NumberOfDays;

            if (model.NumberOfDays >= 28)
            {
                model.DiscountPercent = 25;   // monthly discount
            }
            else if (model.NumberOfDays == 7)
            {
                model.DiscountPercent = 10;   // weekly discount
            }
            else
            {
                model.DiscountPercent = 0;
            }

            model.TotalAmount = grossAmount - (grossAmount * model.DiscountPercent / 100);
            model.Address  = model.ServiceAddress ?? "N/A";
            model.City     = "N/A";
            model.ZipCode  = "N/A";
            model.Country  = "India";
            model.Method   = model.PaymentMethod ?? "Cash";
            model.CardHolderName        = string.IsNullOrEmpty(model.CardHolderName)
                                          ? (model.FullName ?? "N/A")
                                          : model.CardHolderName;
            model.CardLastFourDigits    = "N/A";
            model.PlatformFee           = 50m;
            model.NursePayableAmount    = model.TotalAmount - 50m;
            model.CustomerPaymentStatus = "Unpaid";
            model.NursePaymentStatus    = "Unpaid";

            _context.OrderConfirm.Add(model);
            _context.SaveChanges();

            TempData["SuccessMessage"] =
                "Booking request sent! " +
                "The nurse will confirm within 24 hours.";
            return RedirectToAction("MyBookings");
        }

        [HttpPost]
public IActionResult MarkAsPaid(int bookingId)
{
    var booking = _context.OrderConfirm.FirstOrDefault(o => o.Id == bookingId);
    if (booking == null) return NotFound();

    booking.PaymentStatus = "Paid";
    booking.PaymentDate   = DateTime.Now;

    // Add earnings to nurse
    int caretakerId = int.Parse(booking.ProductDetails);
    var nurse = _context.Caretaker.FirstOrDefault(c => c.ID == caretakerId);
    if (nurse != null)
        nurse.TotalEarned += booking.TotalAmount;

    _context.SaveChanges();

    TempData["SuccessMessage"] = "Payment of ₹" + booking.TotalAmount + " completed!";
    return RedirectToAction("MyBookings");
}


// ────────────────────────────────────
        //  RENEW BOOKING
        // ────────────────────────────────────
        [UserAuthenication]
        public IActionResult RenewBooking(int bookingId)
        {
            var original = _context.OrderConfirm.FirstOrDefault(b => b.Id == bookingId);
            if (original == null) return NotFound();

            int caretakerId = int.Parse(original.ProductDetails);
            var nurse = _context.Caretaker.FirstOrDefault(c => c.ID == caretakerId);
            if (nurse == null) return NotFound();

            // Check renewal limit
            if (nurse.CurrentRenewalCount >= nurse.MaxRenewalsAllowed)
            {
                TempData["ErrorMessage"] = $"This nurse has reached the maximum renewal limit of {nurse.MaxRenewalsAllowed}.";
                return RedirectToAction("MyBookings");
            }

            // Pre-fill form with previous booking data, new dates start from last end date
            var model = new OrderConfirm
            {
                ProductDetails       = original.ProductDetails,
                PatientName          = original.PatientName,
                PatientAge           = original.PatientAge,
                PatientCondition     = original.PatientCondition,
                PatientNotes         = original.PatientNotes,
                ServiceAddress       = original.ServiceAddress,
                ShiftType            = original.ShiftType,
                StartDate            = original.EndDate,           // continues from last end
                EndDate              = original.EndDate.AddDays(original.NumberOfDays),
                NumberOfDays         = original.NumberOfDays,
                FullName             = original.FullName,
                PhoneNumber          = original.PhoneNumber,
                EmergencyContactName = original.EmergencyContactName,
                EmergencyContactPhone= original.EmergencyContactPhone,
                IsRenewal            = true,
                PreviousBookingId    = original.Id,
                RenewalCount         = original.RenewalCount + 1
            };

            ViewBag.Nurse             = nurse;
            ViewBag.PricePerDay       = nurse.Price;
            ViewBag.OriginalBookingId = bookingId;
            ViewBag.RenewalsLeft      = nurse.MaxRenewalsAllowed - nurse.CurrentRenewalCount;

            return View(model);
        }

        [HttpPost]
        [UserAuthenication]
        public IActionResult RenewBooking(OrderConfirm model, int CaretakerId, decimal PricePerDay)
        {
            var userId = HttpContext.Session.GetString("userId");
            if (userId == null)
                return RedirectToAction("Login", "Home");

            var nurse = _context.Caretaker.FirstOrDefault(c => c.ID == CaretakerId);
            if (nurse == null) return NotFound();

            // Check renewal limit again (safety check)
            if (nurse.CurrentRenewalCount >= nurse.MaxRenewalsAllowed)
            {
                TempData["ErrorMessage"] = "Renewal limit reached for this nurse.";
                return RedirectToAction("MyBookings");
            }

            model.UserId          = userId;
            model.OrderDate       = DateTime.Now;
            model.BookingStatus   = "Requested";
            model.PaymentStatus   = "Pending";
            model.OrderStatus     = "Confirmed";
            model.TermsAcceptedOn = DateTime.Now;
            model.ProductDetails  = CaretakerId.ToString();
            model.IsRenewal       = true;

            decimal grossAmount = PricePerDay * model.NumberOfDays;

            if (model.NumberOfDays >= 28)
                model.DiscountPercent = 25;
            else if (model.NumberOfDays == 7)
                model.DiscountPercent = 10;
            else
                model.DiscountPercent = 0;

            model.TotalAmount        = grossAmount - (grossAmount * model.DiscountPercent / 100);
            model.OriginalAmount     = grossAmount;
            model.Address            = model.ServiceAddress ?? "N/A";
            model.City               = "N/A";
            model.ZipCode            = "N/A";
            model.Country            = "India";
            model.Method             = model.PaymentMethod ?? "Cash";
            // Use submitted CardHolderName if provided, else fallback to FullName
            model.CardHolderName     = string.IsNullOrEmpty(model.CardHolderName) 
                                       ? (model.FullName ?? "N/A") 
                                       : model.CardHolderName;
            model.CardLastFourDigits = "N/A";

            // Increment nurse renewal count
            nurse.CurrentRenewalCount += 1;

            _context.OrderConfirm.Add(model);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Renewal booking request sent! The nurse will confirm within 24 hours.";
            return RedirectToAction("MyBookings");
        }
        // ────────────────────────────────────
        //  MY BOOKINGS (customer tracks)
        // ────────────────────────────────────
        public IActionResult MyBookings()
        {
            var userId = HttpContext.Session.GetString("userId");
            if (userId == null)
                return RedirectToAction("Login", "Home");

            var bookings = _context.OrderConfirm
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.OrderDate)
                .ToList();

            return View(bookings);
        }

       // ── Customer Profile ──
        public IActionResult CustomerProfile()
        {
            var userId = HttpContext.Session.GetString("userId");
            if (userId == null)
                return RedirectToAction("Login", "Home");

            if (!int.TryParse(userId, out int userIdInt))
                return RedirectToAction("Login", "Home");

            var user = _context.Register.FirstOrDefault(u => u.ID == userIdInt);
            if (user == null)
                return RedirectToAction("Login", "Home");

            var bookings = _context.OrderConfirm
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.OrderDate)
                .ToList();

            var model = new CustomerProfileViewModel
            {
                FullName = user.FirstName + " " + user.LastName,
                Email    = user.Email,
                BookingHistory = bookings.Select(b => new BookingHistoryItem
                {
                    BookingId   = b.Id,
                    NurseName   = b.ProductDetails ?? "N/A",
                    ServiceType = "Elder Care",
                    BookingDate = b.OrderDate,
                    Status      = b.BookingStatus ?? b.OrderStatus ?? "Pending",
                    TotalAmount = b.TotalAmount
                }).ToList()
            };

            return View(model);
        }

    }   
}       