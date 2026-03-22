using Starwars.App.Models.APIModels;
using Starwars.App.Models.DomainModels;
using Starwars.App.Models.ViewModels;

namespace Starwars.App.Models.Mappers;

public static class StarshipMapper
{
    public static StarshipDbSet MapToDb(this StarshipAPIModel starship)
    {
        return new()
        {
            Name = starship.Name,
            Model = starship.Model,
            Manufacturer = starship.Manufacturer,
            CostInCredits = starship.CostInCredits,
            Length = starship.Length,
            MaxAtmospheringSpeed = starship.MaxAtmospheringSpeed,
            Crew = starship.Crew,
            Passengers = starship.Passengers,
            CargoCapacity = starship.CargoCapacity,
            Consumables = starship.Consumables,
            HyperdriveRating = starship.HyperdriveRating,
            MGLT = starship.MGLT,
            StarshipClass = starship.StarshipClass,
            Created = starship.Created,
            Edited = starship.Edited,
            Films = starship.Films,
            Pilots = starship.Pilots,
            Url = starship.Url
        };
    }

    public static StarshipDbSet ToStarshipDbSet(this StarshipViewModel vm)
    {
        return new StarshipDbSet
        {
            Name = vm.Name,
            Model = vm.Model,
            Manufacturer = vm.Manufacturer,
            CostInCredits = vm.CostInCredits,
            Length = vm.Length,
            MaxAtmospheringSpeed = vm.MaxAtmospheringSpeed,
            Crew = vm.Crew,
            Passengers = vm.Passengers,
            CargoCapacity = vm.CargoCapacity,
            Consumables = vm.Consumables,
            HyperdriveRating = vm.HyperdriveRating,
            MGLT = vm.MGLT,
            StarshipClass = vm.StarshipClass,
            Pilots = ParseCommaSeparatedList(vm.PilotsCsv),
            Films = ParseCommaSeparatedList(vm.FilmsCsv),
            Created = string.Empty,
            Edited = string.Empty,
            Url = string.Empty
        };
    }

    public static StarshipViewModel ToStarshipViewModel(this StarshipDbSet entity)
    {
        return new StarshipViewModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Model = entity.Model,
            Manufacturer = entity.Manufacturer,
            CostInCredits = entity.CostInCredits,
            Length = entity.Length,
            MaxAtmospheringSpeed = entity.MaxAtmospheringSpeed,
            Crew = entity.Crew,
            Passengers = entity.Passengers,
            CargoCapacity = entity.CargoCapacity,
            Consumables = entity.Consumables,
            HyperdriveRating = entity.HyperdriveRating,
            MGLT = entity.MGLT,
            StarshipClass = entity.StarshipClass,
            PilotsCsv = entity.Pilots.Count > 0 ? string.Join(", ", entity.Pilots) : null,
            FilmsCsv = entity.Films.Count > 0 ? string.Join(", ", entity.Films) : null
        };
    }

    public static void ApplyEdit(this StarshipViewModel vm, StarshipDbSet entity)
    {
        entity.Name = vm.Name;
        entity.Model = vm.Model;
        entity.Manufacturer = vm.Manufacturer;
        entity.CostInCredits = vm.CostInCredits;
        entity.Length = vm.Length;
        entity.MaxAtmospheringSpeed = vm.MaxAtmospheringSpeed;
        entity.Crew = vm.Crew;
        entity.Passengers = vm.Passengers;
        entity.CargoCapacity = vm.CargoCapacity;
        entity.Consumables = vm.Consumables;
        entity.HyperdriveRating = vm.HyperdriveRating;
        entity.MGLT = vm.MGLT;
        entity.StarshipClass = vm.StarshipClass;
        entity.Pilots = ParseCommaSeparatedList(vm.PilotsCsv);
        entity.Films = ParseCommaSeparatedList(vm.FilmsCsv);
        entity.Url = RelativeUrlForStarship(entity.Id);
    }

    /// <summary>App-relative path for a starship after the database Id is known.</summary>
    public static string RelativeUrlForStarship(int id) => $"/Starships/Edit/{id}";

    private static List<string> ParseCommaSeparatedList(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return new List<string>();

        return value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
    }
}
