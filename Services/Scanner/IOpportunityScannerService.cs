namespace DigitalisierungsManager.Services.Scanner;

/// <summary>
/// Erstellt aus einer Freitext-Prozessbeschreibung einen vollstaendigen Projekt-Draft.
/// Arbeitet offline mit kuratierten Prozess-Templates; kann optional eine KI fuer
/// Feinabstimmung verwenden, wenn ein Provider konfiguriert ist.
/// </summary>
public interface IOpportunityScannerService
{
    Task<ProjectDraft> AnalyzeAsync(string beschreibung, string verantwortlicher, CancellationToken ct = default);
}
