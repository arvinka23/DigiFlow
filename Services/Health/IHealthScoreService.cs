using DigitalisierungsManager.Models;

namespace DigitalisierungsManager.Services.Health;

/// <summary>
/// Berechnet deterministisch einen Gesundheitsscore fuer ein Projekt.
/// Arbeitet offline und ohne externe Dienste.
/// </summary>
public interface IHealthScoreService
{
    HealthAssessment Evaluate(Projekt projekt);
}
