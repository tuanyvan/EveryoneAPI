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
    public class GenderIdentitiesController : Controller
    {
        private readonly EveryoneDBContext _context;

        public GenderIdentitiesController(EveryoneDBContext context)
        {
            _context = context;
        }

        // GET: GenderIdentities
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var json = Array.Empty<object>().ToList();

            var genderIdentities = _context.GenderIdentities.ToList();

            foreach (var gender in genderIdentities)
            {
                json.Add(new
                {
                    GenderId = gender.GenderId,
                    Name = gender.Name
                });
            }

            return Json(json);
        }

        private bool GenderIdentityExists(int id)
        {
            return _context.GenderIdentities.Any(e => e.GenderId == id);
        }
    }
}
