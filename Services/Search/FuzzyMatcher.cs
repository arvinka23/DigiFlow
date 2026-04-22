namespace DigitalisierungsManager.Services.Search;

/// <summary>
/// Kleine Levenshtein-Implementierung fuer Tippfehler-Toleranz.
/// </summary>
internal static class FuzzyMatcher
{
    /// <summary>True, wenn <paramref name="candidate"/> innerhalb der Toleranz zu <paramref name="target"/> liegt.</summary>
    public static bool IsCloseMatch(string candidate, string target, int maxDistance = 2)
    {
        if (string.IsNullOrEmpty(candidate) || string.IsNullOrEmpty(target)) return false;
        if (Math.Abs(candidate.Length - target.Length) > maxDistance) return false;
        return Levenshtein(candidate, target) <= maxDistance;
    }

    public static int Levenshtein(string a, string b)
    {
        if (a == b) return 0;
        if (a.Length == 0) return b.Length;
        if (b.Length == 0) return a.Length;

        var prev = new int[b.Length + 1];
        var curr = new int[b.Length + 1];
        for (var j = 0; j <= b.Length; j++) prev[j] = j;

        for (var i = 1; i <= a.Length; i++)
        {
            curr[0] = i;
            for (var j = 1; j <= b.Length; j++)
            {
                var cost = a[i - 1] == b[j - 1] ? 0 : 1;
                curr[j] = Math.Min(Math.Min(curr[j - 1] + 1, prev[j] + 1), prev[j - 1] + cost);
            }
            (prev, curr) = (curr, prev);
        }
        return prev[b.Length];
    }
}
