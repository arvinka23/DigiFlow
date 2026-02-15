using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DigitalisierungsManager.Models;

namespace DigitalisierungsManager.Pages.Identity;

/// <summary>
/// Logout-Seite: Meldet den Benutzer ab und leitet auf die Login-Seite weiter.
/// </summary>
public class LogoutModel : PageModel
{
    private readonly SignInManager<AppUser> _signInManager;

    public LogoutModel(SignInManager<AppUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await _signInManager.SignOutAsync();
        return RedirectToPage("/Identity/Login");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _signInManager.SignOutAsync();
        return RedirectToPage("/Identity/Login");
    }
}
