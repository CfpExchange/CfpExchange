using System.Threading.Tasks;
using CfpExchange.Models;
using CfpExchange.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CfpExchange.Controllers
{
    [ValidateAntiForgeryToken]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel viewModel, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    viewModel.EmailAddress,
                    viewModel.Password,
                    false,
                    false);

                if (result.Succeeded)
                {
                    return RedirectLocal(returnUrl);
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Your account is locked.");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt");
                }
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View(viewModel);
        }

        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel viewModel, string returnUrl)
        {
            if (viewModel.Password != viewModel.PasswordConfirmation)
            {
                ModelState.AddModelError("PasswordConfirmation", "Both passwords must match");
            }
            
            if (ModelState.IsValid)
            {
                var applicationUser = new ApplicationUser()
                {
                    Email = viewModel.EmailAddress,
                    UserName = viewModel.EmailAddress,
                    FullName = viewModel.FullName
                };

                var result = await _userManager.CreateAsync(applicationUser, viewModel.Password);

                if (result.Succeeded)
                {
                    return RedirectLocal(returnUrl);
                }
            }

            ViewData["ReturnUrl"] = returnUrl;
            
            return View(viewModel);
        }

        public IActionResult RecoverPassword()
        {
            return View();
        }

        private IActionResult RedirectLocal(string url)
        {
            if (Url.IsLocalUrl(url))
            {
                return Redirect(url);
            }

            return RedirectToAction("Home", "Index");
        }
    }
}