using System.Text.RegularExpressions;
using DigitalisierungsManager.Services.Ai;

namespace DigitalisierungsManager.Services.Scanner;

/// <summary>
/// Matcht Prozessbeschreibung gegen Template-Bibliothek (Keyword-Scoring),
/// und reichert den Draft optional mit einer KI-Feinfassung an,
/// wenn der konfigurierte <see cref="IAiClient"/> verfuegbar ist.
/// </summary>
public class OpportunityScannerService : IOpportunityScannerService
{
    private readonly IAiClient _ai;

    public OpportunityScannerService(IAiClient ai)
    {
        _ai = ai;
    }

    public async Task<ProjectDraft> AnalyzeAsync(string beschreibung, string verantwortlicher, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(beschreibung))
            throw new ArgumentException("Beschreibung darf nicht leer sein.", nameof(beschreibung));

        var (template, score) = MatchTemplate(beschreibung);

        var draft = new ProjectDraft
        {
            Titel = template.TitelTemplate,
            Beschreibung = SummarizeInput(beschreibung),
            Technologie = template.Technologie,
            Verantwortlicher = string.IsNullOrWhiteSpace(verantwortlicher) ? "IT-Team" : verantwortlicher.Trim(),
            Anforderungen = template.Anforderungen.Select(a => new AnforderungDraft { Titel = a.Titel, Beschreibung = a.Beschreibung, Prioritaet = a.Prioritaet }).ToList(),
            Vorschlaege = template.Vorschlaege.Select(v => new VorschlagDraft
            {
                Titel = v.Titel,
                Beschreibung = v.Beschreibung,
                Vorschlagstyp = v.Vorschlagstyp,
                EmpfohleneTechnologie = v.EmpfohleneTechnologie,
                Begruendung = v.Begruendung,
                AufwandTage = v.AufwandTage,
                Komplexitaet = v.Komplexitaet
            }).ToList(),
            Roi = new RoiEstimate
            {
                JaehrlicheErsparnisEuro = template.Roi.JaehrlicheErsparnisEuro,
                EinmaligerAufwandEuro = template.Roi.EinmaligerAufwandEuro,
                AmortisationMonate = template.Roi.AmortisationMonate,
                Begruendung = template.Roi.Begruendung
            },
            Risiken = new List<string>(template.Risiken),
            MatchedTemplateName = template.Name,
            Confidence = score
        };

        if (_ai.IsAvailable)
        {
            await TryEnrichWithAiAsync(draft, beschreibung, ct);
        }

        return draft;
    }

    private static (ProcessTemplateLibrary.ProcessTemplate Template, double Score) MatchTemplate(string input)
    {
        var normalized = Normalize(input);
        ProcessTemplateLibrary.ProcessTemplate? best = null;
        var bestHits = 0;

        foreach (var t in ProcessTemplateLibrary.Templates)
        {
            var hits = t.Keywords.Count(kw => normalized.Contains(kw));
            if (hits > bestHits)
            {
                bestHits = hits;
                best = t;
            }
        }

        if (best == null || bestHits == 0)
            return (ProcessTemplateLibrary.Generic, 0.35);

        var confidence = Math.Min(0.55 + bestHits * 0.15, 0.95);
        return (best, confidence);
    }

    private static string Normalize(string input)
        => Regex.Replace(input.ToLowerInvariant(), @"[^a-z0-9äöüß ]+", " ");

    private static string SummarizeInput(string input)
    {
        var trimmed = input.Trim();
        if (trimmed.Length <= 280) return trimmed;
        return trimmed[..277] + "...";
    }

    private async Task TryEnrichWithAiAsync(ProjectDraft draft, string originalInput, CancellationToken ct)
    {
        const string systemPrompt =
            "Du bist Digitalisierungs-Berater. Du bekommst einen Projekt-Draft und eine freitext-Beschreibung. " +
            "Antworte mit 1 bis 3 kurzen zusaetzlichen Risiken auf deutsch, eines pro Zeile, ohne Nummerierung, " +
            "ohne Praefix. Keine Einleitung, keine weiteren Kommentare.";

        var userPrompt =
            $"Beschreibung: {originalInput}\n\n" +
            $"Projekt-Titel: {draft.Titel}\n" +
            $"Bestehende Risiken: {string.Join(" | ", draft.Risiken)}";

        var result = await _ai.CompleteAsync(systemPrompt, userPrompt, ct);
        if (string.IsNullOrWhiteSpace(result)) return;

        var extra = result
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(l => l.Length > 10 && l.Length < 300)
            .Take(3)
            .ToList();

        foreach (var risk in extra)
            draft.Risiken.Add(risk);
    }
}
