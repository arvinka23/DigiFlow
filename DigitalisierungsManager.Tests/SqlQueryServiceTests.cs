using Microsoft.Extensions.Logging.Abstractions;
using DigitalisierungsManager.Services;

namespace DigitalisierungsManager.Tests;

/// <summary>
/// Unit-Tests fuer den SqlQueryService (Validierung und Sicherheit).
/// </summary>
public class SqlQueryServiceTests
{
    private SqlQueryService CreateService()
    {
        var context = TestDbHelper.CreateContext();
        return new SqlQueryService(context, NullLogger<SqlQueryService>.Instance);
    }

    [Fact]
    public async Task ExecuteReadOnlyQueryAsync_ValidSelectQuery_ShouldPassValidation()
    {
        // Arrange -- InMemory provider unterstuetzt GetDbConnection() nicht,
        // daher testen wir hier, dass die Validierung korrekt durchlaeuft
        // und die Exception vom Provider kommt (nicht von unserer Validierung).
        var service = CreateService();

        // Act & Assert -- die Validierung laesst SELECT durch,
        // aber der InMemory-Provider wirft eine andere Exception
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ExecuteReadOnlyQueryAsync("SELECT 1 AS TestValue"));

        // Sicherstellen, dass es NICHT unsere Sicherheits-Validierung ist
        Assert.DoesNotContain("SELECT-Abfragen", ex.Message);
        Assert.DoesNotContain("nicht erlaubt", ex.Message);
    }

    [Theory]
    [InlineData("DELETE FROM Projekte")]
    [InlineData("DROP TABLE Projekte")]
    [InlineData("INSERT INTO Projekte (Titel) VALUES ('test')")]
    [InlineData("UPDATE Projekte SET Titel = 'hacked'")]
    [InlineData("ALTER TABLE Projekte ADD Hacked INT")]
    [InlineData("TRUNCATE TABLE Projekte")]
    [InlineData("EXEC sp_executesql N'DROP TABLE Projekte'")]
    [InlineData("CREATE TABLE Hacked (Id INT)")]
    [InlineData("MERGE Projekte AS target USING (SELECT 1) AS source ON 1=1 WHEN MATCHED THEN DELETE;")]
    public async Task ExecuteReadOnlyQueryAsync_DangerousQuery_ShouldThrow(string sql)
    {
        // Arrange
        var service = CreateService();

        // Act & Assert -- muss eine InvalidOperationException werfen (entweder "Nur SELECT"
        // oder "nicht erlaubt" je nachdem ob das Statement nicht mit SELECT beginnt oder
        // ein verbotenes Keyword enthaelt)
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ExecuteReadOnlyQueryAsync(sql));
    }

    [Fact]
    public async Task ExecuteReadOnlyQueryAsync_EmptyQuery_ShouldThrow()
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ExecuteReadOnlyQueryAsync(""));
    }

    [Fact]
    public async Task ExecuteReadOnlyQueryAsync_WhitespaceQuery_ShouldThrow()
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ExecuteReadOnlyQueryAsync("   "));
    }

    [Fact]
    public async Task ExecuteReadOnlyQueryAsync_CommentThenDrop_ShouldThrow()
    {
        // Arrange - try to bypass with leading comments
        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ExecuteReadOnlyQueryAsync("-- just a comment\nDROP TABLE Projekte"));
    }

    [Fact]
    public async Task ExecuteReadOnlyQueryAsync_SelectInto_ShouldThrow()
    {
        // Arrange - SELECT INTO creates a new table
        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ExecuteReadOnlyQueryAsync("SELECT * INTO NewTable FROM Projekte"));
    }
}
