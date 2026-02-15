using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DigitalisierungsManager.Data;

namespace DigitalisierungsManager.Tests;

/// <summary>
/// Benutzerdefinierte WebApplicationFactory fuer Integrationstests.
/// Verwendet eine InMemory-Datenbank statt der produktiven Datenbank.
/// Identity-Seeding (Rollen, Admin) wird von Program.cs automatisch ausgefuehrt.
/// </summary>
public class DigiFlowWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            // Bestehende DbContext-Registrierung entfernen
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // InMemory-Datenbank fuer Tests verwenden
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("IntegrationTestDb_" + Guid.NewGuid().ToString());
            });
        });
    }
}
