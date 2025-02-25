using Microsoft.EntityFrameworkCore;
using RDManipulacao.Infrastructure.Interfaces;
using RDManipulacao.Domain.Entities;
using RDManipulacao.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace RDManipulacao.Infrastructure.Repositories
{
    public class VideoRepository(AppDbContext context, ILogger<VideoRepository> logger) : IVideoRepository
    {
        private readonly AppDbContext _context = context;
        private readonly ILogger<VideoRepository> _logger = logger;

        public async Task<IEnumerable<Video>> GetAllAsync(int pageNumber, int pageSize)
        {
            try
            {
                _logger.LogInformation("Obtendo vídeos da página {PageNumber} com tamanho {PageSize} do banco de dados.", pageNumber, pageSize);

                return await _context.Videos
                                     .AsNoTracking()
                                     .Skip((pageNumber - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter vídeos paginados.");
                throw;
            }
        }

        public async Task<Video?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Obtendo vídeo com ID {VideoId}.", id);
                return await _context.Videos.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter o vídeo com ID {VideoId}.", id);
                throw;
            }
        }

        public async Task<Video> AddAsync(Video video)
        {
            try
            {
                _logger.LogInformation("Adicionando novo vídeo com título '{Titulo}'.", video.Titulo);
                await _context.Videos.AddAsync(video);
                await _context.SaveChangesAsync();
                return video;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar vídeo com título '{Titulo}'.", video.Titulo);
                throw;
            }
        }

        public async Task UpdateAsync(Video video)
        {
            try
            {
                _logger.LogInformation("Atualizando vídeo com ID {VideoId}.", video.Id);

                var existingVideo = await _context.Videos.FindAsync(video.Id);
                if (existingVideo == null)
                {
                    _logger.LogWarning("Vídeo com ID {VideoId} não encontrado.", video.Id);
                    throw new KeyNotFoundException($"Vídeo com ID {video.Id} não encontrado.");
                }

                // Atualiza os campos necessários manualmente para evitar conflitos
                _context.Entry(existingVideo).CurrentValues.SetValues(video);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar vídeo com ID {VideoId}.", video.Id);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Realizando exclusão lógica do vídeo com ID {VideoId}.", id);
                var video = await _context.Videos.FindAsync(id);
                if (video == null)
                {
                    _logger.LogWarning("Tentativa de excluir vídeo com ID {VideoId}, mas não foi encontrado.", id);
                    return;
                }

                // Exclusão lógica
                video.IsDeleted = true;
                _context.Videos.Update(video);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir (logicamente) vídeo com ID {VideoId}.", id);
                throw;
            }
        }
    }
}
