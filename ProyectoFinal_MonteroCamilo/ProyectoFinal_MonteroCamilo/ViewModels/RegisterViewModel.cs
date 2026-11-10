using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProyectoFinal_MonteroCamilo.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio."), StringLength(100)]
    [Display(Name = "Nombre completo")]
    public string NombreCompleto { get; set; } = "";

    [Required(ErrorMessage = "La cédula es obligatoria."), StringLength(20)]
    [Display(Name = "Cédula")]
    public string Cedula { get; set; } = "";

    [Required(ErrorMessage = "El correo es obligatorio."), EmailAddress(ErrorMessage = "Correo no válido.")]
    [Display(Name = "Correo institucional")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "La contraseña es obligatoria."), DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = "";

    [DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmPassword { get; set; } = "";

    [Required(ErrorMessage = "Debe seleccionar una carrera.")]
    [Display(Name = "Carrera")]
    public int? CarreraId { get; set; }

    [ValidateNever]
    public List<SelectListItem> Carreras { get; set; } = new();
}