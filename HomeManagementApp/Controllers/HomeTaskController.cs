using Microsoft.AspNetCore.Mvc;
using HomeManagementApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Globalization;

namespace HomeManagementApp.Controllers
{
    [Authorize]
    public class HomeTaskController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<HomeTaskController> _logger;

        public HomeTaskController(ApplicationDbContext context, UserManager<AppUser> userManager, ILogger<HomeTaskController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        private async Task<IEnumerable<SelectListItem>> GetUsersSelectListAsync()
        {
            return await _userManager.Users
                .Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.UserName
                }).ToListAsync();
        }

        private async Task<IEnumerable<SelectListItem>> GetAdminTasksSelectListAsync()
        {
            return await _context.AdminTasks
                .Select(at => new SelectListItem
                {
                    Value = at.Id.ToString(),
                    Text = $"{at.Name} - {at.Points} pkt"
                }).ToListAsync();
        }

        // GET: HomeTask
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var tasks = _context.UserTasks
                                .Include(t => t.AdminTask)
                                .Include(t => t.User)
                                .Where(t => t.UserId == userId);
            return View(await tasks.ToListAsync());
        }

        // GET: HomeTask/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userTask = await _context.UserTasks
                .Include(ut => ut.AdminTask)
                .Include(ut => ut.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userTask == null)
            {
                return NotFound();
            }

            return View(userTask);
        }

        // GET: HomeTask/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Users = new SelectList(await _userManager.Users.ToListAsync(), "Id", "UserName");
            ViewBag.AdminTasks = new SelectList(await GetAdminTasksSelectListAsync(), "Value", "Text");
            return View();
        }

        // POST: HomeTask/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AdminTaskId,UserId,DueDate,IsComplete")] UserTask userTask)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userTask);
                await _context.SaveChangesAsync();

                // Dodaj przypomnienie
                var adminTask = await _context.AdminTasks.FindAsync(userTask.AdminTaskId);
                var user = await _userManager.FindByIdAsync(userTask.UserId);

                if (adminTask != null && user != null)
                {
                    // Aktualizacja punktów użytkownika, jeśli zadanie jest oznaczone jako ukończone
                    if (userTask.IsComplete)
                    {
                        user.Points += adminTask.Points;
                        await _userManager.UpdateAsync(user);
                    }

                    var reminder = new Reminder
                    {
                        Title = adminTask.Name,
                        DueDate = userTask.DueDate,
                        UserId = userTask.UserId
                    };
                    _context.Reminders.Add(reminder);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            ViewBag.Users = new SelectList(await _userManager.Users.ToListAsync(), "Id", "UserName", userTask.UserId);
            ViewBag.AdminTasks = new SelectList(await GetAdminTasksSelectListAsync(), "Value", "Text", userTask.AdminTaskId);
            return View(userTask);
        }



        // GET: HomeTask/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userTask = await _context.UserTasks.FindAsync(id);
            if (userTask == null)
            {
                return NotFound();
            }

            ViewBag.Users = await GetUsersSelectListAsync();
            ViewBag.AdminTasks = await GetAdminTasksSelectListAsync();
            return View(userTask);
        }

        // POST: HomeTask/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AdminTaskId,UserId,DueDate,IsComplete")] UserTask userTask)
        {
            if (id != userTask.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingTask = await _context.UserTasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
                    if (existingTask == null)
                    {
                        return NotFound();
                    }

                    var adminTask = await _context.AdminTasks.FindAsync(userTask.AdminTaskId);
                    var user = await _userManager.FindByIdAsync(userTask.UserId);

                    if (existingTask.IsComplete != userTask.IsComplete && user != null && adminTask != null)
                    {
                        if (userTask.IsComplete)
                        {
                            user.Points += adminTask.Points;
                        }
                        else
                        {
                            user.Points -= adminTask.Points;
                        }

                        await _userManager.UpdateAsync(user);
                    }

                    _context.Update(userTask);
                    await _context.SaveChangesAsync();

                    // Znajdź przypomnienie związane z tym zadaniem
                    if (adminTask != null)
                    {
                        var reminder = await _context.Reminders.FirstOrDefaultAsync(r => r.Title == adminTask.Name && r.UserId == userTask.UserId);
                        if (reminder != null)
                        {
                            // Zaktualizuj przypomnienie
                            reminder.Title = adminTask.Name;
                            reminder.DueDate = userTask.DueDate;
                            _context.Update(reminder);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserTaskExists(userTask.Id))
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
            ViewBag.Users = await GetUsersSelectListAsync();
            ViewBag.AdminTasks = await GetAdminTasksSelectListAsync();
            return View(userTask);
        }

        // GET: HomeTask/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userTask = await _context.UserTasks.Include(ut => ut.AdminTask).Include(ut => ut.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userTask == null)
            {
                return NotFound();
            }

            return View(userTask);
        }

        // POST: HomeTask/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userTask = await _context.UserTasks
                .Include(ut => ut.AdminTask) // Ensure AdminTask is loaded
                .FirstOrDefaultAsync(ut => ut.Id == id);

            if (userTask != null)
            {
                // Znajdź przypomnienia związane z tym zadaniem
                var reminders = await _context.Reminders
                    .Where(r => r.UserId == userTask.UserId && r.Title == userTask.AdminTask.Name)
                    .ToListAsync();

                // Usuń przypomnienia
                _context.Reminders.RemoveRange(reminders);

                // Usuń zadanie
                _context.UserTasks.Remove(userTask);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }



        // Statystyki

        // Liczba ukończonych zadań i liczba nieukończonych zadań
        [HttpGet]
        public async Task<IActionResult> TaskStatistics()
        {
            var userId = _userManager.GetUserId(User);
            var completedTasks = await _context.UserTasks
                .Where(t => t.UserId == userId && t.IsComplete)
                .CountAsync();

            var incompleteTasks = await _context.UserTasks
                .Where(t => t.UserId == userId && !t.IsComplete)
                .CountAsync();

            var statistics = new
            {
                CompletedTasks = completedTasks,
                IncompleteTasks = incompleteTasks
            };

            return View(statistics);
        }

        // Całkowita liczba zdobytych punktów, średnia liczba punktów za zadanie, punkty zdobyte w danym okresie
        [HttpGet]
        public async Task<IActionResult> PointsStatistics()
        {
            var userId = _userManager.GetUserId(User);
            var totalPoints = await _context.UserTasks
                .Where(t => t.UserId == userId && t.IsComplete)
                .SumAsync(t => t.AdminTask.Points);

            var taskCount = await _context.UserTasks
                .Where(t => t.UserId == userId && t.IsComplete)
                .CountAsync();

            var averagePoints = taskCount > 0 ? (double)totalPoints / taskCount : 0;

            var pointsPerPeriod = await _context.UserTasks
                .Where(t => t.UserId == userId && t.IsComplete)
                .GroupBy(t => new { t.DueDate.Year, t.DueDate.Month })
                .Select(g => new
                {
                    Period = g.Key,
                    Points = g.Sum(t => t.AdminTask.Points)
                }).ToListAsync();

            var statistics = new
            {
                TotalPoints = totalPoints,
                AveragePoints = averagePoints,
                PointsPerPeriod = pointsPerPeriod
            };

            return View(statistics);
        }

        // Dzienna/tygodniowa/miesięczna aktywność
        [HttpGet]
        public async Task<IActionResult> ActivityStatistics()
        {
            var userId = _userManager.GetUserId(User);

            // Dzienna aktywność
            var dailyActivity = await _context.UserTasks
                .Where(t => t.UserId == userId)
                .GroupBy(t => t.DueDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    CompletedTasks = g.Count(t => t.IsComplete),
                    IncompleteTasks = g.Count(t => !t.IsComplete)
                }).ToListAsync();

            // Tygodniowa aktywność
            var weeklyActivity = await _context.UserTasks
                .Where(t => t.UserId == userId)
                .ToListAsync();

            var weeklyGrouped = weeklyActivity
                .GroupBy(t => new
                {
                    Week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(t.DueDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday),
                    Year = t.DueDate.Year
                })
                .Select(g => new
                {
                    Week = g.Key.Week,
                    Year = g.Key.Year,
                    CompletedTasks = g.Count(t => t.IsComplete),
                    IncompleteTasks = g.Count(t => !t.IsComplete)
                }).ToList();

            // Miesięczna aktywność
            var monthlyActivity = await _context.UserTasks
                .Where(t => t.UserId == userId)
                .GroupBy(t => new { t.DueDate.Year, t.DueDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    CompletedTasks = g.Count(t => t.IsComplete),
                    IncompleteTasks = g.Count(t => !t.IsComplete)
                }).ToListAsync();

            var statistics = new
            {
                DailyActivity = dailyActivity,
                WeeklyActivity = weeklyGrouped,
                MonthlyActivity = monthlyActivity
            };

            return View(statistics);
        }

        // Porównanie miesiąc do miesiąca i rok do roku
        [HttpGet]
        public async Task<IActionResult> SelfComparisonStatistics()
        {
            var userId = _userManager.GetUserId(User);

            var monthlyComparison = await _context.UserTasks
                .Where(t => t.UserId == userId)
                .GroupBy(t => new { t.DueDate.Year, t.DueDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    CompletedTasks = g.Count(t => t.IsComplete),
                    IncompleteTasks = g.Count(t => !t.IsComplete),
                    PointsEarned = g.Sum(t => t.IsComplete ? t.AdminTask.Points : 0)
                }).ToListAsync();

            var yearlyComparison = await _context.UserTasks
                .Where(t => t.UserId == userId)
                .GroupBy(t => t.DueDate.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    CompletedTasks = g.Count(t => t.IsComplete),
                    IncompleteTasks = g.Count(t => !t.IsComplete),
                    PointsEarned = g.Sum(t => t.IsComplete ? t.AdminTask.Points : 0)
                }).ToListAsync();

            var statistics = new
            {
                MonthlyComparison = monthlyComparison,
                YearlyComparison = yearlyComparison
            };

            return View(statistics);
        }

        private bool UserTaskExists(int id)
        {
            return _context.UserTasks.Any(e => e.Id == id);
        }
    }
}
