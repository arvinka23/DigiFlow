namespace DigitalisierungsManager.Services.Ai;

/// <summary>
/// Konfiguration fuer optionale KI-Provider. Standard: "None" = komplett lokal,
/// kein externer API-Call, keine Kosten. Nutzer kann spaeter auf "Ollama" (lokal)
/// umstellen, ohne Code-Aenderung.
/// </summary>
public class AiOptions
{
    public const string SectionName = "Ai";

    /// <summary>None | Ollama</summary>
    public string Provider { get; set; } = "None";

    /// <summary>Basis-URL fuer Ollama (z.B. http://localhost:11434)</summary>
    public string OllamaBaseUrl { get; set; } = "http://localhost:11434";

    /// <summary>Chat-Modell, z.B. "llama3.1:8b"</summary>
    public string ChatModel { get; set; } = "llama3.1:8b";

    /// <summary>Embedding-Modell, z.B. "nomic-embed-text"</summary>
    public string EmbeddingModel { get; set; } = "nomic-embed-text";

    /// <summary>Timeout pro Call in Sekunden.</summary>
    public int TimeoutSeconds { get; set; } = 60;
}
