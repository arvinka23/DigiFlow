using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DigitalisierungsManager.Services.Ai;

/// <summary>
/// Adapter fuer Ollama (lokaler LLM-Server, 100% kostenlos).
/// Wird nur aktiviert, wenn AiOptions.Provider == "Ollama".
/// </summary>
public class OllamaAiClient : IAiClient
{
    private readonly HttpClient _http;
    private readonly AiOptions _options;
    private readonly ILogger<OllamaAiClient> _logger;

    public OllamaAiClient(HttpClient http, IOptions<AiOptions> options, ILogger<OllamaAiClient> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
        _http.BaseAddress = new Uri(_options.OllamaBaseUrl.TrimEnd('/'));
        _http.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
    }

    public bool IsAvailable => true;

    public async Task<string?> CompleteAsync(string systemPrompt, string userPrompt, CancellationToken ct = default)
    {
        try
        {
            var request = new OllamaChatRequest(
                Model: _options.ChatModel,
                Messages: new[]
                {
                    new OllamaMessage("system", systemPrompt),
                    new OllamaMessage("user", userPrompt)
                },
                Stream: false);

            var response = await _http.PostAsJsonAsync("/api/chat", request, ct);
            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadFromJsonAsync<OllamaChatResponse>(cancellationToken: ct);
            return payload?.Message?.Content;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ollama-Chat-Call fehlgeschlagen.");
            return null;
        }
    }

    public async Task<float[]?> EmbedAsync(string text, CancellationToken ct = default)
    {
        try
        {
            var request = new OllamaEmbedRequest(_options.EmbeddingModel, text);
            var response = await _http.PostAsJsonAsync("/api/embeddings", request, ct);
            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadFromJsonAsync<OllamaEmbedResponse>(cancellationToken: ct);
            return payload?.Embedding;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ollama-Embed-Call fehlgeschlagen.");
            return null;
        }
    }

    private record OllamaChatRequest(
        [property: JsonPropertyName("model")] string Model,
        [property: JsonPropertyName("messages")] OllamaMessage[] Messages,
        [property: JsonPropertyName("stream")] bool Stream);

    private record OllamaMessage(
        [property: JsonPropertyName("role")] string Role,
        [property: JsonPropertyName("content")] string Content);

    private record OllamaChatResponse(
        [property: JsonPropertyName("message")] OllamaMessage? Message);

    private record OllamaEmbedRequest(
        [property: JsonPropertyName("model")] string Model,
        [property: JsonPropertyName("prompt")] string Prompt);

    private record OllamaEmbedResponse(
        [property: JsonPropertyName("embedding")] float[]? Embedding);
}
