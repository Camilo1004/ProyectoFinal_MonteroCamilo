using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_MonteroCamilo.Models;

namespace ProyectoFinal_MonteroCamilo.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider sp)
    {
        var db = sp.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();

        var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var rol in new[] { Roles.Administrador, Roles.Estudiante })
            if (!await roleManager.RoleExistsAsync(rol))
                await roleManager.CreateAsync(new IdentityRole(rol));

        var userManager = sp.GetRequiredService<UserManager<IdentityUser>>();
        const string adminEmail = "admin@fidelitas.ac.cr";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            var r = await userManager.CreateAsync(admin, "Admin123!");
            if (r.Succeeded) await userManager.AddToRoleAsync(admin, Roles.Administrador);
        }

        if (!await db.Carreras.AnyAsync())
        {
            db.Carreras.AddRange(
                new Carrera { Codigo = "ISC", Nombre = "Ingeniería en Sistemas de Computación", TotalCreditos = 180 },
                new Carrera { Codigo = "ADM", Nombre = "Administración de Empresas", TotalCreditos = 160 },
                new Carrera { Codigo = "CON", Nombre = "Contaduría", TotalCreditos = 160 });
            await db.SaveChangesAsync();
        }
    }
}