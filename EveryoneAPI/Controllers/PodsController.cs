using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EveryoneAPI.Models;

namespace EveryoneAPI.Controllers
{
    [Route("api/[controller]")]
    public class PodsController : Controller
    {
        private readonly EveryoneDBContext _context;

        public PodsController(EveryoneDBContext context)
        {
            _context = context;
        }

        // GET: Pods
        [HttpGet]
        [Route("List")]
        public async Task<IActionResult> Index()
        {
            var everyoneDBContext = _context.Pods.Include(p => p.Department);
            return View(await everyoneDBContext.ToListAsync());
        }

        // GET: Pods/Details/5
        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Pods == null)
            {
                return NotFound();
            }

            var pod = await _context.Pods
                .Include(p => p.Department)
                .FirstOrDefaultAsync(m => m.PodId == id);
            if (pod == null)
            {
                return NotFound();
            }

            return View(pod);
        }

        // POST: Pods/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PodId,Name,DepartmentId")] Pod pod)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pod);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", pod.DepartmentId);
            return View(pod);
        }

        // POST: Pods/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PodId,Name,DepartmentId")] Pod pod)
        {
            if (id != pod.PodId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pod);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PodExists(pod.PodId))
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
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", pod.DepartmentId);
            return View(pod);
        }

        // POST: Pods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pods == null)
            {
                return Problem("Entity set 'EveryoneDBContext.Pods'  is null.");
            }
            var pod = await _context.Pods.FindAsync(id);
            if (pod != null)
            {
                _context.Pods.Remove(pod);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PodExists(int id)
        {
          return _context.Pods.Any(e => e.PodId == id);
        }
    }
}
