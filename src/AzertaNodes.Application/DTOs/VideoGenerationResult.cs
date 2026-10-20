namespace AzertaNodes.Application.DTOs
{
    public class VideoGenerationResult
    {
        public bool IsSuccess { get; set; }
        public string? VideoUrl { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
