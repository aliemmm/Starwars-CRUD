using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Starwars.App.Data;
using Starwars.App.Models.DomainModels;
using Starwars.App.Services;

namespace Starwars.App.Tests.Integration;

/// <summary>
/// Runs the real app pipeline with fake SWAPI and authorization relaxed so [Authorize] can be exercised without signing in.
/// Sets <c>IntegrationTests:UseInMemory</c> so <see cref="Program"/> uses EF InMemory and HTTP plus test scopes share one database.
/// </summary>
public class StarwarsWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["IntegrationTests:UseInMemory"] = "true"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<ISwapiService>();
            services.AddScoped<ISwapiService, FakeSwapiService>();

            services.PostConfigure<AuthorizationOptions>(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAssertion(_ => true)
                    .Build();
            });
        });
    }

    public void ClearStarships()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Starships.RemoveRange(db.Starships);
        db.SaveChanges();
    }

    public async Task<int> SeedStarshipAsync(StarshipDbSet entity)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Starships.Add(entity);
        await db.SaveChangesAsync();
        return entity.Id;
    }
}
