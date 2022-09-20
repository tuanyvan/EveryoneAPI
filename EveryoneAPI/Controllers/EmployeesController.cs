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
    public class EmployeesController : Controller
    {
        private readonly EveryoneDBContext _context;

        public EmployeesController(EveryoneDBContext context)
        {
            _context = context;
        }

        // GET: Employees
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var everyoneDBContext = _context.Employees.Include(e => e.Department).Include(e => e.Employer).Include(e => e.EthnicityNavigation).Include(e => e.GenderIdentityNavigation).Include(e => e.Pod).Include(e => e.PronounNavigation).Include(e => e.SexualOrientationNavigation);
            return Json(await everyoneDBContext.ToListAsync());
        }

        // GET: Employees/Details/5
        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Employer)
                .Include(e => e.EthnicityNavigation)
                .Include(e => e.GenderIdentityNavigation)
                .Include(e => e.Pod)
                .Include(e => e.PronounNavigation)
                .Include(e => e.SexualOrientationNavigation)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return Json(employee);
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,Name,GenderIdentity,SexualOrientation,Ethnicity,EmployerId,DepartmentId,PodId,Pronoun")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", employee.DepartmentId);
            ViewData["EmployerId"] = new SelectList(_context.Employers, "EmployerId", "EmployerId", employee.EmployerId);
            ViewData["Ethnicity"] = new SelectList(_context.Ethnicities, "EthnicityId", "EthnicityId", employee.Ethnicity);
            ViewData["GenderIdentity"] = new SelectList(_context.GenderIdentities, "GenderId", "GenderId", employee.GenderIdentity);
            ViewData["PodId"] = new SelectList(_context.Pods, "PodId", "PodId", employee.PodId);
            ViewData["Pronoun"] = new SelectList(_context.Pronouns, "PronounId", "PronounId", employee.Pronoun);
            ViewData["SexualOrientation"] = new SelectList(_context.SexualOrientations, "OrientationId", "OrientationId", employee.SexualOrientation);
            return Json(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,Name,GenderIdentity,SexualOrientation,Ethnicity,EmployerId,DepartmentId,PodId,Pronoun")] Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeId))
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
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", employee.DepartmentId);
            ViewData["EmployerId"] = new SelectList(_context.Employers, "EmployerId", "EmployerId", employee.EmployerId);
            ViewData["Ethnicity"] = new SelectList(_context.Ethnicities, "EthnicityId", "EthnicityId", employee.Ethnicity);
            ViewData["GenderIdentity"] = new SelectList(_context.GenderIdentities, "GenderId", "GenderId", employee.GenderIdentity);
            ViewData["PodId"] = new SelectList(_context.Pods, "PodId", "PodId", employee.PodId);
            ViewData["Pronoun"] = new SelectList(_context.Pronouns, "PronounId", "PronounId", employee.Pronoun);
            ViewData["SexualOrientation"] = new SelectList(_context.SexualOrientations, "OrientationId", "OrientationId", employee.SexualOrientation);
            return Json(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Employees == null)
            {
                return Problem("Entity set 'EveryoneDBContext.Employees'  is null.");
            }
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
          return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
