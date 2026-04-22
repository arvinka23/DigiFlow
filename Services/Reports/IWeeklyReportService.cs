namespace DigitalisierungsManager.Services.Reports;

public interface IWeeklyReportService
{
    Task<WeeklyReportResult> GenerateAsync(string userId, DateTime? referenzDatum = null, CancellationToken ct = default);
}
