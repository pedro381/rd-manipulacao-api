
namespace RDManipulacao.Domain.Entities
{
    public class VideoBase
    {
        public string? Titulo { get; set; }
        public string? Descricao { get; set; }
        public string? Autor { get; set; }
        public string? Duracao { get; set; }
        public DateTime DataPublicacao { get; set; }
        public string? Url { get; set; }
    }
}