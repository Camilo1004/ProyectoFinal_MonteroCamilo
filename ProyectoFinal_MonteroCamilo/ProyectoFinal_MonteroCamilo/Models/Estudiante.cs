using System.ComponentModel.DataAnnotations;
namespace ProyectoFinal_MonteroCamilo.Models;

public class Estudiante
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    [Required, StringLength(100)] public string NombreCompleto { get; set; } = "";
    [Required, StringLength(20)] public string Cedula { get; set; } = "";
    [StringLength(20)] public string? Telefono { get; set; }
    public bool Activo { get; set; } = true;

    public int CarreraId { get; set; }
    public Carrera? Carrera { get; set; }

    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}