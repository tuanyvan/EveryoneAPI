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
    public class EthnicitiesController : Controller
    {
        private readonly EveryoneDBContext _context;

        public EthnicitiesController(EveryoneDBContext context)
        {
            _context = context;
        }

        // GET: Ethnicities
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Return a list of ethnicities.
            var json = Array.Empty<object>().ToList();

            var ethnicities = _context.Ethnicities.ToList();
            
            foreach (var ethnicity in ethnicities)
            {
                json.Add(new
                {
                    EthnicityId = ethnicity.EthnicityId,
                    Name = ethnicity.Name
                });
            }

            return Json(json);
        }

        private bool EthnicityExists(int id)
        {
            return _context.Ethnicities.Any(e => e.EthnicityId == id);
        }
    }
}
