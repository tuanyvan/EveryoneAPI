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
using System.Security.Cryptography;
using System.Text;

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

            if (string.IsNullOrEmpty(employerInfo.Email) || string.IsNullOrEmpty(employerInfo.Password))
            {
                return BadRequest("The email and password field cannot be empty.");
            }

            foreach (Employer e in _context.Employers.ToList())
            {
                if (e.Email == employerInfo.Email)
                {
                    return BadRequest("That email already exists.");
                }
            }

            Employer employer = new Employer();
            employer.Email = employerInfo.Email;

            using (var sha256 = SHA256.Create())
            {
                var password = Convert.ToBase64String(sha256.ComputeHash(Encoding.Default.GetBytes(employerInfo.Password)));
                employer.Password = password.ToString();
            }

            employer.Uuid = Guid.NewGuid().ToString();

            _context.Add(employer);
            await _context.SaveChangesAsync();
         
            return Ok("The employer was successfully registered.");
        }

        // POST: Employers/Login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] EmployerInfo employerInfo)
        {

            if (string.IsNullOrEmpty(employerInfo.Email) || string.IsNullOrEmpty(employerInfo.Password))
            {
                return BadRequest("The email and password must be filled out.");
            }

            string hashedPassword;

            using (var sha256 = SHA256.Create())
            {
                hashedPassword = Convert.ToBase64String(sha256.ComputeHash(Encoding.Default.GetBytes(employerInfo.Password)));
            }

            Employer user = _context.Employers.Where(e => e.Email.Equals(employerInfo.Email) && e.Password.Equals(hashedPassword)).SingleOrDefault();

            if (user == null)
            {
                return BadRequest("There was no user found with those credentials.");
            }

            user.Uuid = Guid.NewGuid().ToString();

            _context.Update(user);
            await _context.SaveChangesAsync();

            return Ok(user.Uuid);
        }

        private bool EmployerExists(int id)
        {
          return _context.Employers.Any(e => e.EmployerId == id);
        }
    }
}
