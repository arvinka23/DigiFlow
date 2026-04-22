using DigitalisierungsManager.Models;
using DigitalisierungsManager.Services;
using DigitalisierungsManager.Services.Search;

namespace DigitalisierungsManager.Tests;

public class SemanticSearchServiceTests
{
    private sealed class FakeProjektService : IProjektService
    {
        public List<Projekt> Data { get; set; } = new();
        public Task<List<Projekt>> GetAllProjekteAsync(string userId) => Task.FromResult(Data);
        public Task<Projekt?> GetProjektByIdAsync(int id, string userId) => throw new NotImplementedException();
        public Task<Projekt> CreateProjektAsync(Projekt projekt, string userId) => throw new NotImplementedException();
        public Task<Projekt> UpdateProjektAsync(Projekt projekt, string userId) => throw new NotImplementedException();
        public Task<bool> DeleteProjektAsync(int id, string userId) => throw new NotImplementedException();
        public Task<List<Projekt>> GetProjekteByStatusAsync(ProjektStatus status, string userId) => throw new NotImplementedException();
        public Task<List<Projekt>> SearchProjekteAsync(string searchTerm, string userId) => throw new NotImplementedException();
        public Task<List<Projekt>> CreateProjekteBatchAsync(List<Projekt> projekte, string userId) => throw new NotImplementedException();
        public Task<PaginatedResult<Projekt>> GetProjektePaginatedAsync(string userId, int page, int pageSize, ProjektStatus? status = null, string? searchTerm = null) => throw new NotImplementedException();
    }

    [Fact]
    public async Task SynonymMatch_FindsInvoiceWhenSearchingRechnung()
    {
        var fake = new FakeProjektService
        {
            Data = new()
            {
                new() { Id = 1, Titel = "Invoice OCR", Beschreibung = "Automatische Invoice-Extraktion", Technologie = "Azure" },
                new() { Id = 2, Titel = "Urlaubsantrag", Beschreibung = "Power Apps Formular", Technologie = "Power Platform" },
                new() { Id = 3, Titel = "Reporting", Beschreibung = "PowerBI Dashboard", Technologie = "PowerBI" },
            }
        };
        var sut = new SemanticSearchService(fake);

        var results = await sut.SearchAsync("Rechnungsworkflow", "user1");

        Assert.NotEmpty(results);
        Assert.Equal(1, results[0].Projekt.Id);
    }

    [Fact]
    public async Task FuzzyMatch_HandlesTypo()
    {
        var fake = new FakeProjektService
        {
            Data = new() { new() { Id = 1, Titel = "Urlaubsantrag", Beschreibung = "", Technologie = "" } }
        };
        var sut = new SemanticSearchService(fake);

        var results = await sut.SearchAsync("Urlaubsantreg", "user1");

        Assert.Single(results);
    }

    [Fact]
    public async Task EmptyQuery_ReturnsEmpty()
    {
        var sut = new SemanticSearchService(new FakeProjektService());
        var results = await sut.SearchAsync("", "user1");
        Assert.Empty(results);
    }
}
