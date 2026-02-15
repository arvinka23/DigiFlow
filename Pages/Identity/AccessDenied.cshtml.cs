using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DigitalisierungsManager.Pages.Identity;

/// <summary>
/// Zugriff-verweigert-Seite fuer nicht autorisierte Benutzer.
/// </summary>
public class AccessDeniedModel : PageModel
{
    public void OnGet()
    {
    }
}
