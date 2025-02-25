namespace RDManipulacao.Domain.Configurations
{
    public class YouTubeApiSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://www.googleapis.com/youtube/v3";
        public string Query { get; set; } = "manipulação de medicamentos";
        public string RegionCode { get; set; } = "BR";
        public string PublishedAfter { get; set; } = "2025-01-01T00:00:00Z";
        public string PublishedBefore { get; set; } = "2026-01-01T00:00:00Z";
    }
}
