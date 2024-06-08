using System.ComponentModel.DataAnnotations;

namespace HomeManagementApp.Models
{
    public class AdminTask
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Nazwa")]
        public string? Name { get; set; }

        [Required]
        [Display(Name = "Punkty")]
        public int Points { get; set; }
    }
}
