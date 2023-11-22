using FrontToBack.Areas.Manage.ViewModel;
using FrontToBack.DAL;
using FrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Tag> tags = await _context.Tags.Include(t=>t.ProductTags).ToListAsync();
            DashboardVm vm = new DashboardVm
            {
                Tags= tags,
            };
            return View(vm);
        }

        //========================================== Create =======================================//

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Tag tag)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            bool result = await _context.Tags.AnyAsync(t=>t.Name==tag.Name);
            if(result)
            {
                ModelState.AddModelError("Name", "This tag is aviable");
                return View();
            }
            await _context.AddAsync(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //========================================== Update =======================================//
         public async Task<IActionResult> Update(int id)
         {
            if (id <= 0) BadRequest();
            Tag tag = await _context.Tags.FirstOrDefaultAsync(t=>t.Id==id);
            if (tag != null) NotFound();
            return View(tag);
         }

         [HttpPost]
         public async Task<IActionResult> Update(int id,Tag tag)
         {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Tag exist= await _context.Tags.FirstOrDefaultAsync(t=>t.Id==id);

            if (exist != null) NotFound();
            
            bool result = await _context.Tags.AnyAsync(t=>t.Name==tag.Name &&  t.Id!=id);
            if (result)
            {
                ModelState.AddModelError("Name", "This Tag is aviable");
                return View();
            }

            exist.Name= tag.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
         }


        //========================================== Delete =======================================//
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) BadRequest();
            Tag tag= await _context.Tags.FirstOrDefaultAsync(t=> t.Id==id);

            if (tag== null) NotFound();

            _context.Tags.Remove(tag);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
