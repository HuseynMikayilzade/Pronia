using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using FrontToBack.Utilities.Extention;
using System.Drawing;
using FrontToBack.Utilities.Extentions;
using FrontToBack.Models;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;

namespace FrontToBack.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager ,SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVm registerVm)
        {
            if(!ModelState.IsValid) return View(registerVm);


            if(Stringformat.IsDigit(registerVm.Name)) 
            {
                ModelState.AddModelError("Name", "Name cannot contain numbers");
                return View (registerVm);
            }
            if (Stringformat.IsDigit(registerVm.Surname))
            {
                ModelState.AddModelError("Surname", "Surname cannot contain numbers");
                return View(registerVm);
            }
            if (Stringformat.CheckEmail(registerVm.Email)==false)
            {
                ModelState.AddModelError("Email", "Email is not entered correctly");
                return View(registerVm);
            }


            AppUser appUser = new AppUser
            {
                
                Name = Stringformat.Capitalize(registerVm.Name),
                Surname = Stringformat.Capitalize(registerVm.Surname),
                Email=registerVm.Email,
                UserName=registerVm.Username,
                Gender=registerVm.Gender
                
            };

            IdentityResult result = await _userManager.CreateAsync(appUser,registerVm.Password);
            if (!result.Succeeded)
            {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(String.Empty, error.Description);
                    }
                return View();
            }
           
            await _signInManager.SignInAsync(appUser, isPersistent: false);
            return RedirectToAction(nameof(Index),"Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index), "Home");
        }

    }
}
