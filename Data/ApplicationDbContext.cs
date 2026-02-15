using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DigitalisierungsManager.Models;

namespace DigitalisierungsManager.Data;

/// <summary>
/// Entity Framework Datenbankkontext fuer die DigiFlow-Anwendung.
/// Erbt von IdentityDbContext fuer Benutzer- und Rollenverwaltung.
/// Verwaltet Projekte, Benutzeranforderungen und Digitalisierungsvorschlaege.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>Tabelle der Digitalisierungsprojekte.</summary>
    public DbSet<Projekt> Projekte { get; set; }

    /// <summary>Tabelle der Benutzeranforderungen.</summary>
    public DbSet<Benutzeranforderung> Benutzeranforderungen { get; set; }

    /// <summary>Tabelle der Digitalisierungsvorschlaege.</summary>
    public DbSet<DigitalisierungsVorschlag> DigitalisierungsVorschlaege { get; set; }

    /// <summary>
    /// Konfiguriert das Datenbankschema via Fluent API.
    /// Enum-Werte werden als Strings gespeichert fuer bessere Lesbarkeit.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Projekt>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titel).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Beschreibung).HasMaxLength(2000);
            entity.Property(e => e.Status).HasConversion<string>();
            entity.Property(e => e.Technologie).HasMaxLength(100);
            entity.Property(e => e.BesitzerId).IsRequired();
            entity.HasIndex(e => e.BesitzerId);
            entity.HasOne(e => e.Besitzer)
                  .WithMany()
                  .HasForeignKey(e => e.BesitzerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Benutzeranforderung>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titel).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Beschreibung).HasMaxLength(2000);
            entity.Property(e => e.Prioritaet).HasConversion<string>();
            entity.Property(e => e.Status).HasConversion<string>();
            entity.HasOne(e => e.Projekt)
                  .WithMany(p => p.Benutzeranforderungen)
                  .HasForeignKey(e => e.ProjektId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DigitalisierungsVorschlag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titel).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Beschreibung).HasMaxLength(2000);
            entity.Property(e => e.Vorschlagstyp).HasConversion<string>();
            entity.HasOne(e => e.Projekt)
                  .WithMany(p => p.Vorschlaege)
                  .HasForeignKey(e => e.ProjektId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
