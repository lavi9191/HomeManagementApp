using System;
using System.ComponentModel.DataAnnotations;

namespace HomeManagementApp.Models
{
    public class UserReward
    {
        public int Id { get; set; }
        public int RewardId { get; set; }
        public string? UserId { get; set; }
        public DateTime DateReceived { get; set; }

        public Reward? Reward { get; set; }
        public AppUser? User { get; set; }
    }
}
