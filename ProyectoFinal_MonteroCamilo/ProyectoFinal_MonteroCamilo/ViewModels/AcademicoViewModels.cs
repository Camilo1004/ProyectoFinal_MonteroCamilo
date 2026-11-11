using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoFinal_MonteroCamilo.Models;

namespace ProyectoFinal_MonteroCamilo.ViewModels;

public class CursoFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El código es obligatorio."), StringLength(20)]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = "";

    [Required(ErrorMessage = "El nombre es obligatorio."), StringLength(100)]
    public string Nombre { get; set; } = "";

    [Range(1, 20, ErrorMessage = "Los créditos deben estar entre 1 y 20.")]
    [Display(Name = "Créditos")]
    public int Creditos { get; set; }

    [StringLength(500)]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Display(Name = "Prerrequisitos")]
    public List<int> PrerrequisitosIds { get; set; } = new();

    [ValidateNever]
    public List<SelectListItem> Opciones { get; set; } = new();
}

public class EstudianteEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio."), StringLength(100)]
    [Display(Name = "Nombre completo")]
    public string NombreCompleto { get; set; } = "";

    [Required(ErrorMessage = "La cédula es obligatoria."), StringLength(20)]
    [Display(Name = "Cédula")]
    public string Cedula { get; set; } = "";

    [StringLength(20)]
    [Display(Name = "Teléfono")]
    public string? Telefono { get; set; }

    [Required(ErrorMessage = "Debe seleccionar una carrera.")]
    [Display(Name = "Carrera")]
    public int? CarreraId { get; set; }

    [ValidateNever]
    public List<SelectListItem> Carreras { get; set; } = new();
}

public class PlanCarreraViewModel
{
    public Carrera Carrera { get; set; } = null!;
    public List<CarreraCurso> Items { get; set; } = new();
    public List<SelectListItem> CursosDisponibles { get; set; } = new();
}

public class PlanEstudianteViewModel
{
    public Carrera Carrera { get; set; } = null!;
    public List<CarreraCurso> Items { get; set; } = new();
}