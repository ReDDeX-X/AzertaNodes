using AzertaNodes.Core.Entities.Database;
using Microsoft.EntityFrameworkCore;

namespace AzertaNodes.Infrastructure.Identity
{
    public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
    }
}
