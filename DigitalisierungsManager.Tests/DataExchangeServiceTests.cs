using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using DigitalisierungsManager.Models;
using DigitalisierungsManager.Services;

namespace DigitalisierungsManager.Tests;

/// <summary>
/// Unit-Tests fuer den DataExchangeService (JSON, CSV, XML Export/Import).
/// </summary>
public class DataExchangeServiceTests
{
    private readonly DataExchangeService _service = new(NullLogger<DataExchangeService>.Instance);

    private List<Projekt> CreateSampleProjekte() => new()
    {
        new Projekt
        {
            Id = 1,
            Titel = "Testprojekt",
            Beschreibung = "Beschreibung",
            Status = ProjektStatus.InBearbeitung,
            Technologie = "C#",
            Verantwortlicher = "Tester",
            ErstelltAm = new DateTime(2024, 6, 15, 10, 0, 0, DateTimeKind.Utc)
        },
        new Projekt
        {
            Id = 2,
            Titel = "Zweites Projekt",
            Beschreibung = "Noch eine Beschreibung",
            Status = ProjektStatus.Geplant,
            Technologie = "PowerShell",
            Verantwortlicher = "Admin",
            ErstelltAm = new DateTime(2024, 7, 1, 12, 0, 0, DateTimeKind.Utc)
        }
    };

    // --- JSON Tests ---

    [Fact]
    public void ExportProjekteToJson_ShouldReturnValidJson()
    {
        // Arrange
        var projekte = CreateSampleProjekte();

        // Act
        var json = _service.ExportProjekteToJson(projekte);

        // Assert
        Assert.NotNull(json);
        Assert.Contains("Testprojekt", json);
        Assert.Contains("Zweites Projekt", json);
        Assert.Contains("InBearbeitung", json); // Enum als String serialisiert
    }

    [Fact]
    public void ImportProjekteFromJson_ShouldDeserializeCorrectly()
    {
        // Arrange
        var projekte = CreateSampleProjekte();
        var json = _service.ExportProjekteToJson(projekte);

        // Act
        var imported = _service.ImportProjekteFromJson(json);

        // Assert
        Assert.Equal(2, imported.Count);
        Assert.Equal("Testprojekt", imported[0].Titel);
        Assert.Equal(ProjektStatus.InBearbeitung, imported[0].Status);
    }

    [Fact]
    public void ImportProjekteFromJson_EmptyArray_ShouldReturnEmptyList()
    {
        // Act
        var result = _service.ImportProjekteFromJson("[]");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void ExportAndImport_Json_ShouldBeRoundTrip()
    {
        // Arrange
        var original = CreateSampleProjekte();

        // Act
        var json = _service.ExportProjekteToJson(original);
        var reimported = _service.ImportProjekteFromJson(json);

        // Assert
        Assert.Equal(original.Count, reimported.Count);
        for (int i = 0; i < original.Count; i++)
        {
            Assert.Equal(original[i].Titel, reimported[i].Titel);
            Assert.Equal(original[i].Status, reimported[i].Status);
            Assert.Equal(original[i].Technologie, reimported[i].Technologie);
        }
    }

    // --- CSV Tests ---

    [Fact]
    public void ExportProjekteToCsv_ShouldReturnValidCsv()
    {
        // Arrange
        var projekte = CreateSampleProjekte();

        // Act
        var csvBytes = _service.ExportProjekteToCsv(projekte);
        var csvString = Encoding.UTF8.GetString(csvBytes);

        // Assert
        Assert.NotNull(csvString);
        Assert.Contains("Testprojekt", csvString);
        Assert.Contains(";", csvString); // Semikolon-getrennt
    }

    [Fact]
    public async Task ImportProjekteFromCsvAsync_ShouldParseCorrectly()
    {
        // Arrange
        var projekte = CreateSampleProjekte();
        var csvBytes = _service.ExportProjekteToCsv(projekte);
        using var stream = new MemoryStream(csvBytes);

        // Act
        var imported = await _service.ImportProjekteFromCsvAsync(stream);

        // Assert
        Assert.Equal(2, imported.Count);
        Assert.Equal("Testprojekt", imported[0].Titel);
    }

    // --- XML Tests ---

    [Fact]
    public void ExportToXml_ShouldReturnValidXml()
    {
        // Arrange
        var projekte = CreateSampleProjekte();

        // Act
        var xml = _service.ExportToXml(projekte);

        // Assert
        Assert.NotNull(xml);
        Assert.Contains("<Projekte>", xml);
        Assert.Contains("<Titel>Testprojekt</Titel>", xml);
        Assert.Contains("<Status>InBearbeitung</Status>", xml);
    }

    [Fact]
    public void ExportToXml_EmptyList_ShouldReturnEmptyRoot()
    {
        // Act
        var xml = _service.ExportToXml(new List<Projekt>());

        // Assert
        Assert.Contains("<Projekte />", xml);
    }
}
