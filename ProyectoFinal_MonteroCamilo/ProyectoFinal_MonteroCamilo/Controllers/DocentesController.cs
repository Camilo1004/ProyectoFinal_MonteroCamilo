using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_MonteroCamilo.Data;
using ProyectoFinal_MonteroCamilo.Models;

namespace ProyectoFinal_MonteroCamilo.Controllers;

[Authorize(Roles = Roles.Administrador)]
public class DocentesController : Controller
{
    private readonly ApplicationDbContext _db;
    public DocentesController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
        => View(await _db.Docentes.OrderBy(d => d.Nombre).ToListAsync());

    public IActionResult Create() => View(new Docente());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombre,Correo,Especialidad")] Docente docente)
    {
        if (!ModelState.IsValid) return View(docente);

        docente.Correo = docente.Correo.Trim().ToLower();
        if (await _db.Docentes.AnyAsync(d => d.Correo == docente.Correo))
        {
            ModelState.AddModelError(nameof(Docente.Correo), "Ya existe un docente con ese correo.");
            return View(docente);
        }

        docente.Activo = true;
        _db.Docentes.Add(docente);
        await _db.SaveChangesAsync();
        TempData["Mensaje"] = "Docente registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var docente = await _db.Docentes.FindAsync(id);
        return docente == null ? NotFound() : View(docente);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Correo,Especialidad")] Docente form)
    {
        if (id != form.Id) return NotFound();
        if (!ModelState.IsValid) return View(form);

        var docente = await _db.Docentes.FindAsync(id);
        if (docente == null) return NotFound();

        var correo = form.Correo.Trim().ToLower();
        if (await _db.Docentes.AnyAsync(d => d.Correo == correo && d.Id != id))
        {
            ModelState.AddModelError(nameof(Docente.Correo), "Ya existe un docente con ese correo.");
            return View(form);
        }

        docente.Nombre = form.Nombre.Trim();
        docente.Correo = correo;
        docente.Especialidad = form.Especialidad;
        await _db.SaveChangesAsync();
        TempData["Mensaje"] = "Docente actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActivo(int id)
    {
        var docente = await _db.Docentes.FindAsync(id);
        if (docente == null) return NotFound();

        if (docente.Activo && await _db.Cursos.AnyAsync(c => c.DocenteId == id && c.Activo))
        {
            TempData["Error"] = "No se puede desactivar: el docente tiene cursos asignados.";
            return RedirectToAction(nameof(Index));
        }

        docente.Activo = !docente.Activo;
        await _db.SaveChangesAsync();
        TempData["Mensaje"] = docente.Activo ? "Docente activado." : "Docente desactivado.";
        return RedirectToAction(nameof(Index));
    }
}