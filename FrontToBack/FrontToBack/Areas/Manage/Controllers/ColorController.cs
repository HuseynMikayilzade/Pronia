using FrontToBack.Areas.Manage.ViewModels;
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
            
            
            return View(colors);
        }

        //========================================== Create =======================================//

        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateColorVm createColorVm)
        {
           if (!ModelState.IsValid)
           {
                return View();
           }
           bool result = _context.Colors.Any(c=>c.Name.Trim() == createColorVm.Name.Trim());
            if (result)
            {
                ModelState.AddModelError("Name","This color is aviable");
                return View();
            }

            Color color = new Color
            {
                Name = createColorVm.Name,
            };
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
            UpdateColorVm updateColorVm = new UpdateColorVm{
                Name=color.Name
            };
            return View(updateColorVm);

        }
        [HttpPost]
        public async Task<IActionResult> Update(int id ,UpdateColorVm updateColorVm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Color exist = await _context.Colors.FirstOrDefaultAsync(c=>c.Id==id);

            if (exist == null) NotFound();

            bool result = await _context.Colors.AnyAsync(c=>c.Name == updateColorVm.Name && c.Id!=id);

            if (result)
            {
                ModelState.AddModelError("Name", "This color is aviable");
                return View();
            }
            exist.Name= updateColorVm.Name;
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

        public async Task<IActionResult> Detail(int id)
        {
            if(id<=0) BadRequest();
            Color color = _context.Colors.Include(c=>c.ProductColors).FirstOrDefault(c => c.Id == id);
            if (color == null) NotFound();
            return View(color);
        }
    }
}
