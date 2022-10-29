namespace Mango.Services.ShoppingCartApi.Models.Dtos;

public class CartDetailDto
{
    public int CartDetailId { get; set; }
    public int CartHeaderId { get; set; }
    public virtual CartHeaderDto CartHeader { get; set; }
    public int ProductId { get; set; }
    public virtual Product Product { get; set; }
    public int Count { get; set; }
}