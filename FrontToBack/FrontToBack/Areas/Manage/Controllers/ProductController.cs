using FrontToBack.Areas.Manage.ViewModels;
using FrontToBack.DAL;
using FrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> products = await  _context.Products
                .Include(p=>p.Category)
                .Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary==true))
                .ToListAsync();
            
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories= await _context.Categories.ToListAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVm createVm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                return View();
            }

            bool result = await _context.Categories.AnyAsync(c => c.Id == createVm.CategoryId);

            if (!result)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();

                ModelState.AddModelError("CategoryId", "Bu idli categori yoxdur");
                return View();
            }
            if (createVm.Price < 0)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                ModelState.AddModelError("Price", "deyer duzgun deyil");
                return View();
            }

            Product product = new Product
            {
                Name = createVm.Name,
                Price = createVm.Price,
                Order = createVm.Order,
                SKU = createVm.SKU,
                Description = createVm.Description,
                CategoryId = (int) createVm.CategoryId
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Detail(int id)
        {
           Product product = await _context.Products
                .Include(p=>p.Category)
                .Include(p=>p.ProductColors).ThenInclude(pc=>pc.Color)
                .Include(p=>p.ProductTags).ThenInclude(pt=>pt.Tag)
                .Include(p=>p.ProductSizes).ThenInclude(pt=>pt.Size)
                .Include(p=>p.ProductImages).FirstOrDefaultAsync(p=>p.Id==id);
               

            if (product == null) return NotFound();
           
            return View(product);
        }
    }
}
