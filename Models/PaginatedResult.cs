namespace DigitalisierungsManager.Models;

/// <summary>
/// Generisches Ergebnis-Modell fuer paginierte Abfragen.
/// </summary>
/// <typeparam name="T">Der Typ der paginierten Elemente.</typeparam>
public class PaginatedResult<T>
{
    /// <summary>Die Elemente der aktuellen Seite.</summary>
    public List<T> Items { get; set; } = new();

    /// <summary>Gesamtanzahl aller Elemente (ueber alle Seiten).</summary>
    public int TotalCount { get; set; }

    /// <summary>Aktuelle Seitennummer (1-basiert).</summary>
    public int Page { get; set; }

    /// <summary>Anzahl Elemente pro Seite.</summary>
    public int PageSize { get; set; }

    /// <summary>Gesamtanzahl der Seiten.</summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>Gibt an, ob eine vorherige Seite existiert.</summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>Gibt an, ob eine naechste Seite existiert.</summary>
    public bool HasNextPage => Page < TotalPages;
}
