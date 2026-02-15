# 🚀 Quick Start Guide

## Schnellstart (2 Minuten)

### 1. Voraussetzungen prüfen
```powershell
# .NET 8 SDK prüfen
dotnet --version
```

Falls nicht installiert: https://dotnet.microsoft.com/download/dotnet/8.0

### 2. Projekt starten
```powershell
# NuGet-Pakete wiederherstellen
dotnet restore

# Projekt starten
dotnet run
```

### 3. Browser öffnen
- Navigieren Sie zu: `https://localhost:5001` oder `http://localhost:5000`
- Die Datenbank wird automatisch beim ersten Start erstellt!

## Alternative: Mit PowerShell Script
```powershell
# Deployment Script ausführen
.\Deploy.ps1

# Dann starten
dotnet run
```

## Was Sie sehen werden

✅ **Dashboard**: Übersicht aller Projekte  
✅ **Projektverwaltung**: CRUD-Operationen für Projekte  
✅ **Datenaustausch**: JSON/CSV/XML Import/Export  
✅ **SQL Interface**: Direkte Datenbankabfragen  

## Tipps

- Die App nutzt LocalDB standardmäßig (keine separate SQL Server Installation nötig)
- Seed-Daten werden automatisch geladen (4 Beispiel-Projekte)
- Alle Daten werden in der SQL Server Datenbank gespeichert

## Probleme?

1. **Port bereits in Verwendung?**  
   Ändern Sie den Port in `Properties/launchSettings.json`

2. **Datenbank-Fehler?**  
   Stellen Sie sicher, dass SQL Server LocalDB installiert ist, oder ändern Sie die Connection String in `appsettings.json`

3. **Build-Fehler?**  
   Führen Sie `dotnet clean` und dann `dotnet restore` aus

---

**Viel Erfolg bei der Bewerbung! 🎯**

