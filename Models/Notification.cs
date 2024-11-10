using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Notification
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public bool Readed {get;set;} //Indica si la notificacion ya ha sido leida o no

    [Required]
    public int IdUser { get; set; }

    [Required]
    [MaxLength(500)]
    public string Text { get; set; }

    [ForeignKey(nameof(IdUser))]
    public User User { get; set; }

    public Notification() { }

    public Notification(int idUser, string text)
    {
        IdUser = idUser;
        Text = text;
        Readed = false;
    }

    public override string ToString()
    {
        return $"Notification ID: {Id}, Text: {Text}";
    }
}
