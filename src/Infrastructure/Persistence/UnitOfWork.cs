using AzertaNodes.Core.Interfaces;
using AzertaNodes.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AzertaNodes.Infrastructure.Persistence
{
    public class UnitOfWork(AppDbContext context, IdentityDbContext identityContext) : IUnitOfWork
    {
        private readonly AppDbContext context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly IdentityDbContext identityContext = identityContext ?? throw new ArgumentNullException(nameof(identityContext));


        public async Task<int> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                int rowsAffected = 0;

                rowsAffected += await context.SaveChangesAsync(cancellationToken);

                await identityContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);
                rowsAffected += await identityContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return rowsAffected;
            }
            finally
            {
                if (identityContext.Database.CurrentTransaction != null)
                {
                    await identityContext.Database.UseTransactionAsync(null, cancellationToken);
                }
            }
        }
    }
}