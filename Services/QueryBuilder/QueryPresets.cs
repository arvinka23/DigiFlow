namespace DigitalisierungsManager.Services.QueryBuilder;

/// <summary>
/// Eine vorgefertigte SELECT-Abfrage mit Klartext-Beschreibung.
/// </summary>
public record QueryPreset(string Name, string Description, string Sql);

/// <summary>
/// Kuratierte Beispielabfragen, die auf dem tatsaechlichen Schema
/// von DigiFlow laufen (SQLite-kompatibel).
/// </summary>
public static class QueryPresets
{
    public static readonly IReadOnlyList<QueryPreset> All = new[]
    {
        new QueryPreset(
            "Alle Projekte im Ueberblick",
            "Zeigt Titel, Status, Technologie, Verantwortlichen und Erstellungsdatum.",
@"SELECT Id, Titel, Status, Technologie, Verantwortlicher, ErstelltAm
FROM Projekte
ORDER BY ErstelltAm DESC"),

        new QueryPreset(
            "Projekte mit Anzahl Anforderungen und Vorschlaege",
            "Join ueber alle drei Tabellen mit Aggregat pro Projekt.",
@"SELECT p.Id, p.Titel, p.Status,
    COUNT(DISTINCT b.Id) AS AnzahlAnforderungen,
    COUNT(DISTINCT v.Id) AS AnzahlVorschlaege
FROM Projekte p
LEFT JOIN Benutzeranforderungen b ON p.Id = b.ProjektId
LEFT JOIN DigitalisierungsVorschlaege v ON p.Id = v.ProjektId
GROUP BY p.Id, p.Titel, p.Status
ORDER BY AnzahlAnforderungen DESC"),

        new QueryPreset(
            "Pausierte Projekte",
            "Alle Projekte mit Status 'Pausiert'.",
@"SELECT Id, Titel, Verantwortlicher, ErstelltAm
FROM Projekte
WHERE Status = 'Pausiert'
ORDER BY ErstelltAm DESC"),

        new QueryPreset(
            "Projekte mit mehr als 3 Anforderungen",
            "Alle Projekte mit > 3 Anforderungen.",
@"SELECT p.Id, p.Titel, COUNT(b.Id) AS Anzahl
FROM Projekte p
JOIN Benutzeranforderungen b ON p.Id = b.ProjektId
GROUP BY p.Id, p.Titel
HAVING COUNT(b.Id) > 3
ORDER BY Anzahl DESC"),

        new QueryPreset(
            "In Bearbeitung seit >60 Tagen (SQLite)",
            "Projekte mit Status 'InBearbeitung', deren Erstellung mehr als 60 Tage zurueckliegt.",
@"SELECT Id, Titel, Verantwortlicher, ErstelltAm
FROM Projekte
WHERE Status = 'InBearbeitung'
  AND ErstelltAm < datetime('now', '-60 days')
ORDER BY ErstelltAm ASC"),

        new QueryPreset(
            "Offene Anforderungen mit Prioritaet Hoch/Kritisch",
            "Alle offenen Anforderungen mit hoher oder kritischer Prioritaet.",
@"SELECT b.Id, b.Titel, b.Prioritaet, p.Titel AS Projekt
FROM Benutzeranforderungen b
JOIN Projekte p ON p.Id = b.ProjektId
WHERE b.Status = 'Offen'
  AND b.Prioritaet IN ('Hoch', 'Kritisch')
ORDER BY b.Prioritaet DESC, b.ErstelltAm ASC"),

        new QueryPreset(
            "Angenommene Vorschlaege pro Projekt",
            "Zeigt pro Projekt die Anzahl angenommener vs. offener Vorschlaege.",
@"SELECT p.Titel,
    SUM(CASE WHEN v.IstAngenommen = 1 THEN 1 ELSE 0 END) AS Angenommen,
    SUM(CASE WHEN v.IstAngenommen = 0 THEN 1 ELSE 0 END) AS Offen
FROM Projekte p
LEFT JOIN DigitalisierungsVorschlaege v ON p.Id = v.ProjektId
GROUP BY p.Titel
HAVING COUNT(v.Id) > 0
ORDER BY Angenommen DESC"),

        new QueryPreset(
            "Status-Verteilung",
            "Anzahl der Projekte je Status.",
@"SELECT Status, COUNT(*) AS Anzahl
FROM Projekte
GROUP BY Status
ORDER BY Anzahl DESC"),

        new QueryPreset(
            "Technologie-Nutzung",
            "Welche Technologien werden wie oft eingesetzt.",
@"SELECT Technologie, COUNT(*) AS Anzahl
FROM Projekte
WHERE Technologie IS NOT NULL AND Technologie <> ''
GROUP BY Technologie
ORDER BY Anzahl DESC"),

        new QueryPreset(
            "Top-5 fleissigste Verantwortliche",
            "Die 5 Personen mit den meisten zugeordneten Projekten.",
@"SELECT Verantwortlicher, COUNT(*) AS Projekte
FROM Projekte
WHERE Verantwortlicher IS NOT NULL AND Verantwortlicher <> ''
GROUP BY Verantwortlicher
ORDER BY Projekte DESC
LIMIT 5"),
    };
}
