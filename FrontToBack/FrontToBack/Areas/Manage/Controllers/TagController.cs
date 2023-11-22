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
    }
}
