using AzertaNodes.Core.Entities.Database;
using Microsoft.EntityFrameworkCore;

namespace AzertaNodes.Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Credential> CredentialSet { get; set; }
        public DbSet<Project> ProjectSet { get; set; }
        public DbSet<WebhookSecret> WebhookSecretSet { get; set; }
    }
}
