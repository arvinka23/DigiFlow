using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DigitalisierungsManager.Data;
using DigitalisierungsManager.Models;

namespace DigitalisierungsManager.Services;

/// <summary>
/// Implementierung des Projekt-Service fuer CRUD-Operationen und Suche.
/// Alle Abfragen sind nach BesitzerId gefiltert -- Daten-Isolation pro User.
/// </summary>
public class ProjektService : IProjektService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProjektService> _logger;

    public ProjektService(ApplicationDbContext context, ILogger<ProjektService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<Projekt>> GetAllProjekteAsync(string userId)
    {
        _logger.LogDebug("Lade alle Projekte fuer User {UserId}", userId);
        return await _context.Projekte
            .Where(p => p.BesitzerId == userId)
            .Include(p => p.Benutzeranforderungen)
            .Include(p => p.Vorschlaege)
            .OrderByDescending(p => p.ErstelltAm)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Projekt?> GetProjektByIdAsync(int id, string userId)
    {
        _logger.LogDebug("Lade Projekt {ProjektId} fuer User {UserId}", id, userId);
        return await _context.Projekte
            .Where(p => p.BesitzerId == userId)
            .Include(p => p.Benutzeranforderungen)
            .Include(p => p.Vorschlaege)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <inheritdoc />
    public async Task<Projekt> CreateProjektAsync(Projekt projekt, string userId)
    {
        projekt.ErstelltAm = DateTime.UtcNow;
        projekt.BesitzerId = userId;
        _context.Projekte.Add(projekt);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Projekt erstellt: {Titel} (Id: {ProjektId}) fuer User {UserId}", projekt.Titel, projekt.Id, userId);
        return projekt;
    }

    /// <inheritdoc />
    public async Task<Projekt> UpdateProjektAsync(Projekt projekt, string userId)
    {
        var existing = await _context.Projekte
            .FirstOrDefaultAsync(p => p.Id == projekt.Id && p.BesitzerId == userId);
        if (existing == null)
        {
            _logger.LogWarning("Projekt {ProjektId} nicht gefunden fuer User {UserId}", projekt.Id, userId);
            throw new InvalidOperationException($"Projekt mit Id {projekt.Id} wurde nicht gefunden.");
        }

        existing.Titel = projekt.Titel;
        existing.Beschreibung = projekt.Beschreibung;
        existing.Status = projekt.Status;
        existing.Technologie = projekt.Technologie;
        existing.Verantwortlicher = projekt.Verantwortlicher;
        existing.Abschlussdatum = projekt.Abschlussdatum;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Projekt aktualisiert: {Titel} (Id: {ProjektId}) fuer User {UserId}", existing.Titel, existing.Id, userId);
        return existing;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteProjektAsync(int id, string userId)
    {
        var projekt = await _context.Projekte
            .FirstOrDefaultAsync(p => p.Id == id && p.BesitzerId == userId);
        if (projekt == null)
        {
            _logger.LogWarning("Projekt {ProjektId} nicht gefunden fuer Loeschung (User {UserId})", id, userId);
            return false;
        }

        _context.Projekte.Remove(projekt);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Projekt geloescht: {Titel} (Id: {ProjektId}) fuer User {UserId}", projekt.Titel, id, userId);
        return true;
    }

    /// <inheritdoc />
    public async Task<List<Projekt>> GetProjekteByStatusAsync(ProjektStatus status, string userId)
    {
        _logger.LogDebug("Lade Projekte mit Status {Status} fuer User {UserId}", status, userId);
        return await _context.Projekte
            .Where(p => p.BesitzerId == userId && p.Status == status)
            .Include(p => p.Benutzeranforderungen)
            .Include(p => p.Vorschlaege)
            .OrderByDescending(p => p.ErstelltAm)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<List<Projekt>> SearchProjekteAsync(string searchTerm, string userId)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllProjekteAsync(userId);

        _logger.LogDebug("Suche Projekte mit Begriff: {SearchTerm} fuer User {UserId}", searchTerm, userId);
        var term = searchTerm.ToLower();
        return await _context.Projekte
            .Where(p => p.BesitzerId == userId &&
                       (p.Titel.ToLower().Contains(term) ||
                        p.Beschreibung.ToLower().Contains(term) ||
                        p.Technologie.ToLower().Contains(term) ||
                        p.Verantwortlicher.ToLower().Contains(term)))
            .Include(p => p.Benutzeranforderungen)
            .Include(p => p.Vorschlaege)
            .OrderByDescending(p => p.ErstelltAm)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<List<Projekt>> CreateProjekteBatchAsync(List<Projekt> projekte, string userId)
    {
        foreach (var projekt in projekte)
        {
            projekt.ErstelltAm = DateTime.UtcNow;
            projekt.BesitzerId = userId;
        }

        _context.Projekte.AddRange(projekte);
        await _context.SaveChangesAsync();
        _logger.LogInformation("{Count} Projekte per Batch-Import erstellt fuer User {UserId}", projekte.Count, userId);
        return projekte;
    }

    /// <inheritdoc />
    public async Task<PaginatedResult<Projekt>> GetProjektePaginatedAsync(
        string userId, int page, int pageSize, ProjektStatus? status = null, string? searchTerm = null)
    {
        _logger.LogDebug("Lade Projekte paginiert fuer User {UserId}: Seite {Page}, Groesse {PageSize}", userId, page, pageSize);

        IQueryable<Projekt> query = _context.Projekte
            .Where(p => p.BesitzerId == userId)
            .Include(p => p.Benutzeranforderungen)
            .Include(p => p.Vorschlaege);

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(p =>
                p.Titel.ToLower().Contains(term) ||
                p.Beschreibung.ToLower().Contains(term) ||
                p.Technologie.ToLower().Contains(term) ||
                p.Verantwortlicher.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.ErstelltAm)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Projekt>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
