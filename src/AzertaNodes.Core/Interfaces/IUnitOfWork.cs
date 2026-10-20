namespace AzertaNodes.Core.Interfaces
{
   public interface IUnitOfWork
    {
        Task<int> ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
