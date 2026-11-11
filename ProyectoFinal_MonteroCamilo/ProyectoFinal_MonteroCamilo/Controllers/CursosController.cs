using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_MonteroCamilo.Data;
using ProyectoFinal_MonteroCamilo.Models;
using ProyectoFinal_MonteroCamilo.ViewModels;

namespace ProyectoFinal_MonteroCamilo.Controllers;

[Authorize(Roles = Roles.Administrador)]
public class CursosController : Controller
{
    private readonly ApplicationDbContext _db;
    public CursosController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? q)
    {
        var query = _db.Cursos.Include(c => c.Prerrequisitos).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(c => c.Codigo.Contains(q) || c.Nombre.Contains(q));
        ViewData["q"] = q;
        return View(await query.OrderBy(c => c.Codigo).ToListAsync());
    }

    private async Task<List<SelectListItem>> Opciones(int excluirId) =>
        await _db.Cursos.Where(c => c.Activo && c.Id != excluirId)
            .OrderBy(c => c.Codigo)
            .Select(c => new SelectListItem(c.Codigo + " - " + c.Nombre, c.Id.ToString()))
            .ToListAsync();

    public async Task<IActionResult> Create()
        => View(new CursoFormViewModel { Opciones = await Opciones(0) });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CursoFormViewModel vm)
    {
        vm.Opciones = await Opciones(0);
        if (!ModelState.IsValid) return View(vm);

        var codigo = vm.Codigo.Trim().ToUpper();
        if (await _db.Cursos.AnyAsync(c => c.Codigo == codigo))
        {
            ModelState.AddModelError(nameof(vm.Codigo), "Ya existe un curso con ese código.");
            return View(vm);
        }

        var curso = new Curso
        {
            Codigo = codigo,
            Nombre = vm.Nombre.Trim(),
            Creditos = vm.Creditos,
            Descripcion = vm.Descripcion,
            Activo = true,
            Prerrequisitos = await _db.Cursos.Where(c => vm.PrerrequisitosIds.Contains(c.Id)).ToListAsync()
        };
        _db.Cursos.Add(curso);
        await _db.SaveChangesAsync();
        TempData["Mensaje"] = "Curso creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var curso = await _db.Cursos.Include(c => c.Prerrequisitos).FirstOrDefaultAsync(c => c.Id == id);
        if (curso == null) return NotFound();

        return View(new CursoFormViewModel
        {
            Id = curso.Id,
            Codigo = curso.Codigo,
            Nombre = curso.Nombre,
            Creditos = curso.Creditos,
            Descripcion = curso.Descripcion,
            PrerrequisitosIds = curso.Prerrequisitos.Select(p => p.Id).ToList(),
            Opciones = await Opciones(curso.Id)
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CursoFormViewModel vm)
    {
        if (id != vm.Id) return NotFound();
        vm.Opciones = await Opciones(id);

        if (vm.PrerrequisitosIds.Contains(id))
            ModelState.AddModelError(nameof(vm.PrerrequisitosIds), "Un curso no puede ser prerrequisito de sí mismo.");
        if (!ModelState.IsValid) return View(vm);

        var curso = await _db.Cursos.Include(c => c.Prerrequisitos).FirstOrDefaultAsync(c => c.Id == id);
        if (curso == null) return NotFound();

        var codigo = vm.Codigo.Trim().ToUpper();
        if (await _db.Cursos.AnyAsync(c => c.Codigo == codigo && c.Id != id))
        {
            ModelState.AddModelError(nameof(vm.Codigo), "Ya existe un curso con ese código.");
            return View(vm);
        }

        curso.Codigo = codigo;
        curso.Nombre = vm.Nombre.Trim();
        curso.Creditos = vm.Creditos;
        curso.Descripcion = vm.Descripcion;

        curso.Prerrequisitos.Clear();
        var nuevos = await _db.Cursos.Where(c => vm.PrerrequisitosIds.Contains(c.Id)).ToListAsync();
        foreach (var p in nuevos) curso.Prerrequisitos.Add(p);

        await _db.SaveChangesAsync();
        TempData["Mensaje"] = "Curso actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActivo(int id)
    {
        var curso = await _db.Cursos.FindAsync(id);
        if (curso == null) return NotFound();
        curso.Activo = !curso.Activo;
        await _db.SaveChangesAsync();
        TempData["Mensaje"] = curso.Activo ? "Curso activado." : "Curso desactivado.";
        return RedirectToAction(nameof(Index));
    }
}