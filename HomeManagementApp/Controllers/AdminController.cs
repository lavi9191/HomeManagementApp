using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeManagementApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace HomeManagementApp.Controllers
{
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

        public IActionResult Index()
        {
            return View();
        }

        // Zarządzanie nagrodami
        public async Task<IActionResult> ManageRewards()
        {
            return View(await _context.Rewards.ToListAsync());
        }

        // GET: Admin/CreateReward
        public IActionResult CreateReward()
        {
            return View();
        }

        // POST: Admin/CreateReward
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReward([Bind("Name,Description,Cost")] Reward reward)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reward);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageRewards));
            }
            return View(reward);
        }

        public async Task<IActionResult> EditReward(int? id)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReward(int id, [Bind("Id,Name,Description,Cost")] Reward reward)
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
                return RedirectToAction(nameof(ManageRewards));
            }
            return View(reward);
        }

        public async Task<IActionResult> DeleteReward(int? id)
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

        [HttpPost, ActionName("DeleteReward")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRewardConfirmed(int id)
        {
            var reward = await _context.Rewards.FindAsync(id);
            if (reward != null)
            {
                _context.Rewards.Remove(reward);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ManageRewards));
        }

        // GET: Admin/DetailsReward/1
        public async Task<IActionResult> DetailsReward(int? id)
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

        // Zarządzanie odebranymi nagrodami
        public async Task<IActionResult> UserRewards()
        {
            var userRewards = await _context.UserRewards
                .Include(ur => ur.User)
                .Include(ur => ur.Reward)
                .ToListAsync();
            return View(userRewards);
        }

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

        [HttpPost, ActionName("DeleteUserReward")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserRewardConfirmed(int id)
        {
            var userReward = await _context.UserRewards
                .Include(ur => ur.Reward) // Ensure Reward is loaded
                .FirstOrDefaultAsync(ur => ur.Id == id);

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

        // Zarządzanie zadaniami
        public async Task<IActionResult> ManageTasks()
        {
            var tasks = await _context.AdminTasks.ToListAsync();
            return View(tasks);
        }

        public IActionResult CreateAdminTask()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdminTask([Bind("Id,Name,Points")] AdminTask adminTask)
        {
            if (ModelState.IsValid)
            {
                _context.Add(adminTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageTasks));
            }
            return View(adminTask);
        }

        public async Task<IActionResult> EditAdminTask(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminTask = await _context.AdminTasks.FindAsync(id);
            if (adminTask == null)
            {
                return NotFound();
            }
            return View(adminTask);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAdminTask(int id, [Bind("Id,Name,Points")] AdminTask adminTask)
        {
            if (id != adminTask.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adminTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminTaskExists(adminTask.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ManageTasks));
            }
            return View(adminTask);
        }

        public async Task<IActionResult> DeleteAdminTask(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminTask = await _context.AdminTasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adminTask == null)
            {
                return NotFound();
            }

            return View(adminTask);
        }

        [HttpPost, ActionName("DeleteAdminTask")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAdminTaskConfirmed(int id)
        {
            var adminTask = await _context.AdminTasks.FindAsync(id);
            if (adminTask != null)
            {
                _context.AdminTasks.Remove(adminTask);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageTasks));
        }

        // GET: Admin/DetailsAdminTask/1
        public IActionResult DetailsAdminTask(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminTask = _context.AdminTasks
                .FirstOrDefault(m => m.Id == id);
            if (adminTask == null)
            {
                return NotFound();
            }

            return View(adminTask);
        }

        // Zarządzanie użytkownikami
        public async Task<IActionResult> ManageUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> EditUser(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string id, [Bind("Id,UserName,Email,Points")] AppUser user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _userManager.FindByIdAsync(id);
                    existingUser.UserName = user.UserName;
                    existingUser.Email = user.Email;
                    existingUser.Points = user.Points;

                    var result = await _userManager.UpdateAsync(existingUser);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to update user.");
                        return View(user);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ManageUsers));
            }
            return View(user);
        }

        private bool AdminTaskExists(int id)
        {
            return _context.AdminTasks.Any(e => e.Id == id);
        }

        private bool RewardExists(int id)
        {
            return _context.Rewards.Any(e => e.Id == id);
        }

        private bool UserExists(string id)
        {
            return _userManager.Users.Any(e => e.Id == id);
        }

        // Przypisanie roli użytkownikowi
        public IActionResult AssignRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                if (!await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    var result = await _userManager.AddToRoleAsync(user, "Admin");
                    if (result.Succeeded)
                    {
                        ViewBag.Message = $"Użytkownik {email} został dodany do roli Admin.";
                    }
                    else
                    {
                        ViewBag.Message = "Wystąpił błąd podczas przypisywania roli.";
                    }
                }
                else
                {
                    ViewBag.Message = $"Użytkownik {email} już jest administratorem.";
                }
            }
            else
            {
                ViewBag.Message = "Użytkownik nie został znaleziony.";
            }

            return View();
        }

        // Odebranie roli użytkownikowi
        [HttpPost]
        public async Task<IActionResult> RemoveRole(string emailRemove)
        {
            var user = await _userManager.FindByEmailAsync(emailRemove);
            if (user != null)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    var result = await _userManager.RemoveFromRoleAsync(user, "Admin");
                    if (result.Succeeded)
                    {
                        ViewBag.Message = $"Rola Admin została odebrana użytkownikowi {emailRemove}.";
                    }
                    else
                    {
                        ViewBag.Message = "Wystąpił błąd podczas odbierania roli.";
                    }
                }
                else
                {
                    ViewBag.Message = $"Użytkownik {emailRemove} nie jest administratorem.";
                }
            }
            else
            {
                ViewBag.Message = "Użytkownik nie został znaleziony.";
            }

            return View("AssignRole");
        }
    }
}
