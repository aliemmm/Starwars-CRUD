using System.Net;
using FluentAssertions;
using Newtonsoft.Json;
using Starwars.App.Models.APIModels;
using Starwars.App.Services;
using Xunit;

namespace Starwars.App.Tests.Services;

public class SwapiServiceTests
{
    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public StubHandler(HttpResponseMessage response) => _response = response;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(_response);
    }

    [Fact]
    public async Task GetAllStarshipsAsync_ReturnsDeserializedList_WhenBodyIsJsonArray()
    {
        var apiModels = new List<StarshipAPIModel>
        {
            new()
            {
                Name = "Y-Wing",
                Model = "BTL-05",
                Manufacturer = "Koensayr",
                CostInCredits = "100",
                Length = "14",
                MaxAtmospheringSpeed = "1000",
                Crew = "2",
                Passengers = "0",
                CargoCapacity = "110",
                Consumables = "1",
                HyperdriveRating = "1",
                MGLT = "80",
                StarshipClass = "Starfighter",
                Pilots = new List<string>(),
                Films = new List<string>(),
                Created = "c",
                Edited = "e",
                Url = "https://example/starships/1/"
            }
        };

        var json = JsonConvert.SerializeObject(apiModels);
        var handler = new StubHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json)
        });

        var service = new SwapiService(new HttpClient(handler));
        var result = await service.GetAllStarshipsAsync();

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Y-Wing");
        result[0].Url.Should().Be("https://example/starships/1/");
    }

    [Fact]
    public async Task GetAllStarshipsAsync_ReturnsEmpty_WhenArrayIsEmpty()
    {
        var handler = new StubHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("[]")
        });

        var service = new SwapiService(new HttpClient(handler));
        var result = await service.GetAllStarshipsAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public void GetDbMappedStarships_MapsEachItemViaMapToDb()
    {
        var api = new List<StarshipAPIModel>
        {
            new()
            {
                Name = "A",
                Model = "M",
                Manufacturer = "MF",
                CostInCredits = "1",
                Length = "1",
                MaxAtmospheringSpeed = "1",
                Crew = "1",
                Passengers = "1",
                CargoCapacity = "1",
                Consumables = "1",
                HyperdriveRating = "1",
                MGLT = "1",
                StarshipClass = "C",
                Pilots = new List<string> { "p" },
                Films = new List<string> { "f" },
                Created = "c",
                Edited = "e",
                Url = "https://u/1"
            },
            new()
            {
                Name = "B",
                Model = "M2",
                Manufacturer = "MF",
                CostInCredits = "2",
                Length = "2",
                MaxAtmospheringSpeed = "2",
                Crew = "2",
                Passengers = "2",
                CargoCapacity = "2",
                Consumables = "2",
                HyperdriveRating = "2",
                MGLT = "2",
                StarshipClass = "C2",
                Pilots = new List<string>(),
                Films = new List<string>(),
                Created = "c2",
                Edited = "e2",
                Url = "https://u/2"
            }
        };

        var handler = new StubHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("[]")
        });
        var service = new SwapiService(new HttpClient(handler));

        var db = service.GetDbMappedStarships(api);

        db.Should().HaveCount(2);
        db[0].Name.Should().Be("A");
        db[0].Pilots.Should().Equal("p");
        db[1].Name.Should().Be("B");
        db[1].Url.Should().Be("https://u/2");
    }
}
