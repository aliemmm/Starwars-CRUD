using OpenAI.Chat;
using Starwars.App.Models.DomainModels;

namespace Starwars.App.Services;


public interface IStarshipAiService
{
    Task<string> GenerateStarshipLoreAsync(StarshipDbSet starship);
}

public class StarshipAiService : IStarshipAiService
{
    private readonly ChatClient _chatClient;

    public StarshipAiService(IConfiguration configuration)
    {
        string apiKey = configuration["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("OpenAI API Key is missing.");

        _chatClient = new ChatClient("gpt-4o", apiKey);
    }

    public async Task<string> GenerateStarshipLoreAsync(StarshipDbSet starship)
    {
        var prompt = $"Write a short, engaging, 2-paragraph sci-fi lore description for the Star Wars starship named '{starship.Name}'. It is a {starship.StarshipClass} manufactured by {starship.Manufacturer} with a hyperdrive rating of {starship.HyperdriveRating}.";

        var completion = await _chatClient.CompleteChatAsync(prompt);
        return completion.Value.Content[0].Text;
    }
}

