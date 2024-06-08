using HomeManagementApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeManagementApp.Controllers
{
    [Authorize]
    public class ShoppingListController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShoppingListController(ApplicationDbContext context)
        {
            _context = context;
        }

        private static readonly List<string> Categories = new List<string>
        {
            "Owoce", "Warzywa", "Mięso", "Nabiał", "Pieczywo", "Napoje", "Przekąski",
            "Środki czystości", "Kosmetyki", "Produkty suche", "Mrożonki", "Przyprawy",
            "Konserwy", "Produkty dla dzieci", "Produkty dla zwierząt", "Artykuły biurowe"
        };

        // GET: ShoppingList
        public async Task<IActionResult> Index(string selectedCategory)
        {
            var items = from s in _context.ShoppingLists
                        select s;

            if (!string.IsNullOrEmpty(selectedCategory))
            {
                items = items.Where(s => s.Category == selectedCategory);
            }

            var viewModel = new ShoppingListViewModel
            {
                ShoppingItems = await items.ToListAsync(),
                Categories = new SelectList(Categories),
                CategoryOptions = Categories.Select(c => new SelectListItem { Value = c, Text = c }),
                SelectedCategory = selectedCategory
            };

            return View(viewModel);
        }

        // GET: ShoppingList/Create
        public IActionResult Create()
        {
            var viewModel = new ShoppingListViewModel
            {
                NewItem = new ShoppingList(),
                CategoryOptions = Categories.Select(c => new SelectListItem { Value = c, Text = c })
            };

            return View(viewModel);
        }

        // POST: ShoppingList/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShoppingListViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(viewModel.NewItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            viewModel.CategoryOptions = Categories.Select(c => new SelectListItem { Value = c, Text = c });
            return View(viewModel);
        }

        // GET: ShoppingList/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingList = await _context.ShoppingLists.FindAsync(id);
            if (shoppingList == null)
            {
                return NotFound();
            }

            var viewModel = new ShoppingListViewModel
            {
                NewItem = shoppingList,
                CategoryOptions = Categories.Select(c => new SelectListItem { Value = c, Text = c })
            };

            return View(viewModel);
        }

        // POST: ShoppingList/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ShoppingListViewModel viewModel)
        {
            if (id != viewModel.NewItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewModel.NewItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShoppingListExists(viewModel.NewItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            viewModel.CategoryOptions = Categories.Select(c => new SelectListItem { Value = c, Text = c });
            return View(viewModel);
        }

        // GET: ShoppingList/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingList = await _context.ShoppingLists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shoppingList == null)
            {
                return NotFound();
            }

            return View(shoppingList);
        }

        // POST: ShoppingList/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shoppingList = await _context.ShoppingLists.FindAsync(id);
            _context.ShoppingLists.Remove(shoppingList);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToShoppingList(int id)
        {
            var item = _context.ShoppingLists.Find(id);
            if (item != null)
            {
                // Tworzenie nowego elementu na podstawie istniejącego
                var newItem = new ShoppingList
                {
                    ItemName = item.ItemName,
                    Category = item.Category,
                    Quantity = item.Quantity,
                    IsPurchased = false,
                };

                _context.ShoppingLists.Add(newItem);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult History()
        {
            var purchasedItems = _context.ShoppingLists.Where(s => s.IsPurchased).ToList();
            return View(purchasedItems);
        }

        [HttpPost]
        public async Task<IActionResult> TogglePurchased(int id)
        {
            var item = await _context.ShoppingLists.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            item.IsPurchased = !item.IsPurchased;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isPurchased = item.IsPurchased });
        }

        private bool ShoppingListExists(int id)
        {
            return _context.ShoppingLists.Any(e => e.Id == id);
        }
    }
}
