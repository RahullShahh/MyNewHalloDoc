using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Trial.DataContext;
using Trial.DataModels;

namespace Trial.Controllers
{
    public class Orderdetails1Controller : Controller
    {
        private readonly ApplicationDbContext _context;

        public Orderdetails1Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orderdetails1
        public async Task<IActionResult> Index()
        {
              return _context.Orderdetails != null ? 
                          View(await _context.Orderdetails.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Orderdetails'  is null.");
        }

        // GET: Orderdetails1/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orderdetails == null)
            {
                return NotFound();
            }

            var orderdetail = await _context.Orderdetails
                .FirstOrDefaultAsync(m => m.Orderid == id);
            if (orderdetail == null)
            {
                return NotFound();
            }

            return View(orderdetail);
        }

        // GET: Orderdetails1/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orderdetails1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Orderid,Productname,Customername,Amount")] Orderdetail orderdetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderdetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(orderdetail);
        }

        // GET: Orderdetails1/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orderdetails == null)
            {
                return NotFound();
            }

            var orderdetail = await _context.Orderdetails.FindAsync(id);
            if (orderdetail == null)
            {
                return NotFound();
            }
            return View(orderdetail);
        }

        // POST: Orderdetails1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Orderid,Productname,Customername,Amount")] Orderdetail orderdetail)
        {
            if (id != orderdetail.Orderid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderdetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderdetailExists(orderdetail.Orderid))
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
            return View(orderdetail);
        }

        // GET: Orderdetails1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orderdetails == null)
            {
                return NotFound();
            }

            var orderdetail = await _context.Orderdetails
                .FirstOrDefaultAsync(m => m.Orderid == id);
            if (orderdetail == null)
            {
                return NotFound();
            }

            return View(orderdetail);
        }

        // POST: Orderdetails1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orderdetails == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Orderdetails'  is null.");
            }
            var orderdetail = await _context.Orderdetails.FindAsync(id);
            if (orderdetail != null)
            {
                _context.Orderdetails.Remove(orderdetail);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderdetailExists(int id)
        {
          return (_context.Orderdetails?.Any(e => e.Orderid == id)).GetValueOrDefault();
        }
    }
}
