using Microsoft.AspNetCore.Mvc;
using RDManipulacao.Api.Controllers;
using RDManipulacao.Application.Interfaces;
using RDManipulacao.Domain.Models;

namespace RDManipulacao.Api.Tests.Controllers
{
    public class YouTubeControllerTests
    {
        // Fake service que retorna uma lista pré-definida de vídeos
        private class FakeYouTubeService : IYouTubeService
        {
            public Task<List<YouTubeVideo>> GetVideosAsync()
            {
                var videos = new List<YouTubeVideo>
                {
                    new YouTubeVideo
                    {
                        VideoId = "video1",
                        Title = "Video 1",
                        Description = "Description 1",
                        ChannelTitle = "Channel 1",
                        PublishedAt = DateTime.UtcNow
                    },
                    new YouTubeVideo
                    {
                        VideoId = "video2",
                        Title = "Video 2",
                        Description = "Description 2",
                        ChannelTitle = "Channel 2",
                        PublishedAt = DateTime.UtcNow
                    }
                };

                return Task.FromResult(videos);
            }
        }

        // Fake service que lança uma exceção para simular erro
        private class ExceptionThrowingYouTubeService : IYouTubeService
        {
            public Task<List<YouTubeVideo>> GetVideosAsync()
            {
                throw new Exception("Simulated exception");
            }
        }

        [Fact]
        public async Task GetVideos_ReturnsOkResult_WithListOfVideos()
        {
            // Arrange
            IYouTubeService fakeService = new FakeYouTubeService();
            var controller = new YouTubeController(fakeService);

            // Act
            var result = await controller.GetVideos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedVideos = Assert.IsAssignableFrom<List<YouTubeVideo>>(okResult.Value);
            Assert.Equal(2, returnedVideos.Count);
        }

        [Fact]
        public async Task GetVideos_WhenExceptionThrown_ReturnsStatusCode500()
        {
            // Arrange
            IYouTubeService throwingService = new ExceptionThrowingYouTubeService();
            var controller = new YouTubeController(throwingService);

            // Act
            var result = await controller.GetVideos();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }
    }
}
