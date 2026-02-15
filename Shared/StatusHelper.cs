using DigitalisierungsManager.Models;

namespace DigitalisierungsManager.Shared;

/// <summary>
/// Hilfsklasse fuer die Zuordnung von Bootstrap-CSS-Klassen zu Projekt-Status-Werten.
/// </summary>
public static class StatusHelper
{
    /// <summary>
    /// Gibt die passende Bootstrap-Badge-CSS-Klasse fuer einen ProjektStatus zurueck.
    /// </summary>
    public static string GetBadgeClass(ProjektStatus status) => status switch
    {
        ProjektStatus.Geplant => "bg-info",
        ProjektStatus.InBearbeitung => "bg-warning text-dark",
        ProjektStatus.InReview => "bg-primary",
        ProjektStatus.Abgeschlossen => "bg-success",
        ProjektStatus.Pausiert => "bg-secondary",
        _ => "bg-secondary"
    };

    /// <summary>
    /// Gibt einen benutzerfreundlichen Anzeigenamen fuer einen ProjektStatus zurueck.
    /// </summary>
    public static string GetDisplayName(ProjektStatus status) => status switch
    {
        ProjektStatus.Geplant => "Geplant",
        ProjektStatus.InBearbeitung => "In Bearbeitung",
        ProjektStatus.InReview => "In Review",
        ProjektStatus.Abgeschlossen => "Abgeschlossen",
        ProjektStatus.Pausiert => "Pausiert",
        _ => status.ToString()
    };
}
