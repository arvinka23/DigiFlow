using DigitalisierungsManager.Models;

namespace DigitalisierungsManager.Services.Scanner;

/// <summary>
/// Bibliothek fertiger Prozess-Templates. Funktioniert komplett offline und ohne KI.
/// Matching erfolgt ueber Keywords in der Prozessbeschreibung.
/// </summary>
internal static class ProcessTemplateLibrary
{
    internal class ProcessTemplate
    {
        public string Name { get; init; } = string.Empty;
        public string[] Keywords { get; init; } = Array.Empty<string>();
        public string TitelTemplate { get; init; } = string.Empty;
        public string Technologie { get; init; } = string.Empty;
        public List<AnforderungDraft> Anforderungen { get; init; } = new();
        public List<VorschlagDraft> Vorschlaege { get; init; } = new();
        public RoiEstimate Roi { get; init; } = new();
        public List<string> Risiken { get; init; } = new();
    }

    internal static readonly List<ProcessTemplate> Templates = new()
    {
        new ProcessTemplate
        {
            Name = "Rechnungseingang",
            Keywords = new[] { "rechnung", "eingangsrechnung", "invoice", "beleg", "kreditor", "lieferant" },
            TitelTemplate = "Rechnungseingang digitalisieren",
            Technologie = "OCR + Power Automate + ERP-Schnittstelle",
            Anforderungen = new()
            {
                new() { Titel = "Automatische Rechnungsdaten-Extraktion", Beschreibung = "Rechnungen werden per Mail oder Upload erfasst; Kopf- und Positionsdaten automatisch erkannt.", Prioritaet = Prioritaet.Hoch },
                new() { Titel = "Freigabe-Workflow mit Vertretung", Beschreibung = "Digitale Freigabe durch Fachbereich und Finanz; automatische Vertretungsregelung bei Abwesenheit.", Prioritaet = Prioritaet.Hoch },
                new() { Titel = "Revisionssichere Archivierung", Beschreibung = "Alle Rechnungen werden GoBD-konform mit Pruefpfad archiviert.", Prioritaet = Prioritaet.Kritisch },
            },
            Vorschlaege = new()
            {
                new() { Titel = "Power Automate + Azure AI Document Intelligence", Vorschlagstyp = Vorschlagstyp.Mischloesung, EmpfohleneTechnologie = "Power Automate, Azure AI Document Intelligence, SharePoint", Beschreibung = "Low-Code-Workflow mit KI-basierter OCR. Gute Balance aus Flexibilitaet und Geschwindigkeit.", Begruendung = "Schnell eingefuehrt, spaeter ausbaubar; nutzt bestehende M365-Lizenz.", AufwandTage = 25, Komplexitaet = "Mittel" },
                new() { Titel = "Fertige SaaS-Loesung (Candis / Circula)", Vorschlagstyp = Vorschlagstyp.FremdSoftware, EmpfohleneTechnologie = "SaaS (Candis / Circula / sevdesk)", Beschreibung = "Vorkonfiguriertes Produkt mit eingebauter OCR, Freigabe-App und ERP-Konnektor.", Begruendung = "Geringster Implementierungsaufwand; monatliche Kosten aber dauerhaft.", AufwandTage = 10, Komplexitaet = "Niedrig" },
                new() { Titel = "UiPath RPA-Bot", Vorschlagstyp = Vorschlagstyp.Eigenentwicklung, EmpfohleneTechnologie = "UiPath", Beschreibung = "RPA-Bot liest Rechnungen aus Postfach, buchtet Eckdaten ins ERP und legt Beleg ab.", Begruendung = "Sinnvoll, wenn ERP keine moderne API bietet; aber wartungsintensiv.", AufwandTage = 40, Komplexitaet = "Hoch" },
            },
            Roi = new() { JaehrlicheErsparnisEuro = 28000, EinmaligerAufwandEuro = 18000, AmortisationMonate = 8, Begruendung = "Ca. 3000 Rechnungen/Jahr, 15 min Einsparung pro Rechnung, 30 EUR/h Kostensatz." },
            Risiken = new()
            {
                "OCR-Qualitaet bei handgeschriebenen oder gescannten Rechnungen variiert.",
                "Integration in ERP benoetigt geklaerten Kontenrahmen und Kostenstellenlogik.",
                "Change-Management: Buchhaltung muss neue Tools akzeptieren."
            }
        },
        new ProcessTemplate
        {
            Name = "Urlaubsantrag",
            Keywords = new[] { "urlaub", "abwesenheit", "ferien", "freistellung" },
            TitelTemplate = "Urlaubsantrag digitalisieren",
            Technologie = "Power Apps + SharePoint + Outlook",
            Anforderungen = new()
            {
                new() { Titel = "Self-Service-Antrag via App", Beschreibung = "Mitarbeiter stellen Urlaubsantraege per Web- oder Mobile-App.", Prioritaet = Prioritaet.Hoch },
                new() { Titel = "Live-Kontingent-Anzeige", Beschreibung = "Verbleibender Urlaubsanspruch wird in Echtzeit angezeigt.", Prioritaet = Prioritaet.Mittel },
                new() { Titel = "Vorgesetzten-Freigabe mit Vertretung", Beschreibung = "Einfaches Approve/Reject durch Vorgesetzten, inkl. Vertretungsregel.", Prioritaet = Prioritaet.Hoch },
                new() { Titel = "Kalender-Integration", Beschreibung = "Genehmigte Abwesenheiten erscheinen im Team-Kalender und Outlook.", Prioritaet = Prioritaet.Mittel },
            },
            Vorschlaege = new()
            {
                new() { Titel = "Power Apps + Power Automate", Vorschlagstyp = Vorschlagstyp.Eigenentwicklung, EmpfohleneTechnologie = "Power Apps, Power Automate, SharePoint", Beschreibung = "Low-Code-App mit Freigabeflow und Kalender-Integration.", Begruendung = "Minimaler Entwicklungsaufwand; nutzt M365-Lizenz.", AufwandTage = 15, Komplexitaet = "Niedrig" },
                new() { Titel = "Personio / HRworks", Vorschlagstyp = Vorschlagstyp.FremdSoftware, EmpfohleneTechnologie = "Personio / HRworks / Factorial", Beschreibung = "Vollstaendiges HR-Tool mit Urlaub, Lohnvorbereitung, Zeiterfassung.", Begruendung = "Sinnvoll wenn mehr als nur Urlaub abgedeckt werden soll.", AufwandTage = 12, Komplexitaet = "Niedrig" },
            },
            Roi = new() { JaehrlicheErsparnisEuro = 9000, EinmaligerAufwandEuro = 6000, AmortisationMonate = 8, Begruendung = "50 Mitarbeiter, 4 Urlaubsantraege/Jahr, 20 min Einsparung pro Antrag (Mitarbeiter + HR + Vorgesetzter)." },
            Risiken = new()
            {
                "Daten aus Altsystem muessen migriert werden (Resturlaub, Kontingente).",
                "Datenschutz: personenbezogene Daten bleiben DSGVO-relevant."
            }
        },
        new ProcessTemplate
        {
            Name = "Mitarbeiter-Onboarding",
            Keywords = new[] { "onboarding", "neuer mitarbeiter", "einarbeitung", "offboarding" },
            TitelTemplate = "Mitarbeiter-Onboarding digitalisieren",
            Technologie = "Workflow-Engine + AD + Ticketing",
            Anforderungen = new()
            {
                new() { Titel = "Checkliste je Rolle", Beschreibung = "Automatisch generierte Aufgaben (Laptop, AD-Konto, Berechtigungen) je Stellentyp.", Prioritaet = Prioritaet.Hoch },
                new() { Titel = "IT-Provisioning-Automatisierung", Beschreibung = "AD-Konto, Mail-Postfach, Standardsoftware werden per Klick ausgeloest.", Prioritaet = Prioritaet.Hoch },
                new() { Titel = "Dokumenten-Sammelmappe", Beschreibung = "Vertrag, Unterweisungen, Richtlinien zentral hinterlegt.", Prioritaet = Prioritaet.Mittel },
            },
            Vorschlaege = new()
            {
                new() { Titel = "Power Automate + Microsoft Graph", Vorschlagstyp = Vorschlagstyp.Eigenentwicklung, EmpfohleneTechnologie = "Power Automate, Microsoft Graph, Entra ID", Beschreibung = "Orchestrator prueft HR-System und legt automatisch IT-Ressourcen an.", Begruendung = "Nutzt bestehende Microsoft-Infrastruktur vollstaendig.", AufwandTage = 20, Komplexitaet = "Mittel" },
                new() { Titel = "Workday / Personio Onboarding-Modul", Vorschlagstyp = Vorschlagstyp.FremdSoftware, EmpfohleneTechnologie = "Personio / Workday Onboarding", Beschreibung = "Fertiger Onboarding-Prozess inkl. Formulare und Aufgabenlisten.", Begruendung = "Konsistenz mit HR-Stammdaten, weniger Eigenentwicklung.", AufwandTage = 10, Komplexitaet = "Niedrig" },
            },
            Roi = new() { JaehrlicheErsparnisEuro = 12000, EinmaligerAufwandEuro = 15000, AmortisationMonate = 15, Begruendung = "20 Neueinstellungen/Jahr, 4h Einsparung pro Fall fuer HR und IT zusammen." },
            Risiken = new() { "AD-Automatisierung muss strikt getestet werden (falsche Rechte = Sicherheitsproblem).", "Change-Kommunikation noetig, damit alle beteiligten Abteilungen mitziehen." }
        },
        new ProcessTemplate
        {
            Name = "Spesenabrechnung",
            Keywords = new[] { "spesen", "reisekosten", "auslagen", "expense", "reisen" },
            TitelTemplate = "Spesen- und Reisekosten digitalisieren",
            Technologie = "Mobile-App + OCR + ERP-Schnittstelle",
            Anforderungen = new()
            {
                new() { Titel = "Beleg per Foto erfassen", Beschreibung = "Mitarbeiter fotografiert Beleg; App liest Datum, Betrag, MwSt automatisch.", Prioritaet = Prioritaet.Hoch },
                new() { Titel = "Kreditkarten-Import", Beschreibung = "Automatischer Import von Firmenkreditkarten-Transaktionen.", Prioritaet = Prioritaet.Mittel },
                new() { Titel = "Policy-Check", Beschreibung = "Abrechnung wird gegen Reise-Richtlinien geprueft (max. Satz, Kategorie, Vorgesetzten-Limit).", Prioritaet = Prioritaet.Hoch },
            },
            Vorschlaege = new()
            {
                new() { Titel = "Circula / Mobilexpense / SAP Concur", Vorschlagstyp = Vorschlagstyp.FremdSoftware, EmpfohleneTechnologie = "Circula / Mobilexpense / SAP Concur", Beschreibung = "Standard-SaaS mit Mobile-App, OCR und Buchungs-Export.", Begruendung = "Reife Loesung, kurze Einfuehrungszeit.", AufwandTage = 12, Komplexitaet = "Niedrig" },
                new() { Titel = "Power Apps Eigenbau", Vorschlagstyp = Vorschlagstyp.Eigenentwicklung, EmpfohleneTechnologie = "Power Apps, Dataverse", Beschreibung = "Eigenentwicklung passend zu bestehenden Richtlinien.", Begruendung = "Maximaler Fit, hoeherer Aufwand.", AufwandTage = 30, Komplexitaet = "Hoch" },
            },
            Roi = new() { JaehrlicheErsparnisEuro = 18000, EinmaligerAufwandEuro = 12000, AmortisationMonate = 8, Begruendung = "100 Abrechnungen/Monat, 20 min Einsparung pro Abrechnung." },
            Risiken = new() { "Kreditkarten-Integration benoetigt Bank-Schnittstelle.", "MwSt-Erkennung international (auslaendische Belege) fehleranfaellig." }
        },
        new ProcessTemplate
        {
            Name = "Vertragsverwaltung",
            Keywords = new[] { "vertrag", "contract", "vertragsmanagement", "nda", "vertragsverlaengerung" },
            TitelTemplate = "Vertragsverwaltung zentralisieren",
            Technologie = "DMS + Volltext-Suche + Erinnerungen",
            Anforderungen = new()
            {
                new() { Titel = "Volltext-Durchsuchbarkeit", Beschreibung = "Alle Vertraege OCR-indiziert und durchsuchbar.", Prioritaet = Prioritaet.Hoch },
                new() { Titel = "Fristen-Erinnerungen", Beschreibung = "Automatische Mails vor Ablauf/Kuendigungsfrist.", Prioritaet = Prioritaet.Kritisch },
                new() { Titel = "Rollen- und Mandantentrennung", Beschreibung = "Nur berechtigte Personen sehen sensible Vertraege.", Prioritaet = Prioritaet.Hoch },
            },
            Vorschlaege = new()
            {
                new() { Titel = "DocuWare / ELO", Vorschlagstyp = Vorschlagstyp.FremdSoftware, EmpfohleneTechnologie = "DocuWare / ELO / M-Files", Beschreibung = "Enterprise-DMS mit Vertragsmodul.", Begruendung = "Reife Loesung, auch fuer regulierte Branchen geeignet.", AufwandTage = 25, Komplexitaet = "Mittel" },
                new() { Titel = "SharePoint + Power Automate", Vorschlagstyp = Vorschlagstyp.Mischloesung, EmpfohleneTechnologie = "SharePoint Online, Power Automate, Syntex", Beschreibung = "Vertragsbibliothek mit Metadaten-Extraktion und Erinnerungen.", Begruendung = "Nutzt bestehende M365-Lizenz, geringe Zusatzkosten.", AufwandTage = 20, Komplexitaet = "Mittel" },
            },
            Roi = new() { JaehrlicheErsparnisEuro = 15000, EinmaligerAufwandEuro = 20000, AmortisationMonate = 16, Begruendung = "Vermeidung unbeabsichtigter Vertragsverlaengerungen (~8 Vertraege/Jahr * 1.500 EUR Risiko)." },
            Risiken = new() { "Migration bestehender Papierarchive ist aufwendig.", "Datenschutz bei personenbezogenen Vertraegen beachten." }
        },
        new ProcessTemplate
        {
            Name = "Bestellprozess / Beschaffung",
            Keywords = new[] { "bestell", "beschaffung", "einkauf", "procure", "purchase", "materialbestellung" },
            TitelTemplate = "Bestell- und Beschaffungsprozess digitalisieren",
            Technologie = "eProcurement-Portal + ERP-Integration",
            Anforderungen = new()
            {
                new() { Titel = "Katalog-basierte Bestellung", Beschreibung = "Mitarbeiter bestellen aus hinterlegtem Lieferantenkatalog mit Preisen.", Prioritaet = Prioritaet.Hoch },
                new() { Titel = "Budget-Pruefung in Echtzeit", Beschreibung = "Bestellanforderungen werden gegen verbleibendes Budget der Kostenstelle geprueft.", Prioritaet = Prioritaet.Hoch },
                new() { Titel = "Mehrstufige Freigabe", Beschreibung = "Genehmigungs-Limits je Rolle; automatische Eskalation bei Ueberschreitung.", Prioritaet = Prioritaet.Mittel },
            },
            Vorschlaege = new()
            {
                new() { Titel = "ERP-eigenes Einkaufsmodul (SAP Ariba / Business Central)", Vorschlagstyp = Vorschlagstyp.FremdSoftware, EmpfohleneTechnologie = "SAP Ariba / MS Business Central", Beschreibung = "Standard-Einkaufsmodul direkt im ERP.", Begruendung = "Nahtlose Integration; keine Doppel-Stammdaten.", AufwandTage = 30, Komplexitaet = "Mittel" },
                new() { Titel = "Power Apps Bestellportal", Vorschlagstyp = Vorschlagstyp.Eigenentwicklung, EmpfohleneTechnologie = "Power Apps, Dataverse, Dynamics", Beschreibung = "Custom-Portal mit ERP-Anbindung ueber REST.", Begruendung = "Maximale Flexibilitaet, passt zu Sonderprozessen.", AufwandTage = 45, Komplexitaet = "Hoch" },
            },
            Roi = new() { JaehrlicheErsparnisEuro = 35000, EinmaligerAufwandEuro = 30000, AmortisationMonate = 11, Begruendung = "Schnellere Prozesszeiten + Maverick-Buying-Reduktion bei 2000 Bestellungen/Jahr." },
            Risiken = new() { "Lieferanten-Stammdaten muessen erstmalig sauber gepflegt werden.", "Change-Resistance in Fachabteilungen." }
        },
        new ProcessTemplate
        {
            Name = "Kundenanfragen",
            Keywords = new[] { "kundenanfrage", "anfrage", "support", "ticket", "helpdesk", "kundenanliegen" },
            TitelTemplate = "Kundenanfragen zentral bearbeiten",
            Technologie = "Ticketing + CRM + AI-Klassifikation",
            Anforderungen = new()
            {
                new() { Titel = "Multi-Channel-Erfassung", Beschreibung = "Anfragen aus E-Mail, Formular und Telefon landen in einer zentralen Queue.", Prioritaet = Prioritaet.Hoch },
                new() { Titel = "Automatische Kategorisierung", Beschreibung = "Regelbasierte Zuordnung zu Team/Kategorie.", Prioritaet = Prioritaet.Mittel },
                new() { Titel = "SLA-Tracking", Beschreibung = "Reaktions- und Loesungszeiten werden gemessen; Eskalation bei Drohverletzung.", Prioritaet = Prioritaet.Hoch },
            },
            Vorschlaege = new()
            {
                new() { Titel = "Zendesk / Freshdesk / Zammad", Vorschlagstyp = Vorschlagstyp.FremdSoftware, EmpfohleneTechnologie = "Zendesk / Freshdesk / Zammad", Beschreibung = "Reifes Ticket-System mit Multi-Channel und Reporting.", Begruendung = "Schneller Go-Live; reichhaltiges Feature-Set.", AufwandTage = 15, Komplexitaet = "Niedrig" },
                new() { Titel = "Microsoft Dynamics Customer Service", Vorschlagstyp = Vorschlagstyp.FremdSoftware, EmpfohleneTechnologie = "Dynamics 365 Customer Service", Beschreibung = "Integration mit weiteren Dynamics-Modulen (Sales, Marketing).", Begruendung = "Sinnvoll bei bestehender Dynamics-Landschaft.", AufwandTage = 25, Komplexitaet = "Mittel" },
            },
            Roi = new() { JaehrlicheErsparnisEuro = 22000, EinmaligerAufwandEuro = 14000, AmortisationMonate = 8, Begruendung = "Schnellere Reaktion, weniger Mehrfachbearbeitung bei ca. 5000 Anfragen/Jahr." },
            Risiken = new() { "Mitarbeiter-Schulung notwendig.", "Alte Mail-Historie muss ggf. migriert werden." }
        },
        new ProcessTemplate
        {
            Name = "Excel-Ersatz",
            Keywords = new[] { "excel", "tabelle", "liste", "xlsx", "access", "manuell" },
            TitelTemplate = "Excel-Liste durch zentrale Datenbank ersetzen",
            Technologie = "Webformular + SQL + Rollen/Rechte",
            Anforderungen = new()
            {
                new() { Titel = "Konsistente Pflichtfelder", Beschreibung = "Validierung ersetzt ad-hoc-Formeln und fehlende Pruefungen.", Prioritaet = Prioritaet.Hoch },
                new() { Titel = "Mehrbenutzerbetrieb ohne Konflikte", Beschreibung = "Gleichzeitige Bearbeitung ohne Datei-Locks.", Prioritaet = Prioritaet.Hoch },
                new() { Titel = "Historie und Audit-Trail", Beschreibung = "Alle Aenderungen werden nachvollziehbar protokolliert.", Prioritaet = Prioritaet.Mittel },
            },
            Vorschlaege = new()
            {
                new() { Titel = "Power Apps + Dataverse", Vorschlagstyp = Vorschlagstyp.Eigenentwicklung, EmpfohleneTechnologie = "Power Apps, Dataverse", Beschreibung = "Schnelle Low-Code-App statt Excel.", Begruendung = "Geringster Einstieg, gute Skalierung.", AufwandTage = 12, Komplexitaet = "Niedrig" },
                new() { Titel = "Blazor-Custom-App (wie DigiFlow)", Vorschlagstyp = Vorschlagstyp.Eigenentwicklung, EmpfohleneTechnologie = "Blazor Server, EF Core, SQL", Beschreibung = "Eigene Web-App mit vollem Funktionsumfang.", Begruendung = "Lizenzkostenfrei, maximale Kontrolle.", AufwandTage = 25, Komplexitaet = "Mittel" },
            },
            Roi = new() { JaehrlicheErsparnisEuro = 8000, EinmaligerAufwandEuro = 6000, AmortisationMonate = 9, Begruendung = "Vermeidung von Mehrfachpflege und Fehlerbehebung bei wiederkehrender Listenarbeit." },
            Risiken = new() { "Migration der bestehenden Daten muss sauber geplant werden.", "Schulung der Nutzer." }
        },
    };

