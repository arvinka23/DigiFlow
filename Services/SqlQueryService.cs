using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DigitalisierungsManager.Data;
using System.Data;
using System.Text.RegularExpressions;

namespace DigitalisierungsManager.Services;

/// <summary>
/// Implementierung des SQL-Query-Service mit Sicherheitspruefungen.
/// Erlaubt ausschliesslich schreibgeschuetzte SELECT-Abfragen.
/// </summary>
public class SqlQueryService : ISqlQueryService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SqlQueryService> _logger;

    /// <summary>Maximale Anzahl zurueckgegebener Zeilen pro Abfrage.</summary>
    private const int MaxRows = 1000;

    /// <summary>
    /// Gefaehrliche SQL-Schluesselwoerter, die in Abfragen blockiert werden.
    /// </summary>
    private static readonly string[] ForbiddenKeywords =
    {
        "INSERT", "UPDATE", "DELETE", "DROP", "ALTER", "TRUNCATE",
        "CREATE", "EXEC", "EXECUTE", "MERGE", "GRANT", "REVOKE",
        "DENY", "BACKUP", "RESTORE", "SHUTDOWN", "DBCC",
        "SP_", "XP_", "INTO"
    };

    public SqlQueryService(ApplicationDbContext context, ILogger<SqlQueryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<Dictionary<string, object>>> ExecuteReadOnlyQueryAsync(string sql)
    {
        ValidateQuery(sql);
        _logger.LogInformation("SQL-Abfrage wird ausgefuehrt: {Query}", sql);

        var connection = _context.Database.GetDbConnection();

        try
        {
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            var results = new List<Dictionary<string, object>>();

            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandTimeout = 10; // Max. 10 Sekunden

            using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleResult);
            var rowCount = 0;
            while (await reader.ReadAsync() && rowCount < MaxRows)
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.IsDBNull(i)
                        ? (object)DBNull.Value
                        : reader.GetValue(i);
                }
                results.Add(row);
                rowCount++;
            }

            _logger.LogInformation("SQL-Abfrage lieferte {RowCount} Zeilen", results.Count);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler bei SQL-Abfrage: {Query}", sql);
            throw;
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
        }
    }

    /// <summary>
    /// Validiert das SQL-Statement und blockiert alles ausser SELECT-Abfragen.
    /// </summary>
    private void ValidateQuery(string sql)
    {
        if (string.IsNullOrWhiteSpace(sql))
        {
            _logger.LogWarning("Leere SQL-Abfrage wurde abgelehnt");
            throw new InvalidOperationException("SQL-Abfrage darf nicht leer sein.");
        }

        // Kommentare und Whitespace am Anfang entfernen
        var cleaned = Regex.Replace(sql.Trim(), @"^\s*--.*$", "", RegexOptions.Multiline).Trim();
        cleaned = Regex.Replace(cleaned, @"/\*.*?\*/", "", RegexOptions.Singleline).Trim();

        // Muss mit SELECT beginnen
        if (!cleaned.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Nicht-SELECT-Abfrage blockiert: {Query}", sql);
            throw new InvalidOperationException(
                "Nur SELECT-Abfragen sind erlaubt. Datenmodifizierende Operationen sind aus Sicherheitsgruenden blockiert.");
        }

        // Auf verbotene Schluesselwoerter pruefen (Wortgrenzen-Match)
        var upperSql = sql.ToUpperInvariant();
        foreach (var keyword in ForbiddenKeywords)
        {
            var pattern = $@"\b{Regex.Escape(keyword)}\b";
            if (Regex.IsMatch(upperSql, pattern))
            {
                _logger.LogWarning("Verbotenes Schluesselwort '{Keyword}' in Abfrage erkannt: {Query}", keyword, sql);
                throw new InvalidOperationException(
                    $"Die Verwendung von '{keyword}' ist aus Sicherheitsgruenden nicht erlaubt. Nur lesende SELECT-Abfragen sind zugelassen.");
            }
        }
    }
}
