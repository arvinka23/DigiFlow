using DigitalisierungsManager.Models;

namespace DigitalisierungsManager.Services;

/// <summary>
/// Service-Interface fuer den Import und Export von Projektdaten in verschiedenen Formaten.
/// </summary>
public interface IDataExchangeService
{
    /// <summary>Exportiert Projekte als formatierten JSON-String.</summary>
    string ExportProjekteToJson(List<Projekt> projekte);

    /// <summary>Importiert Projekte aus einem JSON-String.</summary>
    List<Projekt> ImportProjekteFromJson(string jsonContent);

    /// <summary>Exportiert Projekte als CSV-Byte-Array (UTF-8, Semikolon-getrennt).</summary>
    byte[] ExportProjekteToCsv(List<Projekt> projekte);

    /// <summary>Importiert Projekte aus einem CSV-Stream.</summary>
    Task<List<Projekt>> ImportProjekteFromCsvAsync(Stream csvStream);

    /// <summary>Exportiert Projekte als formatierten XML-String.</summary>
    string ExportToXml(List<Projekt> projekte);
}
