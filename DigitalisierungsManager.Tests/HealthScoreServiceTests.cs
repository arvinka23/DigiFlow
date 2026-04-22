using DigitalisierungsManager.Models;
using DigitalisierungsManager.Services.Health;

namespace DigitalisierungsManager.Tests;

public class HealthScoreServiceTests
{
    private readonly HealthScoreService _sut = new();

    [Fact]
    public void FullyPopulatedProjekt_HasHighScore()
    {
        var p = new Projekt
        {
            Titel = "Rechnungsworkflow",
            Beschreibung = new string('x', 120),
            Status = ProjektStatus.InBearbeitung,
            ErstelltAm = DateTime.UtcNow.AddDays(-5),
            Technologie = "Power Automate",
            Verantwortlicher = "IT-Team",
            Benutzeranforderungen = new List<Benutzeranforderung>
            {
                new() { Titel = "A1", Beschreibung = "x", Ersteller = "X" }
            },
            Vorschlaege = new List<DigitalisierungsVorschlag>
            {
                new() { Titel = "V1", Beschreibung = "x" }
            }
        };

        var result = _sut.Evaluate(p);

        Assert.True(result.Score >= 90);
        Assert.Empty(result.Findings);
    }

    [Fact]
    public void EmptyProjekt_LosesPointsForMissingDetails()
    {
        var p = new Projekt
        {
            Titel = "Leer",
            Beschreibung = "",
            Status = ProjektStatus.Geplant,
            ErstelltAm = DateTime.UtcNow.AddDays(-1),
            Technologie = "",
            Verantwortlicher = ""
        };

        var result = _sut.Evaluate(p);

        Assert.True(result.Score < 70);
        Assert.Contains(result.Findings, f => f.Category == "Vollstaendigkeit");
        Assert.Contains(result.Findings, f => f.Category == "Klarheit");
    }

    [Fact]
    public void CompletedProjektWithOnlyOpenAnforderungen_IsCritical()
    {
        var p = new Projekt
        {
            Titel = "Falsch abgeschlossen",
            Beschreibung = new string('x', 120),
            Status = ProjektStatus.Abgeschlossen,
            ErstelltAm = DateTime.UtcNow.AddDays(-30),
            Technologie = "X",
            Verantwortlicher = "Team",
            Benutzeranforderungen = new List<Benutzeranforderung>
            {
                new() { Titel = "offen", Beschreibung = "x", Status = AnforderungsStatus.Offen, Ersteller = "X" }
            },
            Vorschlaege = new List<DigitalisierungsVorschlag> { new() { Titel = "v", Beschreibung = "x" } }
        };

        var result = _sut.Evaluate(p);

        Assert.Contains(result.Findings, f => f.Severity == HealthSeverity.Kritisch);
    }

    [Fact]
    public void Score_IsClampedBetween_0_And_100()
    {
        var p = new Projekt
        {
            Titel = "x",
            Beschreibung = "",
            Status = ProjektStatus.Abgeschlossen,
            ErstelltAm = DateTime.UtcNow.AddDays(-400),
            Technologie = "",
            Verantwortlicher = "",
            Benutzeranforderungen = new List<Benutzeranforderung>
            {
                new() { Titel = "x", Beschreibung = "x", Status = AnforderungsStatus.Offen, Ersteller = "X" }
            }
        };

        var result = _sut.Evaluate(p);
        Assert.InRange(result.Score, 0, 100);
    }
}
