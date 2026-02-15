using System.ComponentModel.DataAnnotations;

namespace DigitalisierungsManager.Models;

/// <summary>
/// Repraesentiert einen Digitalisierungsvorschlag, der einem Projekt zugeordnet ist.
/// </summary>
public class DigitalisierungsVorschlag
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Titel ist erforderlich.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Titel muss zwischen 3 und 200 Zeichen lang sein.")]
    public string Titel { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Beschreibung darf maximal 2000 Zeichen lang sein.")]
    public string Beschreibung { get; set; } = string.Empty;

    public Vorschlagstyp Vorschlagstyp { get; set; } = Vorschlagstyp.Eigenentwicklung;

    [StringLength(2000, ErrorMessage = "Begruendung darf maximal 2000 Zeichen lang sein.")]
    public string Begruendung { get; set; } = string.Empty;

    public DateTime ErstelltAm { get; set; } = DateTime.UtcNow;

    public bool IstAngenommen { get; set; } = false;

    // Foreign Key
    public int ProjektId { get; set; }
    public virtual Projekt Projekt { get; set; } = null!;
}

/// <summary>
/// Art des Digitalisierungsvorschlags.
/// </summary>
public enum Vorschlagstyp
{
    Eigenentwicklung,
    FremdSoftware,
    Mischloesung
}
