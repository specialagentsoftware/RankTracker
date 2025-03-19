using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RankTracker.Data;
using RankTracker.Models;
using System.Linq;
using System.Threading.Tasks;

namespace GameRankTracker.Controllers
{
    [Authorize]
    public class GameController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public GameController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Game/
        public async Task<IActionResult> Index()
        {
            var games = _context.Games
            .Include(r => r.User);
            return View(await games.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var game = await _context.Games.FindAsync(id);

            if (game == null) return NotFound();

            return View(game);
        }


        // GET: /Game/Create
        [Authorize(Roles = "Admin,User")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Game/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Create([Bind("Name")] Game game)
        {
            var loggedInUser = await _userManager.GetUserAsync(User);
            if (loggedInUser == null)
            {
                return Unauthorized();
            }

            game.UserId = loggedInUser.Id;

            if (game.Name != null && game.UserId != null)
            {
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(game);
        }

        // GET: /Game/Edit/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var game = await _context.Games.FindAsync(id);
            if (game == null) return NotFound();

            var loggedInUserId = _userManager.GetUserId(User);
            if (game.UserId != loggedInUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(game);
        }

        // POST: /Game/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Game game)
        {
            if (id != game.Id) return NotFound();

            var existingGame = await _context.Games.AsNoTracking().FirstOrDefaultAsync(g => g.Id == id);
            if (existingGame == null) return NotFound();

            if (existingGame.UserId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            game.UserId = existingGame.UserId;

            if (game.Name != null && game.UserId != null)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Games.Any(e => e.Id == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(game);
        }

        // GET: /Game/Delete/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var game = await _context.Games.FirstOrDefaultAsync(m => m.Id == id);
            if (game == null) return NotFound();

            var loggedInUserId = _userManager.GetUserId(User);
            if (game.UserId != loggedInUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(game);
        }

        // POST: /Game/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game != null)
            {
                var loggedInUserId = _userManager.GetUserId(User);
                if (game.UserId != loggedInUserId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                _context.Games.Remove(game);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
