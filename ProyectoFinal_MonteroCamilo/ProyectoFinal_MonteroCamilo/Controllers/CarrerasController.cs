using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_MonteroCamilo.Data;
using ProyectoFinal_MonteroCamilo.Models;
using ProyectoFinal_MonteroCamilo.ViewModels;

namespace ProyectoFinal_MonteroCamilo.Controllers;

[Authorize(Roles = Roles.Administrador)]
public class CarrerasController : Controller
{
    private readonly ApplicationDbContext _db;
    public CarrerasController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
        => View(await _db.Carreras.Include(c => c.Estudiantes).OrderBy(c => c.Codigo).ToListAsync());

    public IActionResult Create() => View(new Carrera());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Codigo,Nombre,TotalCreditos")] Carrera carrera)
    {
        if (!ModelState.IsValid) return View(carrera);

        carrera.Codigo = carrera.Codigo.Trim().ToUpper();
        if (await _db.Carreras.AnyAsync(c => c.Codigo == carrera.Codigo))
        {
            ModelState.AddModelError(nameof(Carrera.Codigo), "Ya existe una carrera con ese código.");
            return View(carrera);
        }

        carrera.Activa = true;
        _db.Carreras.Add(carrera);
        await _db.SaveChangesAsync();
        TempData["Mensaje"] = "Carrera creada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var carrera = await _db.Carreras.FindAsync(id);
        return carrera == null ? NotFound() : View(carrera);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Codigo,Nombre,TotalCreditos")] Carrera form)
    {
        if (id != form.Id) return NotFound();
        if (!ModelState.IsValid) return View(form);

        var carrera = await _db.Carreras.FindAsync(id);
        if (carrera == null) return NotFound();

        var codigo = form.Codigo.Trim().ToUpper();
        if (await _db.Carreras.AnyAsync(c => c.Codigo == codigo && c.Id != id))
        {
            ModelState.AddModelError(nameof(Carrera.Codigo), "Ya existe una carrera con ese código.");
            return View(form);
        }

        carrera.Codigo = codigo;
        carrera.Nombre = form.Nombre.Trim();
        carrera.TotalCreditos = form.TotalCreditos;
        await _db.SaveChangesAsync();
        TempData["Mensaje"] = "Carrera actualizada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActivo(int id)
    {
        var carrera = await _db.Carreras.FindAsync(id);
        if (carrera == null) return NotFound();
        carrera.Activa = !carrera.Activa;
        await _db.SaveChangesAsync();
        TempData["Mensaje"] = carrera.Activa ? "Carrera activada." : "Carrera desactivada.";
        return RedirectToAction(nameof(Index));
    }

    // ---------- HU-18: plan de estudios ----------

    public async Task<IActionResult> Plan(int id)
    {
        var vm = await ArmarPlan(id);
        return vm == null ? NotFound() : View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AgregarCurso(int carreraId, int cursoId, int cuatrimestre)
    {
        if (cuatrimestre < 1 || cuatrimestre > 12)
        {
            TempData["Error"] = "El cuatrimestre debe estar entre 1 y 12.";
            return RedirectToAction(nameof(Plan), new { id = carreraId });
        }

        var existe = await _db.CarreraCursos.AnyAsync(x => x.CarreraId == carreraId && x.CursoId == cursoId);
        var validos = await _db.Carreras.AnyAsync(c => c.Id == carreraId)
                   && await _db.Cursos.AnyAsync(c => c.Id == cursoId);

        if (!existe && validos)
        {
            _db.CarreraCursos.Add(new CarreraCurso
            {
                CarreraId = carreraId,
                CursoId = cursoId,
                Cuatrimestre = cuatrimestre
            });
            await _db.SaveChangesAsync();
            TempData["Mensaje"] = "Curso agregado al plan de estudios.";
        }
        return RedirectToAction(nameof(Plan), new { id = carreraId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> QuitarCurso(int carreraId, int cursoId)
    {
        var item = await _db.CarreraCursos.FindAsync(carreraId, cursoId);
        if (item != null)
        {
            _db.CarreraCursos.Remove(item);
            await _db.SaveChangesAsync();
            TempData["Mensaje"] = "Curso quitado del plan de estudios.";
        }
        return RedirectToAction(nameof(Plan), new { id = carreraId });
    }

    private async Task<PlanCarreraViewModel?> ArmarPlan(int id)
    {
        var carrera = await _db.Carreras.FindAsync(id);
        if (carrera == null) return null;

        var items = await _db.CarreraCursos.Include(x => x.Curso)
            .Where(x => x.CarreraId == id)
            .OrderBy(x => x.Cuatrimestre).ThenBy(x => x.Curso!.Codigo)
            .ToListAsync();

        var enPlan = items.Select(i => i.CursoId).ToList();
        var disponibles = await _db.Cursos
            .Where(c => c.Activo && !enPlan.Contains(c.Id))
            .OrderBy(c => c.Codigo)
            .Select(c => new SelectListItem(c.Codigo + " - " + c.Nombre, c.Id.ToString()))
            .ToListAsync();

        return new PlanCarreraViewModel { Carrera = carrera, Items = items, CursosDisponibles = disponibles };
    }
}