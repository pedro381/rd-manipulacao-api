using Microsoft.Extensions.Logging.Abstractions;
using RDManipulacao.Application.Services;
using RDManipulacao.Domain.Entities;
using RDManipulacao.Infrastructure.Interfaces;

namespace RDManipulacao.Tests.Services
{
    public class VideoServiceTests
    {
        private readonly VideoService _videoService;
        private readonly FakeVideoRepository _fakeRepository;

        public VideoServiceTests()
        {
            _fakeRepository = new FakeVideoRepository();
            _videoService = new VideoService(_fakeRepository, NullLogger<VideoService>.Instance);
        }

        [Fact]
        public async Task GetAllVideosAsync_ReturnsPaginatedVideos()
        {
            // Arrange
            _fakeRepository.AddInitialVideos(5);

            // Act
            var page1 = await _videoService.GetAllVideosAsync(1, 3);
            var page2 = await _videoService.GetAllVideosAsync(2, 3);

            // Assert
            Assert.Equal(3, page1.Count());
            Assert.Equal(2, page2.Count());
        }

        [Fact]
        public async Task GetVideoByIdAsync_ReturnsCorrectVideo()
        {
            // Arrange
            var video = new Video
            {
                Id = 1,
                Titulo = "Test Video",
                Descricao = "Test Description",
                Autor = "Test Author",
                Duracao = "5 min",
                DataPublicacao = DateTime.UtcNow,
                Url = "http://example.com/video",
                IsDeleted = false
            };
            await _fakeRepository.AddAsync(video);

            // Act
            var result = await _videoService.GetVideoByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Video", result.Titulo);
        }

        [Fact]
        public async Task AddVideoAsync_AddsVideoSuccessfully()
        {
            // Arrange
            var video = new Video
            {
                Titulo = "New Video",
                Descricao = "New Description",
                Autor = "New Author",
                Duracao = "4 min",
                DataPublicacao = DateTime.UtcNow,
                Url = "http://example.com/newvideo",
                IsDeleted = false
            };

            // Act
            var addedVideo = await _videoService.AddVideoAsync(video);

            // Assert
            Assert.NotNull(addedVideo);
            Assert.True(addedVideo.Id > 0);
            var fetchedVideo = await _videoService.GetVideoByIdAsync(addedVideo.Id);
            Assert.Equal("New Video", fetchedVideo?.Titulo);
        }

        [Fact]
        public async Task UpdateVideoAsync_UpdatesVideoSuccessfully()
        {
            // Arrange
            var video = new Video
            {
                Id = 10,
                Titulo = "Original Title",
                Descricao = "Original Description",
                Autor = "Author",
                Duracao = "5 min",
                DataPublicacao = DateTime.UtcNow,
                Url = "http://example.com/video",
                IsDeleted = false
            };
            await _fakeRepository.AddAsync(video);

            // Act
            video.Titulo = "Updated Title";
            await _videoService.UpdateVideoAsync(video);
            var updatedVideo = await _videoService.GetVideoByIdAsync(10);

            // Assert
            Assert.Equal("Updated Title", updatedVideo?.Titulo);
        }

        // Fake implementation of IVideoRepository for testing purposes
        private class FakeVideoRepository : IVideoRepository
        {
            private readonly List<Video> _videos = [];
            private int _nextId = 1;

            public Task<IEnumerable<Video>> GetAllAsync(int pageNumber, int pageSize)
            {
                var pagedVideos = _videos
                    .Where(v => !v.IsDeleted)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);
                return Task.FromResult(pagedVideos);
            }

            public Task<Video?> GetByIdAsync(int id)
            {
                var video = _videos.FirstOrDefault(v => v.Id == id && !v.IsDeleted);
                return Task.FromResult(video);
            }

            public Task<Video> AddAsync(Video video)
            {
                if (video.Id == 0)
                {
                    video.Id = _nextId++;
                }
                _videos.Add(video);
                return Task.FromResult(video);
            }

            public Task UpdateAsync(Video video)
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

            public Task DeleteAsync(int id)
            {
                var video = _videos.FirstOrDefault(v => v.Id == id);
                if (video != null)
                {
                    video.IsDeleted = true;
                }
                return Task.CompletedTask;
            }

            // Helper method to seed multiple videos for pagination tests
            public void AddInitialVideos(int count)
            {
                for (var i = 0; i < count; i++)
                {
                    _videos.Add(new Video
                    {
                        Id = _nextId++,
                        Titulo = $"Video {_nextId}",
                        Descricao = $"Description {_nextId}",
                        Autor = "Author",
                        Duracao = "5 min",
                        DataPublicacao = DateTime.UtcNow,
                        Url = $"http://example.com/video{_nextId}",
                        IsDeleted = false
                    });
                }
            }
        }
    }
}
