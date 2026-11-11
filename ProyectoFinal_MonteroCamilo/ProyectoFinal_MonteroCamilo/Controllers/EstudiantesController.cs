using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_MonteroCamilo.Data;
using ProyectoFinal_MonteroCamilo.Models;
using ProyectoFinal_MonteroCamilo.ViewModels;

namespace ProyectoFinal_MonteroCamilo.Controllers;

[Authorize(Roles = Roles.Administrador)]
public class EstudiantesController : Controller
{
    private readonly ApplicationDbContext _db;
    public EstudiantesController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? q)
    {
        var query = _db.Estudiantes.Include(e => e.Carrera).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(e => e.NombreCompleto.Contains(q)
                                  || e.Cedula.Contains(q)
                                  || e.Carrera!.Nombre.Contains(q)
                                  || e.Carrera!.Codigo.Contains(q));
        ViewData["q"] = q;
        return View(await query.OrderBy(e => e.NombreCompleto).ToListAsync());
    }

    private async Task<List<SelectListItem>> CargarCarreras(int actualId) =>
        await _db.Carreras.Where(c => c.Activa || c.Id == actualId)
            .OrderBy(c => c.Nombre)
            .Select(c => new SelectListItem(c.Codigo + " - " + c.Nombre, c.Id.ToString()))
            .ToListAsync();

    public async Task<IActionResult> Edit(int id)
    {
        var e = await _db.Estudiantes.FindAsync(id);
        if (e == null) return NotFound();

        return View(new EstudianteEditViewModel
        {
            Id = e.Id,
            NombreCompleto = e.NombreCompleto,
            Cedula = e.Cedula,
            Telefono = e.Telefono,
            CarreraId = e.CarreraId,
            Carreras = await CargarCarreras(e.CarreraId)
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EstudianteEditViewModel vm)
    {
        if (id != vm.Id) return NotFound();
        vm.Carreras = await CargarCarreras(vm.CarreraId ?? 0);
        if (!ModelState.IsValid) return View(vm);

        var e = await _db.Estudiantes.FindAsync(id);
        if (e == null) return NotFound();

        var cedula = vm.Cedula.Trim();
        if (await _db.Estudiantes.AnyAsync(x => x.Cedula == cedula && x.Id != id))
        {
            ModelState.AddModelError(nameof(vm.Cedula), "Esta cédula ya está registrada.");
            return View(vm);
        }

        e.NombreCompleto = vm.NombreCompleto.Trim();
        e.Cedula = cedula;
        e.Telefono = vm.Telefono;
        e.CarreraId = vm.CarreraId!.Value;
        await _db.SaveChangesAsync();
        TempData["Mensaje"] = "Estudiante actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActivo(int id)
    {
        var e = await _db.Estudiantes.FindAsync(id);
        if (e == null) return NotFound();
        e.Activo = !e.Activo;
        await _db.SaveChangesAsync();
        TempData["Mensaje"] = e.Activo ? "Cuenta activada." : "Cuenta desactivada.";
        return RedirectToAction(nameof(Index));
    }
}