using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HomeManagementApp.Models
{
    public class HomeTask
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tytuł")]
        public string? Title { get; set; }

        [Display(Name = "Szczegóły")]
        public string? Description { get; set; }

        [Display(Name = "Ukończone")]
        public bool IsComplete { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Termin")]
        public DateTime DueDate { get; set; }

        [Display(Name = "Punkty")]
        public int Points { get; set; }

        [Required]
        public string? UserId { get; set; }

        [Display(Name = "Użytkownik")]
        public virtual AppUser? User { get; set; }

        public string DueDateFormatted => DueDate.ToShortDateString();
    }
}
