using System.ComponentModel.DataAnnotations;

namespace Mango.Services.ShoppingCartApi.Models;

public class Product
{
    // [Key, DatabaseGenerated(DatabaseGeneratedOption.None)] // I don't see the need of this
    [Key]
    public int ProductId { get; set; }
    [Required]
    public string Name { get; set; }
    [Range(1, 1000)]
    public double Price { get; set; }
    public string Description { get; set; }
    public string CategoryName { get; set; }
    public string ImageUrl { get; set; }
}