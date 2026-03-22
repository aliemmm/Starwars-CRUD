using FluentAssertions;
using Starwars.App.Models.APIModels;
using Starwars.App.Models.DomainModels;
using Starwars.App.Models.Mappers;
using Starwars.App.Models.ViewModels;
using Xunit;

namespace Starwars.App.Tests.Mappers;

public class StarshipMapperTests
{
    [Theory]
    [InlineData(1, "/Starships/Edit/1")]
    [InlineData(42, "/Starships/Edit/42")]
    public void RelativeUrlForStarship_Returns_AppRelativePath(int id, string expected)
    {
        StarshipMapper.RelativeUrlForStarship(id).Should().Be(expected);
    }

    [Fact]
    public void ToStarshipDbSet_MapsScalars_AndParsesCsvLists()
    {
        var vm = new StarshipViewModel
        {
            Name = "Falcon",
            Model = "YT-1300",
            Manufacturer = "Corellian",
            CostInCredits = "100000",
            Length = "34.37",
            MaxAtmospheringSpeed = "1050",
            Crew = "4",
            Passengers = "6",
            CargoCapacity = "100",
            Consumables = "2 months",
            HyperdriveRating = "0.5",
            MGLT = "75",
            StarshipClass = "Light freighter",
            PilotsCsv = " https://swapi.dev/api/people/1/ , https://swapi.dev/api/people/13/ ",
            FilmsCsv = "https://swapi.dev/api/films/1/"
        };

        var db = vm.ToStarshipDbSet();

        db.Name.Should().Be("Falcon");
        db.Pilots.Should().HaveCount(2);
        db.Pilots[0].Should().Be("https://swapi.dev/api/people/1/");
        db.Pilots[1].Should().Be("https://swapi.dev/api/people/13/");
        db.Films.Should().ContainSingle().Which.Should().Be("https://swapi.dev/api/films/1/");
        db.Created.Should().BeEmpty();
        db.Edited.Should().BeEmpty();
        db.Url.Should().BeEmpty();
    }

    [Fact]
    public void ToStarshipDbSet_EmptyCsv_YieldsEmptyLists()
    {
        var vm = new StarshipViewModel
        {
            Name = "A",
            Model = "B",
            Manufacturer = "C",
            CostInCredits = "1",
            Length = "1",
            MaxAtmospheringSpeed = "1",
            Crew = "1",
            Passengers = "1",
            CargoCapacity = "1",
            Consumables = "1",
            HyperdriveRating = "1",
            MGLT = "1",
            StarshipClass = "X",
            PilotsCsv = null,
            FilmsCsv = "   "
        };

        var db = vm.ToStarshipDbSet();

        db.Pilots.Should().BeEmpty();
        db.Films.Should().BeEmpty();
    }

    [Fact]
    public void MapToDb_Copies_AllFields_FromApiModel()
    {
        var api = new StarshipAPIModel
        {
            Name = "X-Wing",
            Model = "T-65",
            Manufacturer = "Incom",
            CostInCredits = "149999",
            Length = "12.5",
            MaxAtmospheringSpeed = "1050",
            Crew = "1",
            Passengers = "0",
            CargoCapacity = "110",
            Consumables = "1 week",
            HyperdriveRating = "1.0",
            MGLT = "100",
            StarshipClass = "Starfighter",
            Created = "2014-12-09T13:50:51.644000Z",
            Edited = "2014-12-20T21:23:49.870000Z",
            Pilots = new List<string> { "https://swapi.dev/api/people/1/" },
            Films = new List<string> { "https://swapi.dev/api/films/1/" },
            Url = "https://swapi.dev/api/starships/12/"
        };

        var db = api.MapToDb();

        db.Name.Should().Be("X-Wing");
        db.Model.Should().Be("T-65");
        db.Manufacturer.Should().Be("Incom");
        db.CostInCredits.Should().Be("149999");
        db.Length.Should().Be("12.5");
        db.MaxAtmospheringSpeed.Should().Be("1050");
        db.Crew.Should().Be("1");
        db.Passengers.Should().Be("0");
        db.CargoCapacity.Should().Be("110");
        db.Consumables.Should().Be("1 week");
        db.HyperdriveRating.Should().Be("1.0");
        db.MGLT.Should().Be("100");
        db.StarshipClass.Should().Be("Starfighter");
        db.Created.Should().Be("2014-12-09T13:50:51.644000Z");
        db.Edited.Should().Be("2014-12-20T21:23:49.870000Z");
        db.Url.Should().Be("https://swapi.dev/api/starships/12/");
        db.Pilots.Should().Equal(api.Pilots);
        db.Films.Should().Equal(api.Films);
    }

