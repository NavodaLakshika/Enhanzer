using Microsoft.EntityFrameworkCore;

namespace Enhanzer.Backend.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<LocationDetail> Location_Details { get; set; }
        public DbSet<PurchaseBill> PurchaseBills { get; set; }
        public DbSet<PurchaseBillItem> PurchaseBillItems { get; set; }
    }
}
