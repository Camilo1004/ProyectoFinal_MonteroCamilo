using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_MonteroCamilo.Data;
using ProyectoFinal_MonteroCamilo.Models;
using ProyectoFinal_MonteroCamilo.ViewModels;

namespace ProyectoFinal_MonteroCamilo.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ApplicationDbContext _db;

    public AccountController(UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager, ApplicationDbContext db)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _db = db;
    }

    private async Task<List<SelectListItem>> CargarCarreras() =>
        await _db.Carreras.Where(c => c.Activa).OrderBy(c => c.Nombre)
            .Select(c => new SelectListItem(
                c.Codigo + " - " + c.Nombre + " (" + c.TotalCreditos + " créditos)",
                c.Id.ToString()))
            .ToListAsync();

    [HttpGet]
    public async Task<IActionResult> Register()
        => View(new RegisterViewModel { Carreras = await CargarCarreras() });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        vm.Carreras = await CargarCarreras();
        if (!ModelState.IsValid) return View(vm);

        if (await _db.Estudiantes.AnyAsync(e => e.Cedula == vm.Cedula))
        {
            ModelState.AddModelError(nameof(vm.Cedula), "Esta cédula ya está registrada.");
            return View(vm);
        }
        if (!await _db.Carreras.AnyAsync(c => c.Id == vm.CarreraId && c.Activa))
        {
            ModelState.AddModelError(nameof(vm.CarreraId), "La carrera seleccionada no está disponible.");
            return View(vm);
        }

        var user = new IdentityUser { UserName = vm.Email, Email = vm.Email, EmailConfirmed = true };
        var result = await _userManager.CreateAsync(user, vm.Password);
        if (!result.Succeeded)
        {
            foreach (var e in result.Errors) ModelState.AddModelError(string.Empty, e.Description);
            return View(vm);
        }

        await _userManager.AddToRoleAsync(user, Roles.Estudiante);
        _db.Estudiantes.Add(new Estudiante
        {
            UserId = user.Id,
            NombreCompleto = vm.NombreCompleto,
            Cedula = vm.Cedula,
            CarreraId = vm.CarreraId!.Value
        });
        await _db.SaveChangesAsync();

        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Index", "Panel");
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(vm);

        var result = await _signInManager.PasswordSignInAsync(
            vm.Email, vm.Password, vm.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);
            return RedirectToAction("Index", "Panel");
        }
        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty,
                "Cuenta bloqueada temporalmente por demasiados intentos fallidos. Intente de nuevo en unos minutos.");
            return View(vm);
        }

        ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied() => View();
}