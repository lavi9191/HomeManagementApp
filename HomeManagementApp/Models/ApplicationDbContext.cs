using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HomeManagementApp.Models
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<HomeTask> HomeTasks { get; set; }
        public DbSet<ShoppingList> ShoppingLists { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<Reward> Rewards { get; set; }
        public DbSet<AdminTask> AdminTasks { get; set; }
        public DbSet<UserTask> UserTasks { get; set; }
        public DbSet<UserReward> UserRewards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}
