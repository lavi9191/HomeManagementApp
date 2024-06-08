using Microsoft.AspNetCore.Identity;

namespace HomeManagementApp.Models
{
    public class AppUser : IdentityUser
    {
        public int Points { get; set; }
    }
}
