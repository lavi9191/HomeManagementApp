using System.ComponentModel.DataAnnotations;

namespace HomeManagementApp.Models
{
    public class Reward
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nazwa")]
        public string? Name { get; set; }

        [Display(Name = "Opis")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Koszt")]
        public int Cost { get; set; }

        
        public string? UserId { get; set; }

        public DateTime DateReceived { get; set; }
    }
}
