namespace DigitalisierungsManager.Services.Health;

/// <summary>
/// Ergebnis einer Gesundheitsbewertung eines Projekts.
/// </summary>
public class HealthAssessment
{
    /// <summary>Score 0..100 -- je hoeher, desto gesuender.</summary>
    public int Score { get; set; } = 100;

    public HealthLevel Level => Score switch
    {
        >= 80 => HealthLevel.Gesund,
        >= 60 => HealthLevel.Beobachten,
        >= 40 => HealthLevel.Handlungsbedarf,
        _     => HealthLevel.Kritisch
    };

    public List<HealthFinding> Findings { get; set; } = new();

    public DateTime BewertetAm { get; set; } = DateTime.UtcNow;
}

public enum HealthLevel
{
    Gesund,
    Beobachten,
    Handlungsbedarf,
    Kritisch
}

public class HealthFinding
{
    public string Category { get; set; } = string.Empty;
    public HealthSeverity Severity { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
    public int ScoreImpact { get; set; }
}

public enum HealthSeverity
{
    Info,
    Warnung,
    Kritisch
}
