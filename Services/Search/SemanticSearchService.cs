using System.Text.RegularExpressions;
using DigitalisierungsManager.Models;

namespace DigitalisierungsManager.Services.Search;

/// <summary>
/// Ranked Scoring:
/// - Exakter Wortmatch im Titel:        +5
/// - Exakter Wortmatch anderswo:        +3
/// - Synonym-Treffer:                   +2
/// - Substring-Treffer:                 +1
/// - Fuzzy-Treffer (Levenshtein <= 2):  +1
///
/// Alle Treffer werden gesammelt und der Score normalisiert (0..1).
/// </summary>
public class SemanticSearchService : ISemanticSearchService
{
    private readonly IProjektService _projekte;

    public SemanticSearchService(IProjektService projekte)
    {
        _projekte = projekte;
    }

    public async Task<List<ScoredProjekt>> SearchAsync(string query, string userId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new List<ScoredProjekt>();

        var all = await _projekte.GetAllProjekteAsync(userId);
        var tokens = Tokenize(query);
        if (tokens.Count == 0) return new List<ScoredProjekt>();

        var expanded = tokens
            .SelectMany(t => SynonymDictionary.Expand(t))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var scored = new List<ScoredProjekt>();
        foreach (var p in all)
        {
            var matches = new List<string>();
            var score = 0.0;
            var titel = (p.Titel ?? "").ToLowerInvariant();
            var body = $"{p.Beschreibung} {p.Technologie} {p.Verantwortlicher}".ToLowerInvariant();
            var titelTokens = Tokenize(titel);
            var bodyTokens = Tokenize(body);

            foreach (var tok in tokens)
            {
                var lower = tok.ToLowerInvariant();
                if (titelTokens.Contains(lower))
                {
                    score += 5; matches.Add(tok); continue;
                }
                if (bodyTokens.Contains(lower))
                {
                    score += 3; matches.Add(tok); continue;
                }
                if (titel.Contains(lower) || body.Contains(lower))
                {
                    score += 1; matches.Add(tok); continue;
                }
            }

            foreach (var syn in expanded)
            {
                if (tokens.Contains(syn, StringComparer.OrdinalIgnoreCase)) continue;
                if (titel.Contains(syn) || body.Contains(syn))
                {
                    score += 2;
                    if (!matches.Contains(syn)) matches.Add(syn);
                }
            }

            foreach (var tok in tokens)
            {
                foreach (var cand in titelTokens.Concat(bodyTokens))
                {
                    if (FuzzyMatcher.IsCloseMatch(cand, tok.ToLowerInvariant(), 2) &&
                        !matches.Contains(cand))
                    {
                        score += 1;
                        matches.Add(cand);
                        break;
                    }
                }
            }

            if (score > 0)
            {
                var normalized = Math.Min(score / (tokens.Count * 5.0), 1.0);
                scored.Add(new ScoredProjekt(p, normalized, matches));
            }
        }

        return scored.OrderByDescending(s => s.Score).ToList();
    }

    private static List<string> Tokenize(string text)
        => Regex.Matches(text.ToLowerInvariant(), @"[a-zäöüß0-9]{3,}")
            .Select(m => m.Value)
            .Distinct()
            .ToList();
}
