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

        // ── Admin Home ──
        public IActionResult Index()
        {
            var viewModel = new ProductViewModel
            {
                Products = _context.Caretaker.OrderByDescending(c => c.RegistrationDate).ToList(),
                Product = new Caretaker()
            };
            return View(viewModel);
        }

        // ── Admin Dashboard ──
        public IActionResult AdminDashboard()
        {
            Console.WriteLine("\n📊 ========== ADMIN DASHBOARD LOAD ==========");
            
            var pendingCount = _context.Caretaker.Count(n => n.VerificationStatus == "Pending");
            Console.WriteLine($"📊 Pending Nurses Count: {pendingCount}");

            var model = new AdminDashboardViewModel
            {
                TotalNurses = _context.Caretaker.Count(),
                PendingVerifications = pendingCount,
                TotalCustomers = _context.Register.Count(),
                TotalBookings = _context.OrderConfirm.Count(),
                ActiveBookings = _context.OrderConfirm.Count(b => b.BookingStatus == "Confirmed"),
                TotalRevenue = _context.OrderConfirm.Sum(b => (decimal?)b.TotalAmount) ?? 0m,
                
                RecentBookings = _context.OrderConfirm
                    .OrderByDescending(b => b.OrderDate)
                    .Take(5)
                    .Select(b => new RecentBookingItem
                    {
                        BookingId = b.Id,
                        CustomerName = b.FullName ?? "N/A",
                        NurseName = b.ProductDetails ?? "N/A",
                        BookingDate = b.OrderDate,
                        Status = b.BookingStatus ?? b.OrderStatus ?? "Pending",
                        Amount = b.TotalAmount
                    }).ToList(),

                // ✅ CRITICAL: Add PendingNurses to show in dashboard
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

            Console.WriteLine($"📊 Pending Nurses in Model: {model.PendingNurses.Count}");
            foreach (var nurse in model.PendingNurses)
            {
                Console.WriteLine($"   - {nurse.Name} ({nurse.NurseId}) - {nurse.AppliedDate}");
            }
            Console.WriteLine($"========== DASHBOARD LOADED ==========\n");

            return View(model);
        }

        // ── Verification Queue ──
        public IActionResult VerificationQueue()
        {
            var pending = _context.Caretaker
                .Where(c => c.VerificationStatus == "Pending"
                         || c.VerificationStatus == "UnderReview")
                .OrderByDescending(c => c.RegistrationDate)
                .ToList();
            return View(pending);
        }

        // ── Approve a nurse ──
        [HttpPost]
        public IActionResult ApproveNurse(int id)
        {
            Console.WriteLine($"\n✅ ========== APPROVE NURSE ==========");
            Console.WriteLine($"✅ Nurse ID: {id}");

            var nurse = _context.Caretaker.FirstOrDefault(c => c.ID == id);
            if (nurse != null)
            {
                nurse.VerificationStatus = "Approved";
                nurse.Available = true;
                nurse.VerifiedOn = DateTime.Now;
                _context.SaveChanges();

                Console.WriteLine($"✅ Nurse: {nurse.FullName}");
                Console.WriteLine($"✅ Status: {nurse.VerificationStatus}");
                Console.WriteLine($"✅ Available: {nurse.Available}");
                Console.WriteLine($"✅ Verified On: {nurse.VerifiedOn}");
                
                TempData["SuccessMessage"] = nurse.FullName + " has been approved and is now live.";
            }
            else
            {
                Console.WriteLine($"❌ Nurse not found!");
            }
            
            Console.WriteLine($"========== APPROVE COMPLETE ==========\n");
            return RedirectToAction("VerificationQueue");
        }

        // ── Reject a nurse ──
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

        // ── All approved nurses ──
        public IActionResult ProductList()
        {
            var data = _context.Caretaker
                .Where(c => c.VerificationStatus == "Approved")
                .ToList();
            return View(data);
        }

        // ── All Bookings ──
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

        // ── Suspend a nurse ──
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
    }
}