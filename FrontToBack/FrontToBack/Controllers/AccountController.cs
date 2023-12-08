using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using FrontToBack.Utilities.Extention;
using System.Drawing;
using FrontToBack.Utilities.Extentions;
using FrontToBack.Models;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using FrontToBack.Utilities.Enum;

namespace FrontToBack.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager ,SignInManager<AppUser> signInManager,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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

            await _userManager.AddToRoleAsync(appUser, Roles.Member.ToString());
            await _signInManager.SignInAsync(appUser, isPersistent: false);
            return RedirectToAction(nameof(Index),"Home");
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVm loginVm,string? returnurl)
        {
            if (!ModelState.IsValid) return View();
            AppUser appUser = await _userManager.FindByNameAsync(loginVm.UsernameOrEmail);
            if (appUser == null)
            {
                appUser = await _userManager.FindByEmailAsync(loginVm.UsernameOrEmail);
                if (appUser == null)
                {
                    ModelState.AddModelError(String.Empty, "Email ,Username or password is incorrect");
                    return View();
                }
            }
            var result = await _signInManager.PasswordSignInAsync(appUser, loginVm.Password, loginVm.İsRemember ,true);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(String.Empty, "Account is locked. Please try again after a few minutes.");
                return View();
            }
            if(!result.Succeeded)
            {
                ModelState.AddModelError(String.Empty, "Email ,Username or password is incorrect");
                return View();
            }
            if (returnurl == null)
            {
                return RedirectToAction(nameof(Index), "Home");
            }
            return Redirect(returnurl);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index), "Home");
        }


        public async Task<IActionResult> CreateRole()
        {

           
            foreach (var item in Enum.GetValues(typeof(Roles)))
            {
                if (!await _roleManager.RoleExistsAsync(item.ToString()))
                {  
                    await _roleManager.CreateAsync(new IdentityRole
                    {
                        Name = item.ToString(),
                    });
                }
              
            }
           return RedirectToAction(nameof(Index), "Home");
        }
    }
}
