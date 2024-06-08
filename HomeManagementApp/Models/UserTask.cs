using System.ComponentModel.DataAnnotations;

namespace HomeManagementApp.Models
{
    public class UserTask
    {
        public int Id { get; set; }

        [Required]
        public int AdminTaskId { get; set; }

        [Required]
        public string? UserId { get; set; }

        [Display(Name = "Termin")]
        public DateTime DueDate { get; set; }

        [Display(Name = "Zakończone?")]
        public bool IsComplete { get; set; }

        [Display(Name = "Zadanie")]
        public virtual AdminTask? AdminTask { get; set; }
        [Display(Name = "Użytkownik")]
        public virtual AppUser? User { get; set; }
    }
}
