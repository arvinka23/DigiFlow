using DigitalisierungsManager.Models;

namespace DigitalisierungsManager.Services;

/// <summary>
/// Service-Interface fuer Projekt-CRUD-Operationen, Suche und Filterung.
/// Alle Methoden sind benutzerbezogen -- jeder User sieht nur seine eigenen Projekte.
/// </summary>
public interface IProjektService
{
    /// <summary>Gibt alle Projekte des Benutzers inkl. Anforderungen und Vorschlaege zurueck.</summary>
    Task<List<Projekt>> GetAllProjekteAsync(string userId);

    /// <summary>Gibt ein einzelnes Projekt anhand seiner ID zurueck (nur wenn es dem User gehoert).</summary>
    Task<Projekt?> GetProjektByIdAsync(int id, string userId);

    /// <summary>Erstellt ein neues Projekt fuer den angegebenen Benutzer.</summary>
    Task<Projekt> CreateProjektAsync(Projekt projekt, string userId);

    /// <summary>Aktualisiert ein bestehendes Projekt (nur wenn es dem User gehoert).</summary>
    Task<Projekt> UpdateProjektAsync(Projekt projekt, string userId);

    /// <summary>Loescht ein Projekt anhand seiner ID (nur wenn es dem User gehoert).</summary>
    Task<bool> DeleteProjektAsync(int id, string userId);

    /// <summary>Gibt Projekte des Benutzers mit einem bestimmten Status zurueck.</summary>
    Task<List<Projekt>> GetProjekteByStatusAsync(ProjektStatus status, string userId);

    /// <summary>Durchsucht Projekte des Benutzers nach einem Suchbegriff.</summary>
    Task<List<Projekt>> SearchProjekteAsync(string searchTerm, string userId);

    /// <summary>Erstellt mehrere Projekte in einer Batch-Operation fuer den Benutzer.</summary>
    Task<List<Projekt>> CreateProjekteBatchAsync(List<Projekt> projekte, string userId);

    /// <summary>
    /// Gibt Projekte des Benutzers paginiert zurueck, optional gefiltert nach Status und/oder Suchbegriff.
    /// </summary>
    Task<PaginatedResult<Projekt>> GetProjektePaginatedAsync(string userId, int page, int pageSize, ProjektStatus? status = null, string? searchTerm = null);
}
