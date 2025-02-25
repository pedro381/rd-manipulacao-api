
namespace RDManipulacao.Domain.Entities
{
    public class Video : VideoBase
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; } = false;

        public static Video BaseToVideo(VideoBase video)
        {
            return new Video
            {
                Titulo = video.Titulo,
                Descricao = video.Descricao,
                Autor = video.Autor,
                Duracao = video.Duracao,
                DataPublicacao = video.DataPublicacao,
                Url = video.Url,
            };
        }
    }
}