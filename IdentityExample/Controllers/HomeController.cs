using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;
using System.Security.Claims;

namespace IdentityExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailService _emailService;

        public HomeController(
            UserManager<IdentityUser> userManager, 
            SignInManager<IdentityUser> signInManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        // verify email has sent
        public IActionResult EmailVerification()
        {
            return View();
        }

        // after verify email
        public async Task<IActionResult> VerifyEmail(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return BadRequest();
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                return View();
            }

            return BadRequest();
        }

        [HttpPost]
        // Login Function
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username); // check usernameInput is in Memory?

            if (user != null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);    // Create Cookie

                if (signInResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");   // trigger when usernameInput correct but passwordInput incorrect. & when usernameInput is not found in Memory.
        }

        [HttpPost]
        // Register Function
        public async Task<IActionResult> Register(string username, string password)
        {
            var user = new IdentityUser
            {
                UserName = username,
                Email = "",
            };

            var result = await _userManager.CreateAsync(user, password);    // add usernameInput in Memory.   

            if (result.Succeeded)
            {

                /* register token */
                //var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);  // Create Cookie

                //if (signInResult.Succeeded)
                //{
                //    return RedirectToAction("Index");
                //}
                //return RedirectToAction("Index");
                /* register token */

                /* email token */
                //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                //var link = Url.Action(nameof(VerifyEmail), "Home", new { userId = user.Id, code }, Request.Scheme, Request.Host.ToString());

                //await _emailService.SendAsync("sirawit_083@hotmail.com", "Emil Verify.", $"<a href=\"{link}\">Verify Email</a>", true);

                //return RedirectToAction("EmailVerification");
                /* email token */
            }

            return RedirectToAction("Index");   // trigger when usernameInput is duplicate in Memory. 
        }

        // Logout Function
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();    // Clear Cookie

            return RedirectToAction("Index");
        }
    }
}
