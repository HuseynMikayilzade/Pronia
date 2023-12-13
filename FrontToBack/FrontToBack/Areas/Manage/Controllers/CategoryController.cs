
using FrontToBack.Areas.Manage.ViewModels;
using FrontToBack.DAL;
using FrontToBack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Admin,Moderator,Designer")]
    [AutoValidateAntiforgeryToken] 
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page=1)
        {
            double count = await _context.Categories.CountAsync();
            double TotalPage = Math.Ceiling(count / 4);
            if (page <= 0)
            {
                return BadRequest();
            }
            else if (page > TotalPage)
            {
                return BadRequest();
            }
            
            List<Category> categories = await _context.Categories.Skip((page-1)*4).Take(4).Include(c=>c.Products).ToListAsync();
            if(categories==null) return NotFound();
            PaginationVm<Category> paginationVm = new PaginationVm<Category>
            {
                Items = categories,
                TotalPage = TotalPage,
                PageCount = page
            };
            return View(paginationVm);
        }
        //========================================== Create =======================================//

        [Authorize(Roles = "Admin,Moderator")]

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

        [Authorize(Roles = "Admin,Moderator")]

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


        [Authorize(Roles = "Admin,Moderator,Designer")]
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

        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) BadRequest();
            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) NotFound();

            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
