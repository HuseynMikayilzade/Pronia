using FrontToBack.Areas.Manage.ViewModel;
using FrontToBack.DAL;
using FrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ColorController : Controller
    {
        private readonly AppDbContext _context;

        public ColorController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Color> colors = await _context.Colors.Include(c=>c.ProductColors).ToListAsync();
            DashboardVm vm = new DashboardVm
            {
                Colors= colors
            };
            return View(vm);
        }

        //========================================== Create =======================================//

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Color color)
        {
           if (!ModelState.IsValid)
           {
                return View();
           }
           bool result = _context.Colors.Any(c=>c.Name.Trim() == color.Name.Trim());
            if (result)
            {
                ModelState.AddModelError("Name","This color is aviable");
                return View();
            }
            await _context.Colors.AddAsync(color);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }


        //========================================== Update =======================================//

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) BadRequest();
            Color color = await _context.Colors.FirstOrDefaultAsync(c=>c.Id == id);
            if (color == null) NotFound();
            return View(color);

        }
        [HttpPost]
        public async Task<IActionResult> Update(int id ,Color color)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Color exist = await _context.Colors.FirstOrDefaultAsync(c=>c.Id==id);

            if (exist == null) NotFound();

            bool result = await _context.Colors.AnyAsync(c=>c.Name == color.Name && c.Id!=id);

            if (result)
            {
                ModelState.AddModelError("Name", "This color is aviable");
                return View();
            }
            exist.Name= color.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //========================================== Delete =======================================//
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) BadRequest();
           Color color = await _context.Colors.FirstOrDefaultAsync(t => t.Id == id);

            if (color == null) NotFound();

            _context.Colors.Remove(color);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
