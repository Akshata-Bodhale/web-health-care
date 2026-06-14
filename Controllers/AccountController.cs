using CareProjct.web.Data;
using CareProjct.web.Models;
using CareProjct.web.Filter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
        //  CART
        // ────────────────────────────────────
        [UserAuthenication]
        public IActionResult AddToCart(int id)
        {
            var product = _context.Caretaker.FirstOrDefault(p => p.ID == id);
            if (product == null) return NotFound();

            List<CartItem> cart =
                HttpContext.Session.GetObject<List<CartItem>>("Cart")
                ?? new List<CartItem>();

            var cartItem = cart.FirstOrDefault(c => c.ID == id);
            if (cartItem != null)
            {
                cartItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ID        = product.ID,
                    FullName  = product.FullName,
                    ImagePath = product.ImagePath,
                    Price     = product.Price,
                    Category  = product.Category,
                    Gender    = product.Gender,
                    Quantity  = 1
                });
            }

            HttpContext.Session.SetObject("Cart", cart);
            return RedirectToAction("Cart");
        }

        [UserAuthenication]
        public IActionResult Cart()
        {
            var cart =
                HttpContext.Session.GetObject<List<CartItem>>("Cart")
                ?? new List<CartItem>();

            if (cart.Count == 0)
                ViewData["Message"] = "Your cart is empty.";

            return View(cart);
        }

        public IActionResult RemoveFromCart(int id)
        {
            var cart = HttpContext.Session
                .GetObject<List<CartItem>>("Cart");

            if (cart != null)
            {
                var item = cart.FirstOrDefault(c => c.ID == id);
                if (item != null)
                {
                    cart.Remove(item);
                    HttpContext.Session.SetObject("Cart", cart);
                }
            }
            return RedirectToAction("Cart");
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

            model.UserId          = userId;
            model.OrderDate       = DateTime.Now;
            model.BookingStatus   = "Requested";
            model.PaymentStatus   = "Pending";
            model.OrderStatus     = "Confirmed";
            model.TermsAcceptedOn = DateTime.Now;
            var nurse = _context.Caretaker.FirstOrDefault(c => c.ID == CaretakerId);
            model.ProductDetails  = CaretakerId.ToString();
            model.TotalAmount     = PricePerDay * model.NumberOfDays;  // FIX 3

            _context.OrderConfirm.Add(model);
            _context.SaveChanges();

            TempData["SuccessMessage"] =
                "Booking request sent! " +
                "The nurse will confirm within 24 hours.";
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

        // ────────────────────────────────────
        //  CHECKOUT (old cart flow)
        // ────────────────────────────────────
        public IActionResult Checkout()
        {
            var cart =
                HttpContext.Session.GetObject<List<CartItem>>("Cart")
                ?? new List<CartItem>();

            if (cart.Count == 0)
                return RedirectToAction("Cart");

            string userId = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Home");

            var order = new Orders1
            {
                UserId      = userId,
                TotalAmount = cart.Sum(item => item.Price * item.Quantity),
                OrderItems  = cart.Select(item => new OrderItems
                {
                    ProductId   = item.ID,
                    ProductName = item.FullName,
                    Price       = item.Price,
                    Quantity    = item.Quantity,
                }).ToList()
            };

            _context.Orders1.Add(order);
            _context.SaveChanges();

            HttpContext.Session.Remove("Cart");

            return RedirectToAction("OrderConfirmation",
                new { orderId = order.Id });
        }

        // ────────────────────────────────────
        //  ORDER CONFIRMATION
        // ────────────────────────────────────
        public IActionResult OrderConfirmation(int orderId)
        {
            var order = _context.Orders1
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null) return NotFound();

            string userId = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Home");

            if (!int.TryParse(userId, out int userIdInt))
                return RedirectToAction("Login", "Home");

            var user = _context.Register
                .FirstOrDefault(u => u.ID == userIdInt);

            if (user == null)
                return RedirectToAction("Login", "Home");

            var orderConfirm = new OrderConfirm
            {
                OrderId        = order.Id,
                ProductDetails = order.OrderItems != null
                    ? string.Join(", ",
                        order.OrderItems.Select(i => i.ProductName))
                    : "",
                UserId        = userId,
                TotalAmount   = order.TotalAmount,
                OrderDate     = DateTime.Now,
                OrderStatus   = "Pending",
                FullName      = user.FirstName + " " + user.LastName,
                PhoneNumber   = "",
                Method        = "Standard",
                Cost          = 5.00m,
                PaymentMethod = "Credit Card",
                PaymentStatus = "Pending"
            };

            return View(orderConfirm);
        }

        // ────────────────────────────────────
        //  PROCESS PAYMENT
        // ────────────────────────────────────
        [HttpPost]
        public IActionResult ProcessPayment(
            OrderConfirm model,
            string userIdBackup)
        {
            try
            {
                if (string.IsNullOrEmpty(model.UserId))
                    model.UserId = userIdBackup
                        ?? HttpContext.Session.GetString("userId");

                var order = _context.Orders1
                    .Include(o => o.OrderItems)
                    .FirstOrDefault(o => o.Id == model.OrderId);

                if (order == null) return NotFound();

                if (string.IsNullOrEmpty(model.UserId))
                    model.UserId = order.UserId;

                model.PaymentStatus = "Completed";
                model.PaymentDate   = DateTime.Now;
                model.PaymentMethod = "Credit Card";
                model.OrderStatus   = "Confirmed";

                if (order.OrderItems != null && order.OrderItems.Any())
                {
                    model.ProductDetails = string.Join(", ",
                        order.OrderItems.Select(i => i.ProductName));

                    var productIds = order.OrderItems
                        .Select(item => item.ProductId).ToList();

                    var products = _context.Caretaker
                        .Where(p => productIds.Contains(p.ID)).ToList();

                    foreach (var product in products)
                        product.Available = false;
                }

                _context.OrderConfirm.Add(model);
                _context.SaveChanges();

                return RedirectToAction("OrderComplete",
                    new { confirmationId = model.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("",
                    "Error processing payment: " + ex.Message);
                return View("OrderConfirmation", model);
            }
        }

        // ────────────────────────────────────
        //  ORDER COMPLETE
        // ────────────────────────────────────
        public IActionResult OrderComplete(int confirmationId)
        {
            var confirmation = _context.OrderConfirm
                .FirstOrDefault(c => c.Id == confirmationId);

            if (confirmation == null) return NotFound();

            return View(confirmation);
        }

        // ────────────────────────────────────
        //  DOWNLOAD RECEIPT
        // ────────────────────────────────────
        public IActionResult DownloadReceipt(int confirmationId)
        {
            var confirmation = _context.OrderConfirm
                .FirstOrDefault(o => o.Id == confirmationId);

            if (confirmation == null) return NotFound();

            byte[] pdfBytes =
                _receiptService.GenerateOrderReceiptPdf(confirmation);

            return File(pdfBytes, "application/pdf",
                $"ElderCare_Receipt_{confirmation.Id}.pdf");
        }

        // ────────────────────────────────────
        //  PAYMENT PAGE
        // ────────────────────────────────────
        public IActionResult Payment()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Payment(PaymentInfo model)
        {
            if (ModelState.IsValid)
            {
                _context.PaymentInfo.Add(model);
                _context.SaveChanges();
            }
            return RedirectToAction("Payment");
        }

        // ── Customer Profile ──────────────────────────────────────────
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