using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations;

public class UserDTO
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "El campo Username es requerido.")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "El campo FirstName es requerido.")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "El campo LastName es requerido.")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "El campo Email es requerido.")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "El campo Password es requerido.")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    public string? Password { get; set; }

    public int? ProfilePhotoId { get; set; } // Este campo es opcional

    [RegularExpression(@"^\d{8,}$", ErrorMessage = "El DNI debe contener al menos 8 dígitos.")]
    public string? DNI { get; set; } // Este campo es opcional y validamos que tenga al menos 8 dígitos

    public string? Address { get; set; } // Este campo también es opcional
}
public class UserPostPutDTO
{
    [Required(ErrorMessage = "El campo Username es requerido.")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "El campo FirstName es requerido.")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "El campo LastName es requerido.")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "El campo Email es requerido.")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "El campo Password es requerido.")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    [MaxLength(100, ErrorMessage = "La contraseña no puede tener más de 100 caracteres.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{6,}$", 
        ErrorMessage = "La contraseña debe contener al menos una letra mayúscula, una letra minúscula, un número y un carácter especial.")]
    public string? Password { get; set; }

    public int? ProfilePhotoId { get; set; } // Este campo es opcional

    [RegularExpression(@"^\d{8,}$", ErrorMessage = "El DNI debe contener al menos 8 dígitos.")]
    public string? DNI { get; set; } // Este campo es opcional y validamos que tenga al menos 8 dígitos

    public string? Address { get; set; } // Este campo también es opcional
}

public class UserPutDTO
{
    public string? Username { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
    public string? Email { get; set; }


    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    [MaxLength(100, ErrorMessage = "La contraseña no puede tener más de 100 caracteres.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{6,}$", 
        ErrorMessage = "La contraseña debe contener al menos una letra mayúscula, una letra minúscula, un número y un carácter especial.")]
    public string? newPassword { get; set; }

    [Required(ErrorMessage = "El campo Password es requerido.")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    [MaxLength(100, ErrorMessage = "La contraseña no puede tener más de 100 caracteres.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{6,}$", 
        ErrorMessage = "La contraseña debe contener al menos una letra mayúscula, una letra minúscula, un número y un carácter especial.")]
    public string? Password { get; set; }

    public int? ProfilePhotoId { get; set; } // Este campo es opcional

    [RegularExpression(@"^\d{7,}$", ErrorMessage = "El DNI debe contener al menos 7 dígitos.")]
    public string? DNI { get; set; } // Este campo es opcional y validamos que tenga al menos 8 dígitos

    public string? Address { get; set; } // Este campo también es opcional
}