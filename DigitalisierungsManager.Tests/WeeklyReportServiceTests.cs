using DigitalisierungsManager.Models;
using DigitalisierungsManager.Services;
using DigitalisierungsManager.Services.Health;
using DigitalisierungsManager.Services.Reports;

namespace DigitalisierungsManager.Tests;

public class WeeklyReportServiceTests
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
    public async Task Generate_IncludesKpisAndSummary()
    {
        var now = DateTime.UtcNow;
        var fake = new FakeProjektService
        {
            Data = new()
            {
                new Projekt
                {
                    Id = 1, Titel = "Neu diese Woche", Beschreibung = new string('x', 80),
                    Technologie = "X", Verantwortlicher = "Team",
                    Status = ProjektStatus.InBearbeitung, ErstelltAm = now.AddDays(-2),
                    Benutzeranforderungen = { new() { Titel = "a", Beschreibung = "x", Ersteller = "X" } },
                    Vorschlaege = { new() { Titel = "v", Beschreibung = "x" } }
                },
                new Projekt
                {
                    Id = 2, Titel = "Alt und pausiert", Beschreibung = "",
                    Status = ProjektStatus.Pausiert, ErstelltAm = now.AddDays(-200)
                }
            }
        };
        var sut = new WeeklyReportService(fake, new HealthScoreService());

        var result = await sut.GenerateAsync("user1");

        Assert.Equal(2, result.Data.GesamtProjekte);
        Assert.Equal(1, result.Data.NeueProjekte);
        Assert.Contains("Wochenreport", result.Markdown);
        Assert.Contains("Kennzahlen", result.Markdown);
        Assert.Contains("Status-Verteilung", result.Markdown);
        Assert.True(result.Data.Handlungsbedarf.Any(h => h.Projekt.Id == 2));
    }
}
