using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication_GameStoreIL.Models;

namespace WebApplication_GameStoreIL.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var applicationDbContext = _context.Products.Include(p =>p.Category);
                return View(await applicationDbContext.ToListAsync());

            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }


        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("PageNotFound", "Home");
                }

                var product = await _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (product == null)
                {
                    return RedirectToAction("PageNotFound", "Home");
                }

                return View(product);
            }
            catch { return RedirectToAction("PageNotFound", "Home");  }
        }

        // GET: Products/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,CategoryId,imagePath,IsOnSale")] Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                return View(product);
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("PageNotFound", "Home");
                }

                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return  RedirectToAction("PageNotFound", "Home");
                }
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                return View(product);
            }
            catch { return RedirectToAction("PageNotFound", "Home");}
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,CategoryId,imagePath,IsOnSale")] Product product)
        {
            try
            {
                if (id != product.Id)
                {
                    return RedirectToAction("PageNotFound", "Home");
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(product);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ProductExists(product.Id))
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
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                return View(product);
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("PageNotFound", "Home");
                }

                var product = await _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (product == null)
                {
                    return RedirectToAction("PageNotFound", "Home");
                }

                return View(product);
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        public async Task<IActionResult> AdvancedSearch(string productName, string category, string price)
        {
            try
            { 
                var applicationDbContext = _context.Products.Include(a => a.Category).Where(a => a.Name.Contains(productName) && a.Category.Name.Equals(category) && a.Price <= Int32.Parse(price));
                return View("SearchList", await applicationDbContext.ToListAsync());
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SearchProduct(string query)
        {
            try
            {
                var applicationDbContext = _context.Products.Include(p => p.Category);
                return PartialView(await applicationDbContext.Where(p => p.Name.Contains(query)).ToListAsync());
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }


        public async Task<IActionResult> SearchByName(string productName)
        {
            try
            {
                var applicationDbContext = _context.Products.Include(a => a.Category).Where(a => a.Name.Contains(productName));
                return View("searchList", await applicationDbContext.ToListAsync());
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }

        public async Task<IActionResult> SearchByNameAndCategory(string productName, string category)
        {
            try
            {
                var applicationDbContext = _context.Products.Include(a => a.Category).Where(a => a.Name.Contains(productName) && a.Category.Name.Equals(category));
                return View("SearchList", await applicationDbContext.ToListAsync());
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Statistics()
        {
            try
            {
                //statistic-1- what is the most "popular" product - the one who appears in most carts
                ICollection<Stat> statistic1 = new Collection<Stat>();
                var result = from p in _context.Products.Include(o => o.Carts)
                             where (p.Carts.Count) > 0
                             orderby (p.Carts.Count) descending
                             select p;
                foreach (var v in result)
                {
                    statistic1.Add(new Stat(v.Name, v.Carts.Count));
                }

                ViewBag.data1 = statistic1;


                //statistic-2- what category hava the biggest number of products
                ICollection<Stat> statistic2 = new Collection<Stat>();
                List<Product> products = _context.Products.ToList();
                List<Category> categories = _context.Categories.ToList();
                var result2 = from prod in products
                              join cat in categories on prod.CategoryId equals cat.Id
                              group cat by cat.Id into G
                              select new { id = G.Key, num = G.Count() };

                var porqua = from popo in result2
                             join cat in categories on popo.id equals cat.Id
                             select new { category = cat.Name, count = popo.num };
                foreach (var v in porqua)
                {
                    if (v.count > 0)
                        statistic2.Add(new Stat(v.category, v.count));
                }

                ViewBag.data2 = statistic2;
                return View();
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }
    }
}

public class Stat
{
    public string Key;
    public int Values;
    public Stat(string key, int values)
    {
        Key = key;
        Values = values;
    }
}
