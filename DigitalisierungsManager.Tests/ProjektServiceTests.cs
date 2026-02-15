using Microsoft.Extensions.Logging.Abstractions;
using DigitalisierungsManager.Models;
using DigitalisierungsManager.Services;

namespace DigitalisierungsManager.Tests;

/// <summary>
/// Unit-Tests fuer den ProjektService (CRUD, Suche, Filterung, Batch).
/// Alle Tests verwenden eine feste Test-User-ID fuer die Daten-Isolation.
/// </summary>
public class ProjektServiceTests
{
    private const string TestUserId = "test-user-id-123";
    private const string OtherUserId = "other-user-id-456";

    private ProjektService CreateService()
    {
        var context = TestDbHelper.CreateContext();
        return new ProjektService(context, NullLogger<ProjektService>.Instance);
    }

    [Fact]
    public async Task CreateProjektAsync_ShouldAddProjekt()
    {
        var service = CreateService();
        var projekt = new Projekt
        {
            Titel = "Testprojekt",
            Beschreibung = "Beschreibung",
            Technologie = "C#",
            Verantwortlicher = "Tester"
        };

        var result = await service.CreateProjektAsync(projekt, TestUserId);

        Assert.True(result.Id > 0);
        Assert.Equal("Testprojekt", result.Titel);
        Assert.Equal(TestUserId, result.BesitzerId);
    }

    [Fact]
    public async Task GetAllProjekteAsync_ShouldReturnOnlyOwnProjekte()
    {
        var service = CreateService();
        await service.CreateProjektAsync(new Projekt { Titel = "Mein Projekt", Verantwortlicher = "A" }, TestUserId);
        await service.CreateProjektAsync(new Projekt { Titel = "Anderer Projekt", Verantwortlicher = "B" }, OtherUserId);

        var result = await service.GetAllProjekteAsync(TestUserId);

        Assert.Single(result);
        Assert.Equal("Mein Projekt", result[0].Titel);
    }

    [Fact]
    public async Task GetProjektByIdAsync_ShouldReturnCorrectProjekt()
    {
        var service = CreateService();
        var created = await service.CreateProjektAsync(new Projekt { Titel = "Findbar", Verantwortlicher = "Tester" }, TestUserId);

        var result = await service.GetProjektByIdAsync(created.Id, TestUserId);

        Assert.NotNull(result);
        Assert.Equal("Findbar", result.Titel);
    }

    [Fact]
    public async Task GetProjektByIdAsync_ShouldReturnNullForOtherUser()
    {
        var service = CreateService();
        var created = await service.CreateProjektAsync(new Projekt { Titel = "Geheim", Verantwortlicher = "T" }, OtherUserId);

        var result = await service.GetProjektByIdAsync(created.Id, TestUserId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetProjektByIdAsync_ShouldReturnNullForInvalidId()
    {
        var service = CreateService();

        var result = await service.GetProjektByIdAsync(999, TestUserId);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateProjektAsync_ShouldUpdateFields()
    {
        var service = CreateService();
        var created = await service.CreateProjektAsync(new Projekt
        {
            Titel = "Original",
            Beschreibung = "Alt",
            Verantwortlicher = "Alt"
        }, TestUserId);

        var updated = await service.UpdateProjektAsync(new Projekt
        {
            Id = created.Id,
            Titel = "Aktualisiert",
            Beschreibung = "Neu",
            Technologie = "Blazor",
            Status = ProjektStatus.InBearbeitung,
            Verantwortlicher = "Neu"
        }, TestUserId);

        Assert.Equal("Aktualisiert", updated.Titel);
        Assert.Equal("Neu", updated.Beschreibung);
        Assert.Equal(ProjektStatus.InBearbeitung, updated.Status);
    }

    [Fact]
    public async Task UpdateProjektAsync_ShouldThrowForOtherUser()
    {
        var service = CreateService();
        var created = await service.CreateProjektAsync(new Projekt { Titel = "X", Verantwortlicher = "X" }, OtherUserId);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.UpdateProjektAsync(new Projekt { Id = created.Id, Titel = "X", Verantwortlicher = "X" }, TestUserId));
    }

    [Fact]
    public async Task UpdateProjektAsync_ShouldThrowForInvalidId()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.UpdateProjektAsync(new Projekt { Id = 999, Titel = "X", Verantwortlicher = "X" }, TestUserId));
    }

