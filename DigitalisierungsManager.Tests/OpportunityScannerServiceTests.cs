using DigitalisierungsManager.Services.Ai;
using DigitalisierungsManager.Services.Scanner;

namespace DigitalisierungsManager.Tests;

public class OpportunityScannerServiceTests
{
    private readonly OpportunityScannerService _sut = new(new NullAiClient());

    [Fact]
    public async Task MatchesInvoiceTemplate_FromGermanDescription()
    {
        var draft = await _sut.AnalyzeAsync(
            "Wir bekommen jede Rechnung per Post und tragen sie manuell in Excel ein.",
            "Finanz");

        Assert.Equal("Rechnungseingang", draft.MatchedTemplateName);
        Assert.True(draft.Confidence >= 0.6);
        Assert.NotEmpty(draft.Anforderungen);
        Assert.NotEmpty(draft.Vorschlaege);
        Assert.True(draft.Roi.JaehrlicheErsparnisEuro > 0);
        Assert.Equal("Finanz", draft.Verantwortlicher);
    }

    [Fact]
    public async Task FallsBackToGenericTemplate_WhenNoKeywordMatches()
    {
        var draft = await _sut.AnalyzeAsync(
            "Wir haben hier so einen komischen Prozess der total individuell ist.",
            "");

        Assert.Equal("Allgemeiner Prozess", draft.MatchedTemplateName);
        Assert.True(draft.Confidence < 0.5);
        Assert.NotEmpty(draft.Anforderungen);
        Assert.Equal("IT-Team", draft.Verantwortlicher);
    }

    [Fact]
    public async Task EmptyInput_Throws()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _sut.AnalyzeAsync("", ""));
    }
}
