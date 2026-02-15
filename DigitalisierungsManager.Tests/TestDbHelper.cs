using Microsoft.EntityFrameworkCore;
using DigitalisierungsManager.Data;

namespace DigitalisierungsManager.Tests;

/// <summary>
/// Hilfsklasse zum Erstellen von InMemory-Datenbankkontexten fuer Tests.
/// </summary>
public static class TestDbHelper
{
    /// <summary>
    /// Erstellt einen frischen InMemory-ApplicationDbContext fuer einen einzelnen Test.
    /// Jeder Aufruf erzeugt eine neue, isolierte Datenbank.
    /// </summary>
    public static ApplicationDbContext CreateContext(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName ?? Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
