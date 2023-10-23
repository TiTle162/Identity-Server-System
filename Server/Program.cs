using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Server;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

/* Authentication */
builder.Services.AddAuthentication("OAuth")
.AddJwtBearer("OAuth", config =>
{
    var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
    var key = new SymmetricSecurityKey(secretBytes);

    // Validation Access Token (via URL)
    config.Events = new JwtBearerEvents()
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Query.ContainsKey("access_token"))
            {
                context.Token = context.Request.Query["access_token"];
            }

            return Task.CompletedTask;
        }
    };

    // Validation Access Token (via Header)
    config.TokenValidationParameters = new TokenValidationParameters()
    {
        // Test Refresh Token when Access Token expire.
        ClockSkew = TimeSpan.Zero,

        ValidIssuer = Constants.Issuer,
        ValidAudience = Constants.Audiance,
        IssuerSigningKey = key,
    };
});

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
