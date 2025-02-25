using Microsoft.AspNetCore.Mvc;
using RDManipulacao.Application.Interfaces;

namespace RDManipulacao.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class YouTubeController(IYouTubeService youTubeService) : ControllerBase
    {
        private readonly IYouTubeService _youTubeService = youTubeService;

        /// <summary>
        /// Consulta a API do YouTube e retorna uma lista de v�deos.
        /// </summary>
        /// <returns>Lista de v�deos conforme os crit�rios de busca.</returns>
        [HttpGet("videos")]
        public async Task<IActionResult> GetVideos()
        {
            try
            {
                var videos = await _youTubeService.GetVideosAsync();
                return Ok(videos);
            }
            catch
            {
                // Em produ��o, evite expor detalhes do erro.
                return StatusCode(500, "Erro interno ao buscar v�deos da API do YouTube.");
            }
        }
    }
}
