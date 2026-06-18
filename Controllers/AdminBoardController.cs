using CareProjct.web.Data;
using CareProjct.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareProjct.web.Controllers
{
    public class AdminBoardController : Controller
    {
        private readonly ILogger<CaretakerController> _logger;
        private readonly Applicationdbcontext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AdminBoardController(ILogger<CaretakerController> logger,
            Applicationdbcontext ct, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _context = ct;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            var viewModel = new ProductViewModel
            {
                Products = _context.Caretaker.ToList(),
                Product = new Caretaker()
            };
            return View(viewModel);
        }

        public IActionResult AdminDashboard()
        {
            var pendingCount = _context.Caretaker.Count(n => n.VerificationStatus == "Pending");

            var model = new AdminDashboardViewModel
            {
                TotalNurses = _context.Caretaker.Count(n => n.VerificationStatus == "Approved"),
                PendingVerifications = pendingCount,
                TotalCustomers = _context.Register.Count(),
                TotalBookings = _context.OrderConfirm.Count(),
                ActiveBookings = _context.OrderConfirm.Count(b => 
                    b.BookingStatus == "Confirmed" ||
                    b.BookingStatus == "Accepted" ||
                    b.BookingStatus == "ServiceStarted"),
                TotalRevenue = _context.OrderConfirm
                    .Where(b => b.PaymentStatus == "Paid")
                    .Sum(b => (decimal?)b.TotalAmount) ?? 0m,
                
                RecentBookings = _context.OrderConfirm
                    .OrderByDescending(b => b.OrderDate)
                    .Take(5)
                    .ToList()
                    .Select(b => {
                        int.TryParse(b.ProductDetails, out int nid);
                        var nurse = _context.Caretaker.FirstOrDefault(c => c.ID == nid);
                        return new RecentBookingItem
                        {
                            BookingId    = b.Id,
                            CustomerName = b.FullName ?? "N/A",
                            NurseName    = nurse != null ? nurse.FullName : (b.ProductDetails ?? "N/A"),
                            BookingDate  = b.OrderDate,
                            Status       = b.BookingStatus ?? b.OrderStatus ?? "Pending",
                            Amount       = b.TotalAmount
                        };
                    }).ToList(),
                PendingNurses = _context.Caretaker
                    .Where(n => n.VerificationStatus == "Pending")
                    .OrderByDescending(n => n.RegistrationDate)
                    .Select(n => new PendingNurseItem
                    {
                        NurseId = n.ID,
                        Name = n.FullName,
                        Specialization = n.Qualification ?? "General",
                        AppliedDate = n.RegistrationDate
                    }).ToList()
            };

            return View(model);
        }

        public IActionResult VerificationQueue()
        {
            var pending = _context.Caretaker
                .Where(c => c.VerificationStatus == "Pending"
                         || c.VerificationStatus == "UnderReview")
                .OrderByDescending(c => c.RegistrationDate)
                .ToList();
            return View(pending);
        }

        [HttpPost]
        public IActionResult ApproveNurse(int id)
        {
            var nurse = _context.Caretaker.FirstOrDefault(c => c.ID == id);
            if (nurse != null)
            {
                nurse.VerificationStatus = "Approved";
                nurse.Available = true;
                nurse.VerifiedOn = DateTime.Now;
                _context.SaveChanges();
                TempData["SuccessMessage"] = nurse.FullName + " has been approved and is now live.";
            }
            return RedirectToAction("VerificationQueue");
        }

        [HttpPost]
        public IActionResult RejectNurse(int id, string reason)
        {
            var nurse = _context.Caretaker.FirstOrDefault(c => c.ID == id);
            if (nurse != null)
            {
                nurse.VerificationStatus = "Rejected";
                nurse.Available = false;
                nurse.RejectionReason = reason;
                _context.SaveChanges();
                TempData["InfoMessage"] = nurse.FullName + " has been rejected.";
            }
            return RedirectToAction("VerificationQueue");
        }

        public IActionResult ProductList()
        {
            var data = _context.Caretaker
                .Where(c => c.VerificationStatus == "Approved")
                .ToList();
            return View(data);
        }

        public IActionResult AllBookings()
        {
            var bookings = _context.OrderConfirm
                .OrderByDescending(b => b.OrderDate)
                .Select(b => new RecentBookingItem
                {
                    BookingId = b.Id,
                    CustomerName = b.FullName ?? "N/A",
                    NurseName = b.ProductDetails ?? "N/A",
                    BookingDate = b.OrderDate,
                    Status = b.BookingStatus ?? b.OrderStatus ?? "Pending",
                    Amount = b.TotalAmount
                }).ToList();

            return View(bookings);
        }

        [HttpPost]
        public IActionResult SuspendNurse(int id)
        {
            var nurse = _context.Caretaker.FirstOrDefault(c => c.ID == id);
            if (nurse != null)
            {
                nurse.Available = false;
                nurse.VerificationStatus = "Pending";
                _context.SaveChanges();
            }
            return RedirectToAction("ProductList");
        }


        // ── PAYMENTS PAGE ──
        public IActionResult Payments()
        {
            var bookings = _context.OrderConfirm
                .Where(b => b.BookingStatus == "Completed")
                .OrderByDescending(b => b.OrderDate)
                .ToList();

            // Attach nurse bank details
            var nurseIds = bookings
                .Select(b => int.TryParse(b.ProductDetails, out int id) ? id : 0)
                .Where(id => id > 0).Distinct().ToList();

            var nurses = _context.Caretaker
                .Where(c => nurseIds.Contains(c.ID))
                .ToList();

            ViewBag.Nurses = nurses;
            return View(bookings);
        }

        [HttpPost]
        public IActionResult MarkCustomerPaid(int bookingId)
        {
            var booking = _context.OrderConfirm.FirstOrDefault(b => b.Id == bookingId);
            if (booking != null)
            {
                booking.CustomerPaymentStatus  = "PaidToAdmin";
                booking.CustomerPaidToAdminOn  = DateTime.Now;
                booking.PaymentStatus          = "Paid";
                booking.PaymentDate            = DateTime.Now;

                // Update nurse TotalEarned
                if (int.TryParse(booking.ProductDetails, out int nurseId))
                {
                    var nurse = _context.Caretaker.FirstOrDefault(c => c.ID == nurseId);
                    if (nurse != null)
                        nurse.TotalEarned += booking.NursePayableAmount;
                }

                _context.SaveChanges();
                TempData["SuccessMessage"] = "Customer payment received and recorded.";
            }
            return RedirectToAction("Payments");
        }

        [HttpPost]
        public IActionResult MarkNursePaid(int bookingId)
        {
            var booking = _context.OrderConfirm.FirstOrDefault(b => b.Id == bookingId);
            if (booking != null)
            {
                booking.NursePaymentStatus = "PaidToNurse";
                booking.NursePaidOn        = DateTime.Now;
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Nurse payment marked as done.";
            }
            return RedirectToAction("Payments");
        }
    }
}
