public class ShoppingCartDTO
{
    public int Id { get; set; }
    public int IdUser { get; set; }
    public decimal Total { get; set; }
    public List<ShoppingCartItemDTO> Items { get; set; } = new List<ShoppingCartItemDTO>();
}
