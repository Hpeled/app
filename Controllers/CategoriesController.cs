using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication_GameStoreIL.Models;

namespace WebApplication_GameStoreIL.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _context.Categories.ToListAsync());
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }

      
        public async Task<IActionResult> SearchCategory(string query)
        {
            try
            {
                return Json(await _context.Categories.Where(c => c.Name.Contains(query)).ToListAsync());
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }


        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("PageNotFound", "Home");
                }

                var category = await _context.Categories
                    //Hen addition
                    .Include(c=>c.Products)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (category == null)
                {
                    return RedirectToAction("PageNotFound", "Home");
                }

                return View(category);
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }

        // GET: Categories/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    System.Diagnostics.Debug.WriteLine("hello");

                    Category c = new Category()
                    {
                        Name = category.Name,
                        img_path = category.img_path
                    };
                    _context.Add(c);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(category);
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }

        // GET: Categories/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("PageNotFound", "Home");
                }

                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return RedirectToAction("PageNotFound", "Home");
                }
                return View(category);
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }


        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,Category category)
        {
            try
            {
                if (id != category.Id)
                {
                    return RedirectToAction("PageNotFound", "Home");
                }
                if (ModelState.IsValid)
                {
                    try
                    {
                        var category_from_db = _context.Categories.Where(c => c.Id == id).FirstOrDefault();
                        category_from_db.img_path = category.img_path;
                        category_from_db.Name = category.Name;
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CategoryExists(category.Id))
                        {
                            return RedirectToAction("PageNotFound", "Home");
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(category);
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }


        // GET: Categories/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("PageNotFound", "Home");
                }

                var category = await _context.Categories
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (category == null)
                {
                    return RedirectToAction("PageNotFound", "Home");
                }

                return View(category);
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                foreach (Product p in _context.Products)
                {
                    if (p.CategoryId == id)
                    {
                        _context.Products.Remove(p);
                    }
                }
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
        
    }
}
