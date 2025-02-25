using RDManipulacao.Domain.Entities;

namespace RDManipulacao.Infrastructure.Interfaces
{
    public interface IVideoRepository
    {
        Task<IEnumerable<Video>> GetAllAsync(int pageNumber, int pageSize);
        Task<Video?> GetByIdAsync(int id);
        Task<Video> AddAsync(Video video);
        Task UpdateAsync(Video video);
        Task DeleteAsync(int id);
    }
}
