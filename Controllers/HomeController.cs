using CareProjct.web.Data;
using CareProjct.web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CareProjct.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Applicationdbcontext _Context;

        // ── ADMIN CREDENTIALS (hardcoded) ──
        private const string AdminEmail    = "admin@eldercare.com";
        private const string AdminPassword = "Admin@1234";

        public HomeController(ILogger<HomeController> logger,
            Applicationdbcontext ct)
        {
            _logger  = logger;
            _Context = ct;
        }

        public IActionResult Index()   { return View(); }
        public IActionResult Privacy() { return View(); }
        public IActionResult ElderCareService() { return View(); }
        public IActionResult AboutUs() { return View(); }
        public IActionResult Feedback() { return View(); }

        // ────────────────────────────────────
        //  CUSTOMER REGISTRATION
        // ────────────────────────────────────
        
        public IActionResult Register() { return View(); }

        [HttpPost]
        public IActionResult Register(Register model)
        {
            // Check email already exists
            var exists = _Context.Register
                .Any(u => u.Email == model.Email);
            if (exists)
            {
                TempData["ErrorMessage"] =
                    "This email is already registered. Please login.";
                return View(model);
            }

            model.Type = "Customer";
            model.AgreementAcceptedOn = DateTime.Now;
            _Context.Register.Add(model);
            _Context.SaveChanges();

            TempData["SuccessMessage"] =
                "Registration successful! Please login.";
            return RedirectToAction("Login");
        }

        // ────────────────────────────────────
        //  CARETAKER REGISTRATION
        //  (only creates login account here,
        //   full profile submitted separately)
        // ────────────────────────────────────
        public IActionResult CaretakerRegister() { return View(); }

        [HttpPost]
        public IActionResult CaretakerRegister(Register model)
        {
            var exists = _Context.Register
                .Any(u => u.Email == model.Email);
            if (exists)
            {
                TempData["ErrorMessage"] =
                    "This email is already registered.";
                return View(model);
            }

            model.Type = "Caretaker";
            model.AgreementAcceptedOn = DateTime.Now;
            _Context.Register.Add(model);
            _Context.SaveChanges();

                        // ── Set session so nurse is "logged in" immediately ──
            HttpContext.Session.SetString("userId",   model.ID.ToString());
            HttpContext.Session.SetString("UserName", model.FirstName + " " + model.LastName);
            HttpContext.Session.SetString("UserType", "Caretaker");
            HttpContext.Session.SetString("UserEmail", model.Email);
            HttpContext.Session.SetString("Email",     model.Email); 
            
            TempData["SuccessMessage"] =
                "Account created! Please complete your nurse profile.";

            // After login account created, go fill nurse profile form
            return RedirectToAction("Caretaker", "Caretaker");
        }

        // ────────────────────────────────────
        //  LOGIN (handles all 3 roles)
        // ────────────────────────────────────
        public IActionResult Login() { return View(); }

        [HttpPost]
        public IActionResult Login(string email, string Password)
        {
            // ── 1. Admin Check (hardcoded) ──
            if (email == AdminEmail && Password == AdminPassword)
            {
                HttpContext.Session.SetString("userId",    "0");
                HttpContext.Session.SetString("Email",     AdminEmail);
                HttpContext.Session.SetString("UserEmail", AdminEmail);
                HttpContext.Session.SetString("UserType",  "Admin");
                HttpContext.Session.SetString("UserName",  "Admin");
                return RedirectToAction("AdminDashboard", "AdminBoard");
            }

            // ── 2. Check in Register table ──
            var user = _Context.Register
                .FirstOrDefault(u => u.Email    == email
                                  && u.Password == Password);

            if (user == null)
            {
                TempData["ErrorMessage"] =
                    "Invalid email or password. Please try again.";
                return View();
            }

            // ── 3. Store session ──
            HttpContext.Session.SetString("userId",    user.ID.ToString());
            HttpContext.Session.SetString("Email",     user.Email);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserType",  user.Type);
            HttpContext.Session.SetString("UserName",  user.FirstName + " " + user.LastName);

            // ── 4. Redirect based on role ──
            if (user.Type == "Caretaker")
            {
                // Check if nurse profile exists and is approved
                var nurseProfile = _Context.Caretaker
                    .FirstOrDefault(c => c.Email == user.Email);

                if (nurseProfile == null)
                {
                    // Profile not submitted yet
                    TempData["InfoMessage"] =
                        "Please complete your nurse profile first.";
                    return RedirectToAction("Caretaker", "Caretaker");
                }

                if (nurseProfile.VerificationStatus == "Pending"
                 || nurseProfile.VerificationStatus == "UnderReview")
                {
                    return RedirectToAction(
                        "RegistrationPending", "Caretaker");
                }

                if (nurseProfile.VerificationStatus == "Rejected")
                {
                    TempData["ErrorMessage"] =
                        "Your profile was rejected. Reason: "
                        + nurseProfile.RejectionReason
                        + ". Please re-register with correct documents.";
                    return View();
                }

                // Approved — go to nurse dashboard
                return RedirectToAction("MyDashboard", "Caretaker");
            }
            else if (user.Type == "Customer")
            {
                return RedirectToAction("CaretakerData", "Caretaker");
            }

            TempData["ErrorMessage"] = "Unknown user type.";
            return View();
        }

        // ────────────────────────────────────
        //  LOGOUT
        // ────────────────────────────────────
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "You have been logged out.";
            return RedirectToAction("Login");
        }

        // ────────────────────────────────────
        //  FEEDBACK
        // ────────────────────────────────────
        [HttpPost]
        public IActionResult AboutUs(FeedbackViewModel model)
        {
            if (ModelState.IsValid)
            {
                _Context.FeedbackViewModel.Add(model);
                _Context.SaveChanges();
                TempData["SuccessMessage"] = "Thank you for your feedback!";
                return RedirectToAction("DisplayFeedbacks");
                }
                return View();
        }

        public IActionResult DisplayFeedbacks()
        {
            var data = _Context.FeedbackViewModel.ToList();
            return View(data);
        }

        [ResponseCache(Duration = 0,
            Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                         ?? HttpContext.TraceIdentifier
            });
        }
    }
}