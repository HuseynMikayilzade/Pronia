
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

    public class SizeController : Controller
    {
        private readonly AppDbContext _context;
        public SizeController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin,Moderator,Designer")]
        public async Task<IActionResult> Index()
        {
            List<Size> sizes = await _context.Sizes.Include(s => s.ProductSizes).ToListAsync();
         
            return View(sizes);
        }


        //========================================== Create =======================================//
        [Authorize(Roles = "Admin,Moderator,Designer")]

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSizeVM createSizeVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool result = await _context.Sizes.AnyAsync(t=>t.Name == createSizeVM.Name);
            if (result)
            {
                ModelState.AddModelError("Name", "This size is aviable");
                return View();
            }

            Size size = new Size
            {
                Name = createSizeVM.Name,
            };
            await _context.AddAsync(size);
            await _context.SaveChangesAsync();
            //ToastrSuccess("Size created successfully!");
            return RedirectToAction(nameof(Index));
        }

        //========================================== Update =======================================//

        [Authorize(Roles = "Admin,Moderator,Designer")]

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) BadRequest();
            Size size =await  _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);
            if (size == null) NotFound();
            CreateSizeVM createSizeVM = new CreateSizeVM
            {
                Name = size.Name,
            };

            return View(createSizeVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, CreateSizeVM createSizeVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Size exist= await _context.Sizes.FirstOrDefaultAsync(s=>s.Id == id);
            if (exist == null) NotFound();

            bool result = await _context.Sizes.AnyAsync(s=>s.Name == createSizeVM.Name && s.Id!=id);
            if (result)
            {
                ModelState.AddModelError("Name", "This size is aviable");
                return View();
            }
            exist.Name= createSizeVM.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //========================================== Delete =======================================//
        [Authorize(Roles = "Admin,Moderator")]

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) BadRequest();
           Size size = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);

            if (size == null) NotFound();

            _context.Sizes.Remove(size);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Moderator,Designer")]

        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0) return BadRequest();
           Size size = await _context.Sizes.Include(t => t.ProductSizes).FirstOrDefaultAsync(t => t.Id == id);
            if (size == null) NotFound();
            return View(size);

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
