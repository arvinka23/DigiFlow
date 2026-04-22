using System.Text;

namespace DigitalisierungsManager.Services.QueryBuilder;

/// <summary>
/// Baut aus einer strukturierten Anfrage ein parametrisiertes SELECT-Statement.
/// Whitelisted Tabellen/Spalten und Operatoren verhindern Injection.
/// Werte werden als Inline-Literale escaped (einfache Quotes verdoppelt),
/// weil <see cref="ISqlQueryService"/> rohe Strings ausfuehrt.
/// </summary>
public class QueryBuilderService : IQueryBuilderService
{
    private static readonly Dictionary<string, string[]> SchemaWhitelist = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Projekte"] = new[]
        {
            "Id", "Titel", "Beschreibung", "Status", "Technologie",
            "ErstelltAm", "Abschlussdatum", "Verantwortlicher", "BesitzerId"
        },
        ["Benutzeranforderungen"] = new[]
        {
            "Id", "Titel", "Beschreibung", "Prioritaet", "Status",
            "Ersteller", "ErstelltAm", "ProjektId"
        },
        ["DigitalisierungsVorschlaege"] = new[]
        {
            "Id", "Titel", "Beschreibung", "Vorschlagstyp", "Begruendung",
            "ErstelltAm", "IstAngenommen", "ProjektId"
        }
    };

    private static readonly HashSet<string> AllowedOperators = new(StringComparer.OrdinalIgnoreCase)
    {
        "=", "!=", "<", ">", "<=", ">=", "LIKE", "IS NULL", "IS NOT NULL"
    };

    public string BuildSql(QueryBuilderRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        if (!SchemaWhitelist.TryGetValue(request.Table, out var allowedColumns))
            throw new InvalidOperationException($"Unbekannte Tabelle: {request.Table}");

        var columns = (request.Columns?.Count ?? 0) == 0
            ? allowedColumns
            : request.Columns!.Where(c => allowedColumns.Contains(c, StringComparer.OrdinalIgnoreCase)).ToArray();

        if (columns.Length == 0)
            throw new InvalidOperationException("Keine gueltigen Spalten ausgewaehlt.");

        var sb = new StringBuilder();
        sb.Append("SELECT ").Append(string.Join(", ", columns));
        sb.Append(" FROM ").Append(request.Table);

        if (request.Filters.Count > 0)
        {
            sb.Append(" WHERE ");
            var parts = new List<string>();
            foreach (var f in request.Filters)
            {
                if (!allowedColumns.Contains(f.Column, StringComparer.OrdinalIgnoreCase))
                    throw new InvalidOperationException($"Filterspalte nicht erlaubt: {f.Column}");
                if (!AllowedOperators.Contains(f.Operator))
                    throw new InvalidOperationException($"Operator nicht erlaubt: {f.Operator}");

                if (string.Equals(f.Operator, "IS NULL", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(f.Operator, "IS NOT NULL", StringComparison.OrdinalIgnoreCase))
                {
                    parts.Add($"{f.Column} {f.Operator.ToUpperInvariant()}");
                }
                else
                {
                    var value = EscapeLiteral(f.Value);
                    parts.Add($"{f.Column} {f.Operator} '{value}'");
                }
            }
            sb.Append(string.Join(" AND ", parts));
        }

        if (!string.IsNullOrWhiteSpace(request.OrderByColumn) &&
            allowedColumns.Contains(request.OrderByColumn, StringComparer.OrdinalIgnoreCase))
        {
            sb.Append(" ORDER BY ").Append(request.OrderByColumn);
            sb.Append(request.OrderDescending ? " DESC" : " ASC");
        }

        if (request.Limit is > 0)
        {
            sb.Append(" LIMIT ").Append(request.Limit.Value);
        }

        return sb.ToString();
    }

    private static string EscapeLiteral(string value)
        => (value ?? string.Empty).Replace("'", "''");

    public static IReadOnlyDictionary<string, string[]> Schema => SchemaWhitelist;
    public static IReadOnlyCollection<string> Operators => AllowedOperators;
}
