using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.ShoppingCartApi.Models;

public class Product
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)] // I want the value not be inserted from the database
    public int ProductId { get; set; }
    [Required]
    public string Name { get; set; }
    [Range(1, 1000)]
    public double Price { get; set; }
    public string Description { get; set; }
    public string CategoryName { get; set; }
    public string ImageUrl { get; set; }
}