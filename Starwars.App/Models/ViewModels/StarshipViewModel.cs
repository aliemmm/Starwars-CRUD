using System.ComponentModel.DataAnnotations;

namespace Starwars.App.Models.ViewModels;

public class StarshipViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Model { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Manufacturer { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    [Display(Name = "Cost In Credits")]
    public string CostInCredits { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Length { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    [Display(Name = "Max Atmosphering Speed")]
    public string MaxAtmospheringSpeed { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Crew { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Passengers { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    [Display(Name = "Cargo Capacity")]
    public string CargoCapacity { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Consumables { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    [Display(Name = "Hyperdrive Rating")]
    public string HyperdriveRating { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string MGLT { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    [Display(Name = "Starship Class")]
    public string StarshipClass { get; set; } = string.Empty;

    [Display(Name = "Pilots")]
    public string? PilotsCsv { get; set; }

    [Display(Name = "Films")]
    public string? FilmsCsv { get; set; }
}
