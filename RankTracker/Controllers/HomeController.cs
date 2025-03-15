using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using RankTracker.Models;
using System.Diagnostics;
using RankTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace RankTracker.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);

            var totalGames = await _context.Games.CountAsync();
            var totalRankEntries = await _context.RankEntries.CountAsync();
            var userRankEntries = await _context.RankEntries
                                                .Where(re => re.UserId == userId)
                                                .OrderByDescending(re => re.Date)
                                                .Take(5)
                                                .ToListAsync();
            var dashboardData = new
            {
                TotalGames = totalGames,
                TotalRankEntries = totalRankEntries,
                UserRankEntries = userRankEntries
            };
            return View(dashboardData);
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
