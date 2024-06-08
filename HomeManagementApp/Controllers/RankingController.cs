using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeManagementApp.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace HomeManagementApp.Controllers
{
    [Authorize]
    public class RankingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RankingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var rankingViewModel = new RankingViewModel
            {
                PointsRanking = await _context.Users
                    .OrderByDescending(u => u.Points)
                    .Select(u => new RankingEntry
                    {
                        UserName = u.UserName,
                        Value = u.Points
                    }).ToListAsync(),

                TasksRanking = await _context.UserTasks
                    .GroupBy(ut => ut.UserId)
                    .Select(g => new RankingEntry
                    {
                        UserName = g.First().User.UserName,
                        Value = g.Count()
                    }).OrderByDescending(e => e.Value)
                    .ToListAsync(),

                RewardsRanking = await _context.UserRewards
                    .GroupBy(ur => ur.UserId)
                    .Select(g => new RankingEntry
                    {
                        UserName = g.First().User.UserName,
                        Value = g.Count()
                    }).OrderByDescending(e => e.Value)
                    .ToListAsync()
            };

            return View(rankingViewModel);
        }
    }
}