    [Fact]
    public async Task DeleteProjektAsync_ShouldRemoveProjekt()
    {
        var service = CreateService();
        var created = await service.CreateProjektAsync(new Projekt { Titel = "Loeschbar", Verantwortlicher = "T" }, TestUserId);

        var deleted = await service.DeleteProjektAsync(created.Id, TestUserId);
        var all = await service.GetAllProjekteAsync(TestUserId);

        Assert.True(deleted);
        Assert.Empty(all);
    }

    [Fact]
    public async Task DeleteProjektAsync_ShouldReturnFalseForOtherUser()
    {
        var service = CreateService();
        var created = await service.CreateProjektAsync(new Projekt { Titel = "Anderer", Verantwortlicher = "T" }, OtherUserId);

        var result = await service.DeleteProjektAsync(created.Id, TestUserId);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteProjektAsync_ShouldReturnFalseForInvalidId()
    {
        var service = CreateService();

        var result = await service.DeleteProjektAsync(999, TestUserId);

        Assert.False(result);
    }

    [Fact]
    public async Task GetProjekteByStatusAsync_ShouldFilterCorrectly()
    {
        var service = CreateService();
        await service.CreateProjektAsync(new Projekt { Titel = "Geplant", Status = ProjektStatus.Geplant, Verantwortlicher = "T" }, TestUserId);
        await service.CreateProjektAsync(new Projekt { Titel = "In Arbeit", Status = ProjektStatus.InBearbeitung, Verantwortlicher = "T" }, TestUserId);
        await service.CreateProjektAsync(new Projekt { Titel = "Anderer Geplant", Status = ProjektStatus.Geplant, Verantwortlicher = "T" }, OtherUserId);

        var geplant = await service.GetProjekteByStatusAsync(ProjektStatus.Geplant, TestUserId);

        Assert.Single(geplant);
        Assert.Equal("Geplant", geplant[0].Titel);
    }

    [Fact]
    public async Task SearchProjekteAsync_ShouldFindByTitel()
    {
        var service = CreateService();
        await service.CreateProjektAsync(new Projekt { Titel = "Blazor Dashboard", Verantwortlicher = "T" }, TestUserId);
        await service.CreateProjektAsync(new Projekt { Titel = "PowerShell Script", Verantwortlicher = "T" }, TestUserId);
        await service.CreateProjektAsync(new Projekt { Titel = "Blazor Anderer", Verantwortlicher = "T" }, OtherUserId);

        var result = await service.SearchProjekteAsync("blazor", TestUserId);

        Assert.Single(result);
        Assert.Equal("Blazor Dashboard", result[0].Titel);
    }

    [Fact]
    public async Task SearchProjekteAsync_EmptyTerm_ShouldReturnAllOwn()
    {
        var service = CreateService();
        await service.CreateProjektAsync(new Projekt { Titel = "A", Verantwortlicher = "T" }, TestUserId);
        await service.CreateProjektAsync(new Projekt { Titel = "B", Verantwortlicher = "T" }, TestUserId);
        await service.CreateProjektAsync(new Projekt { Titel = "C", Verantwortlicher = "T" }, OtherUserId);

        var result = await service.SearchProjekteAsync("", TestUserId);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task CreateProjekteBatchAsync_ShouldAddMultipleProjekte()
    {
        var service = CreateService();
        var projekte = new List<Projekt>
        {
            new Projekt { Titel = "Batch 1", Verantwortlicher = "T" },
            new Projekt { Titel = "Batch 2", Verantwortlicher = "T" },
            new Projekt { Titel = "Batch 3", Verantwortlicher = "T" }
        };

        var result = await service.CreateProjekteBatchAsync(projekte, TestUserId);
        var all = await service.GetAllProjekteAsync(TestUserId);

        Assert.Equal(3, result.Count);
        Assert.Equal(3, all.Count);
        Assert.All(result, p => Assert.Equal(TestUserId, p.BesitzerId));
    }

    [Fact]
    public async Task CreateProjektAsync_ShouldSetErstelltAmToUtc()
    {
        var service = CreateService();
        var before = DateTime.UtcNow;

        var result = await service.CreateProjektAsync(new Projekt { Titel = "Zeittest", Verantwortlicher = "T" }, TestUserId);
        var after = DateTime.UtcNow;

        Assert.InRange(result.ErstelltAm, before, after);
    }
}
