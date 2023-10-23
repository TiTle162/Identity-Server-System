using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet] // Identity Login Page.
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost] // Submit Identity Login Page.
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            // Check LoginViewModel
            var result = await _signInManager.PasswordSignInAsync(vm.Username, vm.Password, false, false);

            if (result.Succeeded)   // Login Success
            {
                return Redirect(vm.ReturnUrl);
            }else if (result.IsLockedOut) // Login Failed
            {

            }

            return View();
        }

        [HttpGet] // Identity Register Page.
        public IActionResult Register(string returnUrl)
        {
            return View(new RegisterViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost] // Submit Identity Register Page.
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            // If Password not match Confirm Password OR some require property is null.
            if( !ModelState.IsValid)
            {
                return View(vm);
            }

            // Create new user.
            var user = new IdentityUser(vm.Username);
            var result = await _userManager.CreateAsync(user, vm.Password);

            if (result.Succeeded)   // Register Success.
            {
                await _signInManager.SignInAsync(user, false);

                return Redirect(vm.ReturnUrl);
            }

            return View();
        }
    }
}
