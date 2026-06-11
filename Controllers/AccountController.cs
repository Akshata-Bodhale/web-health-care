using CareProjct.web.Data;
using CareProjct.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CareProjct.web.Filter;
using System.Diagnostics;


namespace CareProjct.web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly Applicationdbcontext _Context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AccountController(ILogger<AccountController> logger, Applicationdbcontext ct, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _Context = ct;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Payment()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Payment(PaymentInfo model)
        {
            if (ModelState.IsValid)
            {
                _Context.PaymentInfo.Add(model);
                _Context.SaveChanges();

            }
            return RedirectToAction("Payment");
        }
        //Cart Code
        [UserAuthenication]
        public IActionResult AddToCart(int id)
        {
            var product = _Context.Caretaker.FirstOrDefault(p => p.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            // Retrieve the cart from session or create a new one
            List<CartItem> cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

            //Check if product already exists in cart
            var cartItem = cart.FirstOrDefault(c => c.ID == id);
            if (cartItem != null)
            {
                /*cartItem.Quantity++;*/ // Increase quantity if already in cart
            }
            else
            {
                cart.Add(new CartItem
                {
                    ID = product.ID,
                    FullName = product.FullName,
                    ImagePath = product.ImagePath,

                    Category = product.Category,
                    Gender = product.Gender,
                    
                });
            }

            //Save updated cart in session
            HttpContext.Session.SetObject("Cart", cart);

            return RedirectToAction("Cart");
        }
        [UserAuthenication]
        public IActionResult Cart()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

            if (cart.Count == 0)
            {
                ViewData["Message"] = "Your cart is empty.";
            }

            return View(cart); // Pass cart model to view }

        }
        public IActionResult RemoveFromCart(int id)
        {
            // Retrieve cart from session
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart");

            if (cart != null)
            {
                // Find the item to remove
                var itemToRemove = cart.FirstOrDefault(c => c.ID == id);
                if (itemToRemove != null)
                {
                    cart.Remove(itemToRemove);

                    // Update session with modified cart
                    HttpContext.Session.SetObject("Cart", cart);
                }
            }

            return RedirectToAction("Cart"); // Refresh the cart page
        }
        //checkout
        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

            if (cart.Count == 0)
            {
                return RedirectToAction("Cart");
            }

            // Get the current logged-in user's ID from session
            string userId = HttpContext.Session.GetString("userId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Home"); // Ensure the user is logged in before checkout
            }

            // Create Order and Save it in the Database
            var order = new Orders1
            {
                UserId = userId,
                TotalAmount = cart.Sum(item => item.Price * item.Quantity),
                OrderItems = cart.Select(item => new OrderItems
                {
                    ProductId = item.ID,
                    ProductName = item.FullName,
                    Price = item.Price,
                    Quantity = item.Quantity,
                }).ToList()
            };

            _Context.Orders1.Add(order);
            _Context.SaveChanges();

            // Clear the cart after successful order placement
            HttpContext.Session.Remove("Cart");

            return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
        }
        //Order Confirmation
        public IActionResult OrderConfirmation(int orderId)
        {
            // Get the order from database using the orderId
            var order = _Context.Orders1
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                return NotFound();
            }

            // Get the current user ID from session
            string userId = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Home");
            }

            // Get user details - make sure to use the correct userId (the one from the order)
            // If userId from session is stored as a string but Register.Id is int
            if (!int.TryParse(userId, out int userIdInt))
            {
                return RedirectToAction("Login", "Home");
            }

            var user = _Context.Register.FirstOrDefault(u => u.ID == userIdInt);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Create a new OrderConfirm object - use the correct userId format
            var orderConfirm = new OrderConfirm
            {
                OrderId = order.Id,
                ProductDetails = order.OrderItems != null ? string.Join(", ", order.OrderItems.Select(i => i.ProductName)) : "",
                UserId = userId, // Make sure this matches the type expected in OrderConfirms1
                TotalAmount = order.TotalAmount,
                OrderDate = DateTime.Now,
                OrderStatus = "Pending",
                FullName = user.Email ?? user.Email, // Should use Name if available
                Address = user.Email ?? "", // Should use actual address field
                City = user.Email ?? "", // Should use actual city field
               // ShippingState = user.Email ?? "", // Should use actual state field
                ZipCode = user.Password ?? "", // Should use actual zipcode field
                Country = user.Password ?? "", // Should use actual country field
                PhoneNumber = user.Password ?? "", // Should use actual phone field
                Method = "Standard",
                Cost = 5.00m,
                PaymentMethod = "Credit Card",
                PaymentStatus = "Pending"
            };

            return View(orderConfirm);
        }



        [HttpPost]
        public IActionResult ProcessPayment(OrderConfirm model, string userIdBackup)
        {
            try
            {
                // If UserId is missing, try to get it from the backup or session
                if (string.IsNullOrEmpty(model.UserId))
                {
                    model.UserId = userIdBackup ?? HttpContext.Session.GetString("userId");
                    // Log or debug output
                    System.Diagnostics.Debug.WriteLine($"UserId was missing! Using backup: {model.UserId}");
                }
                // Get the original order
                var order = _Context.Orders1
                    .Include(o => o.OrderItems)
                    .FirstOrDefault(o => o.Id == model.OrderId);
                if (order == null)
                {
                    return NotFound();
                }
                // If still no UserId, try to get it from the order
                if (string.IsNullOrEmpty(model.UserId))
                {
                    model.UserId = order.UserId;
                }
                // Rest of your code...
                model.PaymentStatus = "Completed";
                model.PaymentDate = DateTime.Now;
                model.PaymentMethod = "Credit Card";
                model.OrderStatus = "Confirmed";
                // Set proper product details
                if (order.OrderItems != null && order.OrderItems.Any())
                {
                    model.ProductDetails = string.Join(", ", order.OrderItems.Select(i => i.ProductName));

                    // Get the product IDs from OrderItems
                    var productIds = order.OrderItems.Select(item => item.ProductId).ToList();

                    // Update the Available status of these products to 0 (unavailable)
                    var products = _Context.Caretaker.Where(p => productIds.Contains(p.ID)).ToList();
                    foreach (var product in products)
                    {
                        product.Available = false; // or 0 depending on how your model is set up
                    }
                }

                // Save to database
                _Context.OrderConfirm.Add(model);
                _Context.SaveChanges();
                return RedirectToAction("OrderComplete", new { confirmationId = model.Id });
            }
            catch (Exception ex)
            {
                // Log the error
                ModelState.AddModelError("", "Error processing payment: " + ex.Message);
                return View("OrderConfirmation", model);
            }
        }
        public IActionResult OrderComplete(int confirmationId)
        {
            var confirmation = _Context.OrderConfirm.FirstOrDefault(c => c.Id == confirmationId);
            if (confirmation == null)
            {
                return NotFound();
            }

            return View(confirmation);
        }
    }
}
