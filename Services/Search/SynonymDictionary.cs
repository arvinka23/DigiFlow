namespace DigitalisierungsManager.Services.Search;

/// <summary>
/// Statisches Synonym-Lexikon fuer die deutsche/englische Domaene
/// "Digitalisierung / Prozesse / IT". Damit findet "Rechnung" auch Eintraege
/// mit "Invoice" oder "Beleg", ohne ein externes Embedding-Modell zu brauchen.
/// </summary>
public static class SynonymDictionary
{
    /// <summary>Gruppen von Synonymen -- normalisiert (lowercase).</summary>
    private static readonly string[][] Groups =
    {
        new[] { "rechnung", "rechnungen", "invoice", "invoices", "beleg", "belege", "faktura" },
        new[] { "urlaub", "abwesenheit", "ferien", "leave", "vacation", "freistellung" },
        new[] { "onboarding", "einarbeitung", "neuer mitarbeiter", "neueintritt", "hiring" },
        new[] { "offboarding", "austritt", "kuendigung" },
        new[] { "spesen", "reisekosten", "auslagen", "expense", "expenses" },
        new[] { "vertrag", "vertraege", "contract", "contracts", "nda" },
        new[] { "bestellung", "bestellungen", "beschaffung", "purchase", "procurement", "einkauf" },
        new[] { "kundenanfrage", "kundenanfragen", "ticket", "tickets", "support", "helpdesk" },
        new[] { "mitarbeiter", "personal", "employee", "employees", "staff" },
        new[] { "excel", "spreadsheet", "tabelle", "xlsx" },
        new[] { "ocr", "zeichenerkennung", "texterkennung", "document intelligence" },
        new[] { "rpa", "bot", "automation", "uipath", "automate" },
        new[] { "workflow", "prozess", "ablauf", "process" },
        new[] { "datenbank", "database", "db", "sql" },
        new[] { "dashboard", "auswertung", "bericht", "report", "reporting", "analytics" },
        new[] { "api", "schnittstelle", "integration" },
        new[] { "portal", "webseite", "website", "webapp" },
        new[] { "mobile", "app", "smartphone", "handy" },
        new[] { "ki", "ai", "ml", "kuenstliche intelligenz", "machine learning" },
        new[] { "security", "sicherheit", "datenschutz", "dsgvo", "gdpr" },
    };

    private static readonly Dictionary<string, HashSet<string>> Index = BuildIndex();

    /// <summary>Gibt alle Synonyme zum gegebenen Wort zurueck (inkl. Eingabewort).</summary>
    public static IReadOnlyCollection<string> Expand(string term)
    {
        if (string.IsNullOrWhiteSpace(term)) return Array.Empty<string>();
        var key = term.Trim().ToLowerInvariant();
        return Index.TryGetValue(key, out var set) ? set : new HashSet<string> { key };
    }

    private static Dictionary<string, HashSet<string>> BuildIndex()
    {
        var map = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var group in Groups)
        {
            var set = new HashSet<string>(group, StringComparer.OrdinalIgnoreCase);
            foreach (var word in group)
                map[word] = set;
        }
        return map;
    }
}
