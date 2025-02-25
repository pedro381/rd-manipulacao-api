using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using RDManipulacao.Domain.Entities;
using RDManipulacao.Infrastructure.Data;
using RDManipulacao.Infrastructure.Repositories;

namespace RDManipulacao.Infrastructure.Tests
{
    public class VideoRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly VideoRepository _repository;

        public VideoRepositoryTests()
        {
            // Configura o banco de dados InMemory para testes
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(options);

            // Utiliza o NullLogger para não precisar de uma instância real de ILogger
            _repository = new VideoRepository(_context, NullLogger<VideoRepository>.Instance);
        }

        [Fact]
        public async Task AddAsync_ShouldAddVideo()
        {
            // Arrange
            var video = new Video
            {
                Titulo = "Test Video",
                Descricao = "Descrição de teste",
                Autor = "Autor Teste",
                Duracao = "5 min",
                DataPublicacao = DateTime.UtcNow,
                Url = "http://exemplo.com/video",
                IsDeleted = false
            };

            // Act
            var result = await _repository.AddAsync(video);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            var videoInDb = await _context.Videos.FindAsync(result.Id);
            Assert.NotNull(videoInDb);
            Assert.Equal("Test Video", videoInDb.Titulo);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnVideo_WhenVideoExists()
        {
            // Arrange
            var video = new Video
            {
                Titulo = "Video Existente",
                Descricao = "Descrição",
                Autor = "Autor",
                Duracao = "5 min",
                DataPublicacao = DateTime.UtcNow,
                Url = "http://exemplo.com/video",
                IsDeleted = false
            };
            _context.Videos.Add(video);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(video.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(video.Id, result.Id);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedVideos()
        {
            // Arrange
            // Adiciona 5 vídeos ao banco de dados
            for (int i = 1; i <= 5; i++)
            {
                _context.Videos.Add(new Video
                {
                    Titulo = $"Video {i}",
                    Descricao = $"Descrição {i}",
                    Autor = "Autor",
                    Duracao = "5 min",
                    DataPublicacao = DateTime.UtcNow,
                    Url = $"http://exemplo.com/video{i}",
                    IsDeleted = false
                });
            }
            await _context.SaveChangesAsync();

            // Act
            var page1 = await _repository.GetAllAsync(1, 3);
            var page2 = await _repository.GetAllAsync(2, 3);

            // Assert
            Assert.Equal(3, page1.Count());
            Assert.Equal(2, page2.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateVideo()
        {
            // Arrange
            var video = new Video
            {
                Titulo = "Título Antigo",
                Descricao = "Descrição Antiga",
                Autor = "Autor",
                Duracao = "5 min",
                DataPublicacao = DateTime.UtcNow,
                Url = "http://exemplo.com/video",
                IsDeleted = false
            };
            _context.Videos.Add(video);
            await _context.SaveChangesAsync();

            // Modifica os dados do vídeo
            video.Titulo = "Título Novo";
            video.Descricao = "Descrição Nova";

            // Act
            await _repository.UpdateAsync(video);
            var updatedVideo = await _context.Videos.FindAsync(video.Id);

            // Assert
            Assert.Equal("Título Novo", updatedVideo.Titulo);
            Assert.Equal("Descrição Nova", updatedVideo.Descricao);
        }

        [Fact]
        public async Task DeleteAsync_ShouldMarkVideoAsDeleted()
        {
            // Arrange
            var video = new Video
            {
                Titulo = "Vídeo para Deletar",
                Descricao = "Descrição",
                Autor = "Autor",
                Duracao = "5 min",
                DataPublicacao = DateTime.UtcNow,
                Url = "http://exemplo.com/video",
                IsDeleted = false
            };
            _context.Videos.Add(video);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(video.Id);
            var deletedVideo = await _context.Videos.FindAsync(video.Id);

            // Assert
            Assert.True(deletedVideo.IsDeleted);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
