namespace Mango.Services.ShoppingCartApi.Models.Dtos;

public class CartDto
{
    public CartHeaderDto CartHeader { get; set; }
    public IEnumerable<CartDetailDto> CartDetails { get; set; }
}