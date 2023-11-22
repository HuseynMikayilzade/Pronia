using FrontToBack.Areas.Manage.ViewModel;
using FrontToBack.DAL;
using FrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SizeController : Controller
    {
        private readonly AppDbContext _context;
        public SizeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Size> sizes = await _context.Sizes.Include(s => s.ProductSizes).ToListAsync();
            DashboardVm vm = new DashboardVm
            {
                Sizes= sizes,
            };
            return View(vm);
        }


        //========================================== Create =======================================//

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Size size)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool result = await _context.Sizes.AnyAsync(t=>t.Name == size.Name);
            if (result)
            {
                ModelState.AddModelError("Name", "This size is aviable");
                return View();
            }
            await _context.AddAsync(size);
            await _context.SaveChangesAsync();
            //ToastrSuccess("Size created successfully!");
            return RedirectToAction(nameof(Index));
        }

        //========================================== Update =======================================//


        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) BadRequest();
            Size size =await  _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);
            if (size == null) NotFound();
            return View(size);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id,Size size)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Size exist= await _context.Sizes.FirstOrDefaultAsync(s=>s.Id == id);
            if (exist == null) NotFound();

            bool result = await _context.Sizes.AnyAsync(s=>s.Name == size.Name && s.Id!=id);
            if (result)
            {
                ModelState.AddModelError("Name", "This size is aviable");
                return View();
            }
            exist.Name= size.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //========================================== Delete =======================================//

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) BadRequest();
            Tag tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);

            if (tag == null) NotFound();

            _context.Tags.Remove(tag);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }




        //private void ToastrSuccess(string message)
        //{
        //    Toastr("success", message);
        //}

        //private void Toastr(string type, string message)
        //{
        //    TempData["Toastr"] = $"toastr.{type}('{message}');";
        //}

    }
}
