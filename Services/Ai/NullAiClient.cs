namespace DigitalisierungsManager.Services.Ai;

/// <summary>
/// No-Op-Implementierung: wird verwendet, wenn kein externer Provider konfiguriert ist.
/// Garantiert, dass das Produkt ohne API-Key, ohne Ollama, ohne Internet funktioniert.
/// </summary>
public class NullAiClient : IAiClient
{
    public bool IsAvailable => false;

    public Task<string?> CompleteAsync(string systemPrompt, string userPrompt, CancellationToken ct = default)
        => Task.FromResult<string?>(null);

    public Task<float[]?> EmbedAsync(string text, CancellationToken ct = default)
        => Task.FromResult<float[]?>(null);
}
