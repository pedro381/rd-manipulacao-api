using Microsoft.AspNetCore.Mvc;
using RDManipulacao.Application.Interfaces;
using RDManipulacao.Domain.Entities;

namespace RDManipulacao.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideosController : ControllerBase
    {
        private readonly IVideoService _videoService;

        public VideosController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        // GET: api/videos
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var videos = await _videoService.GetAllVideosAsync(pageNumber, pageSize);
                return Ok(videos);
            }
            catch (Exception ex)
            {
                // Registro do erro pode ser feito via logger (aqui retornamos apenas o erro genérico)
                return StatusCode(500, "Erro interno no servidor.");
            }
        }

        // GET: api/videos/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var video = await _videoService.GetVideoByIdAsync(id);
                if (video == null)
                    return NotFound();

                return Ok(video);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno no servidor.");
            }
        }

        // POST: api/videos
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VideoBase video)
        {
            if (video == null)
                return BadRequest("O vídeo é nulo.");

            try
            {
                var result = await _videoService.AddVideoAsync(Video.BaseToVideo(video));
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, video);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno no servidor.");
            }
        }

        // PUT: api/videos/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] VideoBase video)
        {
            if (video == null || id <= 0)
                return BadRequest("Dados inválidos para atualização.");

            try
            {
                var existingVideo = await _videoService.GetVideoByIdAsync(id);
                if (existingVideo == null || existingVideo.IsDeleted)
                    return NotFound();

                var request = Video.BaseToVideo(video);
                request.Id = id;
                await _videoService.UpdateVideoAsync(request);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno no servidor.");
            }
        }

        // DELETE: api/videos/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var video = await _videoService.GetVideoByIdAsync(id);
                if (video == null)
                    return NotFound();

                await _videoService.DeleteVideoAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno no servidor.");
            }
        }
    }
}
