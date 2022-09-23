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
        public async Task<IActionResult> Details(string uuid)
        {
            // Only return an email.
            string email = _context.Employers.Where(e => e.Uuid == uuid).SingleOrDefault().Email.ToLower();
            if (email == null)
            {
                return StatusCode(401, "The user making the request is invalid.");
            }
            return Ok(email);
        }

        // POST: Employers/Register
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] EmployerInfo employerInfo)
        {
            // Check that the email and password fields are not empty.
            if (string.IsNullOrEmpty(employerInfo.Email.ToLower()) || string.IsNullOrEmpty(employerInfo.Password))
            {
                return BadRequest("The email and password field cannot be empty.");
            }

            // Check if the email is already being used.
            foreach (Employer e in _context.Employers.ToList())
            {
                if (e.Email.ToLower() == employerInfo.Email.ToLower())
                {
                    return BadRequest("That email already exists.");
                }
            }

            // Create the new employer with an email and hashed password.
            Employer employer = new Employer();
            employer.Email = employerInfo.Email.ToLower();

            using (var sha256 = SHA256.Create())
            {
                var password = Convert.ToBase64String(sha256.ComputeHash(Encoding.Default.GetBytes(employerInfo.Password)));
                employer.Password = password.ToString();
            }

            // Token will be different on login.
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
            // Check that the email and password fields are filled out.
            if (string.IsNullOrEmpty(employerInfo.Email.ToLower()) || string.IsNullOrEmpty(employerInfo.Password))
            {
                return BadRequest("The email and password must be filled out.");
            }

            // Hash the request password.
            string hashedPassword;

            using (var sha256 = SHA256.Create())
            {
                hashedPassword = Convert.ToBase64String(sha256.ComputeHash(Encoding.Default.GetBytes(employerInfo.Password)));
            }

            // Check that the request password hash and email match what is on record.
            Employer user = _context.Employers.Where(e => e.Email.ToLower().Equals(employerInfo.Email.ToLower()) && e.Password.Equals(hashedPassword)).SingleOrDefault();

            if (user == null)
            {
                return BadRequest("There was no user found with those credentials.");
            }

            // Update the user's token and send it to them in the response payload.
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
