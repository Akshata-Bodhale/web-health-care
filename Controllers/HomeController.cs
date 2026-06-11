using CareProjct.web.Data;
using CareProjct.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CareProjct.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Applicationdbcontext _Context;

        public HomeController(ILogger<HomeController> logger, Applicationdbcontext ct)
        {
            _logger = logger;
            _Context = ct;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        //Register Form
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Register model)
        {
            if (ModelState.IsValid)
            {
                _Context.Register.Add(model);
                _Context.SaveChanges();

            }
            return View(model);
        }

        //Login

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(String email, String Password)
        {
            var user = _Context.Register.FirstOrDefault(u => u.Email == email && u.Password == Password);
            if (user != null)
            {
                HttpContext.Session.SetString("userId", user.ID.ToString());
                HttpContext.Session.SetString("userFirstName", user.FirstName.ToString());

                TempData["Sucess Message"] = "loginsucessfull";
            }
            return RedirectToAction("Index");

        }
        public IActionResult logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");

        }



        public IActionResult BabyCareService()
        {
            return View();
            
        }
        public IActionResult ElderCareService()
        {
            return View();
        }
        //About us
        
        public IActionResult AboutUs()
        {
            return View();
        }
        //Feedback
        public IActionResult Feedback()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AboutUs(FeedbackViewModel model)
        {
            if (ModelState.IsValid)
            {
                _Context.FeedbackViewModel.Add(model);
                _Context.SaveChanges();

            }
            return View();
        }

        public IActionResult DisplayFeedbacks()
        {
            var data = _Context.FeedbackViewModel.ToList();
            return View(data);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        

       

        
    }
}
