using DigitalisierungsManager.Models;

namespace DigitalisierungsManager.Services.Search;

public record ScoredProjekt(Projekt Projekt, double Score, List<string> MatchedTerms);

/// <summary>
/// Semantisch-aehnliche Suche ohne externe Services: verbindet Keyword-Match,
/// Synonym-Expansion, Substring- und Fuzzy-Match zu einem Gesamtscore.
/// </summary>
public interface ISemanticSearchService
{
    Task<List<ScoredProjekt>> SearchAsync(string query, string userId, CancellationToken ct = default);
}
