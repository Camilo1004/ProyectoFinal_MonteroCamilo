using System.ComponentModel.DataAnnotations;
namespace ProyectoFinal_MonteroCamilo.Models;

public class Carrera
{
    public int Id { get; set; }
    [Required, StringLength(20)] public string Codigo { get; set; } = "";
    [Required, StringLength(100)] public string Nombre { get; set; } = "";
    [Range(1, 1000)] public int TotalCreditos { get; set; }
    public bool Activa { get; set; } = true;

    public ICollection<Estudiante> Estudiantes { get; set; } = new List<Estudiante>();
    public ICollection<CarreraCurso> PlanEstudios { get; set; } = new List<CarreraCurso>();
}