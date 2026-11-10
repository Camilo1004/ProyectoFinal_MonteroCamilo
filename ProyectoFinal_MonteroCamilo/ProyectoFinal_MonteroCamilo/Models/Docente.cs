using System.ComponentModel.DataAnnotations;
namespace ProyectoFinal_MonteroCamilo.Models;

public class Docente
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Nombre { get; set; } = "";
    [Required, EmailAddress, StringLength(100)] public string Correo { get; set; } = "";
    [StringLength(100)] public string? Especialidad { get; set; }
    public bool Activo { get; set; } = true;

    public ICollection<Curso> Cursos { get; set; } = new List<Curso>();
}