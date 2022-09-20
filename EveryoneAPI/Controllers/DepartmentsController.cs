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
    public class DepartmentsController : Controller
    {
        private readonly EveryoneDBContext _context;

        public DepartmentsController(EveryoneDBContext context)
        {
            _context = context;
        }

        // GET: Departments
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var everyoneDBContext = _context.Departments.Include(d => d.Employer);
            return Json(await everyoneDBContext.ToListAsync());
        }

        // GET: Departments/Details/5
        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Departments == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Employer)
                .FirstOrDefaultAsync(m => m.DepartmentId == id);
            if (department == null)
            {
                return NotFound();
            }

            return Json(department);
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentId,Name,EmployerId")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployerId"] = new SelectList(_context.Employers, "EmployerId", "EmployerId", department.EmployerId);
            return Json(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DepartmentId,Name,EmployerId")] Department department)
        {
            if (id != department.DepartmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.DepartmentId))
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
            ViewData["EmployerId"] = new SelectList(_context.Employers, "EmployerId", "EmployerId", department.EmployerId);
            return Json(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Departments == null)
            {
                return Problem("Entity set 'EveryoneDBContext.Departments'  is null.");
            }
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(int id)
        {
          return _context.Departments.Any(e => e.DepartmentId == id);
        }
    }
}
