using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Starwars.App.Data;
using Starwars.App.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var useIntegrationInMemory = builder.Configuration.GetValue<bool>("IntegrationTests:UseInMemory");
if (useIntegrationInMemory)
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase($"IntegrationTests_{Guid.NewGuid():N}"));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
}
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ISwapiService, SwapiService>().AddHttpClient();
builder.Services.AddRazorPages();
var app = builder.Build();


//Auto create database and apply migrations with seeding
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();

    if (!app.Configuration.GetValue<bool>("IntegrationTests:UseInMemory") && !db.Starships.Any())
    {
        var swapiService = scope.ServiceProvider.GetRequiredService<ISwapiService>();
        var starships = await swapiService.GetAllStarshipsAsync();

        db.Starships.AddRange(swapiService.GetDbMappedStarships(starships));
        await db.SaveChangesAsync();
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (!app.Configuration.GetValue<bool>("IntegrationTests:UseInMemory"))
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

public partial class Program
{
}
