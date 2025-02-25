using System.Text.Json;
using Microsoft.Extensions.Logging;
using RDManipulacao.Application.Interfaces;
using RDManipulacao.Domain.Models;

namespace RDManipulacao.Application.Services
{
    public class YouTubeService : IYouTubeService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<YouTubeService> _logger;
        private readonly string _apiKey;

        public YouTubeService(ILogger<YouTubeService> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;
            _apiKey = Environment.GetEnvironmentVariable("YOUTUBE_API_KEY");
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new Exception("A chave da API do YouTube não foi configurada na variável de ambiente 'YOUTUBE_API_KEY'.");
            }
        }

        public async Task<List<YouTubeVideo>> GetVideosAsync()
        {
            var videos = new List<YouTubeVideo>();
            // URL base para a pesquisa
            string baseUrl = "https://www.googleapis.com/youtube/v3/search";
            // Consulta: vídeos relacionados à manipulação de medicamentos
            string query = "manipulação de medicamentos";
            // Filtrando por vídeos brasileiros publicados em 2025
            string url = $"{baseUrl}?part=snippet" +
                         $"&q={Uri.EscapeDataString(query)}" +
                         $"&regionCode=BR" +
                         $"&publishedAfter=2025-01-01T00:00:00Z" +
                         $"&publishedBefore=2026-01-01T00:00:00Z" +
                         $"&key={_apiKey}";

            try
            {
                _logger.LogInformation("Requisitando vídeos da API do YouTube.");
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();

                using (var jsonDoc = JsonDocument.Parse(jsonString))
                {
                    var root = jsonDoc.RootElement;
                    if (root.TryGetProperty("items", out JsonElement items))
                    {
                        foreach (var item in items.EnumerateArray())
                        {
                            // Obter o ID do vídeo (caso exista)
                            string videoId = "";
                            if (item.TryGetProperty("id", out JsonElement idElement))
                            {
                                if (idElement.TryGetProperty("videoId", out JsonElement videoIdElement))
                                {
                                    videoId = videoIdElement.GetString();
                                }
                            }

                            var snippet = item.GetProperty("snippet");
                            string title = snippet.GetProperty("title").GetString();
                            string description = snippet.GetProperty("description").GetString();
                            string channelTitle = snippet.GetProperty("channelTitle").GetString();
                            string publishedAtString = snippet.GetProperty("publishedAt").GetString();
                            DateTime publishedAt = DateTime.Parse(publishedAtString);

                            videos.Add(new YouTubeVideo
                            {
                                VideoId = videoId,
                                Title = title,
                                Description = description,
                                ChannelTitle = channelTitle,
                                PublishedAt = publishedAt
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao acessar a API do YouTube.");
                throw;
            }

            return videos;
        }
    }
}
