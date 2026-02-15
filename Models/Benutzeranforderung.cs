using System.ComponentModel.DataAnnotations;

namespace DigitalisierungsManager.Models;

/// <summary>
/// Repraesentiert eine Benutzeranforderung, die einem Projekt zugeordnet ist.
/// </summary>
public class Benutzeranforderung
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Titel ist erforderlich.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Titel muss zwischen 3 und 200 Zeichen lang sein.")]
    public string Titel { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Beschreibung darf maximal 2000 Zeichen lang sein.")]
    public string Beschreibung { get; set; } = string.Empty;

    public Prioritaet Prioritaet { get; set; } = Prioritaet.Mittel;

    public AnforderungsStatus Status { get; set; } = AnforderungsStatus.Offen;

    [Required(ErrorMessage = "Ersteller ist erforderlich.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Ersteller muss zwischen 2 und 100 Zeichen lang sein.")]
    public string Ersteller { get; set; } = string.Empty;

    public DateTime ErstelltAm { get; set; } = DateTime.UtcNow;

    // Foreign Key
    public int ProjektId { get; set; }
    public virtual Projekt Projekt { get; set; } = null!;
}

/// <summary>
/// Prioritaetsstufen fuer Benutzeranforderungen.
/// </summary>
public enum Prioritaet
{
    Niedrig,
    Mittel,
    Hoch,
    Kritisch
}

/// <summary>
/// Moegliche Status-Werte einer Benutzeranforderung.
/// </summary>
public enum AnforderungsStatus
{
    Offen,
    InBearbeitung,
    Gelöst,
    Abgelehnt
}
