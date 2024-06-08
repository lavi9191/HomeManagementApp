using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace HomeManagementApp.Models
{
    public class ShoppingListViewModel
    {
        public ShoppingList? NewItem { get; set; }
        public List<ShoppingList>? ShoppingItems { get; set; }
        public SelectList? Categories { get; set; }
        public string? SelectedCategory { get; set; }
        public IEnumerable<SelectListItem>? CategoryOptions { get; set; }
    }
}
