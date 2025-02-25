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

            // Utiliza o NullLogger para n�o precisar de uma inst�ncia real de ILogger
            _repository = new VideoRepository(_context, NullLogger<VideoRepository>.Instance);
        }

        [Fact]
        public async Task AddAsync_ShouldAddVideo()
        {
            // Arrange
            var video = new Video
            {
                Titulo = "Test Video",
                Descricao = "Descri��o de teste",
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
                Descricao = "Descri��o",
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
            // Adiciona 5 v�deos ao banco de dados
            for (int i = 1; i <= 5; i++)
            {
                _context.Videos.Add(new Video
                {
                    Titulo = $"Video {i}",
                    Descricao = $"Descri��o {i}",
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
                Titulo = "T�tulo Antigo",
                Descricao = "Descri��o Antiga",
                Autor = "Autor",
                Duracao = "5 min",
                DataPublicacao = DateTime.UtcNow,
                Url = "http://exemplo.com/video",
                IsDeleted = false
            };
            _context.Videos.Add(video);
            await _context.SaveChangesAsync();

            // Modifica os dados do v�deo
            video.Titulo = "T�tulo Novo";
            video.Descricao = "Descri��o Nova";

            // Act
            await _repository.UpdateAsync(video);
            var updatedVideo = await _context.Videos.FindAsync(video.Id);

            // Assert
            Assert.Equal("T�tulo Novo", updatedVideo.Titulo);
            Assert.Equal("Descri��o Nova", updatedVideo.Descricao);
        }

        [Fact]
        public async Task DeleteAsync_ShouldMarkVideoAsDeleted()
        {
            // Arrange
            var video = new Video
            {
                Titulo = "V�deo para Deletar",
                Descricao = "Descri��o",
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
