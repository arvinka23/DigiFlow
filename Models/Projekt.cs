using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DigitalisierungsManager.Models;

/// <summary>
/// Repraesentiert ein Digitalisierungsprojekt mit allen relevanten Metadaten.
/// Jedes Projekt gehoert einem Benutzer (BesitzerId).
/// </summary>
public class Projekt
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Titel ist erforderlich.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Titel muss zwischen 3 und 200 Zeichen lang sein.")]
    public string Titel { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Beschreibung darf maximal 2000 Zeichen lang sein.")]
    public string Beschreibung { get; set; } = string.Empty;

    public ProjektStatus Status { get; set; } = ProjektStatus.Geplant;

    [StringLength(100, ErrorMessage = "Technologie darf maximal 100 Zeichen lang sein.")]
    public string Technologie { get; set; } = string.Empty;

    public DateTime ErstelltAm { get; set; } = DateTime.UtcNow;

    public DateTime? Abschlussdatum { get; set; }

    [Required(ErrorMessage = "Verantwortlicher ist erforderlich.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Verantwortlicher muss zwischen 2 und 100 Zeichen lang sein.")]
    public string Verantwortlicher { get; set; } = string.Empty;

    /// <summary>Fremdschluessel zum Besitzer (AppUser.Id). Jeder User sieht nur eigene Projekte.</summary>
    [JsonIgnore]
    public string BesitzerId { get; set; } = string.Empty;

    /// <summary>Navigation Property zum Besitzer.</summary>
    [JsonIgnore]
    public virtual AppUser? Besitzer { get; set; }

    // Navigation Properties
    public virtual ICollection<Benutzeranforderung> Benutzeranforderungen { get; set; } = new List<Benutzeranforderung>();
    public virtual ICollection<DigitalisierungsVorschlag> Vorschlaege { get; set; } = new List<DigitalisierungsVorschlag>();
}

/// <summary>
/// Moegliche Status-Werte eines Projekts.
/// </summary>
public enum ProjektStatus
{
    Geplant,
    InBearbeitung,
    InReview,
    Abgeschlossen,
    Pausiert
}
