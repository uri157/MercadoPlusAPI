using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Transaction
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }                // Identificador único de la transacción

    public int IdBuyer { get; set; }            // Clave foránea hacia el usuario comprador

    public int IdPublication { get; set; }      // Clave foránea hacia la publicación
    public int IdCard { get; set; }             //Clave foranea hacia la tarjeta con la que se realizo el pago
 
    public DateTime TransactionDate { get; set; } // Fecha de la transacción

    public decimal Amount { get; set; }         // Monto de la transacción

    public int? Calification { get; set; }      // Calificación (1-5), puede ser nula

    public string? ReviewText {get; set;}

    // Propiedades de navegación
    [ForeignKey("IdBuyer")]
    public User Buyer { get; set; }             // Relación con el usuario comprador

    [ForeignKey("IdCard")]                      
    public Card Card {get;set;}                 //Relacion con la tarjeta con la que se efectuo el pago

    [ForeignKey("IdPublication")]
    public Publication Publication { get; set; } // Relación con la publicación

    // Constructor por defecto
    public Transaction() { }

    // Constructor con parámetros
    public Transaction(int idBuyer, int idPublication,int idCard, DateTime transactionDate, decimal amount, int? calification, string? reviewText)
    {
        IdBuyer = idBuyer;
        IdPublication = idPublication;
        IdCard = idCard;
        TransactionDate = transactionDate;
        Amount = amount;
        Calification = calification;
        ReviewText = reviewText;
    }

    // Método ToString sobreescrito para una representación en string
    public override string ToString()
    {
        return $"Id: {Id}, IdBuyer: {IdBuyer}, IdPublication: {IdPublication}, TransactionDate: {TransactionDate}, Amount: {Amount}, Calification: {Calification}";
    }
}
