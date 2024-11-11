using System.ComponentModel.DataAnnotations;

public class NotificationDTO
{
    [Required(ErrorMessage = "El campo Id es requerido.")]
    public int Id { get; set; }

    [Required(ErrorMessage = "El campo userId es requerido.")]
    public int userId { get; set; }

    [Required(ErrorMessage = "El campo Text es requerido.")]
    [MaxLength(500, ErrorMessage = "El campo Text no puede superar los 500 caracteres.")]
    public string Text { get; set; }
}

public class NotificationPostDTO
{
    [Required(ErrorMessage = "El campo Text es requerido.")]
    [MaxLength(500, ErrorMessage = "El campo Text no puede superar los 500 caracteres.")]
    public string Text { get; set; }

    [Required(ErrorMessage = "El campo UserId es requerido.")]
    public int userId { get; set; }

    public bool Readed {get;set;}
}

public class NotificationPutDTO
{
    [Required(ErrorMessage = "El campo Id es requerido.")]
    public int Id { get; set; }

    [Required(ErrorMessage = "El campo Text es requerido.")]
    [MaxLength(500, ErrorMessage = "El campo Text no puede superar los 500 caracteres.")]
    public string Text { get; set; }

    [Required(ErrorMessage = "El campo UserId es requerido.")]
    public int userId { get; set; }

    public bool Readed {get;set;}
}
