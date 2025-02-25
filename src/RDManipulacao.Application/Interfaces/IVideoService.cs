using RDManipulacao.Domain.Entities;

namespace RDManipulacao.Application.Interfaces
{
    public interface IVideoService
    {
        Task<IEnumerable<Video>> GetAllVideosAsync(int pageNumber, int pageSize);
        Task<Video?> GetVideoByIdAsync(int id);
        Task<Video> AddVideoAsync(Video video);
        Task UpdateVideoAsync(Video video);
        Task DeleteVideoAsync(int id);
    }
}
