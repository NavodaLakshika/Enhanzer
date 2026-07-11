using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Enhanzer.Backend.Models;

namespace Enhanzer.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LocationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LocationsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetLocations()
        {
            var locations = await _context.Location_Details.ToListAsync();
            return Ok(locations);
        }
    }
}
