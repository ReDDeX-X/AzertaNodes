namespace AzertaNodes.Core.Entities
{
    public static class UserErrorMessages
    {
        public const string InvalidCredentialIdFormat = "Invalid credential ID format.";
        public const string FailedToRetrieveApiKey = "Failed to retrieve API key.";
        public const string UserPromptCannotBeNullOrEmpty = "User prompt cannot be null or empty.";
        public const string ModelIdCannotBeNullOrEmpty = "Model ID cannot be null or empty.";
        public const string FailedToDecryptData = "An error occurred during decryption.";
        public const string PlainTextCannotBeEmpty = "Plain text cannot be empty.";
        public const string FailedToDecryptProvidedData = "Failed to decrypt the provided data.";
        public const string CredentialsNotFound = "The credentials were not found.";
        public const string DecryptedKeyEmptyOrIncorrect = "The decrypted key is empty or incorrect.";
        public const string DeserializationReturnedNull = "Deserialization returned null.";
        public const string ProjectDataCorrupted = "The project data is corrupted and cannot be loaded.";
    }
}
