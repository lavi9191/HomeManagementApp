using Microsoft.AspNetCore.Mvc;
using HomeManagementApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace HomeManagementApp.Controllers
{
    [Authorize]
    public class RewardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public RewardController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reward
        public async Task<IActionResult> Index()
        {
            var rewards = await _context.Rewards.ToListAsync();
            return View(rewards);
        }

        // GET: Reward/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reward/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Cost")] Reward reward)
        {
            Console.WriteLine("Create method called with: " + reward.Name + ", " + reward.Description + ", " + reward.Cost);

            if (ModelState.IsValid)
            {
                reward.UserId = _userManager.GetUserId(User);  // Assign the UserId
                reward.DateReceived = DateTime.Now;  // Assign the current date
                _context.Add(reward);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reward);
        }


        // GET: Reward/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reward = await _context.Rewards.FindAsync(id);
            if (reward == null)
            {
                return NotFound();
            }
            return View(reward);
        }

        // POST: Reward/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Cost,UserId,DateReceived")] Reward reward)
        {
            if (id != reward.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reward);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RewardExists(reward.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(reward);
        }

        // GET: Reward/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reward = await _context.Rewards
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reward == null)
            {
                return NotFound();
            }

            return View(reward);
        }

        // POST: Reward/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reward = await _context.Rewards.FindAsync(id);
            if (reward != null)
            {
                _context.Rewards.Remove(reward);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Redeem(int id)
        {
            var reward = await _context.Rewards.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);

            if (reward == null || user == null)
            {
                return NotFound();
            }

            if (user.Points >= reward.Cost)
            {
                user.Points -= reward.Cost;
                await _userManager.UpdateAsync(user);

                // Dodajemy odebraną nagrodę do bazy danych jako nowy wpis
                var userReward = new UserReward
                {
                    RewardId = reward.Id,
                    UserId = user.Id,
                    DateReceived = DateTime.Now
                };
                _context.UserRewards.Add(userReward);
                await _context.SaveChangesAsync();

                // Informujemy użytkownika, że nagroda została wymieniona pomyślnie
                TempData["SuccessMessage"] = "Nagroda została wymieniona pomyślnie!";
            }
            else
            {
                TempData["ErrorMessage"] = "Nie masz wystarczającej liczby punktów, aby wymienić tę nagrodę.";
            }

            return RedirectToAction(nameof(Index));
        }



        //Liczba odebranych nagród, punkty wydane na nagrody, lista nagród
        [HttpGet]
        public async Task<IActionResult> RewardsStatistics()
        {
            var user = await _userManager.GetUserAsync(User);
            var userRewards = await _context.UserRewards
                .Where(r => r.UserId == user.Id)
                .Include(r => r.Reward)
                .ToListAsync();

            var totalRewards = userRewards.Count;
            var pointsSpent = userRewards.Sum(r => r.Reward.Cost);
            var rewardsList = userRewards.Select(r => new { r.Reward.Name, r.DateReceived }).ToList();

            var statistics = new
            {
                TotalRewards = totalRewards,
                PointsSpent = pointsSpent,
                RewardsList = rewardsList
            };

            return View(statistics);
        }
        [Authorize(Roles = "Admin")]
        public class AdminController : Controller
        {
            private readonly ApplicationDbContext _context;
            private readonly UserManager<AppUser> _userManager;

            public AdminController(ApplicationDbContext context, UserManager<AppUser> userManager)
            {
                _context = context;
                _userManager = userManager;
            }

            // GET: Admin/UserRewards
            public async Task<IActionResult> UserRewards()
            {
                var userRewards = await _context.UserRewards
                    .Include(ur => ur.User)
                    .Include(ur => ur.Reward)
                    .ToListAsync();
                return View(userRewards);
            }

            // GET: Admin/DeleteUserReward/5
            public async Task<IActionResult> DeleteUserReward(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var userReward = await _context.UserRewards
                    .Include(ur => ur.User)
                    .Include(ur => ur.Reward)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (userReward == null)
                {
                    return NotFound();
                }

                return View(userReward);
            }

            // POST: Admin/DeleteUserReward/5
            [HttpPost, ActionName("DeleteUserReward")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteUserRewardConfirmed(int id)
            {
                var userReward = await _context.UserRewards.FindAsync(id);
                if (userReward != null)
                {
                    // Przywróć punkty użytkownikowi
                    var user = await _userManager.FindByIdAsync(userReward.UserId);
                    if (user != null)
                    {
                        user.Points += userReward.Reward.Cost;
                        await _userManager.UpdateAsync(user);
                    }

                    _context.UserRewards.Remove(userReward);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(UserRewards));
            }
        }



        private bool RewardExists(int id)
        {
            return _context.Rewards.Any(e => e.Id == id);
        }
    }
}
