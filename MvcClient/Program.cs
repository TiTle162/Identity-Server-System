using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Authentication
builder.Services.AddAuthentication(config =>
{
    config.DefaultScheme = "Cookie";
    config.DefaultChallengeScheme = "oidc";
})
    .AddCookie("Cookie")
    .AddOpenIdConnect("oidc", config =>
    {
        config.Authority = "https://localhost:44305/";
        config.ClientId = "client_id_mvc";
        config.ClientSecret = "client_secret_mvc";
        config.SaveTokens = true;

        // Authorization Code
        config.ResponseType = "code";

        // Configure cookie claim mapping.
        config.ClaimActions.DeleteClaim("amr");
        config.ClaimActions.DeleteClaim("s_hash");
        config.ClaimActions.MapUniqueJsonKey("RawCoding.Grandma", "rc.grandma");

        // Load claim in to cookie
        config.GetClaimsFromUserInfoEndpoint = true;

        // Configure Scope
        config.Scope.Clear();
        config.Scope.Add("openid");
        config.Scope.Add("rc.scope");
        config.Scope.Add("ApiOne");
        config.Scope.Add("ApiTwo");
        config.Scope.Add("offline_access");
    });

// HttpClient
builder.Services.AddHttpClient();

// ControllersWithViews
builder.Services.AddControllersWithViews();

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
