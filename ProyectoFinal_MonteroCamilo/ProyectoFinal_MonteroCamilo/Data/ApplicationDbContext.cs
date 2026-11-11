using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_MonteroCamilo.Models;

namespace ProyectoFinal_MonteroCamilo.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Carrera> Carreras => Set<Carrera>();
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<Docente> Docentes => Set<Docente>();
    public DbSet<Estudiante> Estudiantes => Set<Estudiante>();
    public DbSet<CarreraCurso> CarreraCursos => Set<CarreraCurso>();
    public DbSet<Matricula> Matriculas => Set<Matricula>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<Carrera>().HasIndex(c => c.Codigo).IsUnique();
        b.Entity<Curso>().HasIndex(c => c.Codigo).IsUnique();
        b.Entity<Docente>().HasIndex(d => d.Correo).IsUnique();
        b.Entity<Estudiante>().HasIndex(e => e.Cedula).IsUnique();

        b.Entity<CarreraCurso>().HasKey(x => new { x.CarreraId, x.CursoId });

        b.Entity<Curso>()
            .HasMany(c => c.Prerrequisitos)
            .WithMany(c => c.EsPrerrequisitoDe)
            .UsingEntity<Dictionary<string, object>>(
                "CursoPrerrequisito",
                j => j.HasOne<Curso>().WithMany().HasForeignKey("PrerrequisitoId").OnDelete(DeleteBehavior.Restrict),
                j => j.HasOne<Curso>().WithMany().HasForeignKey("CursoId").OnDelete(DeleteBehavior.Restrict));

        b.Entity<Estudiante>()
            .HasOne<IdentityUser>().WithOne()
            .HasForeignKey<Estudiante>(e => e.UserId);

        b.Entity<Estudiante>()
            .HasOne(e => e.Carrera).WithMany(c => c.Estudiantes)
            .HasForeignKey(e => e.CarreraId).OnDelete(DeleteBehavior.Restrict);

        b.Entity<Matricula>()
            .HasOne(m => m.Estudiante).WithMany(e => e.Matriculas)
            .HasForeignKey(m => m.EstudianteId).OnDelete(DeleteBehavior.Restrict);

        b.Entity<Matricula>()
            .HasOne(m => m.Curso).WithMany(c => c.Matriculas)
            .HasForeignKey(m => m.CursoId).OnDelete(DeleteBehavior.Restrict);
    }
}