using System.Collections.Generic;
using System.Threading.Tasks;
using RDManipulacao.Domain.Models;

namespace RDManipulacao.Application.Interfaces
{
    public interface IYouTubeService
    {
        /// <summary>
        /// Consulta a API do YouTube e retorna uma lista de v�deos conforme os crit�rios definidos.
        /// </summary>
        Task<List<YouTubeVideo>> GetVideosAsync();
    }
}
