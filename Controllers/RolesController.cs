// Controllers/RolesController.cs
using System.Threading.Tasks;
using ExpenseTrackerApi.Data;
using ExpenseTrackerApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public RolesController(AppDbContext db) => _db = db;

        // GET api/roles
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _db.Roles.ToListAsync();
            return Ok(roles);
        }
    }
}
