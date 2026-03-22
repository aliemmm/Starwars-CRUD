using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Starwars.App.Data;
using Starwars.App.Models.Mappers;
using Starwars.App.Models.ViewModels;

namespace Starwars.App.Controllers;

[Authorize]
public class StarshipsController : Controller
{
    private readonly ApplicationDbContext _context;

    public StarshipsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var rows = await _context.Starships.AsNoTracking().ToListAsync();
        return View(rows.Select(s => s.ToStarshipViewModel()).ToList());
    }

    public IActionResult Create()
    {
        return View(new StarshipViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StarshipViewModel vm)
    {
        if (ModelState.IsValid)
        {
            var entity = vm.ToStarshipDbSet();
            _context.Add(entity);
            await _context.SaveChangesAsync();
            entity.Url = StarshipMapper.RelativeUrlForStarship(entity.Id);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(vm);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var starship = await _context.Starships.FindAsync(id);
        if (starship == null) return NotFound();
        return View(starship.ToStarshipViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, StarshipViewModel vm)
    {
        if (id != vm.Id) return NotFound();

        if (ModelState.IsValid)
        {
            var entity = await _context.Starships.FindAsync(id);
            if (entity == null) return NotFound();

            vm.ApplyEdit(entity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(vm);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var starship = await _context.Starships.FirstOrDefaultAsync(m => m.Id == id);
        if (starship == null) return NotFound();
        return View(starship);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var starship = await _context.Starships.FindAsync(id);
        if (starship != null) _context.Starships.Remove(starship);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
