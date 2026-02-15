namespace DigitalisierungsManager.Services;

/// <summary>
/// Service fuer die Ausfuehrung von schreibgeschuetzten SQL-Abfragen auf der Datenbank.
/// Nur SELECT-Statements sind erlaubt; gefaehrliche Operationen werden blockiert.
/// </summary>
public interface ISqlQueryService
{
    /// <summary>
    /// Fuehrt eine schreibgeschuetzte SQL-Abfrage (SELECT) aus und gibt die Ergebnisse zurueck.
    /// </summary>
    /// <param name="sql">Das auszufuehrende SELECT-Statement.</param>
    /// <returns>Liste von Zeilen, wobei jede Zeile ein Dictionary aus Spaltenname und Wert ist.</returns>
    /// <exception cref="InvalidOperationException">Wird geworfen, wenn das SQL-Statement keine gueltige SELECT-Abfrage ist.</exception>
    Task<List<Dictionary<string, object>>> ExecuteReadOnlyQueryAsync(string sql);
}
