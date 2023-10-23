using Api;
using Api.AuthRequirement;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

/* Authentication */
builder.Services.AddAuthentication("DefaultAuth")
    .AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandler>("DefaultAuth", null);

/* Authorization */
builder.Services.AddAuthorization(config =>
{
    var defaultAuthBuilder = new AuthorizationPolicyBuilder();
    var defaultAuthPolicy = defaultAuthBuilder
        .AddRequirements(new JwtRequirement())  // Check Access Token in Authorization.
        .Build();

    config.DefaultPolicy = defaultAuthPolicy;
});

// Scoped
builder.Services.AddScoped<IAuthorizationHandler, JwtRequirementHandler>();

// HttpClient
builder.Services.AddHttpClient().AddHttpContextAccessor();

// Controllers
builder.Services.AddControllers();

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
