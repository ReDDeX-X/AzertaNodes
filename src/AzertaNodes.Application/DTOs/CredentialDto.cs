namespace AzertaNodes.Application.DTOs
{
    public class CredentialDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Provider { get; set; }
    }
}
