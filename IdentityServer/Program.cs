using IdentityServer;
using IdentityServer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Dbcontext 
builder.Services.AddDbContext<AppDbContext>(config =>
{
    config.UseInMemoryDatabase("Memory");
});

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(config =>
{
    config.Password.RequiredLength = 4;
    config.Password.RequireDigit = false;   // number
    config.Password.RequireNonAlphanumeric = false; // symbols
    config.Password.RequireUppercase = false;

    // config.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ConfigureApplicationCookie
builder.Services.ConfigureApplicationCookie(config =>
{
    config.Cookie.Name = "IdentityServer.Cookie";
    config.LoginPath = "/Auth/Login";
});

// IdentityServer
builder.Services.AddIdentityServer()
    .AddAspNetIdentity<IdentityUser>()
    .AddInMemoryApiResources(Configuration.GetApis())
    .AddInMemoryApiScopes(Configuration.Scopes)
    .AddInMemoryIdentityResources(Configuration.GetIdentityResources())
    .AddInMemoryClients(Configuration.GetClients())         
    .AddDeveloperSigningCredential();

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

// IdentityServer
app.UseIdentityServer();

// Endpoints
app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.MapRazorPages();

// Setup user manager.
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<IdentityUser>>();

    // Create user.
    var user = new IdentityUser("bob");
    userManager.CreateAsync(user, "password").GetAwaiter().GetResult();

    // Add Claim (key: value).
    userManager.AddClaimAsync(user, 
        new Claim("rc.grandma", "big.cookie"))
        .GetAwaiter().GetResult();
    userManager.AddClaimAsync(user, 
        new Claim("rc.api.grandma", "big.api.cookie"))
        .GetAwaiter().GetResult();
}

app.Run();
