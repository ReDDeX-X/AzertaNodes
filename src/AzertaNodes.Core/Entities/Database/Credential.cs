namespace AzertaNodes.Core.Entities.Database
{
    public class Credential
    {
        public Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public required string Name { get; set; }
        public required string Provider { get; set; }
        public required byte[] EncryptedValue { get; set; }
        public required byte[] InitializationVector { get; set; }
    }
}
