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
    public class PronounsController : Controller
    {
        private readonly EveryoneDBContext _context;

        public PronounsController(EveryoneDBContext context)
        {
            _context = context;
        }

        // GET: Pronouns
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Return a list of pronouns.
            var json = Array.Empty<object>().ToList();

            var pronouns = _context.Pronouns.ToList();

            foreach (var pronoun in pronouns)
            {
                json.Add(new
                {
                    PronounId = pronoun.PronounId,
                    Name = pronoun.Name
                });
            }

            return Json(json);
        }

        private bool PronounExists(int id)
        {
            return _context.Pronouns.Any(e => e.PronounId == id);
        }
    }
}
