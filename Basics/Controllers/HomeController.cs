using Basics.CustomerPolicyProvider;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Basics.Controllers
{
    public class HomeController : Controller
    {
        private IAuthorizationService _authorizationService;

        public HomeController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
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

        [Authorize(Policy = "Claim.DoB")]
        public IActionResult SecretPolicy()
        {
            return View("Secret");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult SecretRole()
        {
            return View("Secret");
        }

        [SecurityLevel(5)]
        public IActionResult SecretLevel()
        {
            return View("Secret");
        }

        [SecurityLevel(10)]
        public IActionResult SecretHigherLevel()
        {
            return View("Secret");
        }

        // create token
        [AllowAnonymous]
        public IActionResult Authenticate()
        {
            var titleClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Sirawit"),
                new Claim(ClaimTypes.Email, "sirawit@csimail.com"),
                new Claim(ClaimTypes.DateOfBirth, "12/12/2000"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(DynamicPilicies.SecurityLevel, "7"),
                new Claim("Word", "Hello, Title."),
            };

            var titleIdentity = new ClaimsIdentity(titleClaims, "Title Identity");

            var userPrinciple = new ClaimsPrincipal(new[] { titleIdentity });

            HttpContext.SignInAsync(userPrinciple);

            return RedirectToAction("Index");   
        }

        public async Task<IActionResult> DoStuff(
            [FromServices] IAuthorizationService authorizationService)
        {
            var builder = new AuthorizationPolicyBuilder("Schema");
            var customPolicy = builder.RequireClaim("Hello").Build();

            var authresult =  await authorizationService.AuthorizeAsync(User, customPolicy);

            if (authresult.Succeeded)
            {
                return View("Index");
            }

            return View("Index");
        }
    }
}
