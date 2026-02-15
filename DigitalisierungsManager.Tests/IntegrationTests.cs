using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DigitalisierungsManager.Tests;

/// <summary>
/// Integrationstests: Pruefen, ob Login-Seite erreichbar ist,
/// Blazor-App ueber _Host.cshtml ausgeliefert wird, und statische Dateien verfuegbar sind.
/// </summary>
public class IntegrationTests : IClassFixture<DigiFlowWebAppFactory>
{
    private readonly HttpClient _client;

    public IntegrationTests(DigiFlowWebAppFactory factory)
    {
        // Redirects NICHT automatisch folgen, damit wir 302 pruefen koennen
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    // --- Login- und Register-Seiten ---

    [Fact]
    public async Task LoginSeite_ShouldReturnHttp200()
    {
        var response = await _client.GetAsync("/Identity/Login");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task LoginSeite_ShouldContainLoginForm()
    {
        var response = await _client.GetAsync("/Identity/Login");
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("DigiFlow", content);
        Assert.Contains("Anmelden", content);
        Assert.Contains("E-Mail", content);
    }

    [Fact]
    public async Task RegisterSeite_ShouldReturnHttp200()
    {
        var response = await _client.GetAsync("/Identity/Register");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // --- Blazor-Seiten werden ueber _Host.cshtml ausgeliefert (immer 200) ---
    // Die Autorisierung erfolgt innerhalb der Blazor-Komponente (AuthorizeRouteView),
    // nicht auf HTTP-Ebene. Daher liefert _Host.cshtml immer 200 OK zurueck.

    [Fact]
    public async Task Startseite_ShouldReturnBlazorShell()
    {
        var response = await _client.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("blazor.server.js", content);
    }

    [Fact]
    public async Task ProjekteSeite_ShouldReturnBlazorShell()
    {
        var response = await _client.GetAsync("/projekte");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DatenaustauschSeite_ShouldReturnBlazorShell()
    {
        var response = await _client.GetAsync("/datenaustausch");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SqlQuerySeite_ShouldReturnBlazorShell()
    {
        var response = await _client.GetAsync("/sql-query");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // --- Statische Dateien sind ohne Login erreichbar ---

    [Fact]
    public async Task StatischesCss_ShouldBeAccessible()
    {
        var response = await _client.GetAsync("/css/app.css");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/css", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task StatischesJs_ShouldBeAccessible()
    {
        var response = await _client.GetAsync("/js/download.js");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
