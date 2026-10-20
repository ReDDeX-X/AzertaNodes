namespace AzertaNodes.Core.Entities
{
    public class Error
    {
        public required ErrorType ErrorType { get; set; }
        public required string UserErrorMessage { get; set; }
        public string? SystemErrorMessage { get; set; }
    }
}
