using Microsoft.AspNetCore.Mvc;
using Moq;
using RDManipulacao.Api.Controllers;
using RDManipulacao.Application.Interfaces;
using RDManipulacao.Domain.Models;

namespace RDManipulacao.Tests.Controllers
{
    public class YouTubeControllerTests
    {
        private readonly Mock<IYouTubeService> _youTubeServiceMock;
        private readonly YouTubeController _controller;

        public YouTubeControllerTests()
        {
            _youTubeServiceMock = new Mock<IYouTubeService>();
            _controller = new YouTubeController(_youTubeServiceMock.Object);
        }

        [Fact]
        public async Task GetVideos_ReturnsOkResult_WithListOfVideos()
        {
            // Arrange
            var sampleVideos = new List<YouTubeVideo>
            {
                new() {
                    VideoId = "video1",
                    Title = "Título 1",
                    Description = "Descrição 1",
                    ChannelTitle = "Canal 1",
                    PublishedAt = DateTime.UtcNow
                },
                new() {
                    VideoId = "video2",
                    Title = "Título 2",
                    Description = "Descrição 2",
                    ChannelTitle = "Canal 2",
                    PublishedAt = DateTime.UtcNow
                }
            };

            _youTubeServiceMock
                .Setup(service => service.GetVideosAsync())
                .ReturnsAsync(sampleVideos);

            // Act
            var result = await _controller.GetVideos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedVideos = Assert.IsType<List<YouTubeVideo>>(okResult.Value);
            Assert.Equal(sampleVideos.Count, returnedVideos.Count);
            Assert.Equal(sampleVideos[0].VideoId, returnedVideos[0].VideoId);
        }

        [Fact]
        public async Task GetVideos_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _youTubeServiceMock
                .Setup(service => service.GetVideosAsync())
                .ThrowsAsync(new Exception("Erro de teste"));

            // Act
            var result = await _controller.GetVideos();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Erro interno ao buscar vídeos da API do YouTube.", objectResult.Value);
        }
    }
}
