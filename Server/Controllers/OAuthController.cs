using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Server.Controllers
{
    public class OAuthController : Controller
    {
        [HttpGet] // Authorization Request
        public IActionResult Authorize(
            string response_type, // authorization flow type 
            string client_id, // client id
            string redirect_uri, // callback url to client
            string scope, // what info I want = email,grandma,tel
            string state) // random string generated to confirm that we are going to back to the same client
        {
            var query = new QueryBuilder();
            query.Add("redirectUri", redirect_uri);
            query.Add("state", state);

            return View(model: query.ToString()); // redirect to user authenticate page.
        }

        [HttpPost] // Authorization Response
        public IActionResult Authorize(
            string username,
            string redirectUri,
            string state)
        {
            // Authorization Code
            const string code = "BABAABABABA";

            var query = new QueryBuilder();
            query.Add("code", code);
            query.Add("state", state);

            return Redirect($"{redirectUri}{query.ToString()}"); // redirect to client callback url
        }
        // Access Token Request
        public async Task<IActionResult> Token(
            string grant_type,
            string code,
            string redirect_uri,
            string client_id,
            string refresh_token)
        {
            // Claim
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "some_id"),
                new Claim("granny", "cookie")
            };

            // Signature
            var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
            var key = new SymmetricSecurityKey(secretBytes);
            
            // Header
            var algorithm = SecurityAlgorithms.HmacSha256;

            // Encrytion
            var signingCredentials = new SigningCredentials(key, algorithm);

            // Token
            var token = new JwtSecurityToken(
                Constants.Issuer,
                Constants.Audiance,
                claims,
                notBefore: DateTime.Now,
                expires: grant_type == "refresh_token"?
                    DateTime.Now.AddMinutes(5) : DateTime.Now.AddMilliseconds(1),
                signingCredentials);

            // Create Token
            var access_token = new JwtSecurityTokenHandler().WriteToken(token);

            return Json(new
            {
                access_token,
                token_type = "Bearer",
                raw_claim = "oauthTutorial",
                refresh_token = "RefreshTokenSampleValueSomething77"
            });
        }

        [Authorize] // Validate Access Token that send from Api server.
        public IActionResult Validate()
        {
            // If Access Token is valid.
            if (HttpContext.Request.Query.TryGetValue("access_token", out var accessToken))
            {
                return Ok();
            }

            // If Access Token is invalid.
            return BadRequest();
        }
    }
}
