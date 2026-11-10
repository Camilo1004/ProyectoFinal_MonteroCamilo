using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal_MonteroCamilo.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "El correo es obligatorio."), EmailAddress]
    [Display(Name = "Correo")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "La contraseña es obligatoria."), DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = "";

    [Display(Name = "Recordarme")]
    public bool RememberMe { get; set; }
}