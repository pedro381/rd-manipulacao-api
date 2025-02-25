using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RDManipulacao.Domain.Configurations;
using RDManipulacao.Application.Interfaces;
using RDManipulacao.Domain.Models;

namespace RDManipulacao.Application.Services
{
    public class YouTubeService : IYouTubeService
    {
        private readonly IYouTubeApi _youTubeApi;
        private readonly ILogger<YouTubeService> _logger;
        private readonly YouTubeApiSettings _settings;

        public YouTubeService(IYouTubeApi youTubeApi, IOptions<YouTubeApiSettings> settings, ILogger<YouTubeService> logger)
        {
            _youTubeApi = youTubeApi;
            _logger = logger;
            _settings = settings.Value;

            if (string.IsNullOrEmpty(_settings.ApiKey))
            {
                throw new Exception("A chave da API do YouTube não foi configurada.");
            }
        }

        public async Task<List<YouTubeVideo>> GetVideosAsync()
        {
            try
            {
                _logger.LogInformation("Requisitando vídeos da API do YouTube.");

                var response = await _youTubeApi.SearchVideosAsync(
                    part: "snippet",
                    query: _settings.Query,
                    regionCode: _settings.RegionCode,
                    publishedAfter: _settings.PublishedAfter,
                    publishedBefore: _settings.PublishedBefore,
                    apiKey: _settings.ApiKey
                );

                var videos = new List<YouTubeVideo>();

                if (response?.Items != null)
                {
                    foreach (var item in response.Items)
                    {
                        if (item.Snippet != null)
                        {
                            videos.Add(new YouTubeVideo
                            {
                                VideoId = item.Id?.VideoId,
                                Title = item.Snippet.Title,
                                Description = item.Snippet.Description,
                                ChannelTitle = item.Snippet.ChannelTitle,
                                PublishedAt = item.Snippet.PublishedAt
                            });
                        }
                    }
                }

                return videos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao acessar a API do YouTube.");
                throw;
            }
        }
    }
}
