namespace AzertaNodes.Application.DTOs
{
    public class ImageGenerationResult
    {
        public bool IsSuccess { get; set; }
        public string? ImageUrl { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
