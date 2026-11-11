using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_MonteroCamilo.Data;
using ProyectoFinal_MonteroCamilo.Models;
using ProyectoFinal_MonteroCamilo.ViewModels;

namespace ProyectoFinal_MonteroCamilo.Controllers;

[Authorize(Roles = Roles.Estudiante)]
public class PlanEstudiosController : Controller
{
    private readonly ApplicationDbContext _db;
    public PlanEstudiosController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var est = await _db.Estudiantes.Include(e => e.Carrera)
            .FirstOrDefaultAsync(e => e.UserId == userId);
        if (est?.Carrera == null) return NotFound();

        var items = await _db.CarreraCursos
            .Include(x => x.Curso).ThenInclude(c => c!.Prerrequisitos)
            .Where(x => x.CarreraId == est.CarreraId && x.Curso!.Activo)
            .OrderBy(x => x.Cuatrimestre).ThenBy(x => x.Curso!.Codigo)
            .ToListAsync();

        return View(new PlanEstudianteViewModel { Carrera = est.Carrera, Items = items });
    }
}