    internal static ProcessTemplate Generic => new()
    {
        Name = "Allgemeiner Prozess",
        Keywords = Array.Empty<string>(),
        TitelTemplate = "Prozess digitalisieren",
        Technologie = "TBD -- je nach Ist-System",
        Anforderungen = new()
        {
            new() { Titel = "Aktuellen Prozess dokumentieren", Beschreibung = "BPMN- oder Flowchart-Darstellung des Ist-Zustands mit Bearbeitungszeiten.", Prioritaet = Prioritaet.Hoch },
            new() { Titel = "Zielbild definieren", Beschreibung = "Soll-Prozess mit messbaren Verbesserungen (Zeit, Fehler, Kosten).", Prioritaet = Prioritaet.Hoch },
            new() { Titel = "Systemintegration planen", Beschreibung = "Anbindungspunkte an vorhandene IT-Systeme definieren.", Prioritaet = Prioritaet.Mittel },
        },
        Vorschlaege = new()
        {
            new() { Titel = "Low-Code (Power Platform)", Vorschlagstyp = Vorschlagstyp.Eigenentwicklung, EmpfohleneTechnologie = "Power Apps, Power Automate", Beschreibung = "Schnelle Umsetzung mit eigenen Ressourcen.", Begruendung = "Minimaler Einstiegsaufwand.", AufwandTage = 20, Komplexitaet = "Mittel" },
            new() { Titel = "Standard-Software evaluieren", Vorschlagstyp = Vorschlagstyp.FremdSoftware, EmpfohleneTechnologie = "Markt-Scan noetig", Beschreibung = "Vergleich existierender SaaS-Loesungen inkl. Pricing und Funktionsabgleich.", Begruendung = "Nicht jede Anforderung verdient eine Eigenentwicklung.", AufwandTage = 10, Komplexitaet = "Niedrig" },
        },
        Roi = new() { JaehrlicheErsparnisEuro = 10000, EinmaligerAufwandEuro = 10000, AmortisationMonate = 12, Begruendung = "Default-Schaetzung -- bitte mit realen Werten verfeinern." },
        Risiken = new() { "ROI-Annahmen muessen durch Ist-Analyse validiert werden.", "Stakeholder-Abstimmung ist essenziell." }
    };
}
