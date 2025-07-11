﻿using FrontToBack.Areas.Manage.ViewModels;
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

    public class ColorController : Controller
    {
        private readonly AppDbContext _context;

        public ColorController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin,Moderator,Designer")]

        public async Task<IActionResult> Index(int page=1)
        {
            double count = await _context.Colors.CountAsync();
            double TotalPage = Math.Ceiling(count / 4);
            if (page <= 0)
            {
                return BadRequest();
            }
            else if (page > TotalPage)
            {
                return BadRequest();
            }

            List<Color> colors = await _context.Colors.Skip((page-1)*4).Take(4).Include(c=>c.ProductColors).ToListAsync();
            if (colors==null) return NotFound();

            PaginationVm<Color> paginationVm = new PaginationVm<Color>
            {
                Items = colors,
                TotalPage = TotalPage,
                PageCount = page
            };

            return View(paginationVm);
        }

        //========================================== Create =======================================//
        [Authorize(Roles = "Admin,Moderator,Designer")]

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
        [Authorize(Roles = "Admin,Moderator,Designer")]

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
        [Authorize(Roles = "Admin,Moderator,Designer")]
        public async Task<IActionResult> Detail(int id)
        {
            if(id<=0) BadRequest();
            Color color = _context.Colors.Include(c=>c.ProductColors).FirstOrDefault(c => c.Id == id);
            if (color == null) NotFound();
            return View(color);
        }

        [Authorize(Roles = "Admin")]
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
