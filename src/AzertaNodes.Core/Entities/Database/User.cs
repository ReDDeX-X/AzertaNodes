namespace AzertaNodes.Core.Entities.Database
{
    public class User
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public required string PasswordHash { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
