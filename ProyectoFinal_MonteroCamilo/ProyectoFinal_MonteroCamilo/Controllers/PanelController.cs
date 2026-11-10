using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoFinal_MonteroCamilo.Models;

namespace ProyectoFinal_MonteroCamilo.Controllers;

[Authorize]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class PanelController : Controller
{
    public IActionResult Index()
        => User.IsInRole(Roles.Administrador)
            ? RedirectToAction(nameof(Admin))
            : RedirectToAction(nameof(Estudiante));

    [Authorize(Roles = Roles.Administrador)]
    public IActionResult Admin() => View();

    [Authorize(Roles = Roles.Estudiante)]
    public IActionResult Estudiante() => View();
}