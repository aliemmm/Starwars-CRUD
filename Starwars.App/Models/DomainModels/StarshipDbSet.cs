using System.ComponentModel.DataAnnotations;

namespace Starwars.App.Models.DomainModels;

public class StarshipDbSet
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public required string Name { get; set; }

    [Required, MaxLength(100)]
    public required string Model { get; set; }

    [Required, MaxLength(100)]
    public required string Manufacturer { get; set; }

    [Required, MaxLength(100)]
    public required string CostInCredits { get; set; }

    [Required, MaxLength(100)]
    public required string Length { get; set; }

    [Required, MaxLength(100)]
    public required string MaxAtmospheringSpeed { get; set; }

    [Required, MaxLength(100)]
    public required string Crew { get; set; }

    [Required, MaxLength(100)]
    public required string Passengers { get; set; }

    [Required, MaxLength(100)]
    public required string CargoCapacity { get; set; }

    [Required, MaxLength(100)]
    public required string Consumables { get; set; }

    [Required, MaxLength(100)]
    public required string HyperdriveRating { get; set; }

    [Required, MaxLength(100)]
    public required string MGLT { get; set; }

    [Required, MaxLength(100)]
    public required string StarshipClass { get; set; }

    [Required]
    public required List<string> Pilots { get; set; }

    [Required]
    public required List<string> Films { get; set; }

    [Required, MaxLength(100)]
    public required string Created { get; set; }

    [Required, MaxLength(100)]
    public required string Edited { get; set; }

    [Required, MaxLength(100)]
    public required string Url { get; set; }
}
