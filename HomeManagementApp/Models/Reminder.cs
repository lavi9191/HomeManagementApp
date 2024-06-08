using System;
using System.ComponentModel.DataAnnotations;

namespace HomeManagementApp.Models
{
    public class Reminder
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Tytuł")]
        public string? Title { get; set; }

        [Required]
        [Display(Name = "Termin")]
        public DateTime DueDate { get; set; }

        [Required]
        public string? UserId { get; set; }

        [Display(Name = "Użytkownik")]
        public virtual AppUser? User { get; set; }

        [StringLength(255)]
        [Display(Name = "Wiadomość")]
        public string Message { get; set; } = "Wykonuj zadania bo będzie zadanie karne !!!";
    }
}
