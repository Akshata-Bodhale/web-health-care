using CareProjct.web.Data;
using CareProjct.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareProjct.web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly Applicationdbcontext _Context;

        public CustomerController(ILogger<CustomerController> logger, Applicationdbcontext ct)
        {
            _logger = logger;
            _Context = ct;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Customer()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Customer(Customer model)
        {
            if (ModelState.IsValid)
            {
                _Context.Customer.Add(model);
                _Context.SaveChanges();

                TempData["SuccessMessage"] = "Customer Details Filled successful!";
            }

            return View(model);
        }

        public IActionResult CustomerMembership()
        {
            return View();
        }
    }
}
