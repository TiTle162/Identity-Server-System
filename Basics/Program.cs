using Basics.AuthorizationRequirements;
using Basics.Controllers;
using Basics.CustomerPolicyProvider;
using Basics.Transformer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages().AddRazorPagesOptions(config =>
{
    config.Conventions.AuthorizePage("/Razor/Secured");
    config.Conventions.AuthorizePage("/Razor/Policy", "Admin");

    config.Conventions.AuthorizePage("/RazorSecured");
});

/* authentication */
builder.Services.AddAuthentication("CookieAuth")
.AddCookie("CookieAuth", config =>
{
    config.LoginPath = "/Home/Authenticate";
    config.Cookie.Name = "Title.Cookie";
});

/* authorization */
builder.Services.AddAuthorization(config =>
{
    //var defaultAuthBuilder = new AuthorizationPolicyBuilder();
    //var defaultAuthPolicy = defaultAuthBuilder
    //.RequireAuthenticatedUser()
    //.RequireClaim(ClaimTypes.DateOfBirth)
    //.Build();

    //config.DefaultPolicy = defaultAuthPolicy;

    //config.AddPolicy("Claim.DoB", policyBuilder =>
    //{
    //    policyBuilder.RequireClaim(ClaimTypes.DateOfBirth)
    //});

    config.AddPolicy("Admin", policyBuilder => policyBuilder.RequireClaim(ClaimTypes.Role, "Admin"));

    config.AddPolicy("Claim.DoB", policyBuilder =>
    {
        policyBuilder.RequireCustomClaim(ClaimTypes.DateOfBirth);
    });
});
builder.Services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, SecurityLevelHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CookieJarAuthorizationHandler>();
builder.Services.AddScoped<IClaimsTransformation, ClaimsTranformation>();

/* Page Route*/
builder.Services.AddControllersWithViews(config =>
{
    var defaultAuthBuilder = new AuthorizationPolicyBuilder();
    var defaultAuthPolicy = defaultAuthBuilder
    .RequireAuthenticatedUser()
    .RequireClaim(ClaimTypes.DateOfBirth)
    .Build();

    // global authorization filter
    //config.Filters.Add(new AuthorizeFilter(defaultAuthPolicy));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// who are you?
app.UseAuthentication();
// are you allowed?
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}"
);

app.MapRazorPages();

app.Run();
