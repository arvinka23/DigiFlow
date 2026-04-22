using DigitalisierungsManager.Models;

namespace DigitalisierungsManager.Services.Health;

/// <summary>
/// Regel-basierte Gesundheitsbewertung (Layer A). Startet bei 100 und
/// zieht pro verletzter Regel Punkte ab. Jede Regel kann einen Finding
/// mit Empfehlung zurueckgeben, der im UI angezeigt wird.
/// </summary>
public class HealthScoreService : IHealthScoreService
{
    public HealthAssessment Evaluate(Projekt projekt)
    {
        if (projekt == null) throw new ArgumentNullException(nameof(projekt));

        var assessment = new HealthAssessment { Score = 100 };
        var now = DateTime.UtcNow;

        if (string.IsNullOrWhiteSpace(projekt.Beschreibung) || projekt.Beschreibung.Trim().Length < 50)
        {
            AddFinding(assessment, "Klarheit", HealthSeverity.Warnung, -10,
                "Beschreibung ist sehr kurz oder fehlt.",
                "Fuege eine aussagekraeftige Beschreibung (>= 50 Zeichen) hinzu.");
        }

        if (projekt.Benutzeranforderungen == null || projekt.Benutzeranforderungen.Count == 0)
        {
            AddFinding(assessment, "Vollstaendigkeit", HealthSeverity.Warnung, -20,
                "Keine Anforderungen erfasst.",
                "Lege mindestens 2-3 konkrete Anforderungen an.");
        }

        if (projekt.Vorschlaege == null || projekt.Vorschlaege.Count == 0)
        {
            AddFinding(assessment, "Vollstaendigkeit", HealthSeverity.Warnung, -15,
                "Keine Digitalisierungsvorschlaege erfasst.",
                "Dokumentiere mindestens einen Umsetzungsvorschlag (Eigenbau, Fremdsoftware oder Mischloesung).");
        }

        if (string.IsNullOrWhiteSpace(projekt.Verantwortlicher) ||
            string.Equals(projekt.Verantwortlicher.Trim(), "N/A", StringComparison.OrdinalIgnoreCase))
        {
            AddFinding(assessment, "Klarheit", HealthSeverity.Warnung, -15,
                "Kein konkreter Verantwortlicher benannt.",
                "Weise das Projekt einer Person oder einem Team zu.");
        }

        if (projekt.Status == ProjektStatus.Geplant && (now - projekt.ErstelltAm).TotalDays > 180)
        {
            AddFinding(assessment, "Realismus", HealthSeverity.Warnung, -20,
                "Projekt ist seit mehr als 180 Tagen im Status 'Geplant'.",
                "Entscheide, ob das Projekt gestartet, verschoben oder verworfen werden soll.");
        }

        if (projekt.Status == ProjektStatus.InBearbeitung && (now - projekt.ErstelltAm).TotalDays > 60)
        {
            var days = (int)(now - projekt.ErstelltAm).TotalDays;
            AddFinding(assessment, "Realismus", HealthSeverity.Warnung, -15,
                $"Seit {days} Tagen in Bearbeitung ohne sichtbaren Abschluss.",
                "Pruefe Fortschritt und dokumentiere Zwischenstaende.");
        }

        if (projekt.Status == ProjektStatus.Abgeschlossen && projekt.Abschlussdatum == null)
        {
            AddFinding(assessment, "Vollstaendigkeit", HealthSeverity.Info, -10,
                "Projekt abgeschlossen, aber Abschlussdatum fehlt.",
                "Hinterlege das tatsaechliche Abschlussdatum.");
        }

        if (projekt.Status == ProjektStatus.Abgeschlossen &&
            projekt.Benutzeranforderungen != null &&
            projekt.Benutzeranforderungen.Any() &&
            projekt.Benutzeranforderungen.All(a => a.Status == AnforderungsStatus.Offen))
        {
            AddFinding(assessment, "Realismus", HealthSeverity.Kritisch, -30,
                "Projekt gilt als abgeschlossen, aber alle Anforderungen sind noch offen.",
                "Aktualisiere den Status der Anforderungen oder korrigiere den Projektstatus.");
        }

        if (projekt.Status == ProjektStatus.Pausiert && (now - projekt.ErstelltAm).TotalDays > 120)
        {
            AddFinding(assessment, "Realismus", HealthSeverity.Info, -10,
                "Projekt ist laenger pausiert.",
                "Ueberpruefe, ob das Projekt reaktiviert oder archiviert werden soll.");
        }

        if (string.IsNullOrWhiteSpace(projekt.Technologie))
        {
            AddFinding(assessment, "Vollstaendigkeit", HealthSeverity.Info, -5,
                "Technologie ist nicht angegeben.",
                "Erfasse die geplante oder genutzte Technologie (z.B. Power Platform, .NET, SaaS).");
        }

        if (assessment.Score < 0) assessment.Score = 0;
        if (assessment.Score > 100) assessment.Score = 100;

        return assessment;
    }

    private static void AddFinding(HealthAssessment assessment, string category, HealthSeverity severity, int impact, string message, string recommendation)
    {
        assessment.Score += impact;
        assessment.Findings.Add(new HealthFinding
        {
            Category = category,
            Severity = severity,
            Message = message,
            Recommendation = recommendation,
            ScoreImpact = impact
        });
    }
}
