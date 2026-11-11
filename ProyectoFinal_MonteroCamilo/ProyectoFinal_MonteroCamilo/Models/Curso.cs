using System.ComponentModel.DataAnnotations;
namespace ProyectoFinal_MonteroCamilo.Models;

public class Curso
{
    public int Id { get; set; }
    [Required, StringLength(20)] public string Codigo { get; set; } = "";
    [Required, StringLength(100)] public string Nombre { get; set; } = "";
    [Range(1, 20)] public int Creditos { get; set; }
    [StringLength(500)] public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;

    public int? DocenteId { get; set; }
    public Docente? Docente { get; set; }

    public ICollection<CarreraCurso> PlanEstudios { get; set; } = new List<CarreraCurso>();
    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
    public ICollection<Curso> Prerrequisitos { get; set; } = new List<Curso>();
    public ICollection<Curso> EsPrerrequisitoDe { get; set; } = new List<Curso>();
}