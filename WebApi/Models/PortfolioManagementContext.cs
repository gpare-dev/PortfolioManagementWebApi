using Microsoft.EntityFrameworkCore;

namespace WebApi.Models
{
    public class PortfolioManagementContext : DbContext
    {
        public PortfolioManagementContext(DbContextOptions<PortfolioManagementContext> options) : base(options) { }

        public DbSet<Holding> Holdings { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
