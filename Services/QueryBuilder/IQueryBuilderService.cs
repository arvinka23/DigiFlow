namespace DigitalisierungsManager.Services.QueryBuilder;

/// <summary>
/// Erzeugt aus einer strukturierten <see cref="QueryBuilderRequest"/> ein
/// sicheres SELECT-Statement (SQLite-kompatibel).
/// </summary>
public interface IQueryBuilderService
{
    string BuildSql(QueryBuilderRequest request);
}

public class QueryBuilderRequest
{
    public string Table { get; set; } = "Projekte";
    public List<string> Columns { get; set; } = new();
    public List<FilterClause> Filters { get; set; } = new();
    public string? OrderByColumn { get; set; }
    public bool OrderDescending { get; set; } = true;
    public int? Limit { get; set; } = 100;
}

public class FilterClause
{
    public string Column { get; set; } = string.Empty;
    public string Operator { get; set; } = "=";  // = | != | < | > | LIKE | IS NULL | IS NOT NULL
    public string Value { get; set; } = string.Empty;
}
