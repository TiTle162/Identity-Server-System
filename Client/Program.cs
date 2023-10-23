using Microsoft.AspNetCore.Authentication.OAuth;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

/* Authentication */
builder.Services.AddAuthentication(config => {
    // Check the cookie to confirm that we are authenticate.
    config.DefaultAuthenticateScheme = "ClientCookie";
    // When we sign in we will deal out a cookie.
    config.DefaultSignInScheme = "ClientCookie";
    // Check if we are allowed to do something.
    config.DefaultChallengeScheme = "OurServer";
})
.AddCookie("ClientCookie")
.AddOAuth("OurServer", config => {
    config.ClientId = "client_id";
    config.ClientSecret = "client_secret";
    config.CallbackPath = "/oauth/callback";

    // Redirect to server (Authorization Code Request).
    config.AuthorizationEndpoint = "https://localhost:44382/oauth/authorize";

    // Redirecet to server (Access Token Request). AND when use Refresh Token.
    config.TokenEndpoint = "https://localhost:44382/oauth/token";

    config.SaveTokens = true;

    // Use Access Token's Claim in Client (Decode Access Token).
    config.Events = new OAuthEvents()
    {
        OnCreatingTicket = context =>
        {
            var accessToken = context.AccessToken;
            var base64payload = accessToken.Split('.')[1];
            var bytes = Convert.FromBase64String(base64payload);
            var jsonPayload = Encoding.UTF8.GetString(bytes);
            var claims = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonPayload);

            foreach (var claim in claims)
            {
                context.Identity.AddClaim(new Claim(claim.Key, claim.Value));
            }

            return Task.CompletedTask;
        }
    };
});

// HttpClient
builder.Services.AddHttpClient();

// ControllersWithViews
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication
app.UseAuthentication();

// Authorization
app.UseAuthorization();     

// Endpoints
app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.MapRazorPages();

app.Run();
