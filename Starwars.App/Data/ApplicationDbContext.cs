using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Starwars.App.Models.DomainModels;

namespace Starwars.App.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<StarshipDbSet> Starships { get; set; }

    public override int SaveChanges()
    {
        ApplyStarshipAuditTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyStarshipAuditTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyStarshipAuditTimestamps()
    {
        var utcNow = DateTime.UtcNow.ToString("o");
        foreach (var entry in ChangeTracker.Entries<StarshipDbSet>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (string.IsNullOrWhiteSpace(entry.Entity.Created))
                        entry.Entity.Created = utcNow;
                    if (string.IsNullOrWhiteSpace(entry.Entity.Edited))
                        entry.Entity.Edited = utcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.Edited = utcNow;
                    break;
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StarshipDbSet>()
            .Property(s => s.Pilots)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<string>>(v) ?? new List<string>()
            );

        modelBuilder.Entity<StarshipDbSet>()
            .Property(s => s.Films)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<string>>(v) ?? new List<string>()
            );
    }
}
