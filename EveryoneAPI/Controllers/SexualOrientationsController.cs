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
    public class SexualOrientationsController : Controller
    {
        private readonly EveryoneDBContext _context;

        public SexualOrientationsController(EveryoneDBContext context)
        {
            _context = context;
        }

        // GET: SexualOrientations
        [HttpGet]
        public async Task<IActionResult> Index()
        {
              return View(await _context.SexualOrientations.ToListAsync());
        }
        
        private bool SexualOrientationExists(int id)
        {
          return _context.SexualOrientations.Any(e => e.OrientationId == id);
        }
    }
}
