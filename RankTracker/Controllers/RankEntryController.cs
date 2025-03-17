using RankTracker.Data;
using RankTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace GameRankTracker.Controllers
{
    [Authorize]
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
        [Authorize(Roles = "Admin,User")]
        public IActionResult Create()
        {
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Name");
            return View();
        }

        // POST: /RankEntry/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin,User")]
        public async Task<IActionResult> Create([Bind("Rank,Date,Description,GameId")] RankEntry rankEntry)
        {
            var loggedInUser = await _userManager.GetUserAsync(User);
            if (loggedInUser == null)
            {
                return Unauthorized();
            }

            rankEntry.UserId = loggedInUser.Id;

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
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var rankEntry = await _context.RankEntries.FindAsync(id);
            if (rankEntry == null) return NotFound();

            var loggedInUserId = _userManager.GetUserId(User); // Get logged-in user
            if (rankEntry == null || (rankEntry.UserId != loggedInUserId && !User.IsInRole("Admin")))
            {
                return Forbid(); // Block users from editing other users' ranks
            }


            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Name", rankEntry.GameId);
            return View(rankEntry);
        }

        // POST: /RankEntry/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles= "Admin,User")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Rank,Date,Description,GameId")] RankEntry rankEntry)
        {
            if (id != rankEntry.Id) return NotFound();

            var existingRankEntry = await _context.RankEntries.AsNoTracking().FirstOrDefaultAsync(re => re.Id == id);
            if (existingRankEntry == null) return NotFound();

            if (rankEntry.UserId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var rankEntry = await _context.RankEntries.Include(r => r.Game).FirstOrDefaultAsync(m => m.Id == id);
            if (rankEntry == null) return NotFound();

            return View(rankEntry);
        }

        //[HttpPost]
        //public async Task<JsonResult> DeleteConfirmed(int id)
        //{
        //    var rankEntry = await _context.RankEntries.FindAsync(id);
        //    if (rankEntry == null)
        //    {
        //        return Json(new { success = false });
        //    }

        //    _context.RankEntries.Remove(rankEntry);
        //    await _context.SaveChangesAsync();
        //    return Json(new { success = true });
        //}
        // POST: /RankEntry/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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

        public async Task<IActionResult> RankProgression()
        {
            ViewData["Games"] = await _context.Games.ToListAsync();
            ViewData["Users"] = await _context.Users.ToListAsync();
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetRankProgression(string userId, int gameId)
        {
            var rankEntries = await _context.RankEntries
                .Where(r => r.UserId == userId && r.GameId == gameId)
                .OrderBy(r => r.Date)
                .Select(r => new {r.Date, r.Rank})
                .ToListAsync();

            return Json(rankEntries);
        }
    }
}

