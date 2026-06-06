using FoodProject.Data;
using FoodProject.Models;
using FoodProject.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AboutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AboutController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index() 
        {
            var about = await _context.Abouts.FirstOrDefaultAsync();
            if (about == null)
            {
                return View(new About());
            }
            return View(about);
        }

        [HttpGet]
        public async Task<IActionResult> Update()
        {
            var about = await _context.Abouts.FirstOrDefaultAsync();

            if (about == null)
            {
                about = new About
                {
                    AboutTitle = "About Us",
                    AboutText = "Write something about your store...",
                    AboutImageUrl = "default.png"
                };
                _context.Abouts.Add(about);
                await _context.SaveChangesAsync();
            }

            var model = new AboutUpdateViewModel
            {
                AboutId = about.AboutId,
                AboutTitle = about.AboutTitle,
                AboutText = about.AboutText,
                CurrentImageUrl = about.AboutImageUrl
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(AboutUpdateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var about = await _context.Abouts.FirstOrDefaultAsync();
            if (about == null) return NotFound();

            about.AboutTitle = model.AboutTitle;
            about.AboutText = model.AboutText;

            if (model.ImageFile != null)
                about.AboutImageUrl = await SaveImageAsync(model.ImageFile);

            await _context.SaveChangesAsync();

            TempData["Success"] = "About section updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        private static async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/about");
            Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(folder, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await imageFile.CopyToAsync(stream);

            return fileName;
        }
    }
}