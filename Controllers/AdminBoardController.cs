using CareProjct.web.Data;
using CareProjct.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace CareProjct.web.Controllers
{
    public class AdminBoardController : Controller
    {
        private object _webHostEnvironment;

        private readonly ILogger<CaretakerController> _logger;
        private readonly Applicationdbcontext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public AdminBoardController(ILogger<CaretakerController> logger, Applicationdbcontext ct, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _context = ct;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            var products = _context.Caretaker.ToList();

            // Create a ProductViewModel and assign the list of products
            var viewModel = new ProductViewModel
            {
                Products = products,
                Product = new Caretaker() // Initialize an empty product for the form
            };

            // Pass the viewModel to the view
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Save(Caretaker product)
        {
            try
            {
                if (product.ImageFile != null && product.ImageFile.Length > 0)
                {
                    // Generate unique filename
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(product.ImageFile.FileName);

                    // Get uploads path
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "Images");

                    // Create directory if it doesn't exist
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Generate full file path
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(fileStream);
                    }

                    // Save relative path to database
                    product.ImagePath = "/images/" + uniqueFileName;
                }

                _context.Caretaker.Add(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Product created successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating product: " + ex.Message);
            }

            return View();

        }
        public IActionResult ProductList()
        {
            // Fetch all data from the database table (e.g., Reg table)
            var data = _context.Caretaker.ToList(); // Replace Reg with your table/model name
            return View(data);
        }
        public IActionResult Dashboard()
        { return View(); }

        public IActionResult Products() { return View(); }

        public IActionResult Register()
        {
            return View();

        }
        [HttpPost]
        public IActionResult Register(Register model)
        {

            if (ModelState.IsValid)
            {
                _context.Register.Add(model);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Registration successful!";
            }

            return RedirectToAction("Register");
        }

    }
}
