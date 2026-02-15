using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using DigitalisierungsManager.Models;
using System.Globalization;

namespace DigitalisierungsManager.Services;

/// <summary>
/// Implementierung des Datenaustausch-Service fuer JSON, CSV und XML Export/Import.
/// </summary>
public class DataExchangeService : IDataExchangeService
{
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<DataExchangeService> _logger;

    public DataExchangeService(ILogger<DataExchangeService> logger)
    {
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Enum-Werte als Strings (z.B. "InBearbeitung") serialisieren/deserialisieren
        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    /// <inheritdoc />
    public string ExportProjekteToJson(List<Projekt> projekte)
    {
        _logger.LogInformation("JSON-Export gestartet fuer {Count} Projekte", projekte.Count);
        return JsonSerializer.Serialize(projekte, _jsonOptions);
    }

    /// <inheritdoc />
    public List<Projekt> ImportProjekteFromJson(string jsonContent)
    {
        _logger.LogInformation("JSON-Import gestartet");
        var projekte = JsonSerializer.Deserialize<List<Projekt>>(jsonContent, _jsonOptions)
               ?? new List<Projekt>();
        _logger.LogInformation("{Count} Projekte aus JSON importiert", projekte.Count);
        return projekte;
    }

    /// <inheritdoc />
    public byte[] ExportProjekteToCsv(List<Projekt> projekte)
    {
        _logger.LogInformation("CSV-Export gestartet fuer {Count} Projekte", projekte.Count);
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            Encoding = Encoding.UTF8
        });

        csv.WriteRecords(projekte.Select(p => new
        {
            p.Id,
            p.Titel,
            p.Beschreibung,
            Status = p.Status.ToString(),
            p.Technologie,
            ErstelltAm = p.ErstelltAm.ToString("yyyy-MM-dd HH:mm:ss"),
            Abschlussdatum = p.Abschlussdatum?.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
            p.Verantwortlicher
        }));

        writer.Flush();
        return memoryStream.ToArray();
    }

    /// <inheritdoc />
    public async Task<List<Projekt>> ImportProjekteFromCsvAsync(Stream csvStream)
    {
        _logger.LogInformation("CSV-Import gestartet");
        using var reader = new StreamReader(csvStream, Encoding.UTF8);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            Encoding = Encoding.UTF8,
            HeaderValidated = null,
            MissingFieldFound = null
        });

        var records = new List<Projekt>();
        await foreach (var record in csv.GetRecordsAsync<dynamic>())
        {
            var projekt = new Projekt
            {
                Titel = record.Titel?.ToString() ?? "",
                Beschreibung = record.Beschreibung?.ToString() ?? "",
                Technologie = record.Technologie?.ToString() ?? "",
                Verantwortlicher = record.Verantwortlicher?.ToString() ?? ""
            };

            if (Enum.TryParse<ProjektStatus>(record.Status?.ToString(), out ProjektStatus status))
                projekt.Status = status;

            if (DateTime.TryParse(record.ErstelltAm?.ToString(), out DateTime erstelltAm))
                projekt.ErstelltAm = erstelltAm;

            var abschlussText = record.Abschlussdatum?.ToString();
            if (!string.IsNullOrEmpty(abschlussText))
            {
                if (DateTime.TryParse(abschlussText, out DateTime ab))
                {
                    projekt.Abschlussdatum = ab;
                }
            }

            records.Add(projekt);
        }

        _logger.LogInformation("{Count} Projekte aus CSV importiert", records.Count);
        return records;
    }

    /// <inheritdoc />
    public string ExportToXml(List<Projekt> projekte)
    {
        _logger.LogInformation("XML-Export gestartet fuer {Count} Projekte", projekte.Count);
        var xml = new XElement("Projekte",
            projekte.Select(p => new XElement("Projekt",
                new XElement("Id", p.Id),
                new XElement("Titel", p.Titel),
                new XElement("Beschreibung", p.Beschreibung),
                new XElement("Status", p.Status.ToString()),
                new XElement("Technologie", p.Technologie),
                new XElement("ErstelltAm", p.ErstelltAm.ToString("yyyy-MM-dd HH:mm:ss")),
                new XElement("Verantwortlicher", p.Verantwortlicher)
            ))
        );

        return xml.ToString();
    }
}
