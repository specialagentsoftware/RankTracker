using RankTracker.Data;
using RankTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GameRankTracker.Controllers
{
    public class RankEntryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public RankEntryController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /RankEntry/
        public async Task<IActionResult> Index()
        {
            var rankEntries = _context.RankEntries.Include(r => r.Game).Include(r => r.User);
            return View(await rankEntries.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var rankEntry = await _context.RankEntries
                .Include(r => r.Game)  // Include related Game data
                .Include(r => r.User)  // Include User info if needed
                .FirstOrDefaultAsync(m => m.Id == id);

            if (rankEntry == null) return NotFound();

            return View(rankEntry);
        }

        // GET: /RankEntry/Create
        public IActionResult Create()
        {
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Name");
            return View();
        }

        // POST: /RankEntry/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Rank,Date,Description,GameId")] RankEntry rankEntry)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(); // User is not logged in
            }

            rankEntry.UserId = user.Id; // Set UserId BEFORE ModelState validation
            var gm = await _context.Games.FindAsync(rankEntry.GameId);
            rankEntry.Game = gm;

            if (rankEntry.GameId == 0)
            {
                ModelState.AddModelError("GameId", "You must select a game.");
            }

            if (rankEntry.UserId != null && rankEntry.GameId != 0 && rankEntry.Date != DateTime.MinValue && rankEntry.Rank != null)
            {
                _context.Add(rankEntry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Name", rankEntry.GameId);
            return View(rankEntry);
        }

        // GET: /RankEntry/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var rankEntry = await _context.RankEntries.FindAsync(id);
            if (rankEntry == null) return NotFound();

            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Name", rankEntry.GameId);
            return View(rankEntry);
        }

        // POST: /RankEntry/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Rank,Date,Description,GameId")] RankEntry rankEntry)
        {
            if (id != rankEntry.Id) return NotFound();

            var existingRankEntry = await _context.RankEntries.AsNoTracking().FirstOrDefaultAsync(re => re.Id == id);
            if (existingRankEntry == null) return NotFound();

            rankEntry.UserId = existingRankEntry.UserId;

            if (rankEntry.UserId != null && rankEntry.GameId != 0 && rankEntry.Date != DateTime.MinValue && rankEntry.Rank != null)
            {
                try
                {
                    _context.Update(rankEntry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.RankEntries.Any(e => e.Id == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Name", rankEntry.GameId);
            return View(rankEntry);
        }

        // GET: /RankEntry/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var rankEntry = await _context.RankEntries.Include(r => r.Game).FirstOrDefaultAsync(m => m.Id == id);
            if (rankEntry == null) return NotFound();

            return View(rankEntry);
        }

        // POST: /RankEntry/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rankEntry = await _context.RankEntries.FindAsync(id);
            if (rankEntry != null)
            {
                _context.RankEntries.Remove(rankEntry);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

