using Refit;
using RDManipulacao.Domain.Models;

namespace RDManipulacao.Application.Interfaces
{
    public interface IYouTubeApi
    {
        [Get("/search")]
        Task<YouTubeSearchResponse> SearchVideosAsync(
            [AliasAs("part")] string part,
            [AliasAs("q")] string query,
            [AliasAs("regionCode")] string regionCode,
            [AliasAs("publishedAfter")] string publishedAfter,
            [AliasAs("publishedBefore")] string publishedBefore,
            [AliasAs("key")] string apiKey
        );
    }
}
