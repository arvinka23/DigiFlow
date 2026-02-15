using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DigitalisierungsManager.Data;
using DigitalisierungsManager.Models;
using DigitalisierungsManager.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Entity Framework - provider selection (Sqlite default for containers)
var dbProvider = Environment.GetEnvironmentVariable("DB_PROVIDER") ?? "Sqlite"; // Sqlite | SqlServer

if (string.Equals(dbProvider, "SqlServer", StringComparison.OrdinalIgnoreCase))
{
    var sqlServer = builder.Configuration.GetConnectionString("SqlServer")
        ?? "Server=(localdb)\\mssqllocaldb;Database=DigitalisierungsManager;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(sqlServer));
}
else
{
    var dataDir = Path.Combine(AppContext.BaseDirectory, "data");
    Directory.CreateDirectory(dataDir);
    var sqlite = builder.Configuration.GetConnectionString("Sqlite")
        ?? $"Data Source={Path.Combine(dataDir, "digiflow.db")}";
    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(sqlite));
}

// ASP.NET Core Identity
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    // Passwort-Anforderungen
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // Benutzer-Einstellungen
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Cookie-Einstellungen
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Login";
    options.LogoutPath = "/Identity/Logout";
    options.AccessDeniedPath = "/Identity/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

// Services
builder.Services.AddScoped<IProjektService, ProjektService>();
builder.Services.AddScoped<IDataExchangeService, DataExchangeService>();
builder.Services.AddScoped<ISqlQueryService, SqlQueryService>();

var app = builder.Build();

// Database seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();

    // Seed roles and admin user first (needed for BesitzerId)
    await SeedData.SeedRolesAndAdminAsync(services);

    // Seed initial data with admin as owner
    if (!dbContext.Projekte.Any())
    {
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var admin = await userManager.FindByEmailAsync("admin@digiflow.ch");
        SeedData.Initialize(dbContext, admin?.Id ?? "");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

// Fuer WebApplicationFactory in Integrationstests erreichbar machen
public partial class Program { }
