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
    public class CaretakerController : Controller
    {
        private readonly ILogger<CaretakerController> _logger;
        private readonly Applicationdbcontext _Context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CaretakerController(ILogger<CaretakerController> logger, Applicationdbcontext ct, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _Context = ct;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Caretaker()
        {
            return View();
        }
        public IActionResult CaretakerData(string category = null)
        {
            var query = _Context.Caretaker.Where(p => p.Available == true);

            // Apply category filter if provided
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }

            var data = query.ToList();

            // Pass the selected category to the view for maintaining state
            ViewBag.SelectedCategory = category;

            // You might want to pass all available categories for the dropdown
            ViewBag.Categories = _Context.Caretaker
                .Where(p => p.Available == true)
                .Select(p => p.Category)
                .Distinct()
                .ToList();


            return View(data);
        }
        [UserAuthenication]
        public IActionResult CaretakerProfile(int ID)
        {
            var user = _Context.Caretaker.FirstOrDefault(u => u.ID == ID);
            if (user == null)
            { return NotFound(); }
            return View(user);
        }
        [HttpPost]

        public async Task<IActionResult> Caretaker(Caretaker product)
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

                _Context.Caretaker.Add(product);
                await _Context.SaveChangesAsync();
                TempData["Success"] = "Product created successfully!";
                return RedirectToAction(nameof(Caretaker));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating product: " + ex.Message);
            }

            return View(product);
        }



    }
}






