using System.Globalization;
using System.Text;
using DigitalisierungsManager.Models;
using DigitalisierungsManager.Services.Health;

namespace DigitalisierungsManager.Services.Reports;

/// <summary>
/// Erzeugt einen wochenbasierten Management-Report. Komplett deterministisch,
/// ohne externe Services. Die "Executive Summary" wird aus Textbausteinen
/// und den aggregierten Kennzahlen zusammengesetzt.
/// </summary>
public class WeeklyReportService : IWeeklyReportService
{
    private static readonly CultureInfo De = CultureInfo.GetCultureInfo("de-DE");
    private readonly IProjektService _projekte;
    private readonly IHealthScoreService _health;

    public WeeklyReportService(IProjektService projekte, IHealthScoreService health)
    {
        _projekte = projekte;
        _health = health;
    }

    public async Task<WeeklyReportResult> GenerateAsync(string userId, DateTime? referenzDatum = null, CancellationToken ct = default)
    {
        var bis = (referenzDatum ?? DateTime.UtcNow).Date.AddDays(1).AddTicks(-1);
        var von = bis.AddDays(-7).Date;

        var alle = await _projekte.GetAllProjekteAsync(userId);
        var imZeitraum = alle.Where(p => p.ErstelltAm >= von && p.ErstelltAm <= bis).ToList();
        var abgeschlossen = alle
            .Where(p => p.Status == ProjektStatus.Abgeschlossen
                        && p.Abschlussdatum.HasValue
                        && p.Abschlussdatum.Value >= von && p.Abschlussdatum.Value <= bis)
            .ToList();

        var statusVerteilung = Enum.GetValues<ProjektStatus>()
            .ToDictionary(s => s, s => alle.Count(p => p.Status == s));

        var assessments = alle.Select(p => (Projekt: p, Health: _health.Evaluate(p))).ToList();
        var avg = assessments.Count > 0
            ? (int)Math.Round(assessments.Average(a => a.Health.Score))
            : 0;

        var handlungsbedarf = assessments
            .Where(a => a.Health.Score < 60)
            .OrderBy(a => a.Health.Score)
            .Take(5)
            .Select(a => new HandlungsbedarfItem(a.Projekt, a.Health))
            .ToList();

        var data = new WeeklyReportData
        {
            UserDisplayName = userId,
            PeriodeVon = von,
            PeriodeBis = bis,
            GesamtProjekte = alle.Count,
            NeueProjekte = imZeitraum.Count,
            Abgeschlossene = abgeschlossen.Count,
            StatusVerteilung = statusVerteilung,
            DurchschnittScore = avg,
            Handlungsbedarf = handlungsbedarf,
            Hervorhebungen = imZeitraum.Take(5).ToList()
        };

        return new WeeklyReportResult
        {
            Data = data,
            Markdown = BuildMarkdown(data)
        };
    }

    internal static string BuildMarkdown(WeeklyReportData d)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"# Wochenreport {d.PeriodeVon.ToString("dd.MM.yyyy", De)} - {d.PeriodeBis.ToString("dd.MM.yyyy", De)}");
        sb.AppendLine();
        sb.AppendLine(BuildExecutiveSummary(d));
        sb.AppendLine();
        sb.AppendLine("## Kennzahlen");
        sb.AppendLine();
        sb.AppendLine($"- Gesamt-Projekte: **{d.GesamtProjekte}**");
        sb.AppendLine($"- Neue Projekte in dieser Woche: **{d.NeueProjekte}**");
        sb.AppendLine($"- Abgeschlossen in dieser Woche: **{d.Abgeschlossene}**");
        sb.AppendLine($"- Durchschnittlicher Health-Score: **{d.DurchschnittScore} / 100**");
        sb.AppendLine();

        sb.AppendLine("## Status-Verteilung");
        sb.AppendLine();
        foreach (var kv in d.StatusVerteilung.OrderByDescending(k => k.Value))
        {
            sb.AppendLine($"- {kv.Key}: {kv.Value}");
        }
        sb.AppendLine();

        if (d.Handlungsbedarf.Any())
        {
            sb.AppendLine("## Top-Handlungsbedarf");
            sb.AppendLine();
            foreach (var h in d.Handlungsbedarf)
            {
                sb.AppendLine($"### {h.Projekt.Titel} (Score {h.Health.Score}/100)");
                foreach (var f in h.Health.Findings.Take(3))
                {
                    sb.AppendLine($"- **{f.Category}** ({f.Severity}): {f.Message}");
                    if (!string.IsNullOrWhiteSpace(f.Recommendation))
                        sb.AppendLine($"    - Empfehlung: {f.Recommendation}");
                }
                sb.AppendLine();
            }
        }

        if (d.Hervorhebungen.Any())
        {
            sb.AppendLine("## Neu gestartete Projekte");
            sb.AppendLine();
            foreach (var p in d.Hervorhebungen)
            {
                sb.AppendLine($"- **{p.Titel}** ({p.Status}) – {p.Verantwortlicher}");
            }
            sb.AppendLine();
        }

        sb.AppendLine("---");
        sb.AppendLine($"_Report automatisch erzeugt am {DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm 'UTC'", De)} durch DigiFlow._");
        return sb.ToString();
    }

    private static string BuildExecutiveSummary(WeeklyReportData d)
    {
        var parts = new List<string>();

        parts.Add(d.NeueProjekte switch
        {
            0 => "In dieser Woche wurden keine neuen Projekte erfasst.",
            1 => "Diese Woche kam ein neues Projekt hinzu.",
            _ => $"Diese Woche kamen {d.NeueProjekte} neue Projekte hinzu."
        });

        parts.Add(d.Abgeschlossene switch
        {
            0 => "Kein Projekt wurde in dieser Woche abgeschlossen.",
            1 => "Ein Projekt wurde erfolgreich abgeschlossen.",
            _ => $"{d.Abgeschlossene} Projekte wurden erfolgreich abgeschlossen."
        });

        parts.Add(d.DurchschnittScore switch
        {
            >= 80 => $"Der durchschnittliche Health-Score liegt bei {d.DurchschnittScore}/100 – das Portfolio ist insgesamt gesund.",
            >= 60 => $"Der durchschnittliche Health-Score liegt bei {d.DurchschnittScore}/100 – Portfolio stabil, einzelne Projekte sollten beobachtet werden.",
            >= 40 => $"Der durchschnittliche Health-Score liegt bei {d.DurchschnittScore}/100 – mehrere Projekte brauchen Aufmerksamkeit.",
            _     => $"Der durchschnittliche Health-Score liegt bei {d.DurchschnittScore}/100 – deutlicher Handlungsbedarf im Portfolio."
        });

        if (d.Handlungsbedarf.Count > 0)
        {
            parts.Add($"{d.Handlungsbedarf.Count} Projekt(e) mit Score < 60 erfordern konkrete Massnahmen.");
        }

        return string.Join(" ", parts);
    }
}
