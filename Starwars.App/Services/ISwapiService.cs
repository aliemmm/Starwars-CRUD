using Starwars.App.Models.APIModels;
using Starwars.App.Models.DomainModels;

namespace Starwars.App.Services;

public interface ISwapiService
{
    Task<List<StarshipAPIModel>> GetAllStarshipsAsync();
    List<StarshipDbSet> GetDbMappedStarships(List<StarshipAPIModel> apiStarships);
}
