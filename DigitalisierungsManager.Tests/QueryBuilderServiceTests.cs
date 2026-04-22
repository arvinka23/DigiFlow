using DigitalisierungsManager.Services.QueryBuilder;

namespace DigitalisierungsManager.Tests;

public class QueryBuilderServiceTests
{
    private readonly QueryBuilderService _sut = new();

    [Fact]
    public void Build_SimpleSelectFromProjekte_ReturnsAllColumns()
    {
        var sql = _sut.BuildSql(new QueryBuilderRequest { Table = "Projekte", Limit = 10 });
        Assert.StartsWith("SELECT Id, Titel", sql);
        Assert.Contains("FROM Projekte", sql);
        Assert.EndsWith("LIMIT 10", sql);
    }

    [Fact]
    public void Build_RejectsUnknownTable()
    {
        Assert.Throws<InvalidOperationException>(() =>
            _sut.BuildSql(new QueryBuilderRequest { Table = "Users" }));
    }

    [Fact]
    public void Build_RejectsUnknownColumn()
    {
        var req = new QueryBuilderRequest
        {
            Table = "Projekte",
            Filters = new() { new FilterClause { Column = "DropTable", Operator = "=", Value = "x" } }
        };
        Assert.Throws<InvalidOperationException>(() => _sut.BuildSql(req));
    }

    [Fact]
    public void Build_EscapesSingleQuotes()
    {
        var req = new QueryBuilderRequest
        {
            Table = "Projekte",
            Columns = new() { "Titel" },
            Filters = new() { new FilterClause { Column = "Titel", Operator = "=", Value = "O'Brien" } },
            Limit = 1
        };

        var sql = _sut.BuildSql(req);
        Assert.Contains("'O''Brien'", sql);
    }

    [Fact]
    public void Build_RejectsUnknownOperator()
    {
        var req = new QueryBuilderRequest
        {
            Table = "Projekte",
            Filters = new() { new FilterClause { Column = "Titel", Operator = ";--", Value = "x" } }
        };
        Assert.Throws<InvalidOperationException>(() => _sut.BuildSql(req));
    }

    [Fact]
    public void Build_IsNullOperator_DoesNotRenderValue()
    {
        var req = new QueryBuilderRequest
        {
            Table = "Projekte",
            Columns = new() { "Id" },
            Filters = new() { new FilterClause { Column = "Abschlussdatum", Operator = "IS NULL" } }
        };

        var sql = _sut.BuildSql(req);
        Assert.Contains("Abschlussdatum IS NULL", sql);
        Assert.DoesNotContain("''", sql);
    }
}
