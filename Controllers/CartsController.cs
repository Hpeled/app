using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication_GameStoreIL.Models;

namespace WebApplication_GameStoreIL.Controllers
{
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Carts
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var applicationDbContext = _context.Carts.Include(c => c.User);
                foreach (Cart cart in applicationDbContext)
                    cart.User = _context.Users.FirstOrDefault(u => u.Id == cart.UserId);
                return View(await applicationDbContext.ToListAsync());
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }

        // GET: Carts/Details/5

        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var cart = await _context.Carts
                    .Include(c => c.User)
                    .Include(p => p.Products)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (cart == null)
                {
                    return RedirectToAction("PageNotFound", "Home"); ;
                }

                cart.User = _context.Users.FirstOrDefault(u => u.Id == cart.UserId);
                return View(cart);
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,TotalPrice")] Cart cart)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(cart);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", cart.UserId);
                return View(cart);
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var cart = await _context.Carts.FindAsync(id);
                if (cart == null)
                {
                    return NotFound();
                }
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", cart.UserId);
                return View(cart);
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,TotalPrice")] Cart cart)
        {
            try
            {
                if (id != cart.Id)
                {
                    return RedirectToAction("PageNotFound", "Home");
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(cart);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CartExists(cart.Id))
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
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", cart.UserId);
                return View(cart);
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }

        // GET: Carts/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("PageNotFound", "Home");
                }

                var cart = await _context.Carts
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (cart == null)
                {
                    return RedirectToAction("PageNotFound", "Home");
                }
                cart.User = _context.Users.FirstOrDefault(u => u.Id == cart.UserId);
                return View(cart);
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.User)
                    .Include(p => p.Products)
                    .FirstOrDefaultAsync(m => m.Id == id);
                cart.Products.Clear();
                cart.TotalPrice = 0;
                _context.Update(cart);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }


        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.Id == id);
        }

        public IActionResult MyCart()
        {
            try
            {
                String userName = HttpContext.User.Identity.Name;
                User user = _context.Users.FirstOrDefault(x => x.Username.Equals(userName));
                Cart cart = _context.Carts.FirstOrDefault(x => x.UserId == user.Id);
                cart.Products = _context.Products.Where(x => x.Carts.Contains(cart)).ToList();

                if (cart == null)
                {
                    return RedirectToAction("PageNotFound", "Home");
                }

                return View(cart);
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }


        [HttpPost, ActionName("AddProductToCart")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddProductToCart(int id) //product id
        {
            try
            {
                Product product = _context.Products.Include(db => db.Carts).FirstOrDefault(x => x.Id == id);
                String userName = HttpContext.User.Identity.Name;
                User user = _context.Users.FirstOrDefault(x => x.Username.Equals(userName));
                Cart cart = _context.Carts.Include(db => db.Products)
                 .FirstOrDefault(x => x.UserId == user.Id);


                if (user.Cart.Products == null)
                    user.Cart.Products = new List<Product>();
                if (product.Carts == null)
                    product.Carts = new List<Cart>();

                if (!(cart.Products.Contains(product) && product.Carts.Contains(cart)))
                {
                    user.Cart.Products.Add(product);
                    product.Carts.Add(cart);
                    user.Cart.TotalPrice += product.Price;
                    _context.Update(cart);
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(MyCart));
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }

        // POST: Carts/removeProduct/5
        [HttpPost, ActionName("RemoveProductFromCart")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveProductFromCart(int id) //product id
        {
            try
            {
                Product product = _context.Products.FirstOrDefault(x => x.Id == id);
                String userName = HttpContext.User.Identity.Name;

                User user = _context.Users.FirstOrDefault(x => x.Username.Equals(userName));
                Cart cart = _context.Carts.Include(db => db.Products)
                    .FirstOrDefault(x => x.UserId == user.Id);

                if (product != null)
                {
                    cart.Products.Remove(product);
                    cart.TotalPrice -= product.Price;
                }

                _context.Attach<Cart>(cart);
                _context.Entry(cart).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyCart));
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }

        public async Task<IActionResult> FinishPay()
        {
            try
            {
                String userName = HttpContext.User.Identity.Name;
                User user = _context.Users.FirstOrDefault(x => x.Username.Equals(userName));
                Cart cart = _context.Carts.Include(db => db.Products).FirstOrDefault(x => x.User.Username == user.Username );

                cart.Products.Clear();
                cart.TotalPrice = 0;

                _context.Attach<Cart>(cart);
                _context.Entry(cart).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return View();
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }


        [Authorize(Roles = "Admin")]
        public async Task <IActionResult> SearchCart(string query)
        {
            try
            {
                List<Cart> carts = await _context.Carts.Where(c => c.User.Username.Contains(query)).Include(p => p.Products).ToListAsync();

                foreach (Cart c in carts)
                {
                    c.User = _context.Users.FirstOrDefault(u => u.Id == c.UserId);
                }

                return PartialView(carts);
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }

        }
    }
}
    