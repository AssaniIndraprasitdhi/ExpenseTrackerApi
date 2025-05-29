using ExpenseTrackerApi.Data;
using ExpenseTrackerApi.DTOs;
using ExpenseTrackerApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly AppDbContext _db;

        public UsersController(IUserService userService, AppDbContext db)
        {
            _userService = userService;
            _db = db;
        }

        // GET api/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _db
                .Users.Include(u => u.Role)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Role = u.Role.Name,
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET api/users/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            var dto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = (await _db.Roles.FindAsync(user.RoleId))?.Name ?? "",
            };

            return Ok(dto);
        }
    }
}
