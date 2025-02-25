using Microsoft.Extensions.Logging;
using RDManipulacao.Application.Interfaces;
using RDManipulacao.Domain.Entities;
using RDManipulacao.Infrastructure.Interfaces;

namespace RDManipulacao.Application.Services
{
    public class VideoService : IVideoService
    {
        private readonly IVideoRepository _videoRepository;
        private readonly ILogger<VideoService> _logger;

        public VideoService(IVideoRepository videoRepository, ILogger<VideoService> logger)
        {
            _videoRepository = videoRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Video>> GetAllVideosAsync(int pageNumber, int pageSize)
        {
            try
            {
                _logger.LogInformation("Obtendo todos os vídeos.");
                return await _videoRepository.GetAllAsync(pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todos os vídeos.");
                throw;
            }
        }

        public async Task<Video> GetVideoByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Obtendo vídeo com ID {VideoId}.", id);
                return await _videoRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter vídeo com ID {VideoId}.", id);
                throw;
            }
        }

        public async Task<Video> AddVideoAsync(Video video)
        {
            try
            {
                _logger.LogInformation("Adicionando vídeo com título '{Titulo}'.", video.Titulo);
                return await _videoRepository.AddAsync(video);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar vídeo com título '{Titulo}'.", video.Titulo);
                throw;
            }
        }

        public async Task UpdateVideoAsync(Video video)
        {
            try
            {
                _logger.LogInformation("Atualizando vídeo com ID {VideoId}.", video.Id);
                await _videoRepository.UpdateAsync(video);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar vídeo com ID {VideoId}.", video.Id);
                throw;
            }
        }

        public async Task DeleteVideoAsync(int id)
        {
            try
            {
                _logger.LogInformation("Excluindo (logicamente) vídeo com ID {VideoId}.", id);
                await _videoRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir vídeo com ID {VideoId}.", id);
                throw;
            }
        }
    }
}
