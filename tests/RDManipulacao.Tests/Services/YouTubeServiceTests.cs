using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RDManipulacao.Application.Interfaces;
using RDManipulacao.Application.Services;
using RDManipulacao.Domain.Configurations;
using RDManipulacao.Domain.Models;

namespace RDManipulacao.Tests.Services
{
    public class YouTubeServiceTests
    {
        private readonly Mock<IYouTubeApi> _youTubeApiMock;
        private readonly Mock<ILogger<YouTubeService>> _loggerMock;
        private readonly IOptions<YouTubeApiSettings> _settingsOptions;

        public YouTubeServiceTests()
        {
            _youTubeApiMock = new Mock<IYouTubeApi>();
            _loggerMock = new Mock<ILogger<YouTubeService>>();

            // Configuração padrão para os testes
            var settings = new YouTubeApiSettings
            {
                ApiKey = "dummy_api_key",
                Query = "manipulação de medicamentos",
                RegionCode = "BR",
                PublishedAfter = "2025-01-01T00:00:00Z",
                PublishedBefore = "2026-01-01T00:00:00Z"
            };
            _settingsOptions = Options.Create(settings);
        }

        [Fact]
        public async Task GetVideosAsync_ReturnsVideos_WhenApiResponseIsValid()
        {
            // Arrange: cria uma resposta simulada da API do YouTube com dois itens
            var now = DateTime.UtcNow;
            var fakeResponse = new YouTubeSearchResponse
            {
                Items =
                [
                    new() {
                        Id = new YouTubeId { VideoId = "video1" },
                        Snippet = new YouTubeSnippet
                        {
                            Title = "Título 1",
                            Description = "Descrição 1",
                            ChannelTitle = "Canal 1",
                            PublishedAt = now
                        }
                    },
                    new() {
                        Id = new YouTubeId { VideoId = "video2" },
                        Snippet = new YouTubeSnippet
                        {
                            Title = "Título 2",
                            Description = "Descrição 2",
                            ChannelTitle = "Canal 2",
                            PublishedAt = now
                        }
                    }
                ]
            };

            _youTubeApiMock.Setup(api => api.SearchVideosAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            )).ReturnsAsync(fakeResponse);

            var service = new YouTubeService(_youTubeApiMock.Object, _settingsOptions, _loggerMock.Object);

            // Act
            var videos = await service.GetVideosAsync();

            // Assert
            Assert.NotNull(videos);
            Assert.Equal(2, videos.Count);
            Assert.Equal("video1", videos[0].VideoId);
            Assert.Equal("Título 1", videos[0].Title);
            Assert.Equal("video2", videos[1].VideoId);
            Assert.Equal("Título 2", videos[1].Title);
        }

        [Fact]
        public async Task GetVideosAsync_ReturnsEmptyList_WhenResponseItemsIsNull()
        {
            // Arrange: simula uma resposta sem itens
            var fakeResponse = new YouTubeSearchResponse
            {
                Items = null
            };

            _youTubeApiMock.Setup(api => api.SearchVideosAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            )).ReturnsAsync(fakeResponse);

            var service = new YouTubeService(_youTubeApiMock.Object, _settingsOptions, _loggerMock.Object);

            // Act
            var videos = await service.GetVideosAsync();

            // Assert
            Assert.NotNull(videos);
            Assert.Empty(videos);
        }

        [Fact]
        public async Task GetVideosAsync_ThrowsException_WhenApiThrowsException()
        {
            // Arrange: configura o mock para lançar uma exceção
            var exceptionMessage = "Erro na API";
            _youTubeApiMock.Setup(api => api.SearchVideosAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            )).ThrowsAsync(new Exception(exceptionMessage));

            var service = new YouTubeService(_youTubeApiMock.Object, _settingsOptions, _loggerMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => service.GetVideosAsync());
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Fact]
        public void Constructor_ThrowsException_WhenApiKeyIsMissing()
        {
            // Arrange: configura as opções com ApiKey vazia
            var invalidSettings = new YouTubeApiSettings
            {
                ApiKey = "",
                Query = "manipulação de medicamentos",
                RegionCode = "BR",
                PublishedAfter = "2025-01-01T00:00:00Z",
                PublishedBefore = "2026-01-01T00:00:00Z"
            };
            var invalidOptions = Options.Create(invalidSettings);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                new YouTubeService(_youTubeApiMock.Object, invalidOptions, _loggerMock.Object));
            Assert.Equal("A chave da API do YouTube não foi configurada.", exception.Message);
        }
    }
}
