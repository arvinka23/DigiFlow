namespace DigitalisierungsManager.Services.Ai;

/// <summary>
/// Abstraktion ueber einen optionalen KI-Provider. Features koennen pruefen,
/// ob ein Provider verfuegbar ist (<see cref="IsAvailable"/>), und sich bei Abwesenheit
/// auf deterministische Logik zurueckziehen.
/// </summary>
public interface IAiClient
{
    /// <summary>True, wenn ein echter Provider konfiguriert ist (Ollama etc.).</summary>
    bool IsAvailable { get; }

    /// <summary>Liefert einen Chat-Completion-Text. Gibt null zurueck wenn kein Provider verfuegbar.</summary>
    Task<string?> CompleteAsync(string systemPrompt, string userPrompt, CancellationToken ct = default);

    /// <summary>Erzeugt einen Embedding-Vektor. Gibt null zurueck wenn kein Provider verfuegbar.</summary>
    Task<float[]?> EmbedAsync(string text, CancellationToken ct = default);
}
