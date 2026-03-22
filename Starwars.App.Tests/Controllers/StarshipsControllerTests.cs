using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Starwars.App.Controllers;
using Starwars.App.Data;
using Starwars.App.Models.DomainModels;
using Starwars.App.Models.Mappers;
using Starwars.App.Models.ViewModels;
using Xunit;

namespace Starwars.App.Tests.Controllers;

public class StarshipsControllerTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static StarshipDbSet CreateStarship(int id, string name = "Ship")
    {
        return new StarshipDbSet
        {
            Id = id,
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

    private static StarshipViewModel ValidViewModel(int id = 0) => new()
    {
        Id = id,
        Name = "N",
        Model = "Model",
        Manufacturer = "Manufacturer",
        CostInCredits = "1",
        Length = "1",
        MaxAtmospheringSpeed = "1",
        Crew = "1",
        Passengers = "1",
        CargoCapacity = "1",
        Consumables = "1",
        HyperdriveRating = "1",
        MGLT = "1",
        StarshipClass = "Class"
    };

    [Fact]
    public async Task Index_ReturnsView_WithMappedStarships()
    {
        await using var context = CreateContext();
        context.Starships.Add(CreateStarship(1, "Alpha"));
        context.Starships.Add(CreateStarship(2, "Beta"));
        await context.SaveChangesAsync();

        var controller = new StarshipsController(context);

        var result = await controller.Index();

        var view = result.Should().BeOfType<ViewResult>().Subject;
        var list = view.Model.Should().BeOfType<List<StarshipViewModel>>().Subject;
        list.Should().HaveCount(2);
        list.Select(x => x.Name).Should().BeEquivalentTo("Alpha", "Beta");
    }

    [Fact]
    public void Create_Get_ReturnsView_WithEmptyViewModel()
    {
        using var context = CreateContext();
        var controller = new StarshipsController(context);

        var result = controller.Create();

        var view = result.Should().BeOfType<ViewResult>().Subject;
        view.Model.Should().BeOfType<StarshipViewModel>().Which.Name.Should().BeEmpty();
    }

    [Fact]
    public async Task Create_Post_Valid_PersistsEntity_SetsUrlAndRedirects()
    {
        await using var context = CreateContext();
        var controller = new StarshipsController(context);
        var vm = ValidViewModel();

        var result = await controller.Create(vm);

        var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be(nameof(StarshipsController.Index));

        var saved = await context.Starships.SingleAsync();
        saved.Name.Should().Be("N");
        saved.Url.Should().Be(StarshipMapper.RelativeUrlForStarship(saved.Id));
    }

    [Fact]
    public async Task Create_Post_Invalid_ReturnsViewWithSameModel()
    {
        await using var context = CreateContext();
        var controller = new StarshipsController(context);
        var vm = ValidViewModel();
        controller.ModelState.AddModelError(nameof(StarshipViewModel.Name), "Required");

        var result = await controller.Create(vm);

        var view = result.Should().BeOfType<ViewResult>().Subject;
        view.Model.Should().BeSameAs(vm);
        context.Starships.Should().BeEmpty();
    }

    [Fact]
    public async Task Edit_Get_NullId_ReturnsNotFound()
    {
        await using var context = CreateContext();
        var controller = new StarshipsController(context);

        var result = await controller.Edit(null);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Edit_Get_MissingId_ReturnsNotFound()
    {
        await using var context = CreateContext();
        var controller = new StarshipsController(context);

        var result = await controller.Edit(99);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Edit_Get_ReturnsView_WithStarshipViewModel()
    {
        await using var context = CreateContext();
        var entity = CreateStarship(1, "Falcon");
        context.Starships.Add(entity);
        await context.SaveChangesAsync();

        var controller = new StarshipsController(context);

        var result = await controller.Edit(1);

        var view = result.Should().BeOfType<ViewResult>().Subject;
        var vm = view.Model.Should().BeOfType<StarshipViewModel>().Subject;
        vm.Id.Should().Be(1);
        vm.Name.Should().Be("Falcon");
    }

    [Fact]
    public async Task Edit_Post_IdMismatch_ReturnsNotFound()
    {
        await using var context = CreateContext();
        var controller = new StarshipsController(context);
        var vm = ValidViewModel(id: 5);

        var result = await controller.Edit(3, vm);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Edit_Post_EntityMissing_ReturnsNotFound()
    {
        await using var context = CreateContext();
        var controller = new StarshipsController(context);
        var vm = ValidViewModel(id: 5);

        var result = await controller.Edit(5, vm);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Edit_Post_Valid_UpdatesEntityAndRedirects()
    {
        await using var context = CreateContext();
        context.Starships.Add(CreateStarship(1, "Old"));
        await context.SaveChangesAsync();

        var controller = new StarshipsController(context);
        var vm = ValidViewModel(id: 1);
        vm.Name = "Updated";

        var result = await controller.Edit(1, vm);

        var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be(nameof(StarshipsController.Index));

        var saved = await context.Starships.SingleAsync();
        saved.Name.Should().Be("Updated");
        saved.Url.Should().Be(StarshipMapper.RelativeUrlForStarship(1));
    }

    [Fact]
    public async Task Edit_Post_Invalid_ReturnsViewModel()
    {
        await using var context = CreateContext();
        context.Starships.Add(CreateStarship(1, "Old"));
        await context.SaveChangesAsync();

        var controller = new StarshipsController(context);
        var vm = ValidViewModel(id: 1);
        controller.ModelState.AddModelError(nameof(StarshipViewModel.Name), "Required");

        var result = await controller.Edit(1, vm);

        var view = result.Should().BeOfType<ViewResult>().Subject;
        view.Model.Should().BeSameAs(vm);
    }

    [Fact]
    public async Task Delete_Get_NullId_ReturnsNotFound()
    {
        await using var context = CreateContext();
        var controller = new StarshipsController(context);

        var result = await controller.Delete(null);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_Get_MissingId_ReturnsNotFound()
    {
        await using var context = CreateContext();
        var controller = new StarshipsController(context);

        var result = await controller.Delete(42);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_Get_ReturnsView_WithEntity()
    {
        await using var context = CreateContext();
        var entity = CreateStarship(1, "ToDelete");
        context.Starships.Add(entity);
        await context.SaveChangesAsync();

        var controller = new StarshipsController(context);

        var result = await controller.Delete(1);

        var view = result.Should().BeOfType<ViewResult>().Subject;
        var model = view.Model.Should().BeOfType<StarshipDbSet>().Subject;
        model.Name.Should().Be("ToDelete");
    }

    [Fact]
    public async Task DeleteConfirmed_RemovesEntity()
    {
        await using var context = CreateContext();
        context.Starships.Add(CreateStarship(1, "Gone"));
        await context.SaveChangesAsync();

        var controller = new StarshipsController(context);

        var result = await controller.DeleteConfirmed(1);

        result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be(nameof(StarshipsController.Index));
        context.Starships.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteConfirmed_WhenMissing_StillRedirects()
    {
        await using var context = CreateContext();
        var controller = new StarshipsController(context);

        var result = await controller.DeleteConfirmed(999);

        result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be(nameof(StarshipsController.Index));
    }
}
