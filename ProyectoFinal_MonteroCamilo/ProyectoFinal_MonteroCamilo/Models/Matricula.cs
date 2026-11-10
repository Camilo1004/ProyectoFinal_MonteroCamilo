namespace ProyectoFinal_MonteroCamilo.Models;

public class Matricula
{
    public int Id { get; set; }
    public int EstudianteId { get; set; }
    public Estudiante? Estudiante { get; set; }
    public int CursoId { get; set; }
    public Curso? Curso { get; set; }
    public string Periodo { get; set; } = "";
    public DateTime FechaMatricula { get; set; } = DateTime.Now;
}