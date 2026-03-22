using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Starwars.App.Data;
using Starwars.App.Models.DomainModels;
using Xunit;

namespace Starwars.App.Tests.Integration;

[Collection("Starships integration")]
public class StarshipsIntegrationTests
{
    private readonly StarwarsWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public StarshipsIntegrationTests(StarwarsWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Get_Index_ReturnsOk_AndListsStarships()
    {
        _factory.ClearStarships();
        await _factory.SeedStarshipAsync(NewStarship(name: "Listed"));

        var response = await _client.GetAsync("/Starships");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("Listed");
    }

    [Fact]
    public async Task Get_Create_ReturnsOk_WithForm()
    {
        _factory.ClearStarships();

        var response = await _client.GetAsync("/Starships/Create");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("Create Starship");
        html.Should().Contain("__RequestVerificationToken");
    }

    [Fact]
    public async Task Post_Create_Valid_RedirectsToIndex_AndPersists()
    {
        _factory.ClearStarships();

        var get = await _client.GetAsync("/Starships/Create");
        var html = await get.Content.ReadAsStringAsync();
        var token = AntiforgeryFormHelper.ExtractRequestVerificationToken(html);

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Name"] = "IntTest Ship",
            ["Model"] = "M1",
            ["Manufacturer"] = "Mfg",
            ["CostInCredits"] = "1",
            ["Length"] = "10",
            ["MaxAtmospheringSpeed"] = "100",
            ["Crew"] = "1",
            ["Passengers"] = "0",
            ["CargoCapacity"] = "50",
            ["Consumables"] = "1 day",
            ["HyperdriveRating"] = "1",
            ["MGLT"] = "50",
            ["StarshipClass"] = "Transport",
            ["PilotsCsv"] = "",
            ["FilmsCsv"] = ""
        };

        var post = await _client.PostAsync("/Starships/Create", new FormUrlEncodedContent(form));

        post.StatusCode.Should().Be(HttpStatusCode.Redirect);
        post.Headers.Location.Should().NotBeNull();
        post.Headers.Location!.ToString().Should().Contain("Starships");

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var row = await db.Starships.SingleAsync();
        row.Name.Should().Be("IntTest Ship");
        row.Url.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Get_Edit_ReturnsOk_WhenStarshipExists()
    {
        _factory.ClearStarships();
        var id = await _factory.SeedStarshipAsync(NewStarship(name: "Editable"));

        var response = await _client.GetAsync($"/Starships/Edit/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("Edit Starship");
        html.Should().Contain("Editable");
    }

    [Fact]
    public async Task Post_Edit_Valid_UpdatesAndRedirects()
    {
        _factory.ClearStarships();
        var id = await _factory.SeedStarshipAsync(NewStarship(name: "Before"));

        var get = await _client.GetAsync($"/Starships/Edit/{id}");
        var html = await get.Content.ReadAsStringAsync();
        var token = AntiforgeryFormHelper.ExtractRequestVerificationToken(html);

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Id"] = id.ToString(),
            ["Name"] = "After",
            ["Model"] = "M2",
            ["Manufacturer"] = "Mfg2",
            ["CostInCredits"] = "2",
            ["Length"] = "20",
            ["MaxAtmospheringSpeed"] = "200",
            ["Crew"] = "2",
            ["Passengers"] = "1",
            ["CargoCapacity"] = "60",
            ["Consumables"] = "2 days",
            ["HyperdriveRating"] = "2",
            ["MGLT"] = "60",
            ["StarshipClass"] = "Freighter",
            ["PilotsCsv"] = "",
            ["FilmsCsv"] = ""
        };

        var post = await _client.PostAsync($"/Starships/Edit/{id}", new FormUrlEncodedContent(form));

        post.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var row = await db.Starships.SingleAsync();
        row.Name.Should().Be("After");
    }

    [Fact]
    public async Task Post_Delete_RemovesStarship_AndRedirects()
    {
        _factory.ClearStarships();
        var id = await _factory.SeedStarshipAsync(NewStarship(name: "RemoveMe"));

        var get = await _client.GetAsync($"/Starships/Delete/{id}");
        var html = await get.Content.ReadAsStringAsync();
        var token = AntiforgeryFormHelper.ExtractRequestVerificationToken(html);

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Id"] = id.ToString()
        };

        var post = await _client.PostAsync("/Starships/Delete", new FormUrlEncodedContent(form));

        post.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Starships.Should().BeEmpty();
    }

    private static StarshipDbSet NewStarship(string name) => new()
    {
        Name = name,
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
        Pilots = new List<string>(),
        Films = new List<string>(),
        Created = "2020-01-01T00:00:00.0000000Z",
        Edited = "2020-01-01T00:00:00.0000000Z",
        Url = "/"
    };
}
