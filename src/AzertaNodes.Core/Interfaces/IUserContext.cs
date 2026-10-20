namespace AzertaNodes.Core.Interfaces
{
    public interface IUserContext
    {
        Guid UserId { get; }
        bool IsAuthenticated { get; }
    }
}
