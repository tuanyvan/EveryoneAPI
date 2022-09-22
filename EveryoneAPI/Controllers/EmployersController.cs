using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EveryoneAPI.Models;
using Newtonsoft.Json.Linq;
using static EveryoneAPI.Models.Employer;
using Microsoft.CodeAnalysis.Operations;

namespace EveryoneAPI.Controllers
{
    [Route("api/[controller]")]
    public class EmployersController : Controller
    {
        private readonly EveryoneDBContext _context;

        public EmployersController(EveryoneDBContext context)
        {
            _context = context;
        }

        // GET: Employers/Details/5
        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Employers == null)
            {
                return NotFound();
            }

            var employer = await _context.Employers
                .FirstOrDefaultAsync(m => m.EmployerId == id);
            if (employer == null)
            {
                return NotFound();
            }

            return Json(employer);
        }

        // POST: Employers/Register
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] EmployerInfo employerInfo)
        {

            foreach (Employer e in _context.Employers.ToList())
            {
                if (e.Email == employerInfo.Email)
                {
                    return BadRequest("That email already exists.");
                }
            }

            Employer employer = new Employer();

            employer.Email = employerInfo.Email;
            employer.Password = employerInfo.Password;
            employer.Uuid = Guid.NewGuid().ToString();

            _context.Add(employer);
            await _context.SaveChangesAsync();
         
            return Ok();
        }

        // POST: Employers/Login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] EmployerInfo employerInfo)
        {

            Employer user = _context.Employers.Where(e => e.Email.Equals(employerInfo.Email) && e.Password.Equals(employerInfo.Password)).SingleOrDefault();

            if (user == null)
            {
                return BadRequest("There was no user found with those credentials.");
            }

            return Ok(user.Uuid);
        }

        // POST: Employers/Delete/5
        [HttpPost, ActionName("Delete")]
        
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Employers == null)
            {
                return Problem("Entity set 'EveryoneDBContext.Employers'  is null.");
            }
            var employer = await _context.Employers.FindAsync(id);
            if (employer != null)
            {
                _context.Employers.Remove(employer);
            }
            
            await _context.SaveChangesAsync();
            return Ok("User deleted");
        }

        private bool EmployerExists(int id)
        {
          return _context.Employers.Any(e => e.EmployerId == id);
        }
    }
}
