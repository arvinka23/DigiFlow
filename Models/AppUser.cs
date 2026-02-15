using Microsoft.AspNetCore.Identity;

namespace DigitalisierungsManager.Models;

/// <summary>
/// Benutzerkonto fuer die DigiFlow-Anwendung.
/// Erweitert IdentityUser um einen Anzeigenamen.
/// </summary>
public class AppUser : IdentityUser
{
    /// <summary>Angezeigter Name des Benutzers in der UI.</summary>
    public string Anzeigename { get; set; } = string.Empty;
}
