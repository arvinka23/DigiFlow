using DigitalisierungsManager.Models;
using DigitalisierungsManager.Services.Health;

namespace DigitalisierungsManager.Services.Reports;

public class WeeklyReportData
{
    public string UserDisplayName { get; set; } = string.Empty;
    public DateTime PeriodeVon { get; set; }
    public DateTime PeriodeBis { get; set; }
    public int GesamtProjekte { get; set; }
    public int NeueProjekte { get; set; }
    public int Abgeschlossene { get; set; }
    public Dictionary<ProjektStatus, int> StatusVerteilung { get; set; } = new();
    public int DurchschnittScore { get; set; }
    public List<HandlungsbedarfItem> Handlungsbedarf { get; set; } = new();
    public List<Projekt> Hervorhebungen { get; set; } = new();
}

public record HandlungsbedarfItem(Projekt Projekt, HealthAssessment Health);

public class WeeklyReportResult
{
    public WeeklyReportData Data { get; set; } = new();
    public string Markdown { get; set; } = string.Empty;
}
