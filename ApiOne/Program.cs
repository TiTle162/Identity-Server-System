var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", config => {
        config.Authority = "https://localhost:44305/"; // Tell ApiOne to pass Access Token.
        config.Audience = "ApiOne";
    });

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
    endpoints.MapControllers();
});

app.MapRazorPages();

app.Run();
