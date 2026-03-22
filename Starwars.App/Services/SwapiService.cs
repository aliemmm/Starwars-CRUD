using Newtonsoft.Json;
using Starwars.App.Models.APIModels;
using Starwars.App.Models.DomainModels;
using Starwars.App.Models.Mappers;

namespace Starwars.App.Services;

public class SwapiService : ISwapiService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://swapi.info/api/";
    private const string StarshipsEndpoint = "starships/";

    public SwapiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(BaseUrl);
    }

    public async Task<List<StarshipAPIModel>> GetAllStarshipsAsync()
    {
        var allStarships = new List<StarshipAPIModel>();
        string? nextUrl = StarshipsEndpoint;

        var response = await _httpClient.GetAsync(nextUrl);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var swapiResponse = JsonConvert.DeserializeObject<List<StarshipAPIModel>>(content);

        if (swapiResponse != null && swapiResponse.Any())
        {
            allStarships.AddRange(swapiResponse);
        }

        return allStarships;
    }

    public List<StarshipDbSet> GetDbMappedStarships(List<StarshipAPIModel> apiStarships)
    {
        return apiStarships.Select(s => s.MapToDb()).ToList();
    }
}
