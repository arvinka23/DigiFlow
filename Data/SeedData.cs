using Microsoft.AspNetCore.Identity;
using DigitalisierungsManager.Models;

namespace DigitalisierungsManager.Data;

/// <summary>
/// Stellt Beispieldaten fuer die Erstinitialisierung der Datenbank bereit.
/// </summary>
public static class SeedData
{
    /// <summary>
    /// Fuegt Beispielprojekte, Benutzeranforderungen und Digitalisierungsvorschlaege ein,
    /// sofern die Datenbank noch leer ist. Projekte werden dem angegebenen Benutzer zugewiesen.
    /// </summary>
    public static void Initialize(ApplicationDbContext context, string besitzerId = "")
    {
        if (context.Projekte.Any())
            return;

        var projekte = new List<Projekt>
        {
            new Projekt
            {
                Titel = "Blazor Dashboard für Projektmanagement",
                Beschreibung = "Entwicklung eines modernen Dashboards zur Verwaltung von Digitalisierungsprojekten mit Blazor Server und Entity Framework.",
                Status = ProjektStatus.InBearbeitung,
                Technologie = "C# .NET 8, Blazor Server, Entity Framework Core, MS SQL Server",
                Verantwortlicher = "IT-Team",
                ErstelltAm = DateTime.UtcNow.AddDays(-30),
                BesitzerId = besitzerId
            },
            new Projekt
            {
                Titel = "PowerShell Automatisierungs-Scripts",
                Beschreibung = "Sammlung von PowerShell-Scripts zur Automatisierung von IT-Prozessen und Deployment-Vorgängen.",
                Status = ProjektStatus.Geplant,
                Technologie = "PowerShell 7.x",
                Verantwortlicher = "IT-Team",
                ErstelltAm = DateTime.UtcNow.AddDays(-20),
                BesitzerId = besitzerId
            },
            new Projekt
            {
                Titel = "Excel VBA Datenimport-Tool",
                Beschreibung = "VBA-Tool zum automatisierten Import von Daten aus verschiedenen Quellen in Excel-Tabellen.",
                Status = ProjektStatus.Geplant,
                Technologie = "VBA, Excel",
                Verantwortlicher = "IT-Team",
                ErstelltAm = DateTime.UtcNow.AddDays(-15),
                BesitzerId = besitzerId
            },
            new Projekt
            {
                Titel = "SharePoint Online Integration",
                Beschreibung = "Integration von internen Applikationen mit SharePoint Online für Dokumentenverwaltung und Collaboration.",
                Status = ProjektStatus.InReview,
                Technologie = "C# .NET Core, SharePoint Online REST API",
                Verantwortlicher = "IT-Team",
                ErstelltAm = DateTime.UtcNow.AddDays(-45),
                BesitzerId = besitzerId
            }
        };

        context.Projekte.AddRange(projekte);
        context.SaveChanges();

        // Beispiel Benutzeranforderungen
        var anforderungen = new List<Benutzeranforderung>
        {
            new Benutzeranforderung
            {
                Titel = "Export-Funktion für Projektdaten",
                Beschreibung = "Benutzer möchten Projektdaten als CSV oder JSON exportieren können.",
                Prioritaet = Prioritaet.Hoch,
                Status = AnforderungsStatus.InBearbeitung,
                Ersteller = "Max Mustermann",
                ProjektId = projekte[0].Id,
                ErstelltAm = DateTime.UtcNow.AddDays(-10)
            },
            new Benutzeranforderung
            {
                Titel = "Berechtigungsverwaltung",
                Beschreibung = "Rollenbasierte Zugriffskontrolle für verschiedene Projektbereiche.",
                Prioritaet = Prioritaet.Mittel,
                Status = AnforderungsStatus.Offen,
                Ersteller = "Anna Schmidt",
                ProjektId = projekte[0].Id,
                ErstelltAm = DateTime.UtcNow.AddDays(-5)
            }
        };

        context.Benutzeranforderungen.AddRange(anforderungen);
        context.SaveChanges();

        // Beispiel Digitalisierungsvorschläge
        var vorschlaege = new List<DigitalisierungsVorschlag>
        {
            new DigitalisierungsVorschlag
            {
                Titel = "Migration zu .NET 8",
                Beschreibung = "Vorschlag zur Migration bestehender .NET Framework Applikationen auf .NET 8 für bessere Performance und moderne Features.",
                Vorschlagstyp = Vorschlagstyp.Eigenentwicklung,
                Begruendung = "Langfristige Wartbarkeit und Performance-Verbesserungen",
                ProjektId = projekte[0].Id,
                ErstelltAm = DateTime.UtcNow.AddDays(-20),
                IstAngenommen = true
            },
            new DigitalisierungsVorschlag
            {
                Titel = "SaaS Lösung für CRM",
                Beschreibung = "Evaluierung von Fremd-Software für Customer Relationship Management.",
                Vorschlagstyp = Vorschlagstyp.FremdSoftware,
                Begruendung = "Schnellere Implementierung und professionelle Features",
                ProjektId = projekte[3].Id,
                ErstelltAm = DateTime.UtcNow.AddDays(-30),
                IstAngenommen = false
            }
        };

        context.DigitalisierungsVorschlaege.AddRange(vorschlaege);
        context.SaveChanges();
    }

    /// <summary>
    /// Erstellt die Rollen "Admin" und "Benutzer" sowie einen Standard-Admin-Account,
    /// sofern diese noch nicht existieren.
    /// </summary>
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

        // Rollen erstellen
        string[] roles = { "Admin", "Benutzer" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Standard-Admin erstellen
        const string adminEmail = "admin@digiflow.ch";
        const string adminPassword = "Admin123!";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                Anzeigename = "Administrator",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
