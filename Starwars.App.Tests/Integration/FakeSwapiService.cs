using Starwars.App.Models.APIModels;
using Starwars.App.Models.DomainModels;
using Starwars.App.Models.Mappers;
using Starwars.App.Services;

namespace Starwars.App.Tests.Integration;

/// <summary>Deterministic SWAPI replacement for integration tests (no HTTP).</summary>
internal sealed class FakeSwapiService : ISwapiService
{
    public Task<List<StarshipAPIModel>> GetAllStarshipsAsync() =>
        Task.FromResult(new List<StarshipAPIModel>());

    public List<StarshipDbSet> GetDbMappedStarships(List<StarshipAPIModel> apiStarships) =>
        apiStarships.Select(s => s.MapToDb()).ToList();
}
