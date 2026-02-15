# PowerShell Deployment Script für Digitalisierungs-Manager
# Demo-Projekt Deployment Script

param(
    [string]$Environment = "Development",
    [string]$DatabaseServer = "(localdb)\mssqllocaldb",
    [string]$DatabaseName = "DigitalisierungsManager"
)

Write-Host "🚀 Digitalisierungs-Manager Deployment Script" -ForegroundColor Cyan
Write-Host "Environment: $Environment" -ForegroundColor Yellow
Write-Host ""

# Prüfe ob .NET SDK installiert ist
Write-Host "📦 Prüfe .NET SDK Installation..." -ForegroundColor Green
$dotnetVersion = dotnet --version 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ .NET SDK nicht gefunden. Bitte installieren Sie .NET 8 SDK." -ForegroundColor Red
    exit 1
}
Write-Host "✅ .NET Version: $dotnetVersion" -ForegroundColor Green

# Stelle NuGet-Pakete wieder her
Write-Host ""
Write-Host "📥 Stelle NuGet-Pakete wieder her..." -ForegroundColor Green
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Fehler beim Wiederherstellen der Pakete." -ForegroundColor Red
    exit 1
}

# Erstelle Datenbank
Write-Host ""
Write-Host "🗄️ Erstelle Datenbank..." -ForegroundColor Green
$connectionString = "Server=$DatabaseServer;Database=$DatabaseName;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
Write-Host "Connection String: $connectionString" -ForegroundColor Gray

# Aktualisiere appsettings.json
$appSettingsPath = "appsettings.json"
$appSettings = Get-Content $appSettingsPath | ConvertFrom-Json
$appSettings.ConnectionStrings.DefaultConnection = $connectionString
$appSettings | ConvertTo-Json -Depth 10 | Set-Content $appSettingsPath
Write-Host "✅ appsettings.json aktualisiert" -ForegroundColor Green

# Erstelle Migrationen (falls EF Core Tools installiert)
Write-Host ""
Write-Host "🔄 Prüfe Entity Framework Migrationen..." -ForegroundColor Green
$efToolsInstalled = dotnet ef --version 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ EF Core Tools gefunden: $efToolsInstalled" -ForegroundColor Green
    Write-Host "Erstelle Initial Migration..." -ForegroundColor Yellow
    dotnet ef migrations add InitialCreate --context ApplicationDbContext 2>&1 | Out-Null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Migration erstellt" -ForegroundColor Green
    }
} else {
    Write-Host "⚠️ EF Core Tools nicht installiert. Überspringe Migrationen." -ForegroundColor Yellow
    Write-Host "Die Datenbank wird bei der ersten Ausführung automatisch erstellt (EnsureCreated)." -ForegroundColor Yellow
}

# Erstelle Release Build
Write-Host ""
Write-Host "🔨 Erstelle Release Build..." -ForegroundColor Green
dotnet build -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build fehlgeschlagen." -ForegroundColor Red
    exit 1
}
Write-Host "✅ Build erfolgreich!" -ForegroundColor Green

# Erstelle Publish
Write-Host ""
Write-Host "📦 Erstelle Publish-Paket..." -ForegroundColor Green
if (-not (Test-Path "publish")) {
    New-Item -ItemType Directory -Path "publish" | Out-Null
}
dotnet publish -c Release -o publish
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Publish fehlgeschlagen." -ForegroundColor Red
    exit 1
}
Write-Host "✅ Publish erfolgreich!" -ForegroundColor Green

# Erstelle Deployment-Informationen
Write-Host ""
Write-Host "📝 Erstelle Deployment-Informationen..." -ForegroundColor Green
$deployInfo = @{
    DeploymentDate = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Environment = $Environment
    DatabaseServer = $DatabaseServer
    DatabaseName = $DatabaseName
    DotNetVersion = $dotnetVersion
    BuildConfiguration = "Release"
} | ConvertTo-Json

$deployInfo | Out-File "publish\deployment-info.json"
Write-Host "✅ Deployment-Info erstellt" -ForegroundColor Green

Write-Host ""
Write-Host "✅ Deployment erfolgreich abgeschlossen!" -ForegroundColor Green
Write-Host ""
Write-Host "Nächste Schritte:" -ForegroundColor Cyan
Write-Host "1. Starten Sie die Anwendung mit: dotnet run" -ForegroundColor White
Write-Host "2. Öffnen Sie: https://localhost:5001" -ForegroundColor White
Write-Host "3. Die Datenbank wird automatisch erstellt." -ForegroundColor White
Write-Host ""
Write-Host "📁 Publish-Ordner: .\publish\" -ForegroundColor Yellow


