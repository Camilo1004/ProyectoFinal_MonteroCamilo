namespace ProyectoFinal_MonteroCamilo.Models;

public class CarreraCurso
{
    public int CarreraId { get; set; }
    public Carrera? Carrera { get; set; }
    public int CursoId { get; set; }
    public Curso? Curso { get; set; }
    public int Cuatrimestre { get; set; }
}