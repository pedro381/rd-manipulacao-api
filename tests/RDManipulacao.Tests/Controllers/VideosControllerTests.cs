using Microsoft.AspNetCore.Mvc;
using RDManipulacao.Api.Controllers;
using RDManipulacao.Application.Interfaces;
using RDManipulacao.Domain.Entities;

namespace RDManipulacao.Api.Tests.Controllers
{
    // Implementação fake do IVideoService para testes
    public class FakeVideoService : IVideoService
    {
        private readonly List<Video> _videos = new();
        private int _nextId = 1;

        public Task<IEnumerable<Video>> GetAllVideosAsync(int pageNumber, int pageSize)
        {
            var pagedVideos = _videos
                .Where(v => !v.IsDeleted)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
            return Task.FromResult(pagedVideos.AsEnumerable());
        }

        public Task<Video> GetVideoByIdAsync(int id)
        {
            var video = _videos.FirstOrDefault(v => v.Id == id && !v.IsDeleted);
            return Task.FromResult(video);
        }

        public Task<Video> AddVideoAsync(Video video)
        {
            video.Id = _nextId++;
            _videos.Add(video);
            return Task.FromResult(video);
        }

        public Task UpdateVideoAsync(Video video)
        {
            var existing = _videos.FirstOrDefault(v => v.Id == video.Id);
            if (existing != null)
            {
                existing.Titulo = video.Titulo;
                existing.Descricao = video.Descricao;
                existing.Autor = video.Autor;
                existing.Duracao = video.Duracao;
                existing.DataPublicacao = video.DataPublicacao;
                existing.Url = video.Url;
                existing.IsDeleted = video.IsDeleted;
            }
            return Task.CompletedTask;
        }

        public Task DeleteVideoAsync(int id)
        {
            var video = _videos.FirstOrDefault(v => v.Id == id);
            if (video != null)
            {
                video.IsDeleted = true;
            }
            return Task.CompletedTask;
        }
    }

    public class VideosControllerTests
    {
        private readonly FakeVideoService _fakeService;
        private readonly VideosController _controller;

        public VideosControllerTests()
        {
            _fakeService = new FakeVideoService();
            _controller = new VideosController(_fakeService);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithVideos()
        {
            // Arrange
            // Adiciona alguns vídeos para teste
            await _fakeService.AddVideoAsync(new Video { Titulo = "Video 1", Descricao = "Desc 1", Autor = "Autor 1", Duracao = "5 min", DataPublicacao = DateTime.UtcNow, Url = "http://example.com/1", IsDeleted = false });
            await _fakeService.AddVideoAsync(new Video { Titulo = "Video 2", Descricao = "Desc 2", Autor = "Autor 2", Duracao = "6 min", DataPublicacao = DateTime.UtcNow, Url = "http://example.com/2", IsDeleted = false });

            // Act
            var result = await _controller.GetAll(pageNumber: 1, pageSize: 10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var videos = Assert.IsAssignableFrom<IEnumerable<Video>>(okResult.Value);
            Assert.Equal(2, videos.Count());
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WhenVideoExists()
        {
            // Arrange
            var video = new Video { Titulo = "Video Existente", Descricao = "Desc", Autor = "Autor", Duracao = "5 min", DataPublicacao = DateTime.UtcNow, Url = "http://example.com", IsDeleted = false };
            var addedVideo = await _fakeService.AddVideoAsync(video);

            // Act
            var result = await _controller.GetById(addedVideo.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedVideo = Assert.IsType<Video>(okResult.Value);
            Assert.Equal(addedVideo.Id, returnedVideo.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenVideoDoesNotExist()
        {
            // Act
            var result = await _controller.GetById(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtActionResult_WhenVideoIsCreated()
        {
            // Arrange
            var videoBase = new VideoBase
            {
                Titulo = "Novo Video",
                Descricao = "Nova Descrição",
                Autor = "Novo Autor",
                Duracao = "7 min",
                DataPublicacao = DateTime.UtcNow,
                Url = "http://example.com/novo"
            };

            // Act
            var result = await _controller.Create(videoBase);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedVideo = Assert.IsType<VideoBase>(createdResult.Value);
            Assert.Equal("Novo Video", returnedVideo.Titulo);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var video = new Video { Titulo = "Antigo Título", Descricao = "Antiga Descrição", Autor = "Autor", Duracao = "5 min", DataPublicacao = DateTime.UtcNow, Url = "http://example.com", IsDeleted = false };
            var addedVideo = await _fakeService.AddVideoAsync(video);

            var videoBase = new VideoBase
            {
                Titulo = "Título Atualizado",
                Descricao = "Descrição Atualizada",
                Autor = "Autor",
                Duracao = "5 min",
                DataPublicacao = DateTime.UtcNow,
                Url = "http://example.com"
            };

            // Act
            var result = await _controller.Update(addedVideo.Id, videoBase);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var updatedVideo = await _fakeService.GetVideoByIdAsync(addedVideo.Id);
            Assert.Equal("Título Atualizado", updatedVideo.Titulo);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenVideoDoesNotExist()
        {
            // Arrange
            var videoBase = new VideoBase
            {
                Titulo = "Título",
                Descricao = "Descrição",
                Autor = "Autor",
                Duracao = "5 min",
                DataPublicacao = DateTime.UtcNow,
                Url = "http://example.com"
            };

            // Act
            var result = await _controller.Update(999, videoBase);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenVideoDoesNotExist()
        {
            // Act
            var result = await _controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
