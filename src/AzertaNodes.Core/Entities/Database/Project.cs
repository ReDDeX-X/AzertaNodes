namespace AzertaNodes.Core.Entities.Database
{
    public class Project
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public string? Description { get; set; }
        public bool IsRunning { get; set; } = false;
        public Guid? MainCredentialId { get; set; }
        public required Guid UserId { get; set; }
        public List<WebhookSecret> WebhookSecrets { get; set; } = [];
        public bool IsAsync { get; set; } = false;

        public string GraphDefinition { get; private set; } = string.Empty;
        public bool IsSuccessfullyValidated { get; private set; } = false;
        public string? ErrorMessage { get; set; }
        public string? GraphHash { get; private set; }

        public void EditGraphDefinition(string graphDefinition, bool isSuccessfullyValidated)
        {
            GraphDefinition = graphDefinition;
            IsSuccessfullyValidated = isSuccessfullyValidated;
            GraphHash = ComputeCurrentHash();
        }
        public void ChangeValidationStatus(bool isValidated)
        {
            IsSuccessfullyValidated = isValidated;
            GraphHash = ComputeCurrentHash();
        }
        public string ComputeCurrentHash()
        {
            string rawData = $"{GraphDefinition}|{IsSuccessfullyValidated}";
            byte[] hashBytes = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(rawData));
            return Convert.ToBase64String(hashBytes);
        }
    }
}
