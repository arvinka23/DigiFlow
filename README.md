# DigiFlow - Digitalisierungs-Manager

Ein modernes Blazor-Server-Webprojekt zur Verwaltung von Digitalisierungsprojekten, Benutzeranforderungen und Digitalisierungsvorschlaegen.

## Tech-Stack

| Komponente | Technologie |
|---|---|
| Framework | .NET 8.0 |
| Frontend | Blazor Server |
| UI-Framework | Bootstrap 5.3 |
| ORM | Entity Framework Core 8.0 |
| Datenbank | SQLite (Standard) / SQL Server |
| Tests | xUnit + EF Core InMemory |
| CI/CD | GitHub Actions |
| Container | Docker |
| Hosting | Render.com (optional) |

## Architektur

```
DigiFlow/
├── Models/              # Datenmodelle (Projekt, Benutzeranforderung, DigitalisierungsVorschlag)
├── Data/                # DbContext und Seed-Daten
├── Services/            # Business-Logik (Interface + Implementierung)
│   ├── IProjektService / ProjektService         # CRUD, Suche, Filterung
│   ├── IDataExchangeService / DataExchangeService # JSON/CSV/XML Import & Export
│   └── ISqlQueryService / SqlQueryService       # Schreibgeschuetzte SQL-Abfragen
├── Pages/               # Blazor-Seiten (Index, Projekte, Datenaustausch, SQL)
├── Shared/              # Layout, Navigation, Toast-Komponente, Hilfsfunktionen
├── wwwroot/             # Statische Dateien (CSS, JS)
└── DigitalisierungsManager.Tests/  # Unit-Tests
```

### Design-Prinzipien

- **Interface-basierte Services** mit Dependency Injection
- **Schichtenarchitektur**: Models -> Data -> Services -> Pages
- **Validierung** via DataAnnotations auf allen Models
- **Sicherheit**: SQL-Query-Interface nur mit Lesezugriff (SELECT), gefaehrliche Keywords blockiert
- **Cross-DB-Kompatibilitaet**: Laeuft mit SQLite (Standard/Docker) und SQL Server

## Schnellstart

### Voraussetzungen

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Starten

```bash
# NuGet-Pakete wiederherstellen
dotnet restore

# Anwendung starten
dotnet run
```

Die Anwendung ist dann unter `https://localhost:5001` oder `http://localhost:5000` erreichbar.
Die SQLite-Datenbank und Seed-Daten werden automatisch beim ersten Start erstellt.

### Mit Docker

```bash
docker build -t digiflow .
docker run -p 8080:8080 digiflow
```

### Tests ausfuehren

```bash
dotnet test DigitalisierungsManager.Tests/DigitalisierungsManager.Tests.csproj
```

## Features

- **Dashboard**: Uebersicht aller Projekte mit Statistiken
- **Projektverwaltung**: Erstellen, Bearbeiten, Loeschen mit Validierung und Bestaetigungsdialog
- **Suche & Filter**: Volltextsuche und Filterung nach Projektstatus
- **Datenaustausch**: Import/Export in JSON, CSV und XML mit echtem Datei-Download
- **SQL-Interface**: Schreibgeschuetztes SQL-Query-Interface mit Sicherheitspruefungen
- **Toast-Benachrichtigungen**: Feedback bei Aktionen (Erstellen, Loeschen, Importieren, etc.)
- **Loading-States**: Spinner waehrend Datenladen auf allen Seiten
- **Fehlerbehandlung**: ErrorBoundary und try/catch auf allen Seiten

## Datenbank-Provider

Die Anwendung unterstuetzt zwei Datenbank-Provider:

| Provider | Umgebungsvariable | Standard |
|---|---|---|
| SQLite | `DB_PROVIDER=Sqlite` | Ja (Docker, Entwicklung) |
| SQL Server | `DB_PROVIDER=SqlServer` | Nein |

Connection Strings werden in `appsettings.json` konfiguriert.

## Lizenz

Dieses Projekt dient als Demo-/Lernprojekt.
