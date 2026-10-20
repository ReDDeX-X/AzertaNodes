using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AzertaNodes.Infrastructure.Identity
{
    public class IdentityDbContextFactory
    : IDesignTimeDbContextFactory<IdentityDbContext>
    {
        public IdentityDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();

            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=mydatabase;Username=myuser;Password=mypassword");

            return new IdentityDbContext(optionsBuilder.Options);
        }
    }
}
