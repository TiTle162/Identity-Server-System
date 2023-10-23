using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize] // Receive Access Token From Server
        public async Task<IActionResult> Secret()
        {
            /* Try to access Server. */
            var serverResponse = await AccessTokenRefreshWrapper(
                () => SecuredGetRequest("https://localhost:44382/secret/index"));

            /* Try to access Api. */
            var apiResponse = await AccessTokenRefreshWrapper(
                () => SecuredGetRequest("https://localhost:44332/secret/index"));

            return View();
        }

        private async Task<HttpResponseMessage> SecuredGetRequest(string url)
        {
            // Client Get Access Token
            var token = await HttpContext.GetTokenAsync("access_token");
            
            // Pass The Access Token 
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            // return URL with Access Token.
            return await client.GetAsync(url);
        }

        public async Task<HttpResponseMessage> AccessTokenRefreshWrapper(
            Func<Task<HttpResponseMessage>> initialRequest)
        {
            // Request to Server and get response from Server.
            var response = await initialRequest();      // 400

            // If Access Token was expired.
            if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Request new Access Token.
                await RefreshAccessToken();
                response = await initialRequest();      // 200
            }

            // If Access Token is valid.
            return response;                            // 200
        }

        // Get new Access Token when Access Token was expired.
        private async Task RefreshAccessToken()
        {
            // Client Get Refresh Token.
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

            var refreshTokenClient = _httpClientFactory.CreateClient();

            // Request data for get new Access Token.
            var requestData = new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken
            };

            // URL to get new Acces Token.
            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:44382/oauth/token")
            {
                Content = new FormUrlEncodedContent(requestData)
            };

            // Encode Refresh Token.
            var basicCredentials = "username:password";
            var encodedCredentials = Encoding.UTF8.GetBytes(basicCredentials);
            var base64Credentials = Convert.ToBase64String(encodedCredentials);

            // Pass The Refresh Token.
            request.Headers.Add("Authorization", $"Basic {base64Credentials}");

            // Get new Access Token.
            var response = await refreshTokenClient.SendAsync(request);

            // Decode new Access Token.
            var responseString = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);

            // Set access_token & refresh_token instead of the same (In ClientCookie). 
            var newAccessToken = responseData.GetValueOrDefault("access_token");
            var newRefreshToken = responseData.GetValueOrDefault("refresh_token");
            var authInfo = await HttpContext.AuthenticateAsync("ClientCookie");
            authInfo.Properties.UpdateTokenValue("access_token", newAccessToken);
            authInfo.Properties.UpdateTokenValue("refresh_token", newRefreshToken);
            await HttpContext.SignInAsync("ClientCookie", authInfo.Principal, authInfo.Properties);
        }
    }
}
