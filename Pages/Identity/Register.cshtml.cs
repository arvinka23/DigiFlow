using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DigitalisierungsManager.Models;

namespace DigitalisierungsManager.Pages.Identity;

/// <summary>
/// Registrierungs-Seite: Erstellt ein neues Benutzerkonto mit der Rolle "Benutzer".
/// </summary>
public class RegisterModel : PageModel
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public RegisterModel(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [BindProperty]
    public string Anzeigename { get; set; } = string.Empty;

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public string ConfirmPassword { get; set; } = string.Empty;

    public List<string> Errors { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Anzeigename) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            Errors.Add("Bitte fuellen Sie alle Felder aus.");
            return Page();
        }

        if (Password != ConfirmPassword)
        {
            Errors.Add("Die Passwoerter stimmen nicht ueberein.");
            return Page();
        }

        var user = new AppUser
        {
            UserName = Email,
            Email = Email,
            Anzeigename = Anzeigename
        };

        var result = await _userManager.CreateAsync(user, Password);

        if (result.Succeeded)
        {
            // Neue Benutzer bekommen die Rolle "Benutzer"
            await _userManager.AddToRoleAsync(user, "Benutzer");
            await _signInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect("/");
        }

        foreach (var error in result.Errors)
        {
            Errors.Add(error.Description);
        }

        return Page();
    }
}
