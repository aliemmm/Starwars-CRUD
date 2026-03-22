using Xunit;

namespace Starwars.App.Tests.Integration;

/// <summary>Serializes integration tests that share one <see cref="StarwarsWebApplicationFactory"/> and in-memory database.</summary>
[CollectionDefinition("Starships integration")]
public class StarshipsIntegrationCollection : ICollectionFixture<StarwarsWebApplicationFactory>
{
}
