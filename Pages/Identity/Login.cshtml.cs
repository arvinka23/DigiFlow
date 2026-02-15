using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DigitalisierungsManager.Models;

namespace DigitalisierungsManager.Pages.Identity;

/// <summary>
/// Login-Seite: Authentifiziert Benutzer per E-Mail und Passwort.
/// </summary>
public class LoginModel : PageModel
{
    private readonly SignInManager<AppUser> _signInManager;

    public LoginModel(SignInManager<AppUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public bool RememberMe { get; set; }

    public string? ErrorMessage { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ErrorMessage = "Bitte fuellen Sie alle Felder aus.";
            return Page();
        }

        var result = await _signInManager.PasswordSignInAsync(Email, Password, RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            return LocalRedirect(ReturnUrl ?? "/");
        }

        ErrorMessage = "Ungueltige E-Mail oder Passwort.";
        return Page();
    }
}
