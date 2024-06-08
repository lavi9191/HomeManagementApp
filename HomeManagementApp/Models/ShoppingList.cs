using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HomeManagementApp.Models
{
    public class ShoppingList
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Produkt")]
        public string? ItemName { get; set; }

        [Display(Name = "Ilość")]
        public int Quantity { get; set; }

        [Display(Name = "Kupione?")]
        public bool IsPurchased { get; set; }

        [Display(Name = "Kategoria")]
        public string? Category { get; set; }

        [Display(Name = "Data zakupu")]
        public DateTime? PurchaseDate { get; set; }

    }
}
