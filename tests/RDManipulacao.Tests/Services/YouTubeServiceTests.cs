using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using RDManipulacao.Application.Services;

namespace RDManipulacao.Application.Tests
{
    // Fake HttpMessageHandler to simular respostas da API do YouTube
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public FakeHttpMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_response);
        }
    }

    public class YouTubeServiceTests
    {
        [Fact]
        public async Task GetVideosAsync_ReturnsExpectedVideos()
        {
            // Arrange
            // Define a variável de ambiente para a API key (valor dummy para testes)
            Environment.SetEnvironmentVariable("YOUTUBE_API_KEY", "dummy_key");

            // JSON simulado de resposta da API do YouTube
            var jsonResponse = @"
            {
                ""items"": [
                    {
                        ""id"": { ""videoId"": ""abc123"" },
                        ""snippet"": {
                            ""title"": ""Test Video"",
                            ""description"": ""Test Description"",
                            ""channelTitle"": ""Test Channel"",
                            ""publishedAt"": ""2025-05-01T00:00:00Z""
                        }
                    },
                    {
                        ""id"": { ""videoId"": ""def456"" },
                        ""snippet"": {
                            ""title"": ""Another Video"",
                            ""description"": ""Another Description"",
                            ""channelTitle"": ""Another Channel"",
                            ""publishedAt"": ""2025-06-01T00:00:00Z""
                        }
                    }
                ]
            }";

            // Cria uma resposta HTTP fake com status OK e o conteúdo JSON acima
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse)
            };

            // Cria um FakeHttpMessageHandler e um HttpClient usando-o
            var fakeHandler = new FakeHttpMessageHandler(httpResponseMessage);
            var fakeHttpClient = new HttpClient(fakeHandler);

            // Cria uma instância do YouTubeService utilizando um logger nulo
            var logger = NullLogger<YouTubeService>.Instance;
            var youTubeService = new YouTubeService(logger);

            // Usa reflection para injetar o fakeHttpClient na instância de YouTubeService
            var httpClientField = typeof(YouTubeService).GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            httpClientField.SetValue(youTubeService, fakeHttpClient);

            // Act
            var videos = await youTubeService.GetVideosAsync();

            // Assert
            Assert.NotNull(videos);
            Assert.Equal(2, videos.Count);

            // Validação do primeiro vídeo
            var firstVideo = videos[0];
            Assert.Equal("abc123", firstVideo.VideoId);
            Assert.Equal("Test Video", firstVideo.Title);
            Assert.Equal("Test Description", firstVideo.Description);
            Assert.Equal("Test Channel", firstVideo.ChannelTitle);

            // Validação do segundo vídeo
            var secondVideo = videos[1];
            Assert.Equal("def456", secondVideo.VideoId);
            Assert.Equal("Another Video", secondVideo.Title);
            Assert.Equal("Another Description", secondVideo.Description);
            Assert.Equal("Another Channel", secondVideo.ChannelTitle);
        }
    }
}
