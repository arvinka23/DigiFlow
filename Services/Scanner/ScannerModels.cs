using DigitalisierungsManager.Models;

namespace DigitalisierungsManager.Services.Scanner;

/// <summary>
/// Draft eines Projekts, das aus einer Prozessbeschreibung erzeugt wurde.
/// Wird NICHT direkt gespeichert -- der Nutzer bestaetigt/editiert zuerst.
/// </summary>
public class ProjectDraft
{
    public string Titel { get; set; } = string.Empty;
    public string Beschreibung { get; set; } = string.Empty;
    public string Technologie { get; set; } = string.Empty;
    public string Verantwortlicher { get; set; } = string.Empty;
    public ProjektStatus Status { get; set; } = ProjektStatus.Geplant;
    public List<AnforderungDraft> Anforderungen { get; set; } = new();
    public List<VorschlagDraft> Vorschlaege { get; set; } = new();
    public RoiEstimate Roi { get; set; } = new();
    public List<string> Risiken { get; set; } = new();
    public string MatchedTemplateName { get; set; } = string.Empty;
    public double Confidence { get; set; }
}

public class AnforderungDraft
{
    public string Titel { get; set; } = string.Empty;
    public string Beschreibung { get; set; } = string.Empty;
    public Prioritaet Prioritaet { get; set; } = Prioritaet.Mittel;
}

public class VorschlagDraft
{
    public string Titel { get; set; } = string.Empty;
    public string Beschreibung { get; set; } = string.Empty;
    public Vorschlagstyp Vorschlagstyp { get; set; } = Vorschlagstyp.Eigenentwicklung;
    public string EmpfohleneTechnologie { get; set; } = string.Empty;
    public string Begruendung { get; set; } = string.Empty;
    public int AufwandTage { get; set; }
    public string Komplexitaet { get; set; } = "Mittel";
}

public class RoiEstimate
{
    public decimal JaehrlicheErsparnisEuro { get; set; }
    public decimal EinmaligerAufwandEuro { get; set; }
    public int AmortisationMonate { get; set; }
    public string Begruendung { get; set; } = string.Empty;
}
