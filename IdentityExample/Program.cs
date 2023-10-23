using IdentityExample.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

//builder.Services.AddAuthentication("CookieAuth")
//.AddCookie("CookieAuth", config =>
//{
//    config.LoginPath = "/Home/Authenticate";
//    config.Cookie.Name = "Title.Cookie";
//});

/* Dbcontext */
builder.Services.AddDbContext<AppDbContext>(config =>
{
    config.UseInMemoryDatabase("Memory");
});

/* Identity */
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

/* cookie */
builder.Services.ConfigureApplicationCookie(config =>
{
    config.LoginPath = "/Home/Login";
    config.Cookie.Name = "Identity.Cookie";
});

/* mailkit */
var provider = builder.Services.BuildServiceProvider();
var _config = provider.GetRequiredService<IConfiguration>();
builder.Services.AddMailKit(config => config.UseMailKit(_config.GetSection("Email").Get<MailKitOptions>()));

// page route
builder.Services.AddControllersWithViews();

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

// default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}"
);

app.MapRazorPages();

app.Run();
