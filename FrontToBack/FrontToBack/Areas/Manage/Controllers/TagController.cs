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

    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin,Moderator,Designer")]

        public async Task<IActionResult> Index()
        {
            List<Tag> tags = await _context.Tags.Include(t=>t.ProductTags).ToListAsync();
            
            return View(tags);
        }

        //========================================== Create =======================================//
        [Authorize(Roles = "Admin,Moderator,Designer")]

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSizeVm createTagVm)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            bool result = await _context.Tags.AnyAsync(t=>t.Name== createTagVm.Name);
            if(result)
            {
                ModelState.AddModelError("Name", "This tag is aviable");
                return View();
            }

            Tag tag = new Tag { Name = createTagVm.Name};

            await _context.AddAsync(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //========================================== Update =======================================//
        [Authorize(Roles = "Admin,Moderator,Designer")]

         public async Task<IActionResult> Update(int id)
         {
            if (id <= 0) BadRequest();
            Tag tag = await _context.Tags.FirstOrDefaultAsync(t=>t.Id==id);
            if (tag != null) NotFound();
            UpdateTagVm updateTagVm = new UpdateTagVm
            {
                Name = tag.Name,
            };
            return View(updateTagVm);
         }

         [HttpPost]
         public async Task<IActionResult> Update(int id,UpdateTagVm updateTagVm)
         {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Tag exist= await _context.Tags.FirstOrDefaultAsync(t=>t.Id==id);

            if (exist != null) NotFound();
            
            bool result = await _context.Tags.AnyAsync(t=>t.Name== updateTagVm.Name &&  t.Id!=id);
            if (result)
            {
                ModelState.AddModelError("Name", "This Tag is aviable");
                return View();
            }

            exist.Name= updateTagVm.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
         }


        //========================================== Delete =======================================//
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) BadRequest();
            Tag tag= await _context.Tags.FirstOrDefaultAsync(t=> t.Id==id);

            if (tag== null) NotFound();

            _context.Tags.Remove(tag);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Moderator,Designer")]
        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0) return BadRequest();
            Tag tag = await _context.Tags
                .Include(t=>t.ProductTags)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tag== null) NotFound();

            return View(tag);
            
        }



    }
}
