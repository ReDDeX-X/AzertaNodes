using System.Security.Cryptography;
using System.Text;

namespace AzertaNodes.Core.Entities.Database
{
    public class WebhookSecret
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public required string KeyId { get; set; }
        public required string SecretHash { get; set; }
        public required string MaskedSecret { get; set; }
        public Guid ProjectId { get; set; }
        public required Project Project { get; set; }
        public DateTime CreatedAt { get; } = DateTime.UtcNow;


        public static (string plainSecret, WebhookSecret entity) Create(string name, Project project)
        {
            var keyId = $"wh_{GenerateId()}";
            var secretPart = GenerateSecretPart(); 

            var plain = $"{keyId}.{secretPart}";
            var entity = new WebhookSecret
            {
                Name = name,
                Project = project,
                KeyId = keyId,
                SecretHash = ComputeHash(secretPart),
                MaskedSecret = $"{keyId}*****{secretPart[^4..]}"
            };

            return (plain, entity);
        }
        public bool Validate(string providedSecret)
        {
            if (string.IsNullOrWhiteSpace(providedSecret))
                return false;

            var parts = providedSecret.Split('.', 2);
            if (parts.Length != 2)
                return false;

            var providedKeyId = parts[0];
            var providedSecretPart = parts[1];

            if (!string.Equals(providedKeyId, KeyId, StringComparison.Ordinal))
                return false;

            var providedHash = ComputeHash(providedSecretPart);

            return CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(providedHash),
                Convert.FromBase64String(SecretHash)
            );
        }
        private static string GenerateId()
        {
            var bytes = RandomNumberGenerator.GetBytes(6);
            return ToBase64Url(bytes);
        }
        private static string GenerateSecretPart()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return ToBase64Url(bytes);
        }
        private static string ToBase64Url(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');
        }
        private static string ComputeHash(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}