
using FrontToBack.DAL;
using FrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Category> categories = await _context.Categories.Include(c=>c.Products).ToListAsync();

       
            return View(categories);
        }


        //========================================== Create =======================================//


        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if(!ModelState.IsValid)
            {
                return  View();
            }
            bool result = _context.Categories.Any(c => c.Name.Trim() == category.Name.Trim());
            if(result)
            {
                ModelState.AddModelError("Name", "This category is already available");
                return View();
            }
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //========================================== Update =======================================//


        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) BadRequest();
            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) NotFound();

            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Category exist = await _context.Categories.FirstOrDefaultAsync(c=>c.Id == id);
            if (exist == null) NotFound();

            bool result = await _context.Categories.AnyAsync(c => c.Name == category.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "This category is aviable");
                return View();
            }
            exist.Name= category.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //========================================== Delete =======================================//

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) BadRequest();
            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) NotFound();

            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Detail(int id)
        {
            if(id<0) BadRequest();
            Category category = await _context.Categories
                .Include(c=>c.Products)
                    .ThenInclude(c=>c.ProductImages)
                
                .Include(c=>c.Products)
                        .ThenInclude(p=>p.ProductTags).ThenInclude(pt=>pt.Tag)

                .Include(c=>c.Products)
                    .ThenInclude(p=>p.ProductSizes).ThenInclude(ps=>ps.Size)

                .Include(c=>c.Products)
                    .ThenInclude(p=>p.ProductColors).ThenInclude(pc=>pc.Color)
                        .FirstOrDefaultAsync(c => c.Id == id);
                
            if (category == null) NotFound();
            return View(category);

        }
    }
}
