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
                _logger.LogInformation("Obtendo v�deos da p�gina {PageNumber} com tamanho {PageSize} do banco de dados.", pageNumber, pageSize);

                return await _context.Videos
                                     .AsNoTracking()
                                     .Skip((pageNumber - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter v�deos paginados.");
                throw;
            }
        }

        public async Task<Video?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Obtendo v�deo com ID {VideoId}.", id);
                return await _context.Videos.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter o v�deo com ID {VideoId}.", id);
                throw;
            }
        }

        public async Task<Video> AddAsync(Video video)
        {
            try
            {
                _logger.LogInformation("Adicionando novo v�deo com t�tulo '{Titulo}'.", video.Titulo);
                await _context.Videos.AddAsync(video);
                await _context.SaveChangesAsync();
                return video;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar v�deo com t�tulo '{Titulo}'.", video.Titulo);
                throw;
            }
        }

        public async Task UpdateAsync(Video video)
        {
            try
            {
                _logger.LogInformation("Atualizando v�deo com ID {VideoId}.", video.Id);

                var existingVideo = await _context.Videos.FindAsync(video.Id);
                if (existingVideo == null)
                {
                    _logger.LogWarning("V�deo com ID {VideoId} n�o encontrado.", video.Id);
                    throw new KeyNotFoundException($"V�deo com ID {video.Id} n�o encontrado.");
                }

                // Atualiza os campos necess�rios manualmente para evitar conflitos
                _context.Entry(existingVideo).CurrentValues.SetValues(video);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar v�deo com ID {VideoId}.", video.Id);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Realizando exclus�o l�gica do v�deo com ID {VideoId}.", id);
                var video = await _context.Videos.FindAsync(id);
                if (video == null)
                {
                    _logger.LogWarning("Tentativa de excluir v�deo com ID {VideoId}, mas n�o foi encontrado.", id);
                    return;
                }

                // Exclus�o l�gica
                video.IsDeleted = true;
                _context.Videos.Update(video);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir (logicamente) v�deo com ID {VideoId}.", id);
                throw;
            }
        }
    }
}
