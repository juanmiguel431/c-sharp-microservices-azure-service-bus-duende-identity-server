using Mango.Services.ProductApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductApi.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
    }
}