    [Fact]
    public void ToStarshipViewModel_BuildsCsv_FromLists()
    {
        var entity = new StarshipDbSet
        {
            Id = 7,
            Name = "A",
            Model = "B",
            Manufacturer = "C",
            CostInCredits = "1",
            Length = "1",
            MaxAtmospheringSpeed = "1",
            Crew = "1",
            Passengers = "1",
            CargoCapacity = "1",
            Consumables = "1",
            HyperdriveRating = "1",
            MGLT = "1",
            StarshipClass = "X",
            Pilots = new List<string> { "p1", "p2" },
            Films = new List<string>(),
            Created = "c",
            Edited = "e",
            Url = "/u"
        };

        var vm = entity.ToStarshipViewModel();

        vm.Id.Should().Be(7);
        vm.PilotsCsv.Should().Be("p1, p2");
        vm.FilmsCsv.Should().BeNull();
    }

    [Fact]
    public void ToStarshipViewModel_JoinsFilms_WhenFilmsPresent()
    {
        var entity = new StarshipDbSet
        {
            Id = 1,
            Name = "A",
            Model = "B",
            Manufacturer = "C",
            CostInCredits = "1",
            Length = "1",
            MaxAtmospheringSpeed = "1",
            Crew = "1",
            Passengers = "1",
            CargoCapacity = "1",
            Consumables = "1",
            HyperdriveRating = "1",
            MGLT = "1",
            StarshipClass = "X",
            Pilots = new List<string>(),
            Films = new List<string> { "https://swapi.dev/api/films/1/", "https://swapi.dev/api/films/2/" },
            Created = "c",
            Edited = "e",
            Url = "/u"
        };

        var vm = entity.ToStarshipViewModel();

        vm.FilmsCsv.Should().Be("https://swapi.dev/api/films/1/, https://swapi.dev/api/films/2/");
        vm.PilotsCsv.Should().BeNull();
    }

    [Fact]
    public void ToStarshipDbSet_ParseCommaSeparatedPilots()
    {
        var vm = new StarshipViewModel
        {
            Name = "A",
            Model = "B",
            Manufacturer = "C",
            CostInCredits = "1",
            Length = "1",
            MaxAtmospheringSpeed = "1",
            Crew = "1",
            Passengers = "1",
            CargoCapacity = "1",
            Consumables = "1",
            HyperdriveRating = "1",
            MGLT = "1",
            StarshipClass = "X",
            PilotsCsv = "alpha,beta,gamma",
            FilmsCsv = ""
        };

        var db = vm.ToStarshipDbSet();

        db.Pilots.Should().Equal("alpha", "beta", "gamma");
    }

    [Fact]
    public void ApplyEdit_UpdatesEntity_AndSetsRelativeUrl()
    {
        var entity = new StarshipDbSet
        {
            Id = 3,
            Name = "Old",
            Model = "Old",
            Manufacturer = "Old",
            CostInCredits = "0",
            Length = "0",
            MaxAtmospheringSpeed = "0",
            Crew = "0",
            Passengers = "0",
            CargoCapacity = "0",
            Consumables = "0",
            HyperdriveRating = "0",
            MGLT = "0",
            StarshipClass = "Old",
            Pilots = new List<string>(),
            Films = new List<string>(),
            Created = "unchanged",
            Edited = "unchanged",
            Url = "/old"
        };

        var vm = new StarshipViewModel
        {
            Id = 3,
            Name = "New",
            Model = "N",
            Manufacturer = "N",
            CostInCredits = "1",
            Length = "1",
            MaxAtmospheringSpeed = "1",
            Crew = "1",
            Passengers = "1",
            CargoCapacity = "1",
            Consumables = "1",
            HyperdriveRating = "1",
            MGLT = "1",
            StarshipClass = "N",
            PilotsCsv = "a,b",
            FilmsCsv = null
        };

        vm.ApplyEdit(entity);

        entity.Name.Should().Be("New");
        entity.Model.Should().Be("N");
        entity.Manufacturer.Should().Be("N");
        entity.CostInCredits.Should().Be("1");
        entity.Length.Should().Be("1");
        entity.MaxAtmospheringSpeed.Should().Be("1");
        entity.Crew.Should().Be("1");
        entity.Passengers.Should().Be("1");
        entity.CargoCapacity.Should().Be("1");
        entity.Consumables.Should().Be("1");
        entity.HyperdriveRating.Should().Be("1");
        entity.MGLT.Should().Be("1");
        entity.StarshipClass.Should().Be("N");
        entity.Created.Should().Be("unchanged");
        entity.Edited.Should().Be("unchanged");
        entity.Url.Should().Be("/Starships/Edit/3");
        entity.Pilots.Should().Equal("a", "b");
        entity.Films.Should().BeEmpty();
    }
}